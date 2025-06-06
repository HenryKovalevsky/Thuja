namespace Thuja.Elements

open System

open Thuja
open Thuja.View
open Thuja.Styles
open Thuja.Elements.Helpers

type TextProps =
  | Color of Color
  | Background of Color

type internal Text =
  { Props: TextProps list
    Content: string }
  interface IElement with
    member this.Render(region : Region): Command list = 
      let color = 
        this.Props 
        |> Seq.choose ^function | Color color -> Some color | _ -> None
        |> Seq.tryHead
        |> Option.defaultValue Reset

      let background = 
        this.Props 
        |> Seq.choose ^function | Background color -> Some color | _ -> None
        |> Seq.tryHead
        |> Option.defaultValue Reset
        
      let lines = 
        this.Content.Split Environment.NewLine
        |> Seq.mapi ^fun index line ->
            index, line.Truncate region.Width
        |> Seq.truncate region.Height

      [ for index, line in lines do
          yield MoveTo (region.X1, region.Y1 + index)
          yield PrintWith <| line.Styled(color, background) ]

[<AutoOpen>]
module Text = 
  let text props content (region : Region) : ViewTree =  
    ViewTree.create
      { Props = props; Content = content }
      region
      []