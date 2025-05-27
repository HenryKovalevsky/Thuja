namespace Thuja.Elements

open System

module internal Helpers =

  type String with
    member this.TruncateWithEllipsis (length : int)=
      if this.Length > length
      then this[..length - 2] + "⋯"
      else this.PadRight length

    member this.Truncate (length : int)=
      if this.Length > length
      then this[..length - 1]
      else this.PadRight length