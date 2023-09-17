namespace Fling.Store.Common

open System.IO
open FsToolbox.Core.Results


type IFlingStore =

    abstract member AddEmailRequest:
        id: string *
        subscriptionId: string *
        requestBlob: MemoryStream *
        maxRetryAttempts: int *
        ?transactionId: string *
        ?templateVersionId: string *
        ?dataBlobId: string ->
            unit


    abstract member AddEmailRequest:
        id: string *
        subscriptionId: string *
        requestData: EmailRequestData *
        maxRetryAttempts: int *
        ?transactionId: string *
        ?templateVersionId: string *
        ?dataBlobId: string ->
            unit

    abstract member AddEmailSendAttempt:
        id: string * requestId: string * wasSuccessful: bool * responseBlob: MemoryStream -> unit

    abstract member AddEmailSendAttempt: id: string * requestId: string * wasSuccessful: bool * response: string -> unit

    abstract member AddEmailOutQueueItem: requestId: string -> unit

    abstract member AddEmailHtmlContent: requestId: string * content: MemoryStream -> unit

    abstract member AddEmailHtmlContentString: requestId: string * content: string -> unit

    abstract member AddEmailPlainTextContent: requestId: string * content: MemoryStream -> unit

    abstract member AddEmailPlainTextContentString: requestId: string * content: string -> unit

    abstract member AddDataBlob: id: string * subscriptionId: string * rawBlob: MemoryStream * createdBy: string -> unit

    abstract member AddJsonDataBlob: id: string * subscriptionId: string * data: 'T * createdBy: string -> unit

    abstract member AddDataBlobString: id: string * subscriptionId: string * data: string * createdBy: string -> unit

    abstract member AddEmailTemplate: id: string * subscriptionId: string * name: string -> unit

    abstract member AddEmailTemplateVersion:
        id: string * templateId: string * templateBlob: MemoryStream * createdBy: string -> unit

    abstract member AddEmailTemplateVersionString:
        id: string * templateId: string * template: string * createdBy: string -> unit

    abstract member AddEmailAttachment:
        requestId: string * attachmentBlob: MemoryStream * fileName: string * contentType: string -> unit

    abstract member AddEmailAttachment:
        requestId: string * attachment: byte array * fileName: string * contentType: string -> unit

    abstract member DeleteEmailOutQueueItem: requestId: string -> unit

    abstract member GetEmailRequest: requestId: string -> EmailRequestDetails option

    abstract member GetEmailSendAttempts: requestId: string -> EmailSendAttempt list

    abstract member GetEmailHtmlContent: requestId: string -> string option

    abstract member GetEmailPlainTextContent: requestId: string -> string option

    abstract member GetEmailOutQueueIds: unit -> string list

    abstract member GetEmailTemplate: subscriptionId: string * name: string -> EmailTemplate option

    abstract member GetEmailTemplateLatestNonDraftVersion: subscriptionId: string * name: string -> string option

    abstract member GetEmailTemplateDetails: subscriptionId: string * name: string -> EmailTemplateDetails option

    abstract member TestConnection: unit -> ActionResult<unit>