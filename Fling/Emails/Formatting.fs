namespace Fling.Emails

module Formatting =

    open System
    open FDOM.Core.Common
    open FDOM.Rendering
    open Fluff.Core

    let h1 content =
        ({ Style = DOM.Style.Default
           Level = DOM.HeaderLevel.H1
           Content = content
           Indexed = false }
        : DOM.HeaderBlock)
        |> DOM.BlockContent.Header

    let h2 content =
        ({ Style = DOM.Style.Default
           Level = DOM.HeaderLevel.H2
           Content = content
           Indexed = false }
        : DOM.HeaderBlock)
        |> DOM.BlockContent.Header

    let h3 content =
        ({ Style = DOM.Style.Default
           Level = DOM.HeaderLevel.H3
           Content = content
           Indexed = false }
        : DOM.HeaderBlock)
        |> DOM.BlockContent.Header

    let h4 content =
        ({ Style = DOM.Style.Default
           Level = DOM.HeaderLevel.H4
           Content = content
           Indexed = false }
        : DOM.HeaderBlock)
        |> DOM.BlockContent.Header

    let h5 content =
        ({ Style = DOM.Style.Default
           Level = DOM.HeaderLevel.H5
           Content = content
           Indexed = false }
        : DOM.HeaderBlock)
        |> DOM.BlockContent.Header

    let h6 content =
        ({ Style = DOM.Style.Default
           Level = DOM.HeaderLevel.H6
           Content = content
           Indexed = false }
        : DOM.HeaderBlock)
        |> DOM.BlockContent.Header

    let p content =
        ({ Style = DOM.Style.Default
           Content = content }
        : DOM.ParagraphBlock)
        |> DOM.BlockContent.Paragraph

    let ol items =
        ({ Style = DOM.Style.Default
           Items = items
           Ordered = true }
        : DOM.ListBlock)
        |> DOM.BlockContent.List

    let ul items =
        ({ Style = DOM.Style.Default
           Items = items
           Ordered = false }
        : DOM.ListBlock)
        |> DOM.BlockContent.List

    let li content =
        ({ Style = DOM.Style.Default
           Content = content }
        : DOM.ListItem)

    let img src title alt height width =
        ({ Style = DOM.Style.Default
           Source = src
           Title = title
           AltText = alt
           Height = height
           Width = width }
        : DOM.ImageBlock)
        |> DOM.BlockContent.Image

    let text content =
        ({ Content = content }: DOM.InlineText) |> DOM.InlineContent.Text

    let space content =
        ({ Content = " " }: DOM.InlineText) |> DOM.InlineContent.Text

    let comma content =
        ({ Content = "," }: DOM.InlineText) |> DOM.InlineContent.Text

    let fullStop content =
        ({ Content = "." }: DOM.InlineText) |> DOM.InlineContent.Text

    let link url content =
        ({ Content = content
           Url = url
           Style = DOM.Style.Default }
        : DOM.InlineLink)
        |> DOM.InlineContent.Link

    // TODO add support for tables!

    let parseTemplate (template: string) = Fluff.Core.Mustache.parse template

    let createFromTemplate (template: string) (data: Mustache.Data) =
        Fluff.Core.Mustache.parse template
        |> Mustache.replace data true
        |> fun r -> r.Split Environment.NewLine
        |> List.ofArray
        |> FDOM.Core.Parsing.Parser.ParseLines
        |> fun p -> p.CreateBlockContent()

    let createHtml (content: DOM.BlockContent list) = Html.renderFromBlocks content

    let renderPlainText (addSpaces: bool) (content: DOM.BlockContent list) =
        let concat (values: string list) =
            values
            |> String.concat (
                match addSpaces with
                | true -> " "
                | false -> ""
            )

        let concatLines (values: string list) =
            values |> String.concat $"{Environment.NewLine}{Environment.NewLine}"

        let renderContent (content: DOM.InlineContent) =
            match content with
            | DOM.InlineContent.Text t -> t.Content
            | DOM.InlineContent.Span s -> s.Content
            | DOM.InlineContent.Link l -> $"{l.Content} ({l.Url})"

        content
        |> List.map (fun bc ->
            match bc with
            | DOM.BlockContent.Header h ->
                let content = h.Content |> List.map renderContent |> concat
                $"{content}{Environment.NewLine}{String('-', content.Length)}"

            | DOM.BlockContent.Paragraph p -> p.Content |> List.map renderContent |> concat
            | DOM.BlockContent.List l ->
                match l.Ordered with
                | true ->
                    l.Items
                    |> List.mapi (fun i item -> $"{i + 1}. {item.Content |> List.map renderContent |> concat}")
                    |> concatLines
                | false ->
                    l.Items
                    |> List.map (fun item -> $"* {item.Content |> List.map renderContent |> concat}")
                    |> concatLines
            | DOM.BlockContent.Code c -> c.Content |> List.map renderContent |> concat
            | DOM.BlockContent.Image i -> $"[{i.AltText}]({i.Source})")
        |> concatLines

    type Document = { Elements: Element list }

    and Element =
        | H1 of Content: Content list
        | H2 of Content: Content list
        | H3 of Content: Content list
        | H4 of Content: Content list
        | H5 of Content: Content list
        | H6 of Content: Content list
        | P of Content: Content list
        | OL of Items: Content list list
        | UL of Items: Content list list
        | IMG of Source: string * Title: string * AltText: string * Height: string option * Width: string option

        member b.ToBlock() =
            match b with
            | H1 content -> h1 (content |> List.map (fun c -> c.ToValue()))
            | H2 content -> h2 (content |> List.map (fun c -> c.ToValue()))
            | H3 content -> h3 (content |> List.map (fun c -> c.ToValue()))
            | H4 content -> h4 (content |> List.map (fun c -> c.ToValue()))
            | H5 content -> h5 (content |> List.map (fun c -> c.ToValue()))
            | H6 content -> h6 (content |> List.map (fun c -> c.ToValue()))
            | P content -> p (content |> List.map (fun c -> c.ToValue()))
            | OL items -> ol (items |> List.map (fun i -> i |> List.map (fun c -> c.ToValue()) |> li))
            | UL items -> ul (items |> List.map (fun i -> i |> List.map (fun c -> c.ToValue()) |> li))
            | IMG(source, altText, title, height, width) -> img source title altText height width

    and Content =
        | Literal of Content: string
        | Link of Url: string * Content: string

        member c.ToValue() =
            match c with
            | Literal content -> text content
            | Link(url, content) -> link url content
