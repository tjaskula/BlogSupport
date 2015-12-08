#r "../packages/MathNet.Numerics/lib/net40/MathNet.Numerics.dll"
#r "../packages/MathNet.Numerics.FSharp/lib/net40/MathNet.Numerics.FSharp.dll"
#r "../packages/MathNet.Numerics.Data.Text/lib/net40/MathNet.Numerics.Data.Text.dll"
#r "../packages/XPlot.Plotly/lib/net45/XPlot.Plotly.dll"
#r "../packages/Http.fs/lib/net40/HttpClient.dll"
#r "../packages/Newtonsoft.Json/lib/net45/Newtonsoft.Json.dll"
#r "../packages/Suave/lib/net40/Suave.dll"

#I "../packages/MathNet.Numerics/lib/net40/"
#I "../packages/XPlot.Plotly/lib/net45/"
#I "../packages/Newtonsoft.Json/lib/net45/"

#load "plotlycredentials.fsx"

open System
open System.IO
open System.Linq
open System.Globalization

open XPlot.Plotly
open HttpClient

open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.Data.Text

open Suave
open Suave.Http.Successful
open Suave.Web

Plotly.Signin PlotlyCredentials.UserAndKey

let dataCulture = CultureInfo("en-US")
let dataPath = Path.Combine(__SOURCE_DIRECTORY__, "data.csv")

// loads a matrix of 97x2
let data = DelimitedReader.Read<double>(dataPath, false, ",", false, dataCulture)
let x = data.Column(0).ToColumnMatrix()
let y = data.Column(1).ToColumnMatrix()
let points = Array.zip (x.ToColumnWiseArray()) (y.ToColumnWiseArray()) |> Seq.ofArray
let m = y.RowCount

// print scatter plot
let trainingData =
    Scatter(
        x = x.ToColumnWiseArray().ToList(),
        y = y.ToColumnWiseArray().ToList(),
        mode = "markers"
    )

let figure = Figure(Data.From [trainingData])
figure.Plot("tesrt")

let theta = (vector [ 0.0; 0.0; ]).ToColumnMatrix()
let xIntercept = x.InsertColumn(0, (Vector<double>.Build.Dense(m, 1.0)))
let iterations = 1500
let alpha = 0.01

// the cost function
let computeCost (x : Matrix<double>) (y : Matrix<double>) (theta : Matrix<float>) =
    let h = xIntercept * theta
    let squaredErrors = h.Subtract(y).PointwisePower(2.0);
    let j = (1.0 / (2.0 * (m|> double))) * squaredErrors.ColumnSums();
    j

let cost = computeCost xIntercept y theta
printfn "The intitial cost is: %f" (cost.[0])

// gradient descent
let gradientDescent (x : Matrix<double>) (y : Matrix<double>) (theta : Matrix<float>) alpha num_iters =
    let m = y.RowCount |> float
    let currentIter = 1
    let rec walkGradient (theta : Matrix<float>) costH currentIter =
        if currentIter <= num_iters then
            let h = x * theta
            let gradient = alpha * (1.0 / m) * (x.Transpose() * (h.Subtract(y)))
            let newTheta = theta - gradient
            let cost = computeCost x y newTheta
            let newCostHistory = costH @ [cost.[0]]
            walkGradient newTheta newCostHistory (currentIter + 1)
        else
           theta, costH
    walkGradient theta [] currentIter

let optimizedThetaCostHistory = gradientDescent xIntercept y theta alpha iterations
let optimizedTheta = fst optimizedThetaCostHistory
printfn "Theta found by gradient descent: %f %f" (optimizedTheta.[0, 0]) (optimizedTheta.[1, 0])


// print scatter plot
let linearRegression = xIntercept * optimizedTheta
let linearRegressionLine =  Array.zip (xIntercept.Column(1).ToArray()) (linearRegression.ToColumnWiseArray()) |> Seq.ofArray
// chart


let template = Path.Combine(__SOURCE_DIRECTORY__, "web/test.html")
let html = File.ReadAllText(template)
let chartHtml = html.Replace("[BODY]", googleChart.InlineHtml)

let app = OK(html)

let cts = startServer app
stopServer cts
