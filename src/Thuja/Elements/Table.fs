namespace Thuja.Elements

open System

open Thuja
open Thuja.Elements
open Thuja.Styles

type TableProps =
  | HeadersStyle of foreground: Color * background: Color
  | RowsStyle of foreground: Color * background: Color
  | Columns of LayoutProps list

[<AutoOpen>]
module Table = 
  let table (props : TableProps list) (headers : string list) (data : string list list) =
    // props
    let headersFg, headersBg = 
      props 
      |> Seq.choose ^function | HeadersStyle (fg, bg) -> Some (fg, bg) | _ -> None
      |> Seq.tryHead
      |> Option.defaultValue (Color.White, Color.Reset)

    let rowsFg, rowsBg = 
      props 
      |> Seq.choose ^function | RowsStyle (fg, bg) -> Some (fg, bg) | _ -> None
      |> Seq.tryHead
      |> Option.defaultValue (Color.DarkGrey, Color.Reset)
      
    let columnsProps = 
      props 
      |> Seq.choose ^function | Columns layout -> Some layout | _ -> None
      |> Seq.tryHead
      |> Option.defaultValue (List.init headers.Length ^fun _ -> Fraction 1)

    // layout
    let rowsProps = 
      Seq.init (data.Length + 1) ^fun _ -> Absolute 1
      |> Seq.toList

    let columnsProps = // with gaps
      columnsProps
      |> Seq.map ^fun prop -> [ prop; Absolute 2 ] 
      |> Seq.collect id
      |> Seq.truncate (2 * headers.Length - 1)
      |> Seq.toList

    let gap = text [] ""

    rows rowsProps [
      yield columns columnsProps [
        for header in headers do
          text [ Color headersFg; Background headersBg; Overflow Ellipsis ] header
          gap
      ] 
      for row in data do
        yield columns columnsProps [
          for cell in row do
            text [ Color rowsFg; Background rowsBg; Overflow Ellipsis ] cell
            gap
        ]
    ] 
