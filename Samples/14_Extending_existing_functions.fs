module _14_Extending_existing_functions

// Now say that we want to decorate these existing functions with some logging behavior. We can 
// compose these as well, to make a new function with the logging built in.

let add2 x = x + 2
let mult3 x = x * 3
let square x = x * x

// helper functions;
let logMsg msg x = printf "%s%i" msg x; x     //without linefeed 
let logMsgN msg x = printfn "%s%i" msg x; x   //with linefeed

// new composed function with new improved logging!
let mult3ThenSquareLogged = 
   logMsg "before=" 
   >> mult3 
   >> logMsg " after mult3=" 
   >> square
   >> logMsgN " result=" 

// test
mult3ThenSquareLogged 5 |> printfn "%d"
[1..10] 
    |> List.map mult3ThenSquareLogged // apply (prints and calculations) to a whole list
    |> ignore // ignore result

// Here is an example of using the composition operator to collapse a list of functions into a 
// single operation.
let listOfFunctions = [
   mult3; 
   square;
   add2;
   logMsgN "result=";
   ]

// compose all functions in the list into a single one
let allFunctions = List.reduce (>>) listOfFunctions 

//test
allFunctions 5 // apply calculations and print (functions order)
    |> ignore // ignore result
