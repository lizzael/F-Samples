module _13_Functions_as_building_blocks
// Function composition and mini-languages make code more readable

// building blocks
let add2 x = x + 2
let mult3 x = x * 3
let square x = x * x

// test
[1..10] |> List.map add2 |> printfn "%A"
[1..10] |> List.map mult3 |> printfn "%A"
[1..10] |> List.map square |> printfn "%A" 

// new composed functions
// The ">>" operator is the composition operator. It means: do the first function, and then do 
// the second.
// Note how concise this way of combining functions is. There are no parameters, types or other 
// irrelevant noise.
let add2ThenMult3 = add2 >> mult3
let mult3ThenSquare = mult3 >> square 

// To be sure, the examples could also have been written less concisely and more explicitly as:
// let add2ThenMult3 x = mult3 (add2 x)
// let mult3ThenSquare x = square (mult3 x)

// But this more explicit style is also a bit more cluttered:

// In the explicit style, the x parameter and the parentheses must be added, even though they 
// don't add to the meaning of the code.
// And in the explicit style, the functions are written back-to-front from the order they are 
// applied. In my example of add2ThenMult3 I want to add 2 first, and then multiply. The 
// add2 >> mult3 syntax makes this visually clearer than mult3(add2 x).

// test compositions:
add2ThenMult3 5 |> printfn "%d"
mult3ThenSquare 5 |> printfn "%d"
[1..10] |> List.map add2ThenMult3 |> printfn "%A"
[1..10] |> List.map mult3ThenSquare |> printfn "%A"