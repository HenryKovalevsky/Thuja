namespace Thuja.Elements

open Thuja
open Thuja.View

type internal Empty private () =
  interface IElement with
    member _.Render(_ : Region): Command list = 
      []
    member _.Equals(obj : obj, _): bool = 
      obj :? Empty
    member this.GetHashCode _: int = 
      hash this
  end
    static member singleton = Empty()

[<AutoOpen>]
module Empty =
  let empty (_ : Region) : ViewTree =
    ViewTree.create
      Empty.singleton
      Region.empty
      []