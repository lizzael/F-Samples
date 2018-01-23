module _37_Expression_evaluation_order
// Understanding expression evaluation order

// In F#, expressions are evaluated from the "inside out" -- that is, as soon as a 
// complete subexpression is "seen", it is evaluated.
// Have a look at the following code and try to guess what will happen, then evaluate the 
// code and see.

// create a clone of if-then-else
let test1 b t f = if b then t else f

// call it with two different choices
test1 true (printfn "true") (printfn "false")

// What happens is that both "true" and "false" are printed, even though the test function 
// will never actually evaluate the "else" branch. Why? Because the (printfn "false") 
// expression is evaluated immediately, regardless of how the test function will be using 
// it.
// This style of evaluation is called "eager". It has the advantage that it is easy to 
// understand, but it does mean that it can be inefficient on occasion.
// The alternative style of evaluation is called "lazy", whereby expressions are only 
// evaluated when they are needed. The Haskell language follows this approach, so a 
// similar example in Haskell would only print "true".
// In F#, there are a number of techniques to force expressions not to be evaluated 
// immediately. The simplest it to wrap it in a function that only gets evaluated on 
// demand:

// create a clone of if-then-else that accepts functions rather than simple values
let test2 b t f = if b then t() else f()

// call it with two different functions
test2 true (fun () -> printfn "true") (fun () -> printfn "false")

// The problem with this is that now the "true" function might be evaluated twice by 
// mistake, when we only wanted to evaluate it once!
// So, the preferred way for expressions not to be evaluated immediately is to use the 
// Lazy<> wrapper.

// create a clone of if-then-else with no restrictions...
let test3 b t f = if b then t else f

// ...but call it with lazy values
let f = test3 true (lazy (printfn "true")) (lazy (printfn "false"))

// The final result value f is also a lazy value, and can be passed around without being 
// evaluated until you are finally ready to get the result.
f.Force()     // use Force() to force the evaluation of a lazy value

// If you never need the result, and never call Force(), then the wrapped value will 
// never be evaluated.
// There will much more on laziness in an upcoming series on performance.