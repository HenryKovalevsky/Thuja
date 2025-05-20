module Thuja.DSL

open Thuja.Core
open Thuja.Elements

let columns ratios (subs : (Region -> Tree<IElement * Region>) list) (region : Region) : Tree<IElement * Region> =
  Tree (
    (Empty(), region), 
    Layout.columns ratios region
    |> Seq.zip subs
    |> Seq.map (fun (s, r) -> s r)
    |> Seq.toList
  )

let rows ratios (subs : (Region -> Tree<IElement * Region>) list) (region : Region) : Tree<IElement * Region> =
  Tree (
    (Empty(), region), 
    Layout.rows ratios region
    |> Seq.zip subs
    |> Seq.map (fun (s, r) -> s r)
    |> Seq.toList
  )

let panel (subs : (Region -> Tree<IElement * Region>) list) (region : Region) : Tree<IElement * Region> =
  Tree (
    ({ Border = Unchecked.defaultof<_> }, region), 
    subs |> List.map (fun s -> s region.Inner)
  )

let list items marked (region : Region) : Tree<IElement * Region> =
  Tree(
    ({ Items = items; Marked = marked }, region),
    []
  )

let text content (region : Region) : Tree<IElement * Region> =
  Tree(
    ({ Content = content }, region),
    []
  )
  
let raw proc model (region : Region) : Tree<IElement * Region> =
  Tree(
    ({ Model = model; Process = proc }, region),
    []
  )