namespace Thuja.Elements

open Thuja.View
open Thuja.Elements
open Thuja.Elements.Helpers

[<AutoOpen>]
module Layout = 
  let columns ratios (subs : (Region -> ViewTree) list) (region : Region) : ViewTree =
    let subs = 
      Layout.columns ratios region
      |> Seq.zip subs
      |> Seq.map ^fun (sub, region) -> sub region
      |> Seq.toList

    ViewTree.create
      Empty.singleton
      Region.empty
      subs
      
  let rows ratios (subs : (Region -> ViewTree) list) (region : Region) : ViewTree =
    let subs =
      Layout.rows ratios region
      |> Seq.zip subs
      |> Seq.map ^fun (sub, region) -> sub region
      |> Seq.toList

    ViewTree.create
      Empty.singleton
      Region.empty
      subs