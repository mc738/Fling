namespace Fling.Service


open System.IO
open System.Text.Json
open System.Threading.Tasks
open Fling.Emails
open Fling.Service.Agent
open Fling.Store.Common
//open Microsoft.Extensions.DependencyInjection
//open Microsoft.Extensions.Diagnostics.HealthChecks
open Microsoft.Extensions.Logging

type CommsService(store: IFlingStore, provider: IEmailProvider, log: ILogger<CommsService>) =

    let agent = Agent.start store log provider

    member _.SendEmail(subscriptionId, userId, email) =
        ({ SubscriptionId = subscriptionId
           UserId = userId
           Email = email }: QueueEmailRequest)
        |> Request.QueueEmail
        |> agent.Post

    member _.SendTemplatedEmail(subscriptionId, userId, email) =
        ({ SubscriptionId = subscriptionId
           UserId = userId
           Email = email }: QueueTemplatedEmailRequest)
        |> Request.QueueTemplatedEmail
        |> agent.Post

    member _.Test() =
        agent.PostAndReply(Request.Test, timeout = 5000)

    member _.Ping() =
        agent.PostAndReply(Request.Ping, timeout = 5000)

(*
module HealthChecks =

    type CommsServiceConnectHealthCheck(commsService: CommsService) =

        interface IHealthCheck with

            member this.CheckHealthAsync(context, cancellationToken) =
                let description = "Reports unhealthy if comms service connection check fails."

                let result =
                    match commsService.Test() with
                    | ActionResult.Success _ -> HealthCheckResult(HealthStatus.Healthy, description)
                    | ActionResult.Failure f ->
                        match f.Exception with
                        | Some e -> HealthCheckResult(HealthStatus.Unhealthy, description, e)
                        | None -> HealthCheckResult(HealthStatus.Unhealthy, description)

                Task.FromResult(result)

    type CommsServiceAgentHealthCheck(commsService: CommsService) =

        interface IHealthCheck with

            member this.CheckHealthAsync(context, cancellationToken) =
                let _ = commsService.Ping()
                Task.FromResult(HealthCheckResult.Healthy("Reports unhealthy if comms service ping fails."))

    type CommsStoreConnectHealthCheck(commsStore: CommsStore) =

        interface IHealthCheck with

            member this.CheckHealthAsync(context, cancellationToken) =
                let description = "Reports unhealthy if local comms store connection check fails."

                let result =
                    match commsStore.TestConnection() with
                    | ActionResult.Success _ -> HealthCheckResult(HealthStatus.Healthy, description)
                    | ActionResult.Failure f ->
                        match f.Exception with
                        | Some e -> HealthCheckResult(HealthStatus.Unhealthy, description, e)
                        | None -> HealthCheckResult(HealthStatus.Unhealthy, description)

                Task.FromResult(result)

open HealthChecks

type CommsServiceConfiguration =
    { StorePath: string
      EmailService: EmailService }

    static member Load(path: string) =
        try
            let el = File.ReadAllText path |> fun json -> (JsonDocument.Parse json).RootElement

            let unwrap (opt: 'T option) =
                opt |> Option.defaultWith (fun _ -> failwith "Missing property")

            { StorePath = Json.tryGetStringProperty "storePath" el |> unwrap
              EmailService =
                Json.tryGetProperty "emailService" el
                |> unwrap
                |> fun es ->
                    Json.tryGetStringProperty "type" es
                    |> unwrap
                    |> fun est ->
                        match est.ToLower() with
                        | "sendgrid" ->
                            Json.tryGetStringProperty "token" es
                            |> unwrap
                            |> fun t -> EmailService.SendGrid { Token = t }
                        | _ -> failwith $"Unknown email service: `{est}`"

            }
            |> Ok
        with exn ->
            Error $"Failed to load comms configuration. Error: {exn.Message}"

[<AutoOpen>]
module Extensions =

    type IServiceCollection with

        member sc.AddCommsService(cfg: CommsServiceConfiguration) =
            sc
                .AddSingleton<EmailService>(fun _ -> cfg.EmailService)
                .AddSingleton<CommsStoreContext>(fun _ -> CommsStoreContext.Create cfg.StorePath)
                .AddSingleton<CommsStore>()
                .AddSingleton<CommsService>()
                .AddSingleton<CommsService>()

    type IHealthChecksBuilder with

        member hcb.AddCommsServiceHealthChecks() =
            hcb
                .AddCheck<CommsServiceConnectHealthCheck>(
                    "comms-service-connection",
                    HealthStatus.Unhealthy,
                    [| "comms"; "database"; "basic" |]
                )
                .AddCheck<CommsServiceAgentHealthCheck>(
                    "comms-service-agent",
                    HealthStatus.Unhealthy,
                    [| "comms"; "database"; "basic" |]
                )

        member hcb.AddCommsStoreHealthChecks() =
            hcb.AddCheck<CommsStoreConnectHealthCheck>(
                "comms-store-connection",
                HealthStatus.Unhealthy,
                [| "comms_store"; "database"; "basic" |]
            )
*)

