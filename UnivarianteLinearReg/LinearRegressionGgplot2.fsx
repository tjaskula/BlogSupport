System.Environment.CurrentDirectory <- __SOURCE_DIRECTORY__
#I "packages/Deedle/lib/net40"
#I "packages/Deedle.RPlugin/lib/net40"
#I "packages/DynamicInterop/lib/net40"
#I "packages/RProvider/lib/net40"
#I "packages/R.NET.Community/lib/net40"
#I "packages/R.NET.Community.FSharp/lib/net40"

#I "packages/MathNet.Numerics/lib/net40/"
#I "packages/MathNet.Numerics.FSharp/lib/net40/"
#I "packages/MathNet.Numerics.Data.Text/lib/net40/"

#r "RProvider.Runtime.dll"
#r "RProvider.dll"
#r "RDotNet.dll"
#r "RDotNet.NativeLibrary.dll"
#r "RDotNet.FSharp.dll"
#r "Deedle.dll"
#r "Deedle.RProvider.Plugin.dll"
#r "DynamicInterop.dll"

#r "MathNet.Numerics.dll"
#r "MathNet.Numerics.FSharp.dll"
#r "MathNet.Numerics.Data.Text.dll"

#load "paket-files/evelinag/ffplot/ggplot.fs"

open Deedle
open RProvider
open RProvider.ggplot2
open RProvider.datasets
open ggplot

open RDotNet
open RProvider.graphics
open RProvider.grDevices

R.x11()

fsi.AddPrinter(fun (synexpr:RDotNet.SymbolicExpression) -> synexpr.Print())

let sizeSettings () =
    R.theme(namedParams["axis.text", R.element__text(namedParams["size", 12])])
    ++ R.theme(namedParams["legend.text", R.element__text(namedParams["size", 12])])
    ++ R.theme(namedParams["axis.title", R.element__text(namedParams["size", 14])])
    ++ R.theme(namedParams["plot.title", R.element__text(namedParams["size", 18])])

open System.IO
open System.Globalization
open System.Collections.Generic

open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.Data.Text

let dataCulture = CultureInfo("en-US")
let dataPath = Path.Combine(__SOURCE_DIRECTORY__, "data.csv")

// loads a matrix of 97x2
let data = DelimitedReader.Read<double>(dataPath, false, ",", false, dataCulture)
let x = data.Column(0).ToColumnMatrix()
let y = data.Column(1).ToColumnMatrix()
let df =
    namedParams ["Population", x.ToColumnWiseArray() |> List.ofArray; "Profit" , y.ToColumnWiseArray() |> List.ofArray]
    |> R.data_frame

let m = y.RowCount

// print scatter plot
G.ggplot(df, G.aes(x="Population", y="Profit"))
++ R.xlab("Population of City in 10,000s")
++ R.ylab("Profit in $10,000s")
++ R.geom__point(namedParams["shape", box 4; "size", box 2; "colour", box "red"])
++ R.theme__bw()
++ sizeSettings()

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
let df2 =
        namedParams ["intercept", xIntercept.Column(1).ToArray(); "regression" , linearRegression.ToColumnWiseArray()]
        |> R.data_frame

// print scatter plot with linear regression after gradient descent
let cols = R.c(namedParams["'Training Data'", "red"; "'Linear Regression'", "blue"])

G.ggplot()
++ R.xlab("Population of City in 10,000s")
++ R.ylab("Profit in $10,000s")
++ R.geom__point(
    namedParams["data", box df;
                "mapping",
                    box (
                        G.aes(x = "Population", y = "Profit", colour = "'Training Data'"));
                "shape", box 4;
                "size", box 2])
++ R.geom__line(
    namedParams["data", box df2
                "mapping",
                    box (
                        G.aes(x = "intercept", y = "regression", colour = "'Linear Regression'"));
                    "size", box 1])
++ R.scale__colour__manual(namedParams[
                            "name", box "";
                            "values", box cols;
                            "breaks", box(R.c("Training Data", "Linear Regression"));
                            "guide", box(R.guide__legend(namedParams["override.aes", R.aes__string(namedParams["fill", R.eval(expr = R.parse(namedParams["text", "NA"]))])]))])
++ R.guides(namedParams["fill", R.guide__legend(namedParams["override.aes", R.list(namedParams["linetype", box 0; "shape", box "''"])]);
                        "colour", R.guide__legend(namedParams["override.aes",
                                                    R.list(namedParams["linetype", R.c(0, 1);
                                                                       "shape", R.c(4, R.as_numeric("NA"))])])])
++ R.theme__bw()
++ R.theme(namedParams["legend.position", R.c(1, 0); "legend.justification", R.c(1, 0)])
++ sizeSettings()