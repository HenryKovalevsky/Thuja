[<RequireQualifiedAccess>]
module Menu

open System

open Thuja.Elements

type Model private =
  { Items: string list 
    Index: int }

type Model with
  member this.Selected with get() = this.Items.[this.Index]

let init items = 
  { Items = items
    Index = 0 }
  
type Msg =
  | Next
  | Previous

let update (model : Model) = function
  | Next -> { model with Index = Math.Min(model.Items.Length - 1, model.Index + 1) }
  | Previous -> { model with Index = Math.Max(0, model.Index - 1) }

let view (model : Model) = list model.Items model.Index