namespace Thuja.Elements

module internal Helpers =
  open System
  open System.Globalization
  
  type String with
    member this.TruncateWithEllipsis (length : int) =
      if this.Length > length
      then this[..length - 2] + "⋯"
      else this.PadRight length

    member this.Truncate (length : int) =
      if this.Length > length
      then this[..length - 1]
      else this.PadRight length

    member this.Wrap (width : int) =
      let rec loop (words: string list) position = seq {
        match words with
        | word::rest -> 
            let stuff, position =
              if position > 0 then
                if position + word.Length < width then
                  " ", position + 1
                else
                  Environment.NewLine, 0
              else "", 0
            yield stuff + word
            yield! loop rest (position + word.Length)
        | _ -> ()
      }
      let words = this.Split(" ", StringSplitOptions.RemoveEmptyEntries) |> Seq.toList
      let text = loop words 0 |> String.concat ""
      
      text.Split Environment.NewLine
      
  module String =
    let truncate withEllipsis width (string : string) =
      if withEllipsis then
        string.TruncateWithEllipsis width
      else
        string.Truncate width

    let rev (string : string) =
      StringInfo.ParseCombiningCharacters string
      |> Array.rev
      |> Seq.map ^fun ch -> StringInfo.GetNextTextElement(string, ch)
      |> String.concat ""