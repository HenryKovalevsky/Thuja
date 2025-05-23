module Thuja.Core

open System
open System.Collections

open Tutu

type Region = 
  { X1: int; Y1: int
    X2: int; Y2: int }

type IRenderable =
  abstract Render: Region -> ICommand list

type IElement =
  inherit IRenderable
  inherit IStructuralEquatable

type ViewTree =
  | Tree of (IElement * Region) * ViewTree list

type KeyInput =
  | Char of char | FKey of int
  | Up | Down | Left | Right 
  | Enter | Backspace | Delete 
  | Tab | BackTab

[<Flags>]
type KeyModifiers =
  | Shift = 0b001 | Ctrl = 0b010 | Alt = 0b100

type ApplicationEvent<'msg> =
  | KeyboardInput of KeyInput * KeyModifiers
  | UserMessage of 'msg

type Cmd<'msg> = Async<'msg> list

type View<'model> = 'model -> Region -> ViewTree
type Update<'model, 'msg> = 'model -> ApplicationEvent<'msg> -> 'model * Cmd<'msg>

type Program<'model, 'msg> = 'model -> View<'model> -> Update<'model, 'msg> -> unit