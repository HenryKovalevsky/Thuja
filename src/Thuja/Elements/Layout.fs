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