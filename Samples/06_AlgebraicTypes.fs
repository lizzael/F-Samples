module _06_AlgebraicTypes

// For example, given existing types int and bool, we can create a new product 
// type that must have one of each:

//declare it
type IntAndBool = {intPart: int; boolPart: bool}

//use it
let x = {intPart=1; boolPart=false}

// Alternatively, we can create a new union/sum type that has a choice between each type:

//declare it
type IntOrBool = 
  | IntChoice of int
  | BoolChoice of bool

//use it
let y = IntChoice 42
let z = BoolChoice true

let a = y
let b = z

y |> printf "%A"
z |> printf "%A"

a |> printf "%A"
b |> printf "%A"
