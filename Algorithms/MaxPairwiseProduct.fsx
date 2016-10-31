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

let findTwoMaxFold l =
    l |> List.fold (fun state elem -> 
                    if List.length state < 2 then
                        elem :: state
                    elif List.exists (fun e -> e < elem) state then
                        [(List.max state); elem]
                    else state
                    ) []

let l1 = [7; 5; 14; 2; 8; 8; 10; 1; 2; 3]
let l2 = [7; 5; 14; 2; 15; 15; 10; 1; 2; 3]
let l3 = [1]
let l4 = [100000; 90000]


l1 |> findTwoMaxFold
l1 |> findTwoMax

l2 |> findTwoMaxFold
l2 |> findTwoMax

l3 |> findTwoMaxFold
l3 |> findTwoMax

l4 |> findTwoMaxFold
l4 |> findTwoMax