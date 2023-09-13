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
