module _79_Exercise_How_well_do_you_understand
// Here is a little exercise for you.

// Part 1 - create a workflow
// First, create a function that parses a string into a int:
let strToInt str = 
    match System.Int32.TryParse str with
    | true, v -> Some v
    | false, _ -> None

// and then create your own computation expression builder class (YourWorkflow) 
type YourWorkflow() =
        member this.Bind(m, f) = Option.bind f m
        member this.Return(x) = Some x

let yourWorkflow = new YourWorkflow()

// so that you can use it in a workflow, as shown below.
let stringAddWorkflow x y z = 
    yourWorkflow 
        {
        let! a = strToInt x
        let! b = strToInt y
        let! c = strToInt z
        return a + b + c
        }    

// test
let good = stringAddWorkflow "12" "3" "2"
printfn "%A" good
let bad = stringAddWorkflow "12" "xyz" "2"
printfn "%A" bad

// Part 2 – create a bind function
// Once you have the first part working, extend the idea by adding two more functions:
let strAdd str i = 
    yourWorkflow 
        {
        let! a = strToInt str
        return a + i
        }    

let (>>=) m f = Option.bind f m

// And then with these functions, you should be able to write code like this:
let good' = strToInt "1" >>= strAdd "2" >>= strAdd "3"
printfn "%A" good'
let bad' = strToInt "1" >>= strAdd "xyz" >>= strAdd "3"
printfn "%A" bad'