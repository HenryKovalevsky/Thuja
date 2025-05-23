module internal Thuja.Helpers

open System
open System.Collections

open Thuja.Core
 
open Tutu.Events 
open Tutu.Commands
open Tutu.Terminal
open Tutu.Extensions

type Region with
  member this.Width = this.X2 - this.X1
  member this.Height = this.Y2 - this.Y1
  member this.Inner =
    { X1 = this.X1 + 1; Y1 = this.Y1 + 1
      X2 = this.X2 - 1; Y2 = this.Y2 - 1 }
  static member create(x1, y1, x2, y2) : Region =
    { X1 = x1; Y1 = y1
      X2 = x2; Y2 = y2 }
  static member create(width, height) : Region =
    { X1 = 0;  Y1 = 0
      X2 = width; Y2 = height }
  static member empty : Region =
    { X1 = 0;  Y1 = 0
      X2 = 0; Y2 = 0 }

module ViewTree =
  let rec private toSeq (Tree (value, subtrees)) = seq {
    yield value
    for tree in subtrees do
      yield! toSeq tree
  }

  let difference (aTree : ViewTree) (bTree : ViewTree) : (IElement * Region) seq =
    toSeq aTree |> Seq.filter ^fun a -> 
      toSeq bTree |> Seq.cast<IStructuralEquatable> |> Seq.forall ^fun b -> 
        not <| b.Equals(a, StructuralComparisons.StructuralEqualityComparer)

module Console =
  let execute commands =
    commands
    |> Seq.toArray
    |> Console.Out.Execute
    |> ignore

module Terminal =
  let beginSession () =
    SystemTerminal.Instance.EnableRawMode()
    Console.execute [ 
      Terminal.EnterAlternateScreen
      Events.EnableMouseCapture
    ]

  let endSession () =
    Console.execute [
      Events.DisableMouseCapture
      Terminal.LeaveAlternateScreen 
    ]
    SystemTerminal.Instance.DisableRawMode()

[<AutoOpen>]
module Render =
  let render (element : IElement, region : Region) = element.Render region
  let clear (region : Region)  =
    [ for y in region.Y1..region.Y2 do
        yield Cursor.MoveTo (region.X1, y)
        yield Style.Print <| String.Empty.PadRight(region.Width, ' ') ]

[<AutoOpen>]
module Events =
  let private mapKey : KeyCode.IKeyCode -> KeyInput option = function
    | :? KeyCode.CharKeyCode as code -> Char code.Character.[0] |> Some 
    | :? KeyCode.FKeyCode as code ->    FKey code.Number |> Some 
    | :? KeyCode.UpKeyCode ->           Up |> Some 
    | :? KeyCode.DownKeyCode ->         Down |> Some 
    | :? KeyCode.LeftKeyCode ->         Left |> Some 
    | :? KeyCode.RightKeyCode ->        Right |> Some 
    | :? KeyCode.EnterKeyCode ->        Enter |> Some 
    | :? KeyCode.BackspaceKeyCode ->    Backspace |> Some 
    | :? KeyCode.TabKeyCode ->          Tab |> Some 
    | :? KeyCode.BackTabKeyCode ->      BackTab |> Some 
    | :? KeyCode.DeleteKeyCode ->       Delete |> Some 
    | _ -> None // todo: extend 

  let private mapModifiers : KeyModifiers -> Core.KeyModifiers =
    int >> enum<Core.KeyModifiers>

  let (|KeyCodeEvent|_|) (event : IEvent) =
    match event with
    | :? Event.KeyEventEvent as event when event.Event.Kind = KeyEventKind.Press ->
        mapKey event.Event.Code
        |> Option.map ^fun key ->
            let modifiers = mapModifiers event.Event.Modifiers 
            KeyCodeEvent (key, modifiers)
        
    | _ -> None