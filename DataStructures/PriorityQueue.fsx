open System
open System.Collections.Generic
open Microsoft.FSharp.Core

type PriorityQueue<'T when 'T : comparison>(values: seq<'T>, isDescending: bool) =
    let heap : System.Collections.Generic.List<'T> = System.Collections.Generic.List<'T>(values)
    
    let isGreater x y =
        if isDescending then x > y else x < y

    let isLower x y = not (isGreater x y)

    let mutable size = heap.Count

    let shrinkHeap() =
        let shouldShrink = size < heap.Count / 2
        if shouldShrink then heap.RemoveRange(size, heap.Count - size - 1)

    let parent i = (i - 1) / 2
    let leftChild i = 2 * i + 1
    let rightChild i = 2 * i + 2

    let swap i maxIndex =
        let temp = heap.[i]
        heap.[i] <- heap.[maxIndex]
        heap.[maxIndex] <- temp

    let siftUp i =
        let mutable indx = i
        while indx > 0 && isLower heap.[parent indx] heap.[indx] do
            swap (parent indx) indx
            indx <- parent indx

    let rec siftDown i =
        let l = leftChild i
        let r = rightChild i
        let maxIndexLeft = if l < size && isGreater heap.[l] heap.[i] then l else i
        let maxIndex = if r < size && isGreater heap.[r] heap.[maxIndexLeft] then r else maxIndexLeft
        if i <> maxIndex then
            swap i maxIndex
            siftDown maxIndex
        else ()

    let rec preOrderTraversal predicate indx =
        if indx >= size then
            None
        else
            let elem = heap.[indx]
            if predicate(elem) then
                Some(indx)
            else
                let leftChildIndx = leftChild indx
                let rightChildIndex = rightChild indx
                match preOrderTraversal predicate leftChildIndx with
                | None -> preOrderTraversal predicate rightChildIndex
                | found -> found 
    
    let build() =
        for i = size / 2 downto 0 do
            siftDown i
    
    do build()

    new (values) = PriorityQueue<'T>(values, true)
    new () = PriorityQueue<'T>([], true)

    member this.IsEmpty = size = 0

    member this.Count = size

    member this.Dequeue() =
        if this.IsEmpty then raise (new Exception("No more elements to dequeue"))
        let result = heap.[0]
        heap.[0] <- heap.[size - 1]
        // we limit the boundary but the last element stays in memory
        // we could use heap.Remove but it's O(n) operation so too slow
        size <- size - 1
        shrinkHeap()
        siftDown 0
        result

    member this.Enqueue p =
        if heap.Count = size then
            heap.Add(p)
        else
            heap.[size] <- p
        size <- size + 1
        siftUp (size - 1)

    member this.TryFind predicate =
        match preOrderTraversal predicate 0 with
        | None -> None
        | Some(i) -> Some(i, heap.[i])

    member this.Update indx v =
        if indx < 0 || indx >= size then failwith "The index is out of range"
        else
            let old = heap.[indx]
            heap.[indx] <- v
            if isGreater v old then
                siftUp(indx)
            else siftDown(indx)

type Edge = { DestinationVertexId: int; Distance: double }

[<CustomComparison; StructuralEquality>]
type Vertex = { Id: int; ShortestDistance: double; Edges: Edge list; Path: int list }
                interface IComparable<Vertex> with
                        member this.CompareTo other =
                            compare this.ShortestDistance other.ShortestDistance
                interface IComparable with
                    member this.CompareTo(obj: obj) =
                        match obj with
                        | :? Vertex -> compare this.ShortestDistance (unbox<Vertex> obj).ShortestDistance
                        | _ -> invalidArg "obj" "Must be of type Vertex"

type Graph = { Vertices: Vertex list }

let addEdge vertex e = { vertex with Edges = e::vertex.Edges }

type Vertex with
    member this.AddEdge = addEdge this

let addVertex graph v = { graph with Vertices = v::graph.Vertices }
let getVertices graph = graph.Vertices
let setSource vertexId graph = 
    { Vertices = graph.Vertices
                |> List.map (fun e -> if e.Id = vertexId then {e with ShortestDistance = 0.0 } else e) }

type Graph with
    member this.AddVertex = addVertex this
    member this.GetVertices () = getVertices this


let rawGraph = Map.empty
                 .Add(1, [(9.0, 3); (7.0, 2); (14.0, 6)])
                 .Add(2, [(7.0, 1); (10.0, 3); (15.0, 4)])
                 .Add(3, [(9.0, 1); (2.0, 6); (11.0, 4); (10.0, 2)])
                 .Add(4, [(15.0, 2); (11.0, 3); (6.0, 5)])
                 .Add(5, [(6.0, 4); (9.0, 6)])
                 .Add(6, [(14.0, 1); (2.0, 3); (9.0, 5)])

let makeEdge (distance, destVertexId) =
    { DestinationVertexId = destVertexId; Distance = distance}

let makeVertex vertexId edges =
    { Id = vertexId;
      ShortestDistance = Double.PositiveInfinity;
      Edges = edges |> List.map makeEdge
      Path = []
    }

let graph = rawGraph
            |> Map.map makeVertex
            |> Map.fold (fun (graph: Graph) _ v -> graph.AddVertex(v)) { Vertices = [] }
            |> setSource 1

let shortestPath (graph: Dictionary<int, Vertex>) destinationId =

    let pq = PriorityQueue<Vertex>(graph.Values, false)
    let mutable dest = Option<Vertex>.None
    let visited = Dictionary<int, Vertex>()

    while not pq.IsEmpty do
        let vertex = pq.Dequeue()
        if vertex.ShortestDistance <> Double.PositiveInfinity && not (visited.ContainsKey(vertex.Id)) then
            if vertex.Id = destinationId then
                dest <- Some(vertex)

            //printfn "Vertex '%i' accessed from : %s" vertex.Id (vertex.Path |> List.rev |> List.fold (fun state elem -> state + ", " + elem.ToString()) "")
            for edge in vertex.Edges do
                let destinationId = edge.DestinationVertexId
                if not (visited.ContainsKey(destinationId)) then
                    let newDistance = edge.Distance + vertex.ShortestDistance
                    let destination = graph.[destinationId]
                    if newDistance < destination.ShortestDistance then
                        let newDestination = { destination with ShortestDistance = newDistance; Path = destination.Id :: vertex.Path }
                        pq.Enqueue newDestination
                        graph.[destinationId] <- newDestination
                    else ()
                else ()
            visited.Add(vertex.Id, vertex)
    dest

// real use case
// CA
// starting point 512 Partridge Ave, Bakersfield, CA 93309, États-Unis
// nodeId: 1215934
// destination US-101, San Francisco, CA 94129, États-Unis Golden Gate Bridge
// nodeId: 1598609

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

let readLines filePath = System.IO.File.ReadLines(filePath)

let lines = readLines "USA-road-d.CAL.gr"

let graphMap =
    lines
    |> Seq.map (fun l -> let a = l.Split()
                         (int a.[1], int a.[2], float a.[3]))
    |> Seq.groupBy (fun (f, t, d) -> f)
    |> Seq.map (fun (key, values) -> (key, values |> Seq.map (fun (k, v, z) -> z, v) |> Seq.toList))
    |> Map.ofSeq


//let s =[(0, 1, 0.002025); (0, 6, 0.005952); (1, 2, 0.014350)]
//s |> Seq.groupBy (fun (f, t, d) -> f)
//  |> Seq.map (fun (key, values) -> (key, values |> Seq.map (fun (k, v, z) -> z, v) |> Seq.toList))
//  |> Map.ofSeq

let roadNetwork = graphMap
                    |> Map.map makeVertex
                    |> Map.fold (fun (graph: Dictionary<int, Vertex>) _ v -> graph.Add(v.Id, v); graph) (Dictionary<int, Vertex>())
                    //|> setSource 1215934
let start = roadNetwork.[1215934]
roadNetwork.[1215934] <- {start with ShortestDistance = 0.0 }

let coord = readLines "USA-road-d.CAL.co"
let graphCoordinates =
    coord
    |> Seq.map (fun l -> let a = l.Split()
                         int a.[1], (float (a.[2].Insert(4, ".")), float (a.[3].Insert(2, "."))))
    |> Map.ofSeq

let arrival = shortestPath roadNetwork 1598609
arrival.Value.Path
|> Seq.rev
|> Seq.iteri (fun i e -> if i % 50 = 0 then printfn "%i" e )

let outFile = new System.IO.StreamWriter("RoadData.txt")
outFile.WriteLine("latitude,longitude")
arrival.Value.Path
|> Seq.rev
|> Seq.iter (fun e -> outFile.WriteLine(sprintf "%f, %f" (snd graphCoordinates.[e]) (fst graphCoordinates.[e])))

outFile.Flush()
outFile.Close()