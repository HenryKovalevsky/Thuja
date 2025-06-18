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
  | Attributes of Attribute list
  | Overflow of Overflow
  | TextAlign of Align

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
        |> Option.defaultValue Color.Reset

      let background = 
        this.Props 
        |> Seq.choose ^function | Background color -> Some color | _ -> None
        |> Seq.tryHead
        |> Option.defaultValue Color.Reset
        
      let attributes = 
        this.Props 
        |> Seq.choose ^function | Attributes attributes -> Some attributes | _ -> None
        |> Seq.tryHead
        |> Option.defaultValue []

      let overflow = 
        this.Props 
        |> Seq.choose ^function | Overflow overflow -> Some overflow | _ -> None
        |> Seq.tryHead
        |> Option.defaultValue Clip

      let align = 
        this.Props 
        |> Seq.choose ^function | TextAlign align -> Some align | _ -> None
        |> Seq.tryHead
        |> Option.defaultValue Align.TopLeft

      // content
      let lines = 
        if overflow = Wrap then
          this.Content.Wrap region.Width
        else
          this.Content.Split Environment.NewLine

      let lines = 
        lines
        |> Seq.map ^fun line ->
            match align with // horizontal alignment
            | Left | TopLeft | BottomLeft -> 
                String.truncate (overflow = Ellipsis) region.Width line

            | Center | Top | Bottom -> 
                let margin = Math.Max((region.Width - line.Length) / 2 - 1, 0)
                let gap = String.replicate margin " "
                let line = gap + line

                String.truncate (overflow = Ellipsis) region.Width line
            
            | Right | TopRight | BottomRight -> 
                let margin = Math.Max(region.Width - line.Length + 1, 0)
                let gap = String.replicate margin " "
                let line = gap + line
                
                line
                |> String.rev
                |> String.truncate (overflow = Ellipsis) region.Width
                |> String.rev
                
        |> Seq.truncate region.Height

      let lines =
        match align with // verticals alignment
        | TopLeft | Top| TopRight -> 
            lines

        | Left | Center | Right -> 
            let margin = Math.Max((region.Height - Seq.length lines) / 2, 0)
            let gap = Seq.init margin ^fun _ -> String.Empty.Truncate region.Width
            Seq.concat [ gap; lines ]

        | BottomLeft | Bottom | BottomRight ->
            let margin = Math.Max(region.Height - Seq.length lines, 0)
            let gap = Seq.init margin ^fun _ -> String.Empty.Truncate region.Width
            Seq.concat [ gap; lines ]

      [ for index, line in Seq.indexed lines do
          yield MoveTo (region.X1, region.Y1 + index)
          yield PrintWith <| line.Styled(color, background, attributes) ]

[<AutoOpen>]
module Text = 
  let text props content (region : Region) : ViewTree =  
    ViewTree.create
      { Props = props; Content = content }
      region
      []