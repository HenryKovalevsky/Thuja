open System

open Thuja
open Thuja.Tutu
open Thuja.Styles
open Thuja.Elements

// model
let selected = 0

// view
let view selected =
  columns [ Fraction 40; Fraction 60 ] [
    panel [] [ 
      list [
        "The quick brown fox jumps over the lazy dog"
        "съешь же ещё этих мягких французских булок, да выпей чаю"
        "exploited by the real owners of this enterprise, for it is they who take advantage of the needs of the poor" 
      ] selected
    ]
    panel [] [
      text [ Overflow Wrap ] "Use '↑' and '↓' keys to navigate through the list items, 'Ctrl+C' for exit."
    ]
  ]

// update
let update selected event  =
  let length = 2
  match event with
  | Char 'q', _
  | Char 'c', KeyModifiers.Ctrl -> selected, Program.exit()
  | Down, _ -> Math.Min(length, selected + 1), Cmd.none
  | Up, _ -> Math.Max(0, selected - 1), Cmd.none
  | _ -> selected, Cmd.none

// program
Program.make selected view update
|> Program.withKeyBindings Cmd.ofMsg
|> Program.withTutuBackend
|> Program.run
