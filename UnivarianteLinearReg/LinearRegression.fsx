#r "../packages/MathNet.Numerics/lib/net40/MathNet.Numerics.dll"
#r "../packages/MathNet.Numerics.FSharp/lib/net40/MathNet.Numerics.FSharp.dll"
#r "../packages/MathNet.Numerics.Data.Text/lib/net40/MathNet.Numerics.Data.Text.dll"

#I "../packages/MathNet.Numerics/lib/net40/"

open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.Data.Text

let data = DelimitedReader.Read<double>("data.csv", false, ",", false)
__SOURCE_DIRECTORY__
