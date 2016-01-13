#r "../packages/MathNet.Numerics/lib/net40/MathNet.Numerics.dll"
#r "../packages/MathNet.Numerics.FSharp/lib/net40/MathNet.Numerics.FSharp.dll"
#r "../packages/MathNet.Numerics.Data.Text/lib/net40/MathNet.Numerics.Data.Text.dll"
#r "../packages/RProvider/lib/net40/RProvider.Runtime.dll"
#r "../packages/RProvider/lib/net40/RProvider.dll"
#r "../packages/R.NET.Community/lib/net40/RDotNet.dll"
#r "../packages/R.NET.Community/lib/net40/RDotNet.NativeLibrary.dll"
#r "../packages/R.NET.Community.FSharp/lib/net40/RDotNet.FSharp.dll"
#r "../packages/Deedle/lib/net40/Deedle.dll"
#r "../packages/Deedle.RPlugin/lib/net40/Deedle.RProvider.Plugin.dll"

#I "../packages/MathNet.Numerics/lib/net40/"
#I "../packages/Deedle/lib/net40"
#I "../packages/Deedle.RPlugin/lib/net40"
#I "../packages/RProvider/lib/net40"
#I "../packages/R.NET.Community/lib/net40"
#I "../packages/R.NET.Community.FSharp/lib/net40"

open Deedle
open RProvider
open RProvider.ggplot2
open RProvider.datasets

#load "../paket-files/evelinag/ffplot/ggplot.fs"

open ggplot

open System
open System.IO
open System.Linq
open System.Globalization

open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.Data.Text

let dataCulture = CultureInfo("en-US")
let dataPath = Path.Combine(__SOURCE_DIRECTORY__, "data.csv")

// loads a matrix of 97x2
let data = DelimitedReader.Read<double>(dataPath, false, ",", false, dataCulture)
let x = data.Column(0).ToColumnMatrix()
let y = data.Column(1).ToColumnMatrix()
let points = Array.zip (x.ToColumnWiseArray()) (y.ToColumnWiseArray()) |> Seq.ofArray
let m = y.RowCount

// print scatter plot