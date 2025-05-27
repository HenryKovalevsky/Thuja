namespace Thuja.Elements

open System

open Thuja.View
open Thuja.Backend
open Thuja.Elements.Helpers

type internal List =
  { Items: string list 
    Marked: int list }
  interface IElement with
    member this.Render(region : Region) : Command list = 
      if Seq.length this.Items > region.Height
      then raise <| NotImplementedException() // todo: design implementation

      let items =
        this.Items
        |> Seq.mapi ^fun index item ->
            index, item.TruncateWithEllipsis region.Width

      [ for index, item in items do
          yield MoveTo (region.X1, region.Y1 + index)
          yield 
            if Seq.contains index this.Marked 
            then PrintWith <| item.On DarkGrey
            else Print <| item ]

[<AutoOpen>]
module List = 
  let list items marked (region : Region) : ViewTree =
    Tree (
      ({ Items = items; Marked = marked }, region),
      []
    )