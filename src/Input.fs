module Thuja.input

open System

open Tutu.Events 

type KeyInput =
  | Char of char | FKey of int
  | Up | Down | Left | Right 
  | Enter | Backspace | Delete 
  | Tab | BackTab

[<Flags>]
type KeyModifiers =
  | Shift = 0b001 | Ctrl = 0b010 | Alt = 0b100

let mapKey : KeyCode.IKeyCode -> KeyInput = function
  | :? KeyCode.CharKeyCode as code -> Char code.Character.[0]
  | :? KeyCode.FKeyCode as code ->    FKey code.Number
  | :? KeyCode.UpKeyCode ->           Up 
  | :? KeyCode.DownKeyCode ->         Down 
  | :? KeyCode.LeftKeyCode ->         Left 
  | :? KeyCode.RightKeyCode ->        Right 
  | :? KeyCode.EnterKeyCode ->        Enter 
  | :? KeyCode.BackspaceKeyCode ->    Backspace 
  | :? KeyCode.TabKeyCode ->          Tab 
  | :? KeyCode.BackTabKeyCode ->      BackTab 
  | :? KeyCode.DeleteKeyCode ->       Delete 
  | _ -> raise <| NotImplementedException() // todo: extend 

let mapModifiers : Tutu.Events.KeyModifiers -> KeyModifiers =
  int >> enum<KeyModifiers>
