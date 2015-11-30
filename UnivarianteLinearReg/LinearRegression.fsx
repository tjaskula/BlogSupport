#r "../packages/MathNet.Numerics/lib/net40/MathNet.Numerics.dll"
#r "../packages/MathNet.Numerics.FSharp/lib/net40/MathNet.Numerics.FSharp.dll"
#r "../packages/MathNet.Numerics.Data.Text/lib/net40/MathNet.Numerics.Data.Text.dll"
#r "../packages/XPlot.GoogleCharts/lib/net45/XPlot.GoogleCharts.dll"
#r "../packages/Google.DataTable.Net.Wrapper/lib/Google.DataTable.Net.Wrapper.dll"
#r "../packages/Newtonsoft.Json/lib/net45/Newtonsoft.Json.dll"
//#r "../packages/FSharp.Charting/lib/net40/FSharp.Charting.dll"

#I "../packages/MathNet.Numerics/lib/net40/"
//#I "../packages/FSharp.Charting.Gtk/lib/net40/"
//#I "../packages/FSharp.Charting.Gtk/"
//#load "FSharp.Charting.Gtk.fsx"
#I "../packages/XPlot.GoogleCharts/lib/net45/"
#I "../packages/Google.DataTable.Net.Wrapper/lib/"
#I "../packages/Newtonsoft.Json/lib/net45/"

open System
open System.IO
open System.Globalization

//open FSharp.Charting
open XPlot.GoogleCharts

open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.Data.Text

let dataCulture = CultureInfo("en-US")
let dataPath = Path.Combine(__SOURCE_DIRECTORY__, "UnivarianteLinearReg/data.csv")

// loads a matrix of 97x2
let data = DelimitedReader.Read<double>(dataPath, false, ",", false, dataCulture)
let x = data.Column(0)
let y = data.Column(1)
let m = y.Count

// print scatter plot
let chartPoints = data.ToArray()
                  |> Seq.cast<double>
//Chart.Point chartPoints


// Draw scatter plot  of points
let rnd = new System.Random()
let next() = rnd.NextDouble() * rnd.NextDouble()
let points = [ for i in 0 .. 100 -> next(), next() ]

points |> Chart.Scatter
