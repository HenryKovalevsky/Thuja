module Thuja.Core

open System.Collections

open Tutu

type Tree<'a> =
  | Tree of 'a * Tree<'a> list

type Region = 
  { X1: int
    Y1: int
    X2: int 
    Y2: int }

type IRenderable =
  abstract Render: Region -> ICommand list // todo: make general commands instead of Tutu

type IElement =
  inherit IRenderable
  inherit IStructuralEquatable
  
type Region with
  member this.Width = this.X2 - this.X1
  member this.Height = this.Y2 - this.Y1
  member this.Inner =
    { X1 = this.X1 + 1
      Y1 = this.Y1 + 1
      X2 = this.X2 - 1 
      Y2 = this.Y2 - 1 }
  static member create(x1, y1, x2, y2) : Region =
    { X1 = x1
      Y1 = y1
      X2 = x2
      Y2 = y2 }
  static member create(width, height) : Region =
    { X1 = 0
      Y1 = 0
      X2 = width
      Y2 = height }