# Thuja

Thuja (_pronounce:_ /ˈθuː.jə/) is a minimalistic [F#](https://fsharp.org) library to build terminal user interfaces. It's inspired by [Elm](https://elm-lang.org/) and based on [Tutu](https://github.com/lillo42/tutu/) as a cross-term backend implementation.

> NB: Really raw version with a bunch of work to do.

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/) to work with F# files and dependencies.

## Usage

_See the [examples directory](sample/)._

```fsharp
// model
let selected = 0

// view
let view selected =
  columns [ 40; 60 ] [
    panel [ 
      list [
        "The quick brown fox jumps over the lazy dog"
        "съешь же ещё этих мягких французских булок, да выпей чаю"
        "exploited by the real owners of this enterprise, for it is they who take advantage of the needs of the poor" 
      ] [selected]
    ]
    panel [
      text "Use '↑' and '↓' keys to navigate through the list items, 'Ctrl+C' for exit."
    ]
  ]

// update
let update selected event  =
  let length = 2
  match event with
  | Char 'c', KeyModifiers.Ctrl -> selected, Program.exit()
  | Down, _ -> Math.Min(length, selected + 1), Cmd.none
  | Up, _ -> Math.Max(0, selected - 1), Cmd.none
  | _ -> selected, Cmd.none

// program
Program.make selected view update
|> Program.withKeyBindings Cmd.ofMsg
|> Program.withBackend TutuBackend.beginSession
|> Program.run
```

## Todo

- implement not implemented parts (see `todo` in a code);
- make comprehensive user input handling:
    - keyboard input (different keys, key kinds?, modifiers, etc);
    - mouse input?
- decide if to use dispatch in view and self-sufficient reactive elements or process elements state through the model;
- add different control elements:
    - text input;
    - button;
    - implement panel borders style (see `todo` in a code);
    - ...
- add different layout options (exact sizing, padding/margin, etc)?;
- handle terminal resizing;
- add styles;
- add tests;
- add debug options;
- make nuget package;
- write documentation;
- consider using elmish as MVU framework?;
- ...