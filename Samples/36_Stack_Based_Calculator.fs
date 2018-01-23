module _46_Stack_Based_Calculator

// ==============================================
// Types
// ==============================================

type Stack = StackContents of float list

// ==============================================
// Stack primitives
// ==============================================

/// Push a value on the stack
let push x (StackContents contents) =   
    StackContents (x::contents)

/// Pop a value from the stack and return it 
/// and the new stack as a tuple
let pop (StackContents contents) = 
    match contents with 
    | top::rest -> 
        let newStack = StackContents rest
        (top,newStack)
    | [] -> 
        failwith "Stack underflow"

// ==============================================
// Operator core
// ==============================================

// pop the top two elements
// do a binary operation on them
// push the result 
let binary mathFn stack = 
    let y,stack' = pop stack    
    let x,stack'' = pop stack'  
    let z = mathFn x y
    push z stack''      

// pop the top element
// do a unary operation on it
// push the result 
let unary f stack = 
    let x,stack' = pop stack  
    push (f x) stack'   

// ==============================================
// Other core 
// ==============================================

/// Pop and show the top value on the stack
let SHOW stack = 
    let x,_ = pop stack
    printfn "The answer is %f" x
    stack  // keep going with same stack

/// Duplicate the top value on the stack
let DUP stack = 
    let x,s = pop stack  
    push x (push x s)   
    
/// Swap the top two values
let SWAP stack = 
    let x,s = pop stack  
    let y,s' = pop s
    push y (push x s')   

/// Drop the top value on the stack
let DROP stack = 
    let _,s = pop stack  //pop the top of the stack
    s                    //return the rest

// ==============================================
// Words based on primitives
// ==============================================

// Constants
// -------------------------------
let EMPTY = StackContents []
let START  = EMPTY

// Numbers
// -------------------------------
let ONE = push 1.0
let TWO = push 2.0
let THREE = push 3.0
let FOUR = push 4.0
let FIVE = push 5.0

// Math functions
// -------------------------------

let ADD = binary (+)

//let ADD stack =
//   let x,s = pop stack  //pop the top of the stack
//   let y,s2 = pop s     //pop the result stack
//   let result = x + y   //do the math
//   push result s2       //push back on the doubly-popped stack

// let ADD aStack = binary (fun x y -> x + y) aStack 

// let ADD aStack = binary (+) aStack 

let MUL = binary (*)

//let MUL stack = 
//   let x,s = pop stack  //pop the top of the stack
//   let y,s2 = pop s     //pop the result stack
//   let result = x * y   //do the math 
//   push result s2       //push back on the doubly-popped stack

let SUB = binary (-)
let DIV = binary (/)

let NEG = unary (fun x -> -x)

// ==============================================
// Words based on composition
// ==============================================

let SQUARE =  
    DUP >> MUL 

let CUBE = 
    DUP >> DUP >> MUL >> MUL 

let SUM_NUMBERS_UPTO = 
    DUP      // n, n           2 items on stack
    >> ONE   // n, n, 1        3 items on stack  
    >> ADD   // n, (n+1)       2 items on stack
    >> MUL   // n(n+1)         1 item on stack
    >> TWO   // n(n+1), 2      2 items on stack  
    >> DIV   // n(n+1)/2       1 item on stack

// With this simple function in place, we can easily define an operation that pushes a 
// particular number onto the stack.
// let ONE stack = push 1.0 stack
// let TWO stack = push 2.0 stack

// Let’s test all of these now:
let stackWith1 = ONE EMPTY 
let stackWith2 = TWO stackWith1
let stackWith3  = THREE stackWith2 

// These intermediate stacks are annoying — can we get rid of them? Yes! Note that these 
// functions ONE, TWO, THREE all have the same signature:
// Stack -> Stack
// This means that they can be chained together nicely! The output of one can be fed into 
// the input of the next, as shown below:
let result123 = EMPTY |> ONE |> TWO |> THREE 
let result312 = EMPTY |> THREE |> ONE |> TWO

// pop tests
let initialStack = EMPTY |> ONE |> TWO 
let popped1, poppedStack = pop initialStack
let popped2, poppedStack2 = pop poppedStack
// let _ = pop EMPTY

// add and mul tests
let add1and2 = EMPTY |> ONE |> TWO |> ADD
let add2and3 = EMPTY |> TWO |> THREE |> ADD
let mult2and3 = EMPTY |> TWO |> THREE |> MUL

// more tests
let threeDivTwo = EMPTY |> THREE |> TWO |> DIV   // Answer: 1.5
let twoSubtractFive = EMPTY |> TWO |> FIVE |> SUB  // Answer: -3.0
let oneAddTwoSubThree = EMPTY |> ONE |> TWO |> ADD |> THREE |> SUB // Answer: 0.0

// unary functions tests
let neg3 = EMPTY |> THREE |> NEG
let square2 = EMPTY |> TWO |> SQUARE

// So now finally, we can write the code example from the original requirements
EMPTY |> ONE |> THREE |> ADD |> TWO |> MUL |> SHOW // (1+3)*2 = 8

// nice examples:
START
    |> ONE |> TWO |> SHOW

START
    |> ONE |> TWO |> ADD |> SHOW 
    |> THREE |> ADD |> SHOW 

START
    |> THREE |> DUP |> DUP |> MUL |> MUL |> SHOW// 27

START
    |> ONE |> TWO |> ADD |> SHOW  // 3
    |> THREE |> MUL |> SHOW       // 9
    |> TWO |> DIV |> SHOW         // 9 div 2 = 4.5

// composition
// define a new function
let ONE_TWO_ADD = 
    ONE >> TWO >> ADD 

// test it
START |> ONE_TWO_ADD |> SHOW

// define a new function
let SQUARE2 = 
    DUP >> MUL 

// test it
START |> TWO |> SQUARE2 |> SHOW

// test it
START |> THREE |> CUBE |> SHOW

// define a new function
// test it with sum of numbers up to 9
START |> THREE |> SQUARE |> SUM_NUMBERS_UPTO |> SHOW  // 45

// Pipes vs Composition
// The difference is that piping is, in a sense, a "realtime transformation" operation. 
// When you use piping you are actually doing the operations right now, passing a 
// particular stack around.
// On the other hand, composition is a kind of "plan" for what you want to do, building 
// an overall function from a set of parts, but not actually running it yet.
// So for example, I can create a "plan" for how to square a number by combining smaller 
// operations:
let COMPOSED_SQUARE = DUP >> MUL 
// I cannot do the equivalent with the piping approach.
// let PIPED_SQUARE = DUP |> MUL 
// This causes a compilation error. I have to have some sort of concrete stack instance 
// to make it work:
let stack2 = EMPTY |> TWO
let twoSquared = stack2 |> DUP |> MUL 
// And even then, I only get the answer for this particular input, not a plan for all 
// possible inputs, as in the COMPOSED_SQUARE example.
// The other way to create a "plan" is to explicitly pass in a lambda to a more primitive 
// function, as we saw near the beginning:
let LAMBDA_SQUARE = unary (fun x -> x * x)
// This is much more explicit (and is likely to be faster) but loses all the benefits and 
// clarity of the composition approach.
// So, in general, go for the composition approach if you can!