namespace Thuja.Elements

open System

open Thuja
open Thuja.View
open Thuja.Styles
open Thuja.Elements.Helpers

type Overflow =
  | Wrap
  | Clip
  | Ellipsis

type TextProps =
  | Color of Color
  | Background of Color
  | Overflow of Overflow

type internal Text =
  { Props: TextProps list
    Content: string }
  interface IElement with
    member this.Render(region : Region): Command list = 
      // props
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
        
      let overflow = 
        this.Props 
        |> Seq.choose ^function | Overflow overflow -> Some overflow | _ -> None
        |> Seq.tryHead
        |> Option.defaultValue Clip

      // content
      let lines = 
        this.Content.Split Environment.NewLine
        |> Seq.map ^fun line ->
            match overflow with
            | Ellipsis -> [ line.TruncateWithEllipsis region.Width ]
            | Clip -> [ line.Truncate region.Width ]
            | Wrap -> // todo: implement smart words wrapping
                line.ToCharArray()
                |> Seq.chunkBySize region.Width
                |> Seq.map String
                |> Seq.map ^fun line -> line.Truncate region.Width
                |> Seq.toList
        |> Seq.collect id
        |> Seq.indexed
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