#r "../packages/MathNet.Numerics/lib/net40/MathNet.Numerics.dll"
#r "../packages/MathNet.Numerics.FSharp/lib/net40/MathNet.Numerics.FSharp.dll"
#r "../packages/MathNet.Numerics.Data.Text/lib/net40/MathNet.Numerics.Data.Text.dll"
#r "../packages/XPlot.Plotly/lib/net45/XPlot.Plotly.dll"
#r "../packages/Newtonsoft.Json/lib/net45/Newtonsoft.Json.dll"
#r "../packages/Suave/lib/net40/Suave.dll"

#I "../packages/MathNet.Numerics/lib/net40/"
#I "../packages/XPlot.Plotly/lib/net45/"
#I "../packages/Newtonsoft.Json/lib/net45/"

#load "plotlycredentials.fsx"

open System
open System.IO
open System.Globalization

open XPlot.Plotly

open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.Data.Text

open Suave
open Suave.Http.Successful
open Suave.Web

Plotly.Signin PlotlyCredentials.UserAndKey

let dataCulture = CultureInfo("en-US")
let dataPath = Path.Combine(__SOURCE_DIRECTORY__, "UnivarianteLinearReg/data.csv")

let startServer app =
  let config = { defaultConfig with homeFolder = Some __SOURCE_DIRECTORY__ }
  let _, server = startWebServerAsync config app
  let cts = new System.Threading.CancellationTokenSource()
  Async.Start(server, cts.Token)
  cts

let stopServer (cts : System.Threading.CancellationTokenSource) =
  cts.Cancel()

// loads a matrix of 97x2
let data = DelimitedReader.Read<double>(dataPath, false, ",", false, dataCulture)
let x = data.Column(0)
let y = data.Column(1)
let m = y.Count


// Draw scatter plot  of points
let rnd = new System.Random()
let next() = rnd.NextDouble() * rnd.NextDouble()
let points = [ for i in 0 .. 100 -> next(), next() ]


let googleChart = points
                  |> Chart.Scatter

let template = Path.Combine(__SOURCE_DIRECTORY__, "web/test.html")
let html = File.ReadAllText(template)
//let chartHtml = html.Replace("[BODY]", googleChart.InlineHtml)

let app = OK(html)

let cts = startServer app
stopServer cts
