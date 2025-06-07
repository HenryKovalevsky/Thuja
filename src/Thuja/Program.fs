namespace Thuja

open System.Threading

open Thuja
open Thuja.View
open Thuja.Elements

type Cmd<'msg> = Async<'msg> list
type Sub<'msg> = ('msg -> unit) -> unit

[<RequireQualifiedAccess>]
module Cmd = 
  let none : Cmd<'msg>= []
  let ofMsg msg : Cmd<'msg> = [async.Return msg]
  let ofAsync (async : Async<'msg>) : Cmd<'msg> = [async]
  let ofFunc (func: unit -> 'msg) : Cmd<'msg> = [async.Delay (func >> async.Return)]

type Program<'model, 'msg> private = 
  { Backend: unit -> IBackend
    Model : 'model
    View : 'model -> Region -> ViewTree
    Update : 'model -> 'msg -> 'model * Cmd<'msg>
    KeyBindings: KeyInput * KeyModifiers -> Cmd<'msg>
    Subscriptions : Sub<'msg> list }

[<RequireQualifiedAccess>]
module Program =
  let private exitEvent = new AutoResetEvent false
  let exit() = ignore <| exitEvent.Set(); []

  let make (model : 'model) (view : 'model -> Region -> ViewTree) (update : 'model -> 'msg -> 'model * Cmd<'msg>) =
    { Backend = fun _ -> failwith "Backend is not provided."
      Model = model
      View = view
      Update = update
      KeyBindings = function | _ -> exit() // any key to exit
      Subscriptions = [] }

  let makeStatic (view : Region -> ViewTree) =
    make () (fun () -> view) (fun ()() -> (), Cmd.none)

  let withBackend<'backend, 'model, 'msg when 'backend : (new : unit -> 'backend) and 'backend :> IBackend> (program : Program<'model, 'msg>) =
    { program with Backend = fun _ -> new 'backend() }

  let withKeyBindings (bindings : KeyInput * KeyModifiers -> Cmd<'msg>) (program : Program<'model, 'msg>) =
    { program with KeyBindings = bindings }

  let withSubscriptions (subscriptions : Sub<'msg> list) (program : Program<'model, 'msg>) =
    { program with Subscriptions = subscriptions }

  let run (program : Program<'model, 'msg>) =

    use backend = program.Backend()
    let model, view, update = program.Model, program.View, program.Update

    // main loop
    use agent = MailboxProcessor<'msg>.Start ^fun inbox ->

      backend.OnInput.Add ^fun (key, modifiers) ->
        let cmd = program.KeyBindings (key, modifiers)
        async {
          try
            for task in cmd do
              let! msg = task
              inbox.Post msg
          with exc -> 
            backend.Dispose() // for graceful shutdown
            raise exc // reraise exception from the app
        } |> Async.Start

      for sub in program.Subscriptions do
        sub inbox.Post

      let rec loop state (model, cmd : Cmd<'msg>) = async {
        // update commands processing
        async {
          try
            for task in cmd do
              let! msg = task
              inbox.Post msg
          with exc -> 
            backend.Dispose() // for graceful shutdown
            raise exc // reraise exception from the app
        } |> Async.Start

        let width, height = backend.TerminalSize
        let region = Region.create(width, height)

        let viewTree = view model region

        // render
        backend.Execute [
          for element, region in ViewTree.difference viewTree state do
            yield! element.Render region
        ]

        let! msg = inbox.Receive()

        return! loop viewTree (update model msg)
      }

      let empty = ViewTree.create Empty.singleton Region.empty []
      loop empty (model, Cmd.none)

    // handle exceptions
    agent.Error.Add ^fun exc -> 
      backend.Dispose() // for graceful shutdown
      raise exc // reraise exception from the app

    // wait for exit event
    ignore <| exitEvent.WaitOne()

    backend.Dispose() // end