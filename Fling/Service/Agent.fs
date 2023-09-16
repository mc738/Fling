namespace Fling.Service


open System
open Fling.Emails
open Fling.Store.Common
open Fluff.Core
open FsToolbox.Core.Results
open Microsoft.Extensions.Logging

module Agent =

    type Request =
        | QueueEmail of QueueEmailRequest
        | QueueTemplatedEmail of QueueTemplatedEmailRequest
        | Ping of AsyncReplyChannel<unit>
        | Test of AsyncReplyChannel<ActionResult<unit>>

    and QueueEmailRequest =
        { SubscriptionId: string
          UserId: string
          Email: EmailMessage }

    and QueueTemplatedEmailRequest =
        { SubscriptionId: string
          UserId: string
          Email: TemplatedEmailMessage }

    let createReference _ = Guid.NewGuid().ToString("n")

    let handleEmail (store: IFlingStore) (log: ILogger) (service: EmailService) (request: QueueEmailRequest) =
        async {
            return!
                async {

                    let requestId = createReference ()

                    let requestData =
                        ({ FromName = request.Email.FromName
                           FromAddress = request.Email.FromAddress
                           ToName = request.Email.ToName
                           ToAddress = request.Email.ToAddress
                           Subject = request.Email.Subject }: EmailRequestData)

                    store.AddEmailRequest(requestId, request.SubscriptionId, requestData, 10)

                    store.AddEmailHtmlContentString(requestId, request.Email.HtmlContent)
                    store.AddEmailPlainTextContentString(requestId, request.Email.PlainTextContent)
                    store.AddEmailOutQueueItem(requestId)

                    request.Email.Attachments
                    |> List.iter (fun ea -> store.AddEmailAttachment(requestId, ea.Attachment, ea.Filename, ea.Type))

                    match service with
                    | EmailService.SendGrid cfg ->
                        match! SendGrid.sendSingleEmail cfg.Token request.Email with
                        | ActionResult.Success r ->
                            store.AddEmailSendAttempt(createReference (), requestId, true, r)
                            store.DeleteEmailOutQueueItem(requestId)
                            return ActionResult.Success()
                        | ActionResult.Failure f ->
                            store.AddEmailSendAttempt(createReference (), requestId, false, f.Message)
                            return ActionResult.Failure f
                }
        }

    let handleTemplatedEmail
        (store: CommsStore)
        (log: ILogger)
        (service: EmailService)
        (request: QueueTemplatedEmailRequest)
        =
        async {
            return!
                store.GetEmailTemplateDetails(request.SubscriptionId, request.Email.TemplateName)
                |> Option.map (fun td ->
                    async {
                        let content = Formatting.createFromTemplate td.Template request.Email.TemplateData

                        let email =
                            ({ FromName = request.Email.FromName
                               FromAddress = request.Email.FromAddress
                               ToName = request.Email.ToName
                               ToAddress = request.Email.ToAddress
                               Subject = request.Email.Subject
                               PlainTextContent = Formatting.renderPlainText false content
                               HtmlContent = Formatting.createHtml content
                               Attachments = request.Email.Attachments }: EmailMessage)


                        let requestId = createReference ()

                        let requestData =
                            ({ FromName = email.FromName
                               FromAddress = email.FromAddress
                               ToName = email.ToName
                               ToAddress = email.ToAddress
                               Subject = email.Subject }: EmailRequestData)

                        let dataBlobId = createReference ()

                        store.AddDataBlobString(
                            dataBlobId,
                            request.SubscriptionId,
                            request.Email.TemplateData.ToJson(),
                            request.UserId
                        )

                        store.AddEmailRequest(
                            requestId,
                            request.SubscriptionId,
                            requestData,
                            10,
                            templateVersionId = td.VersionId,
                            dataBlobId = dataBlobId
                        )

                        store.AddEmailHtmlContentString(requestId, email.HtmlContent)
                        store.AddEmailPlainTextContentString(requestId, email.PlainTextContent)
                        store.AddEmailOutQueueItem(requestId)

                        email.Attachments
                        |> List.iter (fun ea -> store.AddEmailAttachment(requestId, ea.Attachment, ea.Filename, ea.Type))

                        match service with
                        | EmailService.SendGrid cfg ->
                            match! SendGrid.sendSingleEmail cfg.Token email with
                            | ActionResult.Success r ->
                                store.AddEmailSendAttempt(createReference (), requestId, true, r)
                                store.DeleteEmailOutQueueItem(requestId)
                                return ActionResult.Success()
                            | ActionResult.Failure f ->
                                store.AddEmailSendAttempt(createReference (), requestId, false, f.Message)
                                return ActionResult.Failure f
                    })
                |> Option.defaultWith (fun _ ->
                    async {
                        return
                            ({ Message =
                                $"Template `{request.Email.TemplateName}` not found for subscription `{request.SubscriptionId}`."
                               DisplayMessage = "Could not send emails"
                               Exception = None }: FailureResult)
                            |> ActionResult.Failure
                    })
        }

    let start (store: CommsStore) (log: ILogger) (emailService: EmailService) =
        MailboxProcessor<Request>.Start
            (fun inbox ->
                let rec loop () =
                    async {
                        let! request = inbox.TryReceive(1000)


                        try
                            match request with
                            | Some (Request.QueueEmail r) ->
                                match! handleEmail store log emailService r with
                                | ActionResult.Success _ ->
                                    log.LogInformation(
                                        $"Email send to {r.Email.ToName} ({r.Email.ToAddress}). Subject: `{r.Email.Subject}`"
                                    )
                                | ActionResult.Failure f ->
                                    log.LogError(
                                        $"Could not send email {r.Email.ToName} ({r.Email.ToAddress}). Subject: `{r.Email.Subject}`. Error: {f.Message}"
                                    )
                            | Some (Request.QueueTemplatedEmail r) ->
                                match! handleTemplatedEmail store log emailService r with
                                | ActionResult.Success _ ->
                                    log.LogInformation(
                                        $"Email send to {r.Email.ToName} ({r.Email.ToAddress}). Subject: `{r.Email.Subject}`"
                                    )
                                | ActionResult.Failure f ->
                                    log.LogError(
                                        $"Could not send email {r.Email.ToName} ({r.Email.ToAddress}). Subject: `{r.Email.Subject}`. Error: {f.Message}"
                                    )
                            | Some (Request.Ping r) -> r.Reply()
                            | Some (Request.Test r) -> store.TestConnection() |> r.Reply
                            | None ->
                                // TODO handle.
                                ()
                        with exn ->
                            log.LogError($"Error handled request `{request}`. Exception: {exn.Message}")

                        return! loop ()
                    }

                log.LogInformation("Comms service agent starting.")

                loop ())