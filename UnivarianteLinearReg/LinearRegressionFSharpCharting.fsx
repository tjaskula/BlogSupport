#r "../packages/MathNet.Numerics/lib/net40/MathNet.Numerics.dll"
#r "../packages/MathNet.Numerics.FSharp/lib/net40/MathNet.Numerics.FSharp.dll"
#r "../packages/MathNet.Numerics.Data.Text/lib/net40/MathNet.Numerics.Data.Text.dll"
#r "../packages/FSharp.Charting/lib/net40/FSharp.Charting.dll"

#I "../packages/MathNet.Numerics/lib/net40/"
#I "../packages/FSharp.Charting/"
#load "FSharp.Charting.fsx"

open System
open System.IO
open System.Globalization

open FSharp.Charting

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
Chart.Point(points)
    .WithXAxis(Title="Population of City in 10,000s", Min=4.0, Max=24.0)
    .WithYAxis(Title="Profit in $10,000s")
    .WithMarkers(Style=ChartTypes.MarkerStyle.Cross, Color=Drawing.Color.Red, Size=7)


let theta = vector [ 0.0; 0.0; ]
let xIntercept = x.InsertColumn(0, (Vector<double>.Build.Dense(m, 1.0)))
let iterations = 1500
let alpha = 0.01

let h = xIntercept * theta.ToColumnMatrix()
let squaredErrors = h.Subtract(y).PointwisePower(2.0);
let j = (1.0 / (2.0 * (m|> double))) * squaredErrors.ColumnSums();