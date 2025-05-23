module Thuja.DSL

open Thuja.Core
open Thuja.Elements
open Thuja.Helpers

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

let panel (subs : (Region -> ViewTree) list) (region : Region) : ViewTree =
  Tree (
    ({ Border = Unchecked.defaultof<_> }, region), 
    subs |> List.map (fun s -> s region.Inner)
  )

let list items marked (region : Region) : ViewTree =
  Tree (
    ({ Items = items; Marked = marked }, region),
    []
  )

let text content (region : Region) : ViewTree =
  Tree (
    ({ Content = content }, region),
    []
  )
  
let raw proc model (region : Region) : ViewTree =
  Tree (
    ({ Model = model; Process = proc }, region),
    []
  )