module internal Thuja.Helpers

open System
open System.Collections

open Thuja.Core

open Tutu.Extensions

module Tree =
  let rec traverse (Tree (value, subtrees)) = seq {
    yield value
    for tree in subtrees do
      yield! traverse tree
  }

module State =
  let getState view model dispatch region = 
    view model dispatch region 
    |> Tree.traverse 
    |> Seq.toList

  let difference<'item when 'item :> IStructuralEquatable> (a : 'item seq) (b : 'item seq) =
    a |> Seq.filter ^fun i -> 
      b |> Seq.forall ^fun j -> not <| j.Equals(i, StructuralComparisons.StructuralEqualityComparer)
    

module Console =
  let execute commands =
    commands
    |> Seq.toArray
    |> Console.Out.Execute
    |> ignore
