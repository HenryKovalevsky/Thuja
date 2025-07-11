﻿open System.IO

open CliWrap
open CliWrap.Buffered

open Thuja
open Thuja.Tutu
open Thuja.Elements

// https://hpjansson.org/chafa/
let chafa fileName (width, height) =
  let result = 
    Cli.Wrap("chafa")
      .WithArguments(["-f"; "sixels"; "-s"; $"{width}x{height}"; fileName; "--font-ratio"; "1/1"])
      .ExecuteBufferedAsync()
    |> _.Task
    |> Async.AwaitTask
    |> Async.RunSynchronously

  result.StandardOutput

// model
type Model =
  { Menu: Menu.Model } // custom elements example

type Msg = 
  | MenuMsg of Menu.Msg
  | Sort

let images = 
  (".", "*.jpg")
  |> Directory.EnumerateFiles 
  |> Seq.sort
  |> Seq.toList
  
let model =
  { Menu = Menu.init images }

// view
let view (model : Model) =
  columns [ Fraction 45; Fraction 55 ] [
    panel [] [ 
      Menu.view model.Menu
    ]
    panel [] [
      rows [ Fraction 30; Fraction 70 ] [
        panel [] [ text [] model.Menu.Selected ]
        raw chafa model.Menu.Selected
      ]
    ]
  ]

// update
let isAsc items =
    items
    |> Seq.pairwise 
    |> Seq.forall (fun (a, b) -> a <= b)

let update (model : Model) : Msg -> Model * Cmd<_> = function
  | Sort -> 
      if isAsc model.Menu.Items 
      then { model with Model.Menu.Items = List.sortDescending model.Menu.Items }, Cmd.none
      else { model with Model.Menu.Items = List.sort model.Menu.Items }, Cmd.none

  | MenuMsg msg -> { model with Menu = Menu.update model.Menu msg }, Cmd.none

// input
let keyBindings = function
  | Char 'q', _
  | Char 'c', KeyModifiers.Ctrl -> Program.exit()

  | Up, _ | Char 'k', _ -> Cmd.ofMsg (MenuMsg Menu.Previous)
  | Down, _ | Char 'j', _ -> Cmd.ofMsg (MenuMsg Menu.Next)

  | Char 's', _ -> Cmd.ofMsg Sort // user events example
  | Char 'e', _ -> Cmd.ofAsync (async { return! failwith "aaaa! someone pressed e" }) // error example
  
  | _ ->  Cmd.none

// program
Program.make model view update
|> Program.withKeyBindings keyBindings
|> Program.withTutuBackend
|> Program.run
