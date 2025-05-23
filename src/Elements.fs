module internal Thuja.Elements

open System
open System.Collections

open Thuja.Core
open Thuja.Helpers

open Tutu
open Tutu.Commands
open Tutu.Style.Extensions

type internal String with
  member this.TruncateWithEllipsis (length : int)=
    if this.Length > length
    then this[..length - 1] + "⋯"
    else this.PadRight (length + 1)

type Panel =
  { Border: string } // todo: make border style selection
  interface IElement with
    member this.Render (region : Region): ICommand list = 
      [ for x = region.X1 to region.X2 do
          yield! [ Cursor.MoveTo (x, region.Y1)
                   Style.Print '─'
                   Cursor.MoveTo (x, region.Y2)
                   Style.Print '─' ]

        for y = region.Y1 to region.Y2 do  
          yield! [ Cursor.MoveTo (region.X1, y)
                   Style.Print '│'
                   Cursor.MoveTo (region.X2, y)
                   Style.Print '│' ]

        yield! [ Cursor.MoveTo (region.X1, region.Y1)
                 Style.Print '┌'
                 Cursor.MoveTo (region.X1, region.Y2)
                 Style.Print '└'
                 Cursor.MoveTo (region.X2, region.Y1)
                 Style.Print '┐'
                 Cursor.MoveTo (region.X2, region.Y2)
                 Style.Print '┘' ] ]

type List =
  { Items: string list 
    Marked: int list }
  interface IElement with
    member this.Render(region : Region) : ICommand list = 
      if Seq.length this.Items > region.Height
      then raise <| NotImplementedException() // todo: design implementation

      let items =
        this.Items
        |> Seq.mapi ^fun index item ->
            index, item.TruncateWithEllipsis region.Width

      [ for index, item in items do
          yield Cursor.MoveTo (region.X1, region.Y1 + index)
          yield 
            if Seq.contains index this.Marked 
            then Style.PrintStyledContent <| item.OnDarkGrey()
            else Style.Print <| item ]

type Text =
  { Content: string }
  interface IElement with
    member this.Render(region : Region): ICommand list = 
      let lines = 
        this.Content.Split(Environment.NewLine)
        |> Seq.mapi ^fun index line ->
            index, line.TruncateWithEllipsis region.Width

      if Seq.length lines > region.Height
      then raise <| NotImplementedException() // todo: design implementation

      [ for index, line in lines do
          yield Cursor.MoveTo (region.X1, region.Y1 + index)
          yield Style.Print line ]

type Raw<'model when 'model : equality> =
  { Model: 'model
    Process: 'model -> int * int -> string }
  interface IElement with
    member this.Render(region : Region): ICommand list = 
      [ yield Cursor.MoveTo (region.X1, region.Y1)
        yield Style.Print <| this.Process this.Model (region.Width, region.Height) ]
    member this.Equals(obj : obj, comparer : IEqualityComparer): bool = 
      match obj with
      | :? Raw<'model> as other -> comparer.Equals(this.Model, other.Model)
      | _ -> false
    member this.GetHashCode(_): int = 
      hash this.Model

type Empty private () =
  interface IElement with
    member _.Render(region : Region): ICommand list = 
      []
    member _.Equals(obj : obj, _): bool = 
      obj :? Empty
    member this.GetHashCode(_): int = 
      hash this
  end
    static member singleton = Empty()
