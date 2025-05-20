open System
open System.IO

open CliWrap
open CliWrap.Buffered

open Thuja
open Thuja.DSL

// https://hpjansson.org/chafa/
let chafa fileName (width, height) =
  let result = 
    Cli.Wrap("chafa")
      .WithArguments(["-f"; "sixels"; "-s"; $"{width}x{height}"; fileName])
      .ExecuteBufferedAsync()
    |> _.Task
    |> Async.AwaitTask
    |> Async.RunSynchronously

  result.StandardOutput

// model
type Menu =
  { Items: string list 
    Index: int }

type Menu with
  member this.Selected with get() = this.Items.[this.Index]
  member this.Next() = { this with Index = Math.Min(this.Items.Length - 1, this.Index + 1) }
  member this.Previous() = { this with Index = Math.Max(0, this.Index - 1) }

type Msg = | Sort of SortKind
and SortKind = | Asc | Desc

let images = 
  (".", "*.jpg")
  |> Directory.EnumerateFiles 
  |> Seq.sort
  |> Seq.toList
  
let model =
  { Items = images
    Index = 0 }

// view
let view (model : Menu) (dispatch : Msg -> unit) =
  columns [ 45; 55 ] [
    panel [ 
      list model.Items [model.Index]
    ]
    panel [
      rows [ 30; 70 ] [
        panel [ text model.Selected ]
        raw chafa model.Selected
      ]
    ]
  ]

// update
let isAsc items =
    items
    |> Seq.pairwise 
    |> Seq.forall (fun (a, b) -> a <= b)

let handleKey (model : Menu) = function
  | "q" -> model, Cmd.exit
  | "k" -> model.Previous(), Cmd.none
  | "j" -> model.Next(), Cmd.none
  | "s" -> model, Cmd.ofMsg (if isAsc model.Items then Sort Desc else Sort Asc) // user events example
  | "e" -> model, Cmd.ofFunc (fun _ -> failwith "aaaa! someone pressed e") // error example
  | _ -> model, Cmd.none

let handleMsg (model : Menu) = function
  | Sort Asc -> { model with Items = List.sort model.Items }, Cmd.none
  | Sort Desc -> { model with Items = List.sortDescending model.Items }, Cmd.none

let update event (model : Menu) =
  match event with
  | KeyEvent code -> handleKey model code
  | UserEvent msg -> handleMsg model msg

// program
Program.run model view update