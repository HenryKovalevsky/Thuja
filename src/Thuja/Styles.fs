module Thuja.Styles

open System
open System.Globalization
open System.Text.RegularExpressions

open Thuja

// colors
type String with
  member this.With(color : Color) =
    { Foreground =  color
      Background = Reset
      Attributes = [] },
    this

  member this.On(color : Color) =
    { Foreground =  Reset
      Background = color
      Attributes = [] },
    this

  member this.Styled(foreground : Color, background : Color) =
    { Foreground =  foreground
      Background = background
      Attributes = [] },
    this

  member this.Styled(foreground : Color, background : Color, attributes : Attribute list) =
    { Foreground =  foreground
      Background = background
      Attributes = attributes },
    this

module Color = 
  // default
  let Reset = Color.Reset

  // standard ansi set
  let Black =       Ansi 0uy
  let DarkGrey =    Ansi 8uy
  let Red =         Ansi 9uy
  let DarkRed =     Ansi 1uy
  let Green =       Ansi 10uy
  let DarkGreen =   Ansi 2uy
  let Yellow =      Ansi 11uy
  let DarkYellow =  Ansi 3uy
  let Blue =        Ansi 12uy
  let DarkBlue =    Ansi 4uy
  let Magenta =     Ansi 13uy
  let DarkMagenta = Ansi 5uy
  let Cyan =        Ansi 14uy
  let DarkCyan =    Ansi 6uy
  let White =       Ansi 15uy
  let Grey =        Ansi 7uy

  // additional colors
  let Rgb r g b = Rgb (r, g, b)
  let Hex (hex : string) =
    let hex = hex.Trim().ToUpperInvariant()

    if not <| Regex.IsMatch(hex, @"^#[0-9A-F]{6}$") 
    then raise <| InvalidOperationException $"Invalid hex color code: {hex}."

    let r = Byte.Parse(hex.Substring(1, 2), NumberStyles.AllowHexSpecifier)
    let g = Byte.Parse(hex.Substring(3, 2), NumberStyles.AllowHexSpecifier)
    let b = Byte.Parse(hex.Substring(5, 2), NumberStyles.AllowHexSpecifier)

    Rgb r g b

module Attribute =
  // https://en.wikipedia.org/wiki/ANSI_escape_code#SGR_parameters
  let Reset =      SGR "0"
  let Bold =       SGR "1"
  let Dim =        SGR "2"
  let Italic =     SGR "3"
  let Underlined = SGR "4"

// borders
type BorderStyle =
  | Normal
  | Rounded
  | Thick
  | Double

type internal BorderLine =
  | Horizontal
  | Vertical
  | TopLeft
  | TopRight
  | BottomLeft
  | BottomRight

[<RequireQualifiedAccess>]
module internal Border =
  let Styles = Map [
    Normal, Map [
      Horizontal,  "─"
      Vertical,    "│"
      TopLeft,     "┌"
      BottomLeft,  "└"
      TopRight,    "┐"
      BottomRight, "┘"
    ]
    Rounded, Map [
      Horizontal,  "─"
      Vertical,    "│"
      TopLeft,     "╭"
      BottomLeft,  "╰"
      TopRight,    "╮"
      BottomRight, "╯"
    ]
    Thick, Map [
      Horizontal,  "━"
      Vertical,    "┃"
      TopLeft,     "┏"
      BottomLeft,  "┗"
      TopRight,    "┓"
      BottomRight, "┛"
    ]
    Double, Map [
      Horizontal,  "═"
      Vertical,    "║"
      TopLeft,     "╔"
      BottomLeft,  "╚"
      TopRight,    "╗"
      BottomRight, "╝"
    ]
  ]
