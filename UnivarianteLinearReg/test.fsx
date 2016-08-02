// Location of R libraries
#I "/Library/Frameworks/R.framework/Libraries/"
#I "../packages/R.NET.Community/lib/net40"
#I "../packages/R.NET.Community.FSharp/lib/net40"
#I "../packages/DynamicInterop/lib/net40"

#r "RDotNet.dll"
#r "RDotNet.NativeLibrary.dll"
#r "DynamicInterop.dll"

open RDotNet
open System

// Pass location of libR.dylib to R engine
let dllStr = "/Library/Frameworks/R.framework/Libraries/libR.dylib"
let engine = REngine.GetInstance(dll=dllStr)
engine.Initialize()

// Run a simple t-test

let group1 = engine.CreateNumericVector([| 30.02; 29.99; 30.11; 29.97; 30.01; 29.99 |])
engine.SetSymbol("group1", group1)

// Direct parsing from R script.
let group2 = engine.Evaluate("group2 <- c(29.89, 29.93, 29.72, 29.98, 30.02, 29.98)").AsNumeric()

// Test difference of mean and get the P-value.
let testResult = engine.Evaluate("t.test(group1, group2)").AsList()
let p = testResult.["p.value"].AsNumeric().[0]
