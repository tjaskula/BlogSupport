type Tree<'T> =
  | Leaf of 'T
  | Node of Tree<'T> * Tree<'T>
    

let rec reduce t f =
  match t with
  | Leaf v -> v
  | Node (l, r) -> f (reduce l f) (reduce r f)
  
  
let generateTree height =
  let rec generateTreeInternal t i =
    if height = i then t
    else Node(generateTreeInternal t (i + 1), generateTreeInternal t (i + 1))
  
  let nodes = 2 * height - 1  
  if height <= 0 then failwith "The tree cannot have the height of 0 or less"
  else generateTreeInternal (Leaf 1) 1
  
  
type TreeVal<'T> =
  | LeafVal of 'T
  | NodeVal of TreeVal<'T> * 'T * TreeVal<'T>
  
  
let rec reduceVal t f =
  let getValue = function
    | LeafVal v -> v
    | NodeVal(_, v, _) -> v
  
  match t with
  | Leaf v -> LeafVal(v)
  | Node(l, r) ->
    let leftVal, rightVal = (reduceVal l f, reduceVal r f)
    NodeVal(leftVal, f (getValue leftVal) (getValue rightVal), rightVal)

open System.Threading.Tasks
 
let getValue = function
  | LeafVal v -> v
  | NodeVal(_, v, _) -> v
   
let rec upsweep t f =
  match t with
  | Leaf v -> LeafVal(v)
  | Node(l, r) ->
    let leftT = Task.Run(fun _ -> upsweep l f)
    let rightT = Task.Run(fun _ -> upsweep r f)
    Task.WaitAll(leftT, rightT)
    let leftVal, rightVal = leftT.Result, rightT.Result
    NodeVal(leftVal, f (getValue leftVal) (getValue rightVal), rightVal)
    
let rec downsweep t v0 f =
  match t with 
  | LeafVal v -> Leaf(f v0 v)
  | NodeVal (l, _, r) ->
    let leftT = Task.Run(fun _ -> downsweep l v0 f)
    let rightT = Task.Run(fun _ -> downsweep r (f v0 (getValue l)) f)
    Task.WaitAll(leftT, rightT)
    let left, right = leftT.Result, rightT.Result
    Node(left, right)

let rec prepend x = function 
  | Leaf v -> Node(Leaf x, Leaf v)
  | Node (l, r) -> Node(prepend x l, r)
    
let scanLeft t v0 f =
  let tVal = upsweep t f
  let scan = downsweep tVal v0 f
  prepend v0 scan