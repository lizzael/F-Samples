module _69_Single_case_union_types
// Adding meaning to primitive types

// At the end of the previous post, we had values for email addresses, zip codes, etc., 
// defined like this:
// EmailAddress: string;
// State: string;
// Zip: string;
// These are all defined as simple strings. But really, are they just strings? Is an 
// email address interchangable with a zip code or a state abbreviation?
// In a domain driven design, they are indeed distinct things, not just strings. So we 
// would ideally like to have lots of separate types for them so that they cannot 
// accidentally be mixed up.
// This has been known as good practice 
// http://codemonkeyism.com/never-never-never-use-string-in-java-or-at-least-less-often/
// for a long time, but in languages like C# and 
// Java it can be painful to create hundred of tiny types like this, leading to the so 
// called "primitive obsession" 
// http://sourcemaking.com/refactoring/primitive-obsession
// code smell.
// But F# there is no excuse! It is trivial to create simple wrapper types.

// Wrapping primitive types
// The simplest way to create a separate type is to wrap the underlying string type 
// inside another type.
// We can do it using single case union types, like so:
type EmailAddress = EmailAddress of string
type ZipCode = ZipCode of string
type StateCode = StateCode of string

// or alternatively, we could use record types with one field, like this:
type EmailAddress' = { EmailAddress: string }
type ZipCode' = { ZipCode: string }
type StateCode' = { StateCode: string}
// Both approaches can be used to create wrapper types around a string or other primitive 
// type, so which way is better?
// The answer is generally the single case discriminated union. It is much easier to 
// "wrap" and "unwrap", as the "union case" is actually a proper constructor function in 
// its own right. Unwrapping can be done using inline pattern matching.
// Here's some examples of how an EmailAddress type might be constructed and 
// deconstructed:
// using the constructor as a function
"a" |> EmailAddress
["a"; "b"; "c"] |> List.map EmailAddress

// inline deconstruction
let a' = "a" |> EmailAddress
let (EmailAddress a'') = a'

let addresses = 
    ["a"; "b"; "c"] 
    |> List.map EmailAddress

let addresses' = 
    addresses
    |> List.map (fun (EmailAddress e) -> e)

// You can't do this as easily using record types.
// So, let's refactor the code again to use these union types. It now looks like this:
type PersonalName = 
    {
    FirstName: string;
    MiddleInitial: string option;
    LastName: string;
    }

type EmailContactInfo = 
    {
    EmailAddress: EmailAddress;
    IsEmailVerified: bool;
    }

type PostalAddress = 
    {
    Address1: string;
    Address2: string;
    City: string;
    State: StateCode;
    Zip: ZipCode;
    }

type PostalContactInfo = 
    {
    Address: PostalAddress;
    IsAddressValid: bool;
    }

type Contact = 
    {
    Name: PersonalName;
    EmailContactInfo: EmailContactInfo;
    PostalContactInfo: PostalContactInfo;
    }

// Another nice thing about the union type is that the implementation can be encapsulated 
// with module signatures, as we'll discuss below.

// Naming the "case" of a single case union
// In the examples above we used the same name for the case as we did for the type:
//type EmailAddress = EmailAddress of string
//type ZipCode = ZipCode of string
//type StateCode = StateCode of string
// This might seem confusing initially, but really they are in different scopes, so there 
// is no naming collision. One is a type, and one is a constructor function with the same 
// name.
// So if you see a function signature like this:
// val f: string -> EmailAddress
// this refers to things in the world of types, so EmailAddress refers to the type.
// On the other hand, if you see some code like this:
// let x = EmailAddress y
// this refers to things in the world of values, so EmailAddress refers to the 
// constructor function.

// Constructing single case unions
// For values that have special meaning, such as email addresses and zip codes, generally 
// only certain values are allowed. Not every string is an acceptable email or zip code.
// This implies that we will need to do validation at some point, and what better point 
// than at construction time? After all, once the value is constructed, it is immutable, 
// so there is no worry that someone might modify it later.
// Here's how we might extend the above module with some constructor functions:
// ... types as above ...
let CreateEmailAddress (s:string) = 
    if System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$") 
        then Some (EmailAddress s)
        else None

let CreateStateCode (s:string) = 
    let s' = s.ToUpper()
    let stateCodes = ["AZ";"CA";"NY"] //etc
    if stateCodes |> List.exists ((=) s')
        then Some (StateCode s')
        else None

// We can test the constructors now:
CreateStateCode "CA" |> printfn "%A"
CreateStateCode "XX" |> printfn "%A"

CreateEmailAddress "a@example.com" |> printfn "%A"
CreateEmailAddress "example.com" |> printfn "%A"

// Handling invalid input in a constructor
// With these kinds of constructor functions, one immediate challenge is the question of 
// how to handle invalid input. For example, what should happen if I pass in "abc" to the 
// email address constructor?
// There are a number of ways to deal with it.
// First, you could throw an exception. I find this ugly and unimaginative, so I'm 
// rejecting this one out of hand!
// Next, you could return an option type, with None meaning that the input wasn't valid. 
// This is what the constructor functions above do.
// This is generally the easiest approach. It has the advantage that the caller has to 
// explicitly handle the case when the value is not valid.
// For example, the caller's code for the example above might look like:
match (CreateEmailAddress "a@example.com") with
| Some email -> printfn "%A" email
| None -> printfn "Ïnvalid email"

// The disadvantage is that with complex validations, it might not be obvious what went 
// wrong. Was the email too long, or missing a '@' sign, or an invalid domain? We can't 
// tell.
// If you do need more detail, you might want to return a type which contains a more 
// detailed explanation in the error case.
// The following example uses a CreationResult type to indicate the error in the failure 
// case.
type CreationResult<'T> = Success of 'T | Error of string            

let CreateEmailAddress2 (s:string) = 
    if System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$") 
        then Success (EmailAddress s)
        else Error "Email address must contain an @ sign"

// test
CreateEmailAddress2 "example.com"

// Finally, the most general approach uses continuations. That is, you pass in two functions, one for the success case (that takes the newly constructed email as parameter), and another for the failure case (that takes the error string as parameter).
let CreateEmailAddressWithContinuations success failure (s:string) = 
    if System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$") 
        then success (EmailAddress s)
        else failure "Email address must contain an @ sign"

// The success function takes the email as a parameter and the error function takes a 
// string. Both functions must return the same type, but the type is up to you.
// Here is a simple example -- both functions do a printf, and return nothing (i.e. unit).
let success (EmailAddress s) = printfn "success creating email %s" s        
let failure  msg = printfn "error creating email: %s" msg
CreateEmailAddressWithContinuations success failure "example.com"
CreateEmailAddressWithContinuations success failure "x@example.com"

// With continuations, you can easily reproduce any of the other approaches. Here's the 
// way to create options, for example. In this case both functions return an EmailAddress 
// option.
let success' e = Some e
let failure' _  = None
CreateEmailAddressWithContinuations success' failure' "example.com"
CreateEmailAddressWithContinuations success' failure' "x@example.com"

// And here is the way to throw exceptions in the error case:
let success'' e = e
let failure'' _  = failwith "bad email address"
CreateEmailAddressWithContinuations success'' failure'' "example.com"
CreateEmailAddressWithContinuations success'' failure'' "x@example.com"

// This code seems quite cumbersome, but in practice you would probably create a local 
// partially applied function that you use instead of the long-winded one.
// setup a partially applied function
let success''' e = Some e
let failure''' _  = None
let createEmail = CreateEmailAddressWithContinuations success''' failure'''

// use the partially applied function
createEmail "x@example.com"
createEmail "example.com"

// Creating modules for wrapper types
// These simple wrapper types are starting to get more complicated now that we are adding 
// validations, and we will probably discover other functions that we want to associate 
// with the type.
// So it is probably a good idea to create a module for each wrapper type, and put the 
// type and its associated functions there.
module EmailAddress = 
    type T = EmailAddress of string
    // wrap
    let create (s:string) = 
        if System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$") 
            then Some (EmailAddress s)
            else None
    // unwrap
    let value (EmailAddress e) = e

// The users of the type would then use the module functions to create and unwrap the 
// type. For example:

// create email addresses
let address1 = EmailAddress.create "x@example.com"
let address2 = EmailAddress.create "example.com"

// unwrap an email address
match address1 with
| Some e -> EmailAddress.value e |> printfn "the value is %s"
| None -> ()

// Forcing use of the constructor
// One issue is that you cannot force callers to use the constructor. Someone could just 
// bypass the validation and create the type directly.
// In practice, that tends not to be a problem. One simple techinique is to use naming 
// conventions to indicate a "private" type, and provide "wrap" and "unwrap" functions so 
// that the clients never need to interact with the type directly.
// Here's an example:
module EmailAddress' = 
    // private type
    type _T = EmailAddress of string
    // wrap
    let create (s:string) = 
        if System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$") 
            then Some (EmailAddress s)
            else None
    
    // unwrap
    let value (EmailAddress e) = e

// Of course the type is not really private in this case, but you are encouraging the 
// callers to always use the "published" functions.
// If you really want to encapsulate the internals of the type and force callers to use a 
// constructor function, you can use module signatures.
// (Note that module signatures only work in compiled projects, not in interactive 
// scripts, so to test this, you will need to create three files in an F# project, 
// with the filenames as shown here.)
// Files:
// - EmailAddress.fsi
// - EmailAddress.fs
// - EmailAddressClient.fs
// The type EmailAddress.T exported by the module signature is opaque, so clients cannot 
// access the internals.
// As you can see, this approach enforces the use of the constructor. Trying to create 
// the type directly (T.EmailAddress "bad email") causes a compile error.