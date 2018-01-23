﻿module _49_Records
// Extending tuples with labels

// As we noted in the previous post, plain tuples are useful in many cases. But they have 
// some disadvantages too. Because all tuple types are pre-defined, you can't distinguish 
// between a pair of floats used for geographic coordinates say, vs. a similar tuple used 
// for complex numbers. And when tuples have more than a few elements, it is easy to get 
// confused about which element is in which place.
// In these situations, what you would like to do is label each slot in the tuple, which 
// will both document what each element is for and force a distinction between tuples made 
// from the same types.
// Enter the "record" type. A record type is exactly that, a tuple where each element is 
// labeled.
type ComplexNumber = { real: float; imaginary: float }
type GeoCoord = { lat: float; long: float }

// A record type has the standard preamble: type [typename] = followed by curly braces. 
// Inside the curly braces is a list of label: type pairs, separated by semicolons 
// (remember, all lists in F# use semicolon separators -- commas are for tuples).
// Let's compare the "type syntax" for a record type with a tuple type:
type ComplexNumberRecord = { real: float; imaginary: float }
type ComplexNumberTuple = float * float

// In the record type, there is no "multiplication", just a list of labeled types.
// Relational database theory uses a similar "record type" concept. In the relational 
// model, a relation is a (possibly empty) finite set of tuples all having the same finite 
// set of attributes. This set of attributes is familiarly referred to as the set of 
// column names.

// Making and matching records
// To create a record value, use a similar format to the type definition, but using equals 
// signs after the labels. This is called a "record expression."
let myComplexNumber = { real = 1.1; imaginary = 2.2 } // use equals!
let myGeoCoord = { lat = 1.1; long = 2.2 }  // use equals in let

// And to "deconstruct" a record, use the same syntax:
let { lat=myLat; long=myLong } = myGeoCoord  // "deconstruct"

// As always, if you don't need some of the values, you can use the underscore as a 
// placeholder; or more cleanly, just leave off the unwanted label altogether.
let { lat=_; long=myLong2 } = myGeoCoord  // "deconstruct"
let { long=myLong3 } = myGeoCoord         // "deconstruct"

// If you just need a single property, you can use dot notation rather than pattern 
// matching.
let x = myGeoCoord.lat
let y = myGeoCoord.long

// Note that you can leave a label off when deconstructing, but not when constructing:
// let myGeoCoord = { lat = 1.1; }  // error FS0764: No assignment
                                    // given for field 'long' 
// One of the most noticeable features of record types is use of curly braces. Unlike 
// C-style languages, curly braces are rarely used in F# -- only for records, sequences, 
// computation expressions (of which sequences are a special case), and object expressions 
// (creating implementations of interfaces on the fly). These other uses will be discussed 
// later.

// Label order
// Unlike tuples, the order of the labels is not important. So the following two values 
// are the same:
let myGeoCoordA = { lat = 1.1; long = 2.2 }    
let myGeoCoordB = { long = 2.2; lat = 1.1 }   // same as above

// Naming conflicts
// In the examples above, we could construct a record by just using the label names "lat" 
// and "long". Magically, the compiler knew what record type to create. (Well, in truth, 
// it was not really that magical, as only one record type had those exact labels.)
// But what happens if there are two record types with the same labels? How can the 
// compiler know which one you mean? The answer is that it can't -- it will use the most 
// recently defined type, and in some cases, issue a warning. Try evaluating the 
// following:
type Person1 = {first:string; last:string}
type Person2 = {first:string; last:string}
let p = {first="Alice"; last="Jones"}  

// What type is p? Answer: Person2, which was the last type defined with those labels.
// And if you try to deconstruct, you will get a warning about ambiguous field labels.
let {first=f; last=l} = p
// How can you fix this? Simply by adding the type name as a qualifier to at least one of 
// the labels.
let p' = {Person1.first="Alice"; last="Jones"}
let { Person1.first=f'; last=l'} = p'
// If needed, you can even add a fully qualified name (with namespace). Here's an example 
// using modules.
module Module1 = 
  type Person = {first:string; last:string}

module Module2 = 
  type Person = {first:string; last:string}

module Module3 = 
  let p = {Module1.Person.first="Alice"; 
           Module1.Person.last="Jones"}
// Of course, if you can ensure there is only one version in the local namespace, you can 
// avoid having to do this at all.
module Module3b = 
  open Module1                   // bring into the local namespace
  let p = {first="Alice"; last="Jones"}  // will be Module1.Person
// The moral of the story is that when defining record types, you should try to use unique 
// labels if possible, otherwise you will get ugly code at best, and unexpected behavior 
// at worst.
// Note that in F#, unlike some other functional languages, two types with exactly the 
// same structural definition are not the same type. This is called a "nominal" type 
// system, where two types are only equal if they have the same name, as opposed to a 
// "structural" type system, where definitions with identical structures will be the same 
// type regardless of what they are called.

// Using records in practice
// How can we use records? Let us count the ways...

// Using records for function results
// Just like tuples, records are useful for passing back multiple values from a function. 
// Let's revisit the tuple examples described earlier, rewritten to use records instead:

// the tuple version of TryParse
let tryParseTuple intStr = 
   try
      let i = System.Int32.Parse intStr
      (true,i)
   with _ -> (false,0)  // any exception

// for the record version, create a type to hold the return result
type TryParseResult = {success:bool; value:int} 

// the record version of TryParse
let tryParseRecord intStr = 
   try
      let i = System.Int32.Parse intStr
      {success=true;value=i}
   with _ -> {success=false;value=0}  

//test it
tryParseTuple "99" |> printfn "%A"
tryParseRecord "99" |> printfn "%A"
tryParseTuple "abc" |> printfn "%A"
tryParseRecord "abc" |> printfn "%A"

// You can see that having explicit labels in the return value makes it much easier to 
// understand (of course, in practice we would probably use an Option type, discussed 
// later).
// And here's the word and letter count example using records rather than tuples:

//define return type
type WordAndLetterCountResult = {wordCount:int; letterCount:int} 

let wordAndLetterCount (s:string) = 
   let words = s.Split [|' '|]
   let letterCount = words |> Array.sumBy (fun word -> word.Length ) 
   {wordCount=words.Length; letterCount=letterCount}

//test
wordAndLetterCount "to be or not to be" |> printfn "%A"

// Creating records from other records
// Again, as with most F# values, records are immutable and the elements within them 
// cannot be assigned to. So how do you change a record? Again the answer is that you 
// can't -- you must always create a new one.
// Say that you need to write a function that, given a GeoCoord record, adds one to each 
// element. Here it is:
let addOneToGeoCoord aGeoCoord =
   let {lat=x; long=y} = aGeoCoord
   {lat = x + 1.0; long = y + 1.0}   // create a new one

// try it
addOneToGeoCoord {lat=1.1; long=2.2} |> printfn "%A"

// But again you can simplify by deconstructing directly in the parameters of a function, 
// so that the function becomes a one liner:
let addOneToGeoCoord' {lat=x; long=y} = {lat=x+1.0; long=y+1.0}

// try it
addOneToGeoCoord' {lat=1.0; long=2.0} |> printfn "%A"
// or depending on your taste, you can also use dot notation to get the properties:
let addOneToGeoCoord'' aGeoCoord =
   {lat=aGeoCoord.lat + 1.0; long= aGeoCoord.long + 1.0}   

// In many cases, you just need to tweak one or two fields and leave all the others alone. 
// To make life easier, there is a special syntax for this common case, the "with" keyword.
// You start with the original value, followed by "with" and then the fields you want to 
// change. Here are some examples:
let g1 = {lat=1.1; long=2.2}
let g2 = {g1 with lat=99.9}   // create a new one

let p1 = {first="Alice"; last="Jones"}  
let p2 = {p1 with last="Smith"}  

// The technical term for "with" is a copy-and-update record expression.

// Record equality
// Like tuples, records have an automatically defined equality operation: two records are 
// equal if they have the same type and the values in each slot are equal.
// And records also have an automatically defined hash value based on the values in the 
// record, so that records can be used as dictionary keys without problems.
{first="Alice"; last="Jones"}.GetHashCode() |> printfn "%d"

// Record representation
// As noted in a previous post, records have a nice default string representation, and can 
// be serialized easily. But unlike tuples, the ToString() representation is unhelpful.
printfn "%A" {first="Alice"; last="Jones"}   // nice
printfn "%s" ({first="Alice"; last="Jones"}.ToString())     // ugly
printfn "%O" {first="Alice"; last="Jones"}   // ugly

// Sidebar: %A vs. %O in print format strings
// We just saw that print format specifiers %A and %O produce very different results for 
// the same record:
printfn "%A" {first="Alice"; last="Jones"}
printfn "%O" {first="Alice"; last="Jones"}

// So why the difference?
// %A prints the value using the same pretty printer that is used for interactive output. 
// But %O uses Object.ToString(), which means that if the ToString method is not 
// overridden, %O will give the default (and generally unhelpful) output. So in general, 
// you should try to use %A to %O where possible, because the core F# types do have 
// pretty-printing by default.
// But note that the F# "class" types do not have a standard pretty printed format, so %A 
// and %O are equally uncooperative unless you override ToString.