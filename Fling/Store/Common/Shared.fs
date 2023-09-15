namespace Fling.Store.Common

open System
open System.Text.Json.Serialization

[<AutoOpen>]
module Shared =

    [<CLIMutable>]
    type EmailRequestData =
        { [<JsonPropertyName("fromName")>]
          FromName: string
          [<JsonPropertyName("fromAddress")>]
          FromAddress: string
          [<JsonPropertyName("toName")>]
          ToName: string
          [<JsonPropertyName("toAddress")>]
          ToAddress: string
          [<JsonPropertyName("subject")>]
          Subject: string }

    type EmailTemplateDetails = { VersionId: string; Template: string }

    type EmailTemplate =
        { Id: string
          SubscriptionId: string
          Name: string }

    type EmailSendAttempt =
        { Id: string
          RequestId: string
          AttemptedOn: DateTime
          WasSuccessful: bool
          ResponseBlob: byte array
          Hash: string }
        
    type EmailRequestDetails =
        { Id: string
          SubscriptionId: string
          QueuedOn: DateTime
          RequestData: byte array
          Hash: string
          MaxRetryAttempts: int
          TransactionId: string option
          TemplateVersionId: string option
          DataBlobId: string option }
    
