module internal Thuja.Backend.Helpers

open Thuja
open Thuja.Backend.Mappings

open Tutu.Events 
open Tutu.Commands
open Tutu.Terminal
open Tutu.Extensions

let (|KeyCodeEvent|_|) (event : IEvent) =
  match event with
  | :? Event.KeyEventEvent as event when event.Event.Kind = KeyEventKind.Press ->
  
      let key : KeyInput option = Mapper.map event.Event.Code
      let modifiers : Backend.KeyModifiers option = Mapper.map event.Event.Modifiers

      (key, modifiers)
      ||> Option.map2 (fun key modifiers -> KeyCodeEvent (key, modifiers)) 

  | _ -> None

module Terminal =
  let execute commands =
    commands
    |> Seq.toArray
    |> System.Console.Out.Execute
    |> ignore

  let beginSession () =
    SystemTerminal.Instance.EnableRawMode()
    execute [ 
      Terminal.EnterAlternateScreen
      Events.EnableMouseCapture
      Cursor.Hide
    ]

  let endSession () =
    execute [
      Cursor.Show
      Events.DisableMouseCapture
      Terminal.LeaveAlternateScreen 
    ]
    SystemTerminal.Instance.DisableRawMode()