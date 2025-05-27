module Thuja.Backend.Tutu

open Thuja
open Thuja.Backend
open Thuja.Backend.Helpers
open Thuja.Backend.Mappings

open Tutu.Events 
open Tutu.Commands
open Tutu.Terminal

type TutuBackend private () =
  do Terminal.beginSession()

  let input = new Event<KeyInput * Backend.KeyModifiers>()
  do Async.Start(async {
    while true do
      match SystemEventReader.Instance.Read() with
      | KeyCodeEvent (key, modifiers) -> input.Trigger (key, modifiers)
      | _ -> ()
  })

  static member beginSession() : IBackend = new TutuBackend()

  interface IBackend with
    member _.TerminalSize : int * int = 
      SystemTerminal.Size.Width - 1, SystemTerminal.Size.Height - 1

    member _.Execute(commands : Command list) : unit = 
      commands
      |> Seq.map (function
          | MoveTo (x, y) -> Cursor.MoveTo(x, y)
          | Print content -> Style.Print content
          | PrintWith (style, content) -> Style.PrintStyledContent <| Mapper.map (style, content))
      |> Terminal.execute

      Terminal.execute [ Cursor.Hide ]
    
    member _.OnInput : IEvent<KeyInput * Backend.KeyModifiers> = input.Publish

    member _.Dispose() : unit = 
      do Terminal.endSession()
