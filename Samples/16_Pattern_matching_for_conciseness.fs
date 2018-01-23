module _16_Pattern_matching_for_conciseness
// Pattern matching can match and bind (the functional equivalent of assigning to variables) 
// in a single step.

// Pattern matching can compare expressions in a number of ways, matching on values, 
// conditions, and types, and then assign or extract values, all at the same time.

// In the following examples, we are binding to the internal members of tuples and lists 
// directly:

//matching tuples directly
let firstPart, secondPart, _ =  (1,2,3)  // underscore means ignore

//matching lists directly
//let elem1::elem2::rest = [1..10]       // ignore the warning for now

//[elem1; elem2] |> printfn "%A"

//matching lists inside a match..with
let listMatcher aList = 
    match aList with
    | [] -> printfn "the list is empty" 
    | [firstElement] -> printfn "the list has one element %A " firstElement 
    | [first; second] -> printfn "list is %A and %A" first second 
    | _ -> printfn "the list has more than two elements"

listMatcher [1;2;3;4]
listMatcher [1;2]
listMatcher [1]
listMatcher []

// You can also bind values to the inside of complex structures such as records. In the 
// following example, we will create an "Address" type, and then a "Customer" type which 
// contains an address. Next, we will create a customer value, and then match various 
// properties against it.

// create some types
type Address = { Street: string; City: string; }   
type Customer = { ID: int; Name: string; Address: Address}   

// create a customer 
let customer1 = { ID = 1; Name = "Bob"; 
      Address = {Street="123 Main"; City="NY" } }     

// extract name only
let { Name=name1 } =  customer1 
printfn "The customer is called %s" name1

// extract name and id 
let { ID=id2; Name=name2; } =  customer1 
printfn "The customer called %s has id %i" name2 id2

// extract name and address
// note how we could reach right into the Address substructure and pull out the street as well 
// as the customer name.
let { Name=name3;  Address={Street=street3}  } =  customer1   
printfn "The customer is called %s and lives on %s" name3 street3

// This ability to process a nested structure, extract only the fields you want, and assign 
// them to values, all in a single step, is very useful. It removes quite a bit of coding 
// drudgery, and is another factor in the conciseness of typical F# code.