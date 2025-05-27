namespace Thuja.Elements

open System
open System.Collections

open Thuja.Backend
open Thuja.View

type internal Raw<'model when 'model : equality> =
  { Model: 'model
    Process: 'model -> int * int -> string }
  interface IElement with
    member this.Render(region : Region): Command list = 
      [ for y in region.Y1..region.Y2 do
          yield MoveTo (region.X1, y)
          yield Print <| String.Empty.PadRight(region.Width, ' ') // clear

        yield MoveTo (region.X1, region.Y1)
        yield Print <| this.Process this.Model (region.Width, region.Height) ]
    member this.Equals(obj : obj, comparer : IEqualityComparer): bool = 
      match obj with
      | :? Raw<'model> as other -> comparer.Equals(this.Model, other.Model)
      | _ -> false
    member this.GetHashCode(_): int = 
      hash this.Model

[<AutoOpen>]
module Raw = 
  let raw proc (model : 'model) (region : Region) : ViewTree =
    Tree (
      ({ Model = model; Process = proc }, region),
      []
    )