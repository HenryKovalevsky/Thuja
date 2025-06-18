namespace Thuja.Elements

open Thuja.View
open Thuja.Styles

type Margin =
  | Top of size: int
  | Bottom of size: int
  | Right of size: int
  | Left of size: int

type RegionProps =
  | Width of size: int
  | Height of size: int
  | Align of Align
  | Margin of Margin

[<AutoOpen>]
module Region =
  let region (props : RegionProps list) (subs : (Region -> ViewTree) list) (region : Region) : ViewTree =
    // props
    let width = 
      props 
      |> Seq.choose ^function | Width width -> Some width | _ -> None
      |> Seq.tryHead
      |> Option.defaultValue region.Width

    let height = 
      props 
      |> Seq.choose ^function | Height height -> Some height | _ -> None
      |> Seq.tryHead
      |> Option.defaultValue region.Height

    let align = 
      props 
      |> Seq.choose ^function | Align align -> Some align | _ -> None
      |> Seq.tryHead
      |> Option.defaultValue TopLeft

    let margins = 
      props 
      |> Seq.choose ^function | Margin margin -> Some margin | _ -> None

    // margin
    let region =
      margins
      |> Seq.fold (fun region margin ->
          match margin with
          | Left size -> { region with X1 = region.X1 + size }
          | Top size -> { region with Y1 = region.Y1 + size }
          | Right size -> { region with X2 = region.X2 - size }
          | Bottom size -> { region with Y2 = region.Y2 - size })
        region

    // alignment
    let x1, y1 = 
      match align with 
      | Center -> region.X1 + (region.Width - width) / 2 - 1, region.Y1 + (region.Height - height) / 2 - 1

      | Align.Left -> region.X1, region.Y1 + (region.Height - height) / 2 - 1
      | Align.Top -> region.X1 + (region.Width - width) / 2 - 1, region.Y1
      | Align.Bottom -> region.X1 + (region.Width - width) / 2 - 1, region.Y2 - height + 1
      | Align.Right -> region.X2 - width + 1, region.Y1 + (region.Height - height) / 2 - 1

      | TopLeft -> region.X1, region.Y1
      | TopRight -> region.X2 - width + 1, region.Y1
      | BottomLeft -> region.X1, region.Y2 - height + 1
      | BottomRight -> region.X2 - width + 1, region.Y2 - height + 1

    let x2, y2 = x1 + width - 1, y1 + height - 1

    // region borders control
    let x1, y1 = max x1 region.X1, max y1 region.Y1
    let x2, y2 = min x2 region.X2, min y2 region.Y2

    // view tree
    let region = Region.create(x1, y1, x2, y2)
    let subs = subs |> List.map ^fun sub -> sub region

    ViewTree.create 
      Empty.singleton
      Region.empty
      subs