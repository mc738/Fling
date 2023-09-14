namespace Fling.Store.Common

open System.IO


type IFlingStore =

    abstract member AddEmailRequest:
        id: string *
        subscriptionId: string *
        requestBlob: MemoryStream *
        maxRetryAttempts: int *
        transactionId: string option *
        templateVersionId: string option *
        dataBlobId: string option ->
            unit


    abstract member AddEmailRequest:
        id: string *
        subscriptionId: string *
        requestData: EmailRequestData *
        maxRetryAttempts: int *
        transactionId: string option *
        templateVersionId: string option *
        dataBlobId: string option ->
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
