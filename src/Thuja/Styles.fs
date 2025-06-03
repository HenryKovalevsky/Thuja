namespace Thuja.Styles

type Color = Thuja.Backend.Color

type BorderStyle =
  | Normal
  | Rounded
  | Thick
  | Double

type internal Border = // todo: cleaner solution
  static member styles = Map [
    Normal, Map [
      "─", "─"
      "│", "│"
      "┌", "┌"
      "└", "└"
      "┐", "┐"
      "┘", "┘"
    ]
    Rounded, Map [
      "─", "─"
      "│", "│"
      "┌", "╭"
      "└", "╰"
      "┐", "╮"
      "┘", "╯"
    ]
    Thick, Map [
      "─", "━"
      "│", "┃"
      "┌", "┏"
      "└", "┗"
      "┐", "┓"
      "┘", "┛"
    ]
    Double, Map [
      "─", "═"
      "│", "║"
      "┌", "╔"
      "└", "╚"
      "┐", "╗"
      "┘", "╝"
    ]
  ]
