module internal Thuja.Layout 

open Thuja.View

let columns (ratios: int list) (region : Region) : Region list =
  let total = Seq.sum ratios
  let width = region.Width

  ratios
  |> Seq.scan (fun left ratio -> left + width * ratio/total) region.X1
  |> Seq.pairwise
  |> Seq.map ^fun (left, right) -> Region.create (left, region.Y1, right-1, region.Y2)
  |> Seq.toList

let rows (ratios: int list) (region: Region) : Region list =
  let total = Seq.sum ratios
  let height = region.Height

  ratios
  |> Seq.scan (fun top ratio -> top + height * ratio/total) region.Y1
  |> Seq.pairwise
  |> Seq.map ^fun (top, bottom) -> Region.create (region.X1, top, region.X2, bottom-1)
  |> Seq.toList