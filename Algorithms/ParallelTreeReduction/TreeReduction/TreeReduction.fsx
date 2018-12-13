
type Tree<'a> =
  | Leaf of value: 'a
  | Node of left: Tree<'a> * right: Tree<'a>
    

let rec reduce t f =
  match t with
  | Leaf v -> v
  | Node (l, r) -> f (reduce l f) (reduce r f) 