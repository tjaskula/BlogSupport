open System
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
    let leftChild i = 2 * i
    let rightChild i = 2 * i + 1

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

    member this.Enqueue(p: 'T) =
        if heap.Count = size then
            heap.Add(p)
        else
            heap.[size] <- p
        size <- size + 1
        siftUp (size - 1)

type Edge = { DestinationVertexId: int; Distance: double }

 [<CustomComparison; StructuralEquality>]
type Vertex = { Id: int; ShortestDistance: double; Edges: Edge list }
                interface IComparable<Vertex> with
                        member this.CompareTo other =
                            compare this.ShortestDistance other.ShortestDistance
                interface IComparable with
                    member this.CompareTo(obj: obj) =
                        match obj with
                        | :? Vertex -> compare this.ShortestDistance (unbox<Vertex> obj).ShortestDistance
                        | _ -> invalidArg "obj" "Must be of type Vertex"

let addEdge vertex e = { vertex with Edges = e::vertex.Edges }

type Vertex with
    member this.AddEdge = addEdge this

type Graph() =
    
    let mutable vertices: Map<int, Vertex> = Map.empty
    let addVertex (v: Vertex) = vertices <- if vertices.ContainsKey(v.Id) then vertices else vertices.Add(v.Id, v)
    let setSource vertexId = 
        vertices <- match vertices.TryFind(vertexId) with
                     | None -> vertices
                     | Some(v) -> vertices.Add(v.Id, {v with ShortestDistance = 0.0 })
        

    member this.AddVertex (v: Vertex) = addVertex v; this
    member this.GetVertices () = vertices |> Map.toList |> List.map snd
    member this.SetSource vertexId = setSource vertexId; this
    member this.GetVertex vertexId = vertices.[vertexId]


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
    }

let graph = (rawGraph
            |> Map.map makeVertex
            |> Map.fold (fun (graph: Graph) _ v -> graph.AddVertex(v)) (Graph())
            ).SetSource 1

let pq = PriorityQueue<Vertex>(graph.GetVertices(), false)

while not !pq.IsEmpty do
    let vertex = pq.Dequeue()
    for edge in vertex.Edges do
        let destination = graph.GetVertex edge.DestinationVertexId
        let newDistance = edge.Distance + vertex.ShortestDistance
        