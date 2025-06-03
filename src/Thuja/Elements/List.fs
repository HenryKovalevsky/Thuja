namespace Thuja.Elements

open System

open Thuja.View
open Thuja.Backend
open Thuja.Elements.Helpers

type internal List =
  { Items: string list 
    Index: int }
  interface IElement with
    member this.Render(region : Region) : Command list = 
      let page = this.Index / region.Height

      let items =
        this.Items
        |> Seq.map ^fun item ->
            item.TruncateWithEllipsis region.Width
        |> Seq.skip (page * region.Height)
        |> Seq.truncate region.Height

      [ for index = 0 to region.Height - 1 do
          yield MoveTo (region.X1, region.Y1 + index)
          yield
            match Seq.tryItem index items with
            | Some item ->
                if this.Index = page * region.Height + index
                then PrintWith <| item.On DarkGrey
                else Print <| item 
            | None ->
                Print <| String.Empty.Truncate region.Width ] // clear 

[<AutoOpen>]
module List = 
  let list items index (region : Region) : ViewTree =
    ViewTree.create 
      { Items = items; Index = index }
      region
      []