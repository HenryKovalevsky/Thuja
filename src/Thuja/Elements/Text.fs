namespace Thuja.Elements

open System

open Thuja.View
open Thuja.Backend
open Thuja.Elements.Helpers

type internal Text =
  { Content: string }
  interface IElement with
    member this.Render(region : Region): Command list = 
      let lines = 
        this.Content.Split(Environment.NewLine)
        |> Seq.mapi ^fun index line ->
            index, line.Truncate region.Width
        |> Seq.truncate region.Height

      [ for index, line in lines do
          yield MoveTo (region.X1, region.Y1 + index)
          yield Print line ]

[<AutoOpen>]
module Text = 
  let text content (region : Region) : ViewTree =
    Tree (
      ({ Content = content }, region),
      []
    )