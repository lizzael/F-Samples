﻿module _46_Overview_of_types
// A look at the big picture

// Before we dive into all the specific types, let's look at the big picture.

// What are types for?
// If you are coming from an object-oriented design background, one of the paradigm shifts 
// involved in "thinking functionally" is to change how you think about types.
// A well designed object-oriented program will have a strong focus on behavior rather 
// than data, so it will use a lot of polymorphism, either using "duck-typing" or explicit 
// interfaces, and will try to avoid having explicit knowledge of the actual concrete 
// classes being passed around.
// A well designed functional program, on the other hand, will have a strong focus on data 
// types rather than behavior. F# puts much more emphasis on designing types correctly 
// than an imperative language such as C#, and many of the examples in this series and 
// later series will focus on creating and refining type definitions.
// So what is a type? Types are surprisingly hard to define. One definition from a well 
// known textbook says:
// "A type system is a tractable syntactic method of proving the absence of certain 
// program behaviors by classifying phrases according to the kinds of values they compute" 
// (Benjamin Pierce, Types and Programming Languages)
// Ok, that definition is a bit technical. So let's turn it around -- what do we use types 
// for in practice? In the context of F#, you can think of types as being used in two main 
// ways:
// - Firstly, as an annotation to a value that allows certain checks to be made, 
//   especially at compile time. In other words, types allow you to have "compile time 
//   unit tests".
// - Second, as domains for functions to act upon. That is, a type is a sort of data 
//   modeling tool that allows you to represent a real world domain in your code.
// These two definitions interact. The better the type definitions reflect the real-world 
// domain, the better they will statically encode the business rules. And the better they 
// statically encode the business rules, the better the "compile time unit tests" work. In 
// the ideal scenario, if your program compiles, then it really is correct!

// What kinds of types are there?
// F# is a hybrid language, so it has a mixture of types: some from its functional 
// background, and some from its object-oriented background.
// Generally, the types in F# can be grouped into the following categories:
// - Common .NET types. These are types that conform to the .NET Common Language 
//   Infrastructure (CLI), and which are easily portable to every .NET language.
// - F# specific types. These are types that are part of the F# language and are designed 
//   for pure functional programming.
// If you are familiar with C#, you will know all the CLI types. They include:
// - Built-in value types (int, bool, etc).
// - Built-in reference types (string, etc).
// - User-defined value types (enum and struct).
// - Classes and interfaces
// - Delegates
// - Arrays
// - The F# specific types include:
// - Function types (not the same as delegates or C# lambdas)
// - The unit type
// - Tuples (now part of .NET 4.0)
// - Records
// - Discriminated Unions
// - Option types
// - Lists (not the same as the .NET List class)
// I strongly recommend that when creating new types you stick with the F# specific types 
// rather than using classes. They have a number of advantages over the CLI types, such 
// as:
// - They are immutable
// - They cannot be null
// - They have built-in structural equality and comparison
// - They have built-in pretty printing

// Sum and Product types
// The key to understanding the power of types in F# is that most new types are 
// constructed by from other types using two basic operations: sum and product.
// That is, in F# you can define new types almost as if you were doing algebra:
// - define typeZ = typeX "plus" typeY
// - define typeW = typeX "times" typeZ
// I will hold off explaining what sum and product mean in practice until we get to the 
// detailed discussion of tuples (products) and discriminated union (sum) types later in 
// this series.
// The key point is that an infinite number of new types can be made by combining existing 
// types together using these "product" and "sum" methods in various ways. Collectively 
// these are called "algebraic data types" or ADTs (not to be confused with abstract data 
// types, also called ADTs). Algebraic data types can be used to model anything, including 
// lists, trees, and other recursive types.
// The sum or "union" types, in particular, are very valuable, and once you get used to 
// them, you will find them indispensible!

// How types are defined
// Every type definition is similar, even though the specific details may vary. All type 
// definitions start with a "type" keyword, followed by an identifier for the type, 
// followed by any generic type parameters, followed by the definition. For example, here 
// are some type definitions for a variety of types:
type A = int * int
type B = {FirstName:string; LastName:string}
type C = Circle of int | Rectangle of int * int
type D = Day | Month | Year
type E<'a> = Choice1 of 'a | Choice2 of 'a * 'a

type MyClass(initX:int) =
   let x = initX
   member this.Method() = printf "x=%i" x

// There is a special syntax for defining new types that is different from the normal 
// expression syntax. So do be aware of this difference.
// Types can only be declared in namespaces or modules. But that doesn't mean you always 
// have to create them at the top level -- you can create types in nested modules if you 
// need to hide them.
module sub = 
    // type declared in a module
    type A = int * int

    module private helper = 
        // type declared in a submodule
        type B = B of string list

        //internal access is allowed
        let b = B ["a";"b"]

//outside access not allowed
// let b = sub.helper.B ["a";"b"] // Error!!!

// Types cannot be declared inside functions.
// let f x = 
//     type A = int * int  //unexpected keyword "type"
//     x * x

// Constructing and deconstructing types
// After a type is defined, instances of the type are created using a "constructor" 
// expression that often looks quite similar to the type definition itself.
let a = (1,1)
let b = { FirstName="Bob"; LastName="Smith" } 
let c = Circle 99
let c' = Rectangle (2,1)
let d = Month
let e = Choice1 "a"
let myVal = MyClass 99
myVal.Method()

// What is interesting is that the same "constructor" syntax is also used to "deconstruct" 
// the type when doing pattern matching:
let (a1,a2) = a                                // "deconstruct"

let { FirstName = b1 } = b                     // "deconstruct" 

match c with                                   
| Circle c1 -> printf "circle of radius %i" c1 // "deconstruct"
| Rectangle (c2,c3) -> printf "%i %i" c2 c3    // "deconstruct"

match c' with                                   
| Circle c1 -> printf "circle of radius %i" c1 // "deconstruct"
| Rectangle (c2,c3) -> printf "%i %i" c2 c3    // "deconstruct"

// As you read through this series, pay attention to how the constructors are used in 
// both ways.

// Field guide to the "type" keyword
// The same "type" keyword is used to define all the F# types, so they can all look very 
// similar if you are new to F#. Here is a quick list of these types and how to tell the 
// difference between them.

// Types
// - Abbrev (Alias): Uses equal sign only.
type ProductCode = string
type transform<'a> = 'a -> 'a   
// - Tuple: Not explicitly defined with type keyword. Always available to be used and are 
// not explicitly defined with the type keyword. Usage indicated by comma (with optional 
// parentheses).
let t = 1,2
let s = (3,4)   
// - Record: Curly braces. Uses semicolon to separate fields.
type Product = {code:ProductCode; price:float }
type Message<'a> = {id:int; body:'a}
let p = {code="X123"; price=9.99}
let m = {id=1; body="hello"}
// - Discriminated Union: Vertical bar character. Uses "of" for types.
type MeasurementUnit = Cm | Inch | Mile 
type Name = 
    | Nickname of string 
    | FirstLast of string * string
type Tree<'a> = 
    | E 
    | T of Tree<'a> * 'a * Tree<'a>
let u = Inch
let name = Nickname("John")
let t' = T(E,"John",E)   
// - Enum: Similar to Unions, but uses equals and an int value
type Gender = | Male = 1 | Female = 2
let g = Gender.Male
// - Class: Has function-style parameter list after name for use as constructor. Has 
// "member" keyword. Has "new" keyword for secondary constructors.
type Product'(code:string, price:float) = 
   let isFree = price=0.0 
   new (code) = Product'(code,0.0)
   member this.Code = code 
   member this.IsFree = isFree
let p' = Product'("X123",9.99)
let p2 = Product'("X123")    
// - Interface: Same as class but all members are abstract. Abstract members have colon 
// and type signature rather than a concrete implementation.
type IPrintable = 
   abstract member Print : unit -> unit
// - Struct: Has "struct" keyword. Uses "val" to define fields. Can have constructor.
type Product''= 
   struct  
      val code:string
      val price:float
      new(code) = { code = code; price = 0.0 }
   end
let p'' = Product''()
let p''' = Product''("X123")    