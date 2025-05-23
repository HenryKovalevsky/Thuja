namespace Thuja

open System
open System.Threading

open Thuja.Core
open Thuja.Helpers

open Tutu.Events
open Tutu.Commands
open Tutu.Terminal

[<RequireQualifiedAccess>]
module Cmd = 
  let none : Cmd<'msg>= []
  let ofMsg msg : Cmd<'msg> = [async.Return msg]
  let ofAsync (async : Async<'msg>) : Cmd<'msg> = [async]
  let ofFunc (func: unit -> 'msg) : Cmd<'msg> = [async.Delay (func >> async.Return)]

[<RequireQualifiedAccess>]
module Program =
  let private exitEvent = new AutoResetEvent false
  let exit() = ignore <| exitEvent.Set(); Cmd.none

  let run (init : 'model) (view : View<'model>) (update : Update<'model, 'msg>) =
    // begin
    do Terminal.beginSession()

    // main loop
    use agent = MailboxProcessor<ApplicationEvent<'msg>>.Start ^fun inbox ->
      // user input processor
      async {
        while true do
          match SystemEventReader.Instance.Read() with // todo: make overall user input processing
          | KeyCodeEvent (key, modifiers) -> inbox.Post <| KeyboardInput (key, modifiers)
          | _ -> ()

      } |> Async.Start 
       
      let region = Region.create(SystemTerminal.Size.Width - 1, SystemTerminal.Size.Height - 1)

      let rec loop state (model, cmd : Cmd<'msg>) = async {
        // update commands processing
        async {
          try
            for task in cmd do
              let! msg = task
              inbox.Post <| UserMessage msg
          with exc -> 
            do Terminal.endSession() // for graceful shutdown
            raise exc // reraise exception from the app
        } |> Async.Start

        let viewTree = view model region

        // render
        Console.execute [
          for element, region in ViewTree.difference viewTree state do
            yield! clear region
            yield! element.Render region
        ]

        Console.execute [ Cursor.Hide ]

        let! msg = inbox.Receive()

        return! loop viewTree (update model msg)
      }

      let empty = Tree ((Elements.Empty.singleton, Region.empty), [])
      loop empty (init, Cmd.none)

    // handle exceptions
    agent.Error.Add ^fun exc -> 
      do Terminal.endSession() // for graceful shutdown
      raise exc // reraise exception from the app

    // wait for exit event
    ignore <| exitEvent.WaitOne()

    do Terminal.endSession() // end