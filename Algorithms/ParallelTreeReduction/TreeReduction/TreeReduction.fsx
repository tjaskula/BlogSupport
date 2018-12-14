
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
  else Node(generateTreeInternal (Node(Leaf 1, Leaf 2)) 1, generateTreeInternal (Node(Leaf 3, Leaf 4)) 1)