namespace Thuja.Elements

open Thuja
open Thuja.View
open Thuja.Elements

[<AutoOpen>]
module Layout = 
  let columns ratios (subs : (Region -> ViewTree) list) (region : Region) : ViewTree =
    Tree (
      (Empty.singleton, Region.empty), 
      Layout.columns ratios region
      |> Seq.zip subs
      |> Seq.map (fun (s, r) -> s r)
      |> Seq.toList
    )

  let rows ratios (subs : (Region -> ViewTree) list) (region : Region) : ViewTree =
    Tree (
      (Empty.singleton, Region.empty), 
      Layout.rows ratios region
      |> Seq.zip subs
      |> Seq.map (fun (s, r) -> s r)
      |> Seq.toList
    )

  let center (width, height) (subs : (Region -> ViewTree) list) (region : Region) : ViewTree =
    let x1, y1 = (region.Width - width) / 2, (region.Height - height) / 2
    let x2, y2 = x1 + width - 1, y1 + height - 1

    let region = Region.create(x1, y1, x2, y2)
    
    Tree (
      (Empty.singleton, Region.empty), 
      subs |> List.map (fun s -> s region)
    )