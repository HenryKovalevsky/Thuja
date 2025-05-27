module Thuja.Backend

open System 

// style
type Color = 
  | Reset
  | Black
  | DarkGrey
  | Red
  | DarkRed
  | Green
  | DarkGreen
  | Yellow
  | DarkYellow
  | Blue
  | DarkBlue
  | Magenta
  | DarkMagenta
  | Cyan
  | DarkCyan
  | White
  | Grey

type Style = 
  { Foreground: Color
    Background: Color }

type String with
  member this.With(color : Color) =
    { Foreground =  color
      Background = Reset },
    this

  member this.On(color : Color) =
    { Foreground =  Reset
      Background = color },
    this

  member this.Styled(foreground : Color, background : Color) =
    { Foreground =  foreground
      Background = background },
    this

// command
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