open System
open System.Timers

open Thuja
open Thuja.Tutu
open Thuja.Elements

// model
type Model =
  { Time: DateTime }

type Msg = 
  | Tick of DateTime

let model = { Time = DateTime.Now }

// view
let view (model : Model) =
  region [ Width 10; Height 3; Align Center ] [ 
    panel [ 
      text (model.Time.ToString "HH:mm:ss") 
    ]
  ]

// update
let update (model : Model) : Msg -> Model * Cmd<Msg> = function
  | Tick time -> { model with Time = time }, Cmd.none

// subscriptions
let subscription dispatch =
  let timer = new Timer(Interval = 1000)

  timer.Elapsed.Add(fun _ -> dispatch <| Tick DateTime.Now)

  timer.Start()

// program
Program.make model view update
|> Program.withSubscriptions [subscription]
|> Program.withBackend TutuBackend.beginSession
|> Program.run
