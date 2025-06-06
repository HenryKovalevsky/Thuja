namespace Thuja

open System 

// style
type Color = 
  | Reset
  | Ansi of byte
  | Rgb of byte * byte * byte

type Style = 
  { Foreground: Color
    Background: Color }

// render
type Command =
  | MoveTo of x: int * y: int
  | Print of content: string
  | PrintWith of style: Style * content: string

// input
type KeyInput =
  | Char of char | FKey of int
  | Up | Down | Left | Right 
  | Enter | Backspace | Delete 
  | Tab | BackTab

[<Flags>]
type KeyModifiers =
  | Shift = 0b001 | Ctrl = 0b010 | Alt = 0b100

// backend
type IBackend =
  inherit IDisposable

  abstract TerminalSize : width: int * height: int with get
  abstract Execute: Command list -> unit
  abstract OnInput: IEvent<KeyInput * KeyModifiers>