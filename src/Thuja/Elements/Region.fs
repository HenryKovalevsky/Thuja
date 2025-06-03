namespace Thuja.Elements

open Thuja.View
open Thuja.Elements.Helpers

type Align =
  | Top
  | Bottom
  | Right
  | Left
  | Center
  | TopRight
  | TopLeft
  | BottomRight
  | BottomLeft

type RegionProps =
  | Width of int
  | Height of int
  | Align of Align

[<AutoOpen>]
module Region =
  let region (props : RegionProps list) (subs : (Region -> ViewTree) list) (region : Region) : ViewTree =
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

    let x1, y1 = 
      match align with 
      | Center -> region.X1 + (region.Width - width) / 2 - 1, region.Y1 + (region.Height - height) / 2 - 1
      | Top -> region.X1 + (region.Width - width) / 2 - 1, region.Y1
      | Bottom -> region.X1 + (region.Width - width) / 2 - 1, region.Y2 - height + 1
      | Left -> region.X1, region.Y1 + (region.Height - height) / 2 - 1
      | Right -> region.X1 + (region.Width - width) / 2 - 1, region.Y1 + (region.Height - height) / 2 - 1
      | TopRight -> region.X2 - width + 1, region.Y1
      | TopLeft -> region.X1, region.Y1
      | BottomRight -> region.X2 - width + 1, region.Y2 - height + 1
      | BottomLeft -> region.X1, region.Y2 - height + 1

    let x2, y2 = x1 + width - 1, y1 + height - 1

    let x1, y1 = max x1 region.X1, max y1 region.Y1
    let x2, y2 = min x2 region.X2, min y2 region.Y2

    let region = Region.create(x1, y1, x2, y2)
    let subs = subs |> List.map ^fun sub -> sub region

    ViewTree.create 
      Empty.singleton
      Region.empty
      subs