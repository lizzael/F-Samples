module EmailAddressClient

open EmailAddress

// code works when using the published functions
let address1 = EmailAddress.create "x@example.com"
let address2 = EmailAddress.create "example.com"

// code that uses the internals of the type fails to compile
//let address3 = T.EmailAddress "bad email"

match address1 with
| Some v -> 
    let s = EmailAddress.value v
    printfn "%s" s
| None -> 
    printfn "Invalid"