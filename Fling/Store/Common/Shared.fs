namespace Fling.Store.Common

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
