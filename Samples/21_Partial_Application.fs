module _21_Partial_Application

// define a adding function
let add x y = x + y

// normal use 
let z = add 1 2
printfn "%d" z

// Call the function with only one parameter.
// The result is a new function that has the “42” baked in, and now takes only one 
// parameter instead of two! This technique is called "partial application", and it means 
// that, for any function, you can "fix" some of the parameters and leave other ones open 
// to be filled in later.
let add42 = add 42

// use the new function
add42 2 |> printfn "%d"
add42 3 |> printfn "%d"

// Redoing the generic logger.
let genericLogger anyFunc input = 
   printfn "input is %A" input   //log the input
   let result = anyFunc input    //evaluate the function
   printfn "result is %A" result //log the result
   result                        //return the result

// Unfortunately, logging operations are hard-coded the. Ideally, I’d like to make this 
// more generic so that I can choose how logging is done.
// Of course, F# being a functional programming language, we will do this by passing 
// functions around.
// In this case we would pass "before" and "after" callback functions to the library 
// function, like this:
let genericLogger2 before after anyFunc input = 
   before input               //callback for custom behavior
   let result = anyFunc input //evaluate the function
   after result               //callback for custom behavior
   result                     //return the result

// You can see that the logging function now has four parameters. The "before" and "after" 
// actions are passed in as explicit parameters as well as the function and its input. To 
// use this in practice, we just define the functions and pass them in to the library 
// function along with the final int parameter:
let add1 input = input + 1

// reuse case 1
genericLogger2
    (fun x -> printf "before=%i. " x) // function to call before 
    (fun x -> printfn " after=%i." x) // function to call after
    add1                              // main function
    2                                 // parameter 
    |> ignore

// reuse case 2
genericLogger2
    (fun x -> printf "started with=%i " x) // different callback 
    (fun x -> printfn " ended with=%i" x) 
    add1                              // main function
    2                                 // parameter 
    |> ignore

// This is a lot more flexible. I don't have to create a new function every time I want 
// to change the behavior – I can define the behavior on the fly.
// But you might be thinking that this is a bit ugly. A library function might expose a 
// number of callback functions and it would be inconvenient to have to pass the same 
// functions in over and over.
// Luckily, we know the solution for this. We can use partial application to fix some of 
// the parameters. So in this case, let's define a new function which fixes the before 
// and after functions, as well as the add1 function, but leaves the final parameter open.
// define a reusable function with the "callback" functions fixed
let add1WithConsoleLogging = 
    genericLogger2
        (fun x -> printf "input=%i. " x) 
        (fun x -> printfn " result=%i" x)
        add1
        // last parameter NOT defined here yet!

// The new "wrapper" function is called with just an int now, so the code is much cleaner. 
// As in the earlier example, it can be used anywhere the original add1 function could be 
//used without any changes.
add1WithConsoleLogging 2 |> ignore
add1WithConsoleLogging 3 |> ignore
add1WithConsoleLogging 4 |> ignore
[1..5] |> List.map add1WithConsoleLogging |> ignore