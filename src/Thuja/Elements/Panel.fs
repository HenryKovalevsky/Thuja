namespace Thuja.Elements

open Thuja
open Thuja.View
open Thuja.Styles

open type Border.Line

type PanelProps =
  | BorderStyle of BorderStyle
  | BorderColor of Color

type internal Panel =
  { Props: PanelProps list }
  interface IElement with
    member this.Render (region : Region): Command list =
      // props
      let style = 
        this.Props 
        |> Seq.choose ^function | BorderStyle style -> Some style | _ -> None
        |> Seq.tryHead
        |> Option.defaultValue Normal

      let color = 
        this.Props 
        |> Seq.choose ^function | BorderColor color -> Some color | _ -> None
        |> Seq.tryHead
        |> Option.defaultValue Reset

      // lines
      let line symbol = Border.Styles.[style].[symbol].With color

      [ for x = region.X1 to region.X2 do
          yield! [ MoveTo (x, region.Y1)
                   PrintWith <| line Horizontal 
                   MoveTo (x, region.Y2)
                   PrintWith <| line Horizontal ]

        for y = region.Y1 to region.Y2 do  
          yield! [ MoveTo (region.X1, y)
                   PrintWith <| line Vertical 
                   MoveTo (region.X2, y)
                   PrintWith <| line Vertical ]

        yield! [ MoveTo (region.X1, region.Y1)
                 PrintWith <| line TopLeft 
                 MoveTo (region.X1, region.Y2)
                 PrintWith <| line BottomLeft 
                 MoveTo (region.X2, region.Y1)
                 PrintWith <| line TopRight 
                 MoveTo (region.X2, region.Y2)
                 PrintWith <| line BottomRight ] ]

[<AutoOpen>]
module Panel =
  let panel props (subs : (Region -> ViewTree) list) (region : Region) : ViewTree =
    let subs = subs |> List.map ^fun sub -> sub region.Inner

    ViewTree.create
      { Props = props }
      region
      subs