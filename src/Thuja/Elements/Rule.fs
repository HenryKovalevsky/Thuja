namespace Thuja.Elements

open Thuja
open Thuja.View
open Thuja.Styles

type RuleProps =
  | RuleStyle of BorderStyle
  | RuleColor of Color

type internal RuleType =
  | Horizontal
  | Vertical

type internal Rule =
  { Props: RuleProps list
    Type: RuleType }
  interface IElement with
    member this.Render (region : Region): Command list =
      // props
      let style = 
        this.Props 
        |> Seq.choose ^function | RuleStyle style -> Some style | _ -> None
        |> Seq.tryHead
        |> Option.defaultValue Normal

      let color = 
        this.Props 
        |> Seq.choose ^function | RuleColor color -> Some color | _ -> None
        |> Seq.tryHead
        |> Option.defaultValue Reset

      // lines
      let line symbol = Border.Styles.[style].[symbol].With color

      match this.Type with
      | Horizontal ->
        [ for x = region.X1 to region.X2 do
            yield! [ MoveTo (x, region.Y1)
                     PrintWith <| line Border.Line.Horizontal 
                     MoveTo (x, region.Y2)
                     PrintWith <| line Border.Line.Horizontal ] ]
      | Vertical ->
        [ for y = region.Y1 to region.Y2 do  
            yield! [ MoveTo (region.X1, y)
                     PrintWith <| line Border.Line.Vertical 
                     MoveTo (region.X2, y)
                     PrintWith <| line Border.Line.Vertical ] ]

[<AutoOpen>]
module Rule =
  let hr props (region : Region) : ViewTree =
    ViewTree.create
      { Props = props; Type = Horizontal }
      region
      []

  let vr props (region : Region) : ViewTree =
    ViewTree.create
      { Props = props; Type = Vertical }
      region
      []