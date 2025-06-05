namespace Thuja.Elements

open Thuja.View
open Thuja.Elements

type LayoutProps = 
  | Fixed of size: int 
  | Ratio of value: int

[<AutoOpen>]
module Layout = 
  let columns (props : LayoutProps list) (subs : (Region -> ViewTree) list) (region : Region) : ViewTree =
    // props
    let ratios = 
      props
      |> Seq.choose ^function | Ratio value -> Some value | _ -> None
    
    let fixedSize =
      props
      |> Seq.choose ^function | Fixed size -> Some size | _ -> None
      |> Seq.sum

    // calculations
    let total = Seq.sum ratios
    let width = region.Width - fixedSize

    let calculateSize = function
      | Ratio ratio -> width * ratio/total
      | Fixed size -> size

    let regions =
      props
      |> Seq.scan (fun left prop -> left + calculateSize prop) region.X1
      |> Seq.pairwise
      |> Seq.map ^fun (left, right) -> Region.create (left, region.Y1, right-1, region.Y2)
      |> Seq.toList

    // view tree
    let subs = 
      regions
      |> Seq.zip subs
      |> Seq.map ^fun (sub, region) -> sub region
      |> Seq.toList

    ViewTree.create
      Empty.singleton
      Region.empty
      subs
      
  let rows (props : LayoutProps list) (subs : (Region -> ViewTree) list) (region : Region) : ViewTree =
    // props
    let ratios = 
      props
      |> Seq.choose ^function | Ratio value -> Some value | _ -> None

    let fixedSize =
      props
      |> Seq.choose ^function | Fixed size -> Some size | _ -> None
      |> Seq.sum

    // calculations
    let total = Seq.sum ratios
    let height = region.Height - fixedSize

    let calculateSize = function
      | Ratio ratio -> height * ratio/total
      | Fixed size -> size

    let regions =
      props
      |> Seq.scan (fun top prop -> top + calculateSize prop) region.Y1
      |> Seq.pairwise
      |> Seq.map ^fun (top, bottom) -> Region.create (region.X1, top, region.X2, bottom-1)
      |> Seq.toList

    // view tree
    let subs =
      regions
      |> Seq.zip subs
      |> Seq.map ^fun (sub, region) -> sub region
      |> Seq.toList

    ViewTree.create
      Empty.singleton
      Region.empty
      subs

  let grid columnsProps rowsProps (grid : (Region -> ViewTree) list list) (region : Region) : ViewTree =
    rows rowsProps [
      for subs in grid do
        yield columns columnsProps subs
    ] region
