open Thuja
open Thuja.Tutu
open Thuja.Elements

let model = 
  {| Headers = [ "NAMESPACE"; "NAME"; "READY"; "STATUS"; "RESTARTS"; "AGE"; "IP" ]
     Columns = [ Fraction 1; Fraction 2; Fraction 1; Fraction 1; Fraction 1; Fraction 1; Fraction 1 ]
     Data = [
       [ "default";    "nginx-7dfdcf6dd5-2j5bk";    "1/1"; "Running";     "0";  "5d17h"; "10.244.1.105" ]
       [ "dev";        "frontend-59d8b7f8d8-8qg2v"; "2/2"; "Running";     "3";  "2d";    "10.244.2.73"  ]
       [ "prod";       "redis-master-0";            "1/1"; "Running";     "12"; "30d";   "10.244.0.42"  ]
       [ "monitoring"; "prometheus-sts-0";          "1/1"; "Terminating"; "1";  "4h10m"; "10.244.3.19"  ]
       [ "test";       "pending-pod-5f7d8";         "0/1"; "Pending";     "0";  "10m";   "<none>"       ]
     ] |}

let view =
  panel [] [
    table [ Columns model.Columns ] model.Headers model.Data
  ]

// program
Program.makeStatic view
|> Program.withTutuBackend
|> Program.run