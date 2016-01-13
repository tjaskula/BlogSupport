#I "../packages/Deedle/lib/net40"
#I "../packages/Deedle.RPlugin/lib/net40"
#I "../packages/RProvider/lib/net40"
#I "../packages/R.NET.Community/lib/net40"
#I "../packages/R.NET.Community.FSharp/lib/net40"

#I "../packages/MathNet.Numerics/lib/net40/"
#I "../packages/MathNet.Numerics.FSharp/lib/net40/"
#I "../packages/MathNet.Numerics.Data.Text/lib/net40/"

#r "RProvider.Runtime.dll"
#r "RProvider.dll"
#r "RDotNet.dll"
#r "RDotNet.NativeLibrary.dll"
#r "RDotNet.FSharp.dll"
#r "Deedle.dll"
#r "Deedle.RProvider.Plugin.dll"

#r "MathNet.Numerics.dll"
#r "MathNet.Numerics.FSharp.dll"
#r "MathNet.Numerics.Data.Text.dll"

#load "../paket-files/evelinag/ffplot/ggplot.fs"

open Deedle
open RProvider
open RProvider.ggplot2
open RProvider.datasets
open ggplot

fsi.AddPrinter(fun (synexpr:RDotNet.SymbolicExpression) -> synexpr.Print())

let sizeSettings () =
    R.theme(namedParams["axis.text", R.element__text(namedParams["size", 12])])
    ++ R.theme(namedParams["legend.text", R.element__text(namedParams["size", 12])])
    ++ R.theme(namedParams["axis.title", R.element__text(namedParams["size", 14])])
    ++ R.theme(namedParams["plot.title", R.element__text(namedParams["size", 18])])

open System.IO
open System.Globalization

open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.Data.Text

let dataCulture = CultureInfo("en-US")
let dataPath = Path.Combine(__SOURCE_DIRECTORY__, "data.csv")

// loads a matrix of 97x2
let data = DelimitedReader.Read<double>(dataPath, false, ",", false, dataCulture)
let x = data.Column(0).ToColumnMatrix().ToColumnWiseArray() |> List.ofArray
let y = data.Column(1).ToColumnMatrix().ToColumnWiseArray() |> List.ofArray
let df =
    namedParams ["Population", x; "Profit" , y]
    |> R.data_frame

let m = y.Length

// print scatter plot
G.ggplot(df, G.aes(x="Population", y="Profit"))
++ R.xlab("Population of City in 10,000s")
++ R.ylab("Profit in $10,000s")
++ R.geom__point(namedParams["shape", box 4; "size", box 2; "colour", box "red"])
++ R.theme__bw()
++ sizeSettings()