namespace Fling.Store.Sqlite

open System
open System.IO
open System.Security.Cryptography
open System.Text
open System.Text.Json
open System.Text.Json.Serialization
open Fling.Store.Common
open Freql.Core.Common.Types
open Freql.Sqlite
open FsToolbox.Core
open FsToolbox.Core.Results
open FsToolbox.Extensions
// open Microsoft.Extensions.Logging

module Internal =

    open Fling.Store.Sqlite.Persistence

    let initialize (ctx: SqliteContext) =
        [ Records.DataBlob.CreateTableSql()
          Records.EmailTransactions.CreateTableSql()
          Records.EmailTransactionState.CreateTableSql()
          Records.EmailTemplate.CreateTableSql()
          Records.EmailTemplateVersion.CreateTableSql()
          Records.EmailRequest.CreateTableSql()
          Records.EmailTransactionStateRequest.CreateTableSql()
          Records.EmailHtmlContent.CreateTableSql()
          Records.EmailPlainTextContent.CreateTableSql()
          Records.EmailSendAttempt.CreateTableSql()
          Records.EmailAttachment.CreateTableSql()
          Records.EmailOutQueueItem.CreateTableSql()
          Records.EmailPoisonQueueItem.CreateTableSql() ]
        |> List.iter (ctx.ExecuteSqlNonQuery >> ignore)

        ctx

    let addEmailRequest
        (ctx: SqliteContext)
        id
        subscriptionId
        (requestBlob: MemoryStream)
        maxRetryAttempts
        transactionId
        templateVersionId
        dataBlobId
        =
        ({ Id = id
           SubscriptionId = subscriptionId
           QueuedOn = DateTime.UtcNow
           RequestBlob = BlobField.FromBytes requestBlob
           Hash = requestBlob.GetSHA256Hash()
           MaxRetryAttempts = maxRetryAttempts
           TransactionId = transactionId
           TemplateVersionId = templateVersionId
           DataBlobId = dataBlobId }
        : Parameters.NewEmailRequest)
        |> Operations.insertEmailRequest ctx

    let addEmailSendAttempt (ctx: SqliteContext) id requestId wasSuccessful (responseBlob: MemoryStream) =
        ({ Id = id
           RequestId = requestId
           AttemptedOn = DateTime.UtcNow
           WasSuccessful = wasSuccessful
           ResponseBlob = BlobField.FromBytes responseBlob
           Hash = responseBlob.GetSHA256Hash() }
        : Parameters.NewEmailSendAttempt)
        |> Operations.insertEmailSendAttempt ctx

    let addEmailOutQueueItem (ctx: SqliteContext) requestId =
        ({ RequestId = requestId }: Parameters.NewEmailOutQueueItem)
        |> Operations.insertEmailOutQueueItem ctx

    let addEmailHtmlContent ctx requestId (content: MemoryStream) =
        ({ RequestId = requestId
           Content = BlobField.FromBytes content
           Hash = content.GetSHA256Hash() }
        : Parameters.NewEmailHtmlContent)
        |> Operations.insertEmailHtmlContent ctx

    let addEmailPlainTextContent ctx requestId (content: MemoryStream) =
        ({ RequestId = requestId
           Content = BlobField.FromBytes content
           Hash = content.GetSHA256Hash() }
        : Parameters.NewEmailPlainTextContent)
        |> Operations.insertEmailPlainTextContent ctx

    let addDataBlob ctx id subscriptionId (rawBlob: MemoryStream) createdBy =
        ({ Id = id
           SubscriptionId = subscriptionId
           RawBlob = BlobField.FromBytes rawBlob
           Hash = rawBlob.GetSHA256Hash()
           CreatedOn = DateTime.UtcNow
           CreatedBy = createdBy }
        : Parameters.NewDataBlob)
        |> Operations.insertDataBlob ctx

    let addEmailTemplate ctx id subscriptionId name =
        ({ Id = id
           SubscriptionId = subscriptionId
           Name = name }
        : Parameters.NewEmailTemplate)
        |> Operations.insertEmailTemplate ctx

    let addEmailTemplateVersion ctx id templateId (templateBlob: MemoryStream) createdBy =
        let version =
            Operations.selectEmailTemplateVersionRecord ctx [ "WHERE template_id = @0;" ] [ templateId ]
            |> Option.map (fun t -> t.Version + 1)
            |> Option.defaultValue 1

        ({ Id = id
           TemplateId = templateId
           Version = version
           TemplateBlob = BlobField.FromBytes templateBlob
           Hash = templateBlob.GetSHA256Hash()
           CreatedOn = DateTime.UtcNow
           CreatedBy = createdBy }
        : Parameters.NewEmailTemplateVersion)
        |> Operations.insertEmailTemplateVersion ctx

    let addEmailAttachment ctx requestId (attachmentBlob: MemoryStream) fileName contentType =
        ({ RequestId = requestId
           AttachmentBlob = BlobField.FromBytes attachmentBlob
           Hash = attachmentBlob.GetSHA256Hash()
           FileName = fileName
           ContentType = contentType }
        : Parameters.NewEmailAttachment)
        |> Operations.insertEmailAttachment ctx

    let deleteEmailOutQueueItem (ctx: SqliteContext) (requestId: string) =
        ctx.ExecuteVerbatimNonQueryAnon("DELETE FROM email_out_queue WHERE request_id = @0;", [ requestId ])
        |> ignore

    let getEmailRequest (ctx: SqliteContext) (requestId: string) =
        Operations.selectEmailRequestRecord ctx [ "WHERE id = @0;" ] [ requestId ]

    let getEmailSendAttempts (ctx: SqliteContext) (requestId: string) =
        Operations.selectEmailSendAttemptRecords ctx [ "WHERE request_id =@0;" ] [ requestId ]

    let getEmailHtmlContent (ctx: SqliteContext) (requestId: string) =
        Operations.selectEmailHtmlContentRecord ctx [ "WHERE request_id = @0;" ] [ requestId ]

    let getEmailPlainTextContent (ctx: SqliteContext) (requestId: string) =
        Operations.selectEmailPlainTextContentRecord ctx [ "WHERE request_id = @0" ] [ requestId ]

    let getEmailOutQueue (ctx: SqliteContext) =
        Operations.selectEmailOutQueueItemRecords ctx [] []

    let getDataBlob (ctx: SqliteContext) (id: string) =
        Operations.selectDataBlobRecord ctx [ "WHERE id = @0;" ] [ id ]

    let getEmailTemplateByName (ctx: SqliteContext) (subscriptionId: string) (name: string) =
        Operations.selectEmailTemplateRecord
            ctx
            [ "WHERE subscription_id = @0 AND name = @1;" ]
            [ subscriptionId; name ]

    let getEmailTemplateLatestNonDraftVersion (ctx: SqliteContext) (templateId: string) =
        Operations.selectEmailTemplateVersionRecord
            ctx
            [ "WHERE template_id = @0 ORDER BY version DESC LIMIT 1;" ]
            [ templateId ]

// TODO needed?
type FlingSqliteStoreContext(ctx: SqliteContext) =

    static member Create(path: string) =
        match File.Exists path with
        | true -> SqliteContext.Open path |> FlingSqliteStoreContext
        | false -> SqliteContext.Create path |> Internal.initialize |> FlingSqliteStoreContext

    member _.Get() = ctx

type FlingSqliteStore(ctx: SqliteContext (*storeCtx: FlingSqliteStoreContext*) (*, log: ILogger<CommsStore>*) ) =

    // TODO needed?
    //let ctx = storeCtx.Get()

    interface IFlingStore with

        member _.AddEmailRequest
            (
                id,
                subscriptionId,
                requestBlob,
                maxRetryAttempts,
                ?transactionId,
                ?templateVersionId,
                ?dataBlobId
            ) =
            Internal.addEmailRequest
                ctx
                id
                subscriptionId
                requestBlob
                maxRetryAttempts
                transactionId
                templateVersionId
                dataBlobId

        member _.AddEmailRequest
            (
                id,
                subscriptionId,
                requestData: EmailRequestData,
                maxRetryAttempts,
                ?transactionId,
                ?templateVersionId,
                ?dataBlobId
            ) =
            use ms =
                new MemoryStream(JsonSerializer.Serialize requestData |> Encoding.UTF8.GetBytes)

            Internal.addEmailRequest
                ctx
                id
                subscriptionId
                ms
                maxRetryAttempts
                transactionId
                templateVersionId
                dataBlobId

        member _.AddEmailSendAttempt(id, requestId, wasSuccessful, responseBlob) =
            Internal.addEmailSendAttempt ctx id requestId wasSuccessful responseBlob

        member _.AddEmailSendAttempt(id, requestId, wasSuccessful, response: string) =
            use ms = new MemoryStream(response |> Encoding.UTF8.GetBytes)
            Internal.addEmailSendAttempt ctx id requestId wasSuccessful ms

        member _.AddEmailOutQueueItem(requestId) =
            Internal.addEmailOutQueueItem ctx requestId

        member _.AddEmailHtmlContent(requestId, content) =
            Internal.addEmailHtmlContent ctx requestId content

        member cs.AddEmailHtmlContentString(requestId, content: string) =
            use ms = new MemoryStream(content |> Encoding.UTF8.GetBytes)
            (cs :> IFlingStore).AddEmailHtmlContent(requestId, ms)

        member _.AddEmailPlainTextContent(requestId, content) =
            Internal.addEmailPlainTextContent ctx requestId content

        member cs.AddEmailPlainTextContentString(requestId, content: string) =
            use ms = new MemoryStream(content |> Encoding.UTF8.GetBytes)
            (cs :> IFlingStore).AddEmailPlainTextContent(requestId, ms)

        member _.AddDataBlob(id, subscriptionId, rawBlob, createdBy) =
            Internal.addDataBlob ctx id subscriptionId rawBlob createdBy

        member cs.AddJsonDataBlob<'T>(id, subscriptionId, data, createdBy) =
            let content = JsonSerializer.Serialize<'T> data |> Encoding.UTF8.GetBytes
            use ms = new MemoryStream(content)
            (cs: IFlingStore).AddDataBlob(id, subscriptionId, ms, createdBy)

        member cs.AddDataBlobString(id, subscriptionId, data: string, createdBy) =
            use ms = new MemoryStream(data |> Encoding.UTF8.GetBytes)
            (cs :> IFlingStore).AddDataBlob(id, subscriptionId, ms, createdBy)

        member _.AddEmailTemplate(id, subscriptionId, name) =
            Internal.addEmailTemplate ctx id subscriptionId name

        member _.AddEmailTemplateVersion(id, templateId, templateBlob, createdBy) =
            Internal.addEmailTemplateVersion ctx id templateId templateBlob createdBy

        member _.AddEmailTemplateVersionString(id, templateId, template: string, createdBy) =
            use ms = new MemoryStream(template |> Encoding.UTF8.GetBytes)

            Internal.addEmailTemplateVersion ctx id templateId ms createdBy

        member _.AddEmailAttachment(requestId, attachmentBlob, fileName, contentType) =
            Internal.addEmailAttachment ctx requestId attachmentBlob fileName contentType

        member _.AddEmailAttachment(requestId, attachment: byte array, fileName, contentType) =
            use ms = new MemoryStream(attachment)

            Internal.addEmailAttachment ctx requestId ms fileName contentType

        member _.DeleteEmailOutQueueItem(requestId) =
            Internal.deleteEmailOutQueueItem ctx requestId

        member _.GetEmailRequest(requestId) =
            Internal.getEmailRequest ctx requestId
            |> Option.map (fun er ->
                { Id = er.Id
                  SubscriptionId = er.SubscriptionId
                  QueuedOn = er.QueuedOn
                  RequestData = er.RequestBlob.ToBytes()
                  Hash = er.Hash
                  MaxRetryAttempts = er.MaxRetryAttempts
                  TransactionId = er.TransactionId
                  TemplateVersionId = er.TemplateVersionId
                  DataBlobId = er.DataBlobId })

        member _.GetEmailSendAttempts(requestId) =
            Internal.getEmailSendAttempts ctx requestId
            |> List.map (fun das ->
                { Id = das.Id
                  RequestId = das.RequestId
                  AttemptedOn = das.AttemptedOn
                  WasSuccessful = das.WasSuccessful
                  ResponseBlob = das.ResponseBlob.ToBytes()
                  Hash = das.Hash })

        member _.GetEmailHtmlContent(requestId) =
            Internal.getEmailHtmlContent ctx requestId
            |> Option.map (fun c -> c.Content.ToBytes() |> Encoding.UTF8.GetString)

        member _.GetEmailPlainTextContent(requestId) =
            Internal.getEmailPlainTextContent ctx requestId
            |> Option.map (fun c -> c.Content.ToBytes() |> Encoding.UTF8.GetString)

        member _.GetEmailOutQueueIds() =
            Internal.getEmailOutQueue ctx |> List.map (fun oqi -> oqi.RequestId)

        member _.GetEmailTemplate(subscriptionId, name) =
            Internal.getEmailTemplateByName ctx subscriptionId name
            |> Option.map (fun et ->
                { Id = et.Id
                  SubscriptionId = et.SubscriptionId
                  Name = et.Name })

        member _.GetEmailTemplateLatestNonDraftVersion(subscriptionId, name) =
            Internal.getEmailTemplateByName ctx subscriptionId name
            |> Option.bind (fun t -> Internal.getEmailTemplateLatestNonDraftVersion ctx t.Id)
            |> Option.map (fun tv -> tv.TemplateBlob.ToBytes() |> Encoding.UTF8.GetString)

        member _.GetEmailTemplateDetails(subscriptionId, name) =
            Internal.getEmailTemplateByName ctx subscriptionId name
            |> Option.bind (fun t -> Internal.getEmailTemplateLatestNonDraftVersion ctx t.Id)
            |> Option.map (fun tv ->
                { VersionId = tv.Id
                  Template = tv.TemplateBlob.ToBytes() |> Encoding.UTF8.GetString })

        member _.TestConnection() =
            try
                ctx.TestConnection() |> ignore
                ActionResult.Success()
            with exn ->
                ({ Message = $"Connection test failed. Exception: {exn.Message}"
                   DisplayMessage = "Connection test failed."
                   Exception = Some exn }
                : FailureResult)
                |> ActionResult.Failure
