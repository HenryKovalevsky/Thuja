module internal Thuja.Backend.Mappings

open System

open Thuja

open Tutu.Events 
open Tutu.Style
open Tutu.Style.Types

type Mapper =
  static member map(color : Thuja.Color) : Color = 
    match color with
    | Reset -> Color.Reset
    | Ansi code -> Color.AnsiValue code
    | Rgb (r, g, b) -> Color.Rgb(r, g, b)

  static member map(style : Thuja.Style, content : string) : StyledContent<string> =
    let styledContent =
      Seq.fold 
        (fun (sc : StyledContent<_>) (SGR value) -> sc.Attribute(Attribute(value, value)))
        (StyledContent (ContentStyled.Default, content))
        style.Attributes
        
    styledContent
      .With(Mapper.map style.Foreground)
      .On(Mapper.map style.Background)

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

  static member map(keyModifiers : KeyModifiers) : Thuja.KeyModifiers option =
    let all = Seq.reduce (|||) <| Enum.GetValues<Thuja.KeyModifiers>() 
    let keyModifiers = int keyModifiers |> enum<Thuja.KeyModifiers>

    if all &&& keyModifiers = keyModifiers // is defined flag
    then Some keyModifiers else None