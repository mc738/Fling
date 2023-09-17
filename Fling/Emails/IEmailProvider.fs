namespace Fling.Emails

open FsToolbox.Core.Results


type IEmailProvider =
    
    abstract member SendSingleEmail: EmailMessage -> Async<ActionResult<string>>
    

