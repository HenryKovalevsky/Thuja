namespace Thuja.Elements

open System
open System.Collections

open Thuja.Backend
open Thuja.View

type internal Panel =
  { Border: string } // todo: make border selection
  interface IElement with
    member this.Render (region : Region): Command list = 
      [ for x = region.X1 to region.X2 do
          yield! [ MoveTo (x, region.Y1)
                   Print "─"
                   MoveTo (x, region.Y2)
                   Print "─" ]

        for y = region.Y1 to region.Y2 do  
          yield! [ MoveTo (region.X1, y)
                   Print "│"
                   MoveTo (region.X2, y)
                   Print "│" ]

        yield! [ MoveTo (region.X1, region.Y1)
                 Print "┌"
                 MoveTo (region.X1, region.Y2)
                 Print "└"
                 MoveTo (region.X2, region.Y1)
                 Print "┐"
                 MoveTo (region.X2, region.Y2)
                 Print "┘" ] ]

[<AutoOpen>]
module Panel =
  let panel (subs : (Region -> ViewTree) list) (region : Region) : ViewTree =
    Tree (
      ({ Border = Unchecked.defaultof<_> }, region), 
      subs |> List.map (fun s -> s region.Inner)
    )
