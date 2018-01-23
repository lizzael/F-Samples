module _69_4_When_to_wrap_single_case_unions

// Now that we have the wrapper type, when should we construct them?
// Generally you only need to at service boundaries (for example, boundaries in a 
// hexagonal architecture: http://alistair.cockburn.us/Hexagonal+architecture )
// In this approach, wrapping is done in the UI layer, or when loading from a persistence 
// layer, and once the wrapped type is created, it is passed in to the domain layer and 
// manipulated "whole", as an opaque type. It is suprisingly uncommon that you actually 
// need the wrapped contents directly when working in the domain itself.
// As part of the construction, it is critical that the caller uses the provided 
// constructor rather than doing its own validation logic. This ensures that "bad" values 
// can never enter the domain.
// For example, here is some code that shows the UI doing its own validation:
//let processFormSubmit () = 
//    let s = uiTextBox.Text
//    if (s.Length < 50) 
//        then // set email on domain object
//        else // show validation error message        

// A better way is to let the constructor do it, as shown earlier.
//let processFormSubmit () = 
//    let emailOpt = uiTextBox.Text |> EmailAddress.create 
//    match emailOpt with
//    | Some email -> // set email on domain object
//    | None -> // show validation error message

// When to "unwrap" single case unions
// And when is unwrapping needed? Again, generally only at service boundaries. For 
// example, when you are persisting an email to a database, or binding to a UI element or 
// view model.
// One tip to avoid explicit unwrapping is to use the continuation approach again, 
// passing in a function that will be applied to the wrapped value.
// That is, rather than calling the "unwrap" function explicitly:
// address |> EmailAddress.value |> printfn "the value is %s" 
// You would pass in a function which gets applied to the inner value, like this:
// address |> EmailAddress.apply (printfn "the value is %s")
// Putting this together, we now have the complete EmailAddress module.

module EmailAddress = 
    type _T = EmailAddress of string

    // create with continuation
    let createWithCont success failure (s:string) = 
        if System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$") 
            then success (EmailAddress s)
            else failure "Email address must contain an @ sign"

    // create directly
    let create s = 
        let success e = Some e
        let failure _  = None
        createWithCont success failure s

    // unwrap with continuation
    let apply f (EmailAddress e) = f e

    // unwrap directly
    let value e = apply id e

// The create and value functions are not strictly necessary, but are added for the 
// convenience of callers.

