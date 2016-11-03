// brut force
let getMaxPariwiseProduct (numbers: int array): int64 =
    let mutable result = 0L
    let n = numbers.Length
    for i in 0..(n - 1) do
        for j in (i + 1)..(n - 1) do
            if int64(numbers.[i]) * int64(numbers.[j]) > result then
                result <- int64(numbers.[i]) * int64(numbers.[j])
            else ()
    result

let rec removeFirst pred lst =
    match lst with
    | h::t when pred h -> t
    | h::t -> h::removeFirst pred t
    | _ -> []

let findTwoMax l =
    let rec twoMaxIter o l' =
        match o with
        | _ when List.isEmpty(l') -> o
        | [x; y] -> o
        | _ -> 
            let m = List.max l'
            l' 
            |> removeFirst (fun e -> e = m)
            |> twoMaxIter (m :: o)
    twoMaxIter [] l 
    |> List.map int64
    |> List.reduce (*)

let findTwoMaxFold l =
    l |> List.fold (fun state elem -> 
                    if List.length state < 2 then
                        elem :: state
                    elif List.exists (fun e -> e < elem) state then
                        [(List.max state); elem]
                    else state
                    ) [] 
      |> List.map int64
      |> List.reduce (*)

let l1 = [7; 5; 14; 2; 8; 8; 10; 1; 2; 3]
let l2 = [7; 5; 14; 2; 15; 15; 10; 1; 2; 3]
let l3 = [1]
let l4 = [100000; 90000]

l1 |> Array.ofList |> getMaxPariwiseProduct
l1 |> findTwoMaxFold
l1 |> findTwoMax

l2 |> Array.ofList |> getMaxPariwiseProduct
l2 |> findTwoMaxFold 
l2 |> findTwoMax

l3 |> Array.ofList |> getMaxPariwiseProduct
l3 |> findTwoMaxFold 
l3 |> findTwoMax

l4 |> Array.ofList |> getMaxPariwiseProduct
l4 |> findTwoMaxFold
l4 |> findTwoMax

// randomized max
open System
let r = new Random(347)
let n = 200001
let l = [for i in 2..n do
            yield r.Next(0, pown 10 5)]

