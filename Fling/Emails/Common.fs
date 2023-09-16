namespace Fling.Emails

[<AutoOpen>]
module Common =

    open System
    open System.IO
    open Armarium
    open Fluff.Core
    open FsToolbox.Core.Results
    
    type EmailMessage =
        { FromName: string
          FromAddress: string
          ToName: string
          ToAddress: string
          Subject: string
          PlainTextContent: string
          HtmlContent: string
          Attachments: EmailAttachment list }

    and EmailAttachment =
        { Attachment: byte array
          Filename: string
          Type: string }

        static member FromBytes(data: byte array, fileName: string, contentType: string) =
            { Attachment = data
              Filename = fileName
              Type = contentType }

        static member FromFile(path: string, fileName: string, contentType: string) =
            EmailAttachment.FromBytes(File.ReadAllBytes path, fileName, contentType)

        static member FromMemoryStream(stream: MemoryStream, fileName: string, contentType: string) =
            EmailAttachment.FromBytes(stream.ToArray(), fileName, contentType)

        static member FromStream(stream: Stream, fileName: string, contentType: string) =
            use ms = new MemoryStream()
            stream.CopyTo ms

            EmailAttachment.FromMemoryStream(ms, fileName, contentType)

        static member FromFileRepository
            (
                repo: FileRepository,
                readArgs: FileReadOperationArguments,
                path: string,
                fileName: string,
                contentType: string
            ) =
            repo.ReadAllBytes(path, readArgs)
            |> ActionResult.map (fun b -> EmailAttachment.FromBytes(b, fileName, contentType))

        member ea.Base64EncodedContent = ea.Attachment |> Convert.ToBase64String

    and TemplatedEmailMessage =
        { FromName: string
          FromAddress: string
          ToName: string
          ToAddress: string
          Subject: string
          TemplateName: string
          TemplateData: Mustache.Data
          Attachments: EmailAttachment list }