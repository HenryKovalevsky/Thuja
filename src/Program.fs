namespace Thuja

open System
open System.Text
open System.Threading

open Thuja.Core
open Thuja.Helpers

open Tutu.Commands
open Tutu.Events
open Tutu.Terminal

[<RequireQualifiedAccess>]
type Cmd<'msg> = // todo: make more elm-like cmd definition
  | none
  | ofMsg of 'msg
  | ofAsync of (unit -> Async<'msg>)
  | ofFunc of (unit -> 'msg)
  | exit

type ApplicationEvent<'msg> =
  | KeyEvent of string // todo: make comprehensive user input handling
  | UserEvent of 'msg

// todo: consider using elmish
type View<'model, 'msg> = 'model -> ('msg -> unit) -> Region -> Tree<IElement * Region>
type Update<'model, 'msg> = ApplicationEvent<'msg> -> 'model -> 'model * Cmd<'msg>

[<RequireQualifiedAccess>]
module Program =
  let private exitEvent = new AutoResetEvent false
  let private exit() = ignore <| exitEvent.Set()

  let run (init : 'model) (view : View<'model, 'msg>) (update : Update<'model, 'msg>) =
    // begin
    Console.OutputEncoding <- Encoding.UTF8 // todo: find out if it's necessary???
    SystemTerminal.Instance.EnableRawMode()

    Console.execute [ 
      Terminal.EnterAlternateScreen
      Cursor.Hide
    ]
    
    // main loop
    use agent = MailboxProcessor<ApplicationEvent<'msg>>.Start ^fun inbox ->
      // user input processor
      async {
        while true do
          match SystemEventReader.Instance.Read() with // todo: make overall user input processing
          | :? Event.KeyEventEvent as event when (event.Event.Code :? KeyCode.CharKeyCode) ->
                let code = event.Event.Code :?> KeyCode.CharKeyCode 
                if event.Event.Kind = KeyEventKind.Press
                then inbox.Post <| KeyEvent code.Character
          | _ -> ()
      } |> Async.Start 

      let region = Region.create(SystemTerminal.Size.Width, SystemTerminal.Size.Height)

      let dispatch msg = inbox.Post <| UserEvent msg
      let render (element : IElement, region : Region) = element.Render region

      let rec loop state (model, cmd) = async {
        match cmd with 
        | Cmd.exit -> return exit()
        | Cmd.ofFunc func -> func() |> dispatch
        | Cmd.ofAsync func -> async.Bind(func(), dispatch >> async.Return) |> Async.Start // todo: make better exception handling
        | Cmd.ofMsg msg -> msg |> dispatch
        | Cmd.none -> ()

        let newState = State.getState view model dispatch region

        // render
        Console.execute [
          for diff in State.difference newState state do
            yield! render diff
        ]

        Console.execute [ Cursor.Hide ] // todo: not sure

        let! msg = inbox.Receive()

        return! loop newState (update msg model)
      }

      loop [] (init, Cmd.none)

    // handle exceptions
    agent.Error.Add ^fun exc -> 
      // for graceful shutdown
      Console.execute [ 
        Cursor.Show
        Events.EnableMouseCapture
        Terminal.LeaveAlternateScreen
      ]

      SystemTerminal.Instance.DisableRawMode()

      // reraise exception from the app
      raise exc

    // wait for exit event
    ignore <| exitEvent.WaitOne()

    // end
    Console.execute [ 
      Cursor.Show
      Events.EnableMouseCapture
      Terminal.LeaveAlternateScreen
    ]

    SystemTerminal.Instance.DisableRawMode()