module internal Thuja.Backend.Mappings

open System

open Thuja
open Thuja.Backend

open Tutu.Events 
open Tutu.Style
open Tutu.Style.Types

type Mapper =
  static member map(color : Backend.Color) : Color = 
    match color with
    | Reset -> Color.Reset
    | Black -> Color.Black
    | DarkGrey -> Color.DarkGrey
    | Red -> Color.Red
    | DarkRed -> Color.DarkRed
    | Green -> Color.Green
    | DarkGreen -> Color.DarkGreen
    | Yellow -> Color.Yellow
    | DarkYellow -> Color.DarkYellow
    | Blue -> Color.Blue
    | DarkBlue -> Color.DarkBlue
    | Magenta -> Color.Magenta
    | DarkMagenta -> Color.DarkMagenta
    | Cyan -> Color.Cyan
    | DarkCyan -> Color.DarkCyan
    | White -> Color.White
    | Grey -> Color.Grey

  static member map(style : Backend.Style, content : string) : StyledContent<string> =
    StyledContent(ContentStyled.Default
        .With(Mapper.map style.Foreground)
        .On(Mapper.map style.Background), content)
      
  static member map(keyCode : KeyCode.IKeyCode) : KeyInput option =
    match keyCode with 
    | :? KeyCode.CharKeyCode as code -> Char code.Character.[0] |> Some 
    | :? KeyCode.FKeyCode as code ->    FKey code.Number |> Some 
    | :? KeyCode.UpKeyCode ->           Up |> Some 
    | :? KeyCode.DownKeyCode ->         Down |> Some 
    | :? KeyCode.LeftKeyCode ->         Left |> Some 
    | :? KeyCode.RightKeyCode ->        Right |> Some 
    | :? KeyCode.EnterKeyCode ->        Enter |> Some 
    | :? KeyCode.BackspaceKeyCode ->    Backspace |> Some 
    | :? KeyCode.TabKeyCode ->          Tab |> Some 
    | :? KeyCode.BackTabKeyCode ->      BackTab |> Some 
    | :? KeyCode.DeleteKeyCode ->       Delete |> Some 
    | _ -> None // todo: extend 

  static member map(keyModifiers : KeyModifiers) : Backend.KeyModifiers option =
    let all = Seq.reduce (|||) <| Enum.GetValues<Backend.KeyModifiers>() 
    let keyModifiers = int keyModifiers |> enum<Backend.KeyModifiers>

    if all &&& keyModifiers = keyModifiers // is defined flag
    then Some keyModifiers else None