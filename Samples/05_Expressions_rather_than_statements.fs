module _05_Expressions_rather_than_statements

open System

// In functional languages, there are no statements, only expressions. That is, 
// every chunk of code always returns a value, and larger chunks are created by 
// combining smaller chunks using composition rather than a serialized list of 
// statements.

let aBool = true
let result = if aBool then 42 else 0

result |> printf "%d" 