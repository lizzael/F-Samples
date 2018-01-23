﻿module _57__Classes

// This post and the next will cover the basics of creating and using classes and methods 
// in F#.

// Defining a class
// Just like all other data types in F#, class definitions start with the type keyword.
// The thing that distinguishes them from other types is that classes always have some 
// parameters passed in when they are created -- the constructor -- and so there are 
// always parentheses after the class name.
// Also, unlike other types, classes must have functions attached to them as members. This 
// post will explain how you do this for classes, but for a general discussion of 
// attaching functions to other types see the post on type extensions.
// http://fsharpforfunandprofit.com/posts/type-extensions/

// So, for example, if we want to have a class called CustomerName that requires three 
// parameters to construct it, it would be written like this:
type CustomerName(firstName, middleInitial, lastName) = 
    member this.FirstName = firstName
    member this.MiddleInitial = middleInitial
    member this.LastName = lastName  

// Let’s compare this with the C# equivalent:
//public class CustomerName
//{
//    public CustomerName(string firstName, 
//       string middleInitial, string lastName)
//    {
//        this.FirstName = firstName;
//        this.MiddleInitial = middleInitial;
//        this.LastName = lastName;
//    }

//    public string FirstName { get; private set; }
//    public string MiddleInitial { get; private set; }
//    public string LastName { get; private set; }
//}

// You can see that in the F# version, the primary constructor is embedded into the class 
// declaration itself -– it is not a separate method. That is, the class declaration has 
// the same parameters as the constructor, and the parameters automatically become 
// immutable private fields that store the original values that were passed in.
// So in the above example, because we declared the CustomerName class as:
// type CustomerName(firstName, middleInitial, lastName)
// therefore firstName, middleInitial, and lastName automatically became immutable private 
// fields.

// Specifying types in the constructor
// You might not have noticed, but the CustomerName class defined above does not constrain 
// the parameters to be strings, unlike the C# version. In general, type inference from 
// usage will probably force the values to be strings, but if you do need to specify the 
// types explicitly, you can do so in the usual way with a colon followed by the type 
// name.
// Here’s a version of the class with explicit types in the constructor:
type CustomerName2(firstName:string, 
                   middleInitial:string, lastName:string) = 
    member this.FirstName = firstName
    member this.MiddleInitial = middleInitial
    member this.LastName = lastName

// One little quirk about F# is that if you ever need to pass a tuple as a parameter to a 
// constructor, you will have to annotate it explicitly, because the call to the 
// constructor will look identical:
type NonTupledConstructor(x:int,y: int) = 
    do printfn "x=%i y=%i" x y    

type TupledConstructor(tuple:int * int) = 
    let x,y = tuple
    do printfn "x=%i y=%i" x y    
    
// calls look identical
let myNTC = new NonTupledConstructor(1,2)    
let myTC = new TupledConstructor(1,2)    

// Class members
// The example class above has three read-only instance properties. In F#, both properties 
// and methods use the member keyword.
// Also, in the example above, you see the word “this” in front of each member name. This 
// is a “self-identifier” that can be used to refer to the current instance of the class. 
// Every non-static member must have a self-identifier, even it is not used (as in the 
// properties above). There is no requirement to use a particular word, just as long as it 
// is consistent. You could use “this” or “self” or “me” or any other word that commonly 
// indicates a self reference.

// Understanding class signatures
// When a class is compiled (or when you over hover the definition in the editor), you see 
// the “class signature” for the class. For example, for the class definition:
type MyClass(intParam:int, strParam:string) = 
    member this.Two = 2
    member this.Square x = x * x

// the corresponding signature is:
//type MyClass =
//  class
//    new : intParam:int * strParam:string -> MyClass
//    member Square : x:int -> int
//    member Two : int
//  end
// The class signature contains the signatures for all the constructors, methods and 
// properties in the class. It is worth understanding what these signatures mean, because, 
// just as with functions, you can understand what the class does by looking at them. It 
// is also important because you will need to write these signatures when creating 
// abstract methods and interfaces.

// Method signatures
// Method signatures such as are very similar to the signatures for standalone functions, 
// except that the parameter names are part of the signature itself.
// So in this case, the method signature is:
// member Square : x:int -> int
// And for comparison, the corresponding signature for a standalone function would be:
// val Square : int -> int

// Constructor signatures
// Constructor signatures are always called new, but other than that, they look like a 
// method signature.
// Constructor signatures always take tuple values as their only parameter. In this case 
// the tuple type is int * string, as you would expect. The return type is the class 
// itself, again as you would expect.
// Again, we can compare the constructor signature with a similar standalone function:
// class constructor signature
// new : intParam:int * strParam:string -> MyClass
// standalone function signature
// val new : int * string -> MyClass

// Property signatures
// Finally, property signatures such as member Two : int are very similar to the 
// signatures for standalone simple values, except that no explicit value is given.
// member property
// member Two : int
// standalone value
// val Two : int = 2

// Private fields and functions using “let” bindings
// After the class declaration, you can optionally have a set of “let” bindings, typically 
// used for defining private fields and functions.
// Here’s some sample code to demonstrate this:
type PrivateValueExample(seed) = 
    // private immutable value
    let privateValue = seed + 1

    // private mutable value
    let mutable mutableValue = 42

    // private function definition
    let privateAddToSeed input = 
        seed + input

    // public wrapper for private function
    member this.AddToSeed x = 
        privateAddToSeed x

    // public wrapper for mutable value
    member this.SetMutableValue x = 
        mutableValue <- x 
    
// test
let instance = new PrivateValueExample(42)
printf "%i" (instance.AddToSeed 2)
instance.SetMutableValue 43

// In the example above, there are three let bindings:
// - privateValue is set to the initial seed plus 1
// - mutableValue is set to 42
// - The privateAddToSeed function uses the initial seed plus a parameter
// Because they are let bindings, they are automatically private, so to access them 
// externally, there must be a public member to act as a wrapper.
// Note that the seed value passed into the constructor is also available as a private 
// field, just like the let-bound values.

// Mutable constructor parameters
// Sometimes, you want a parameter passed to the constructor to be mutable. You cannot 
// specify this in the parameter itself, so the standard technique is to create a mutable 
// let-bound value and assign it from the parameter, as shown below:
type MutableConstructorParameter(seed) = 
    let mutable mutableSeed = seed 

    // public wrapper for mutable value
    member this.SetSeed x = 
        mutableSeed <- x 

// In cases, like this, it is quite common to give the mutable value the same name as the 
// parameter itself, like this:
type MutableConstructorParameter2(seed) = 
    let mutable seed = seed // shadow the parameter
    
    // public wrapper for mutable value
    member this.SetSeed x = 
        seed <- x 

// Additional constructor behavior with "do" blocks
// In the CustomerName example earlier, the constructor just allowed some values to be 
// passed in but didn’t do anything else. However, in some cases, you might need to 
// execute some code as part of the constructor. This is done using do blocks.
// Here’s an example:
type DoExample(seed) = 
    let privateValue = seed + 1
    
    //extra code to be done at construction time
    do printfn "the privateValue is now %i" privateValue 
    
// test
new DoExample(42)

// The “do” code can also call any let-bound functions defined before it, as shown in this 
// example:
type DoPrivateFunctionExample(seed) =   
    let privateValue = seed + 1
    
    // some code to be done at construction time
    do printfn "hello world"

    // must come BEFORE the do block that calls it
    let printPrivateValue() = 
        do printfn "the privateValue is now %i" privateValue 

    // more code to be done at construction time
    do printPrivateValue()

// test
new DoPrivateFunctionExample(42)

// Accessing the instance via “this” in a do block
// One of the differences between the “do” and “let” bindings is that the “do” bindings 
// can access the instance while “let” bindings cannot. This is because “let” bindings are 
// actually evaluated before the constructor itself (similar to field initializers in C#), 
// so the instance in a sense does not exist yet.
// If you need to call members of the instance from a "do" block, you need some way to 
// refer to the instance itself. This is again done using a “self-identifier”, but this 
// time it is attached to the class declaration itself.
type DoPublicFunctionExample(seed) as this =   
    // Note the "this" keyword in the declaration

    let privateValue = seed + 1
    
    // extra code to be done at construction time
    do this.PrintPrivateValue()

    // member
    member this.PrintPrivateValue() = 
        do printfn "the privateValue is now %i" privateValue 

// test
new DoPublicFunctionExample(42)

// In general though, it is not best practice to call members from constructors unless 
// you have to (e.g. calling a virtual method). Better to call private let-bound 
// functions, and if necessary, have the public members call those same private functions.

// Methods
// A method definition is very like a function definition, except that it has the member 
// keyword and the self-identifier instead of just the let keyword.
// Here are some examples:
type MethodExample() = 
    // standalone method
    member this.AddOne x = 
        x + 1

    // calls another method
    member this.AddTwo x = 
        this.AddOne x |> this.AddOne

    // parameterless method
    member this.Pi() = 
        3.14159

// test
let me = new MethodExample()
printfn "%i" <| me.AddOne 42
printfn "%i" <| me.AddTwo 42
printfn "%f" <| me.Pi()

// You can see that, just like normal functions, methods can have parameters, call other 
// methods, and be parameterless (or to be precise, take a unit parameter)

// Tuple form vs. curried form
// Unlike normal functions, methods with more than one parameter can be defined in two 
// different ways:
// - The curried form, where parameters are separated with spaces, and partial application 
//   is supported.
// - The tuple form, where all the parameters as passed in at the same time, 
//   comma-separated, in a single tuple.
// The curried approach is more functional, and the tuple approach is more 
// object-oriented. Here is an example class with a method for each approach:
type TupleAndCurriedMethodExample() = 
    // curried form
    member this.CurriedAdd x y = 
        x + y

    // tuple form
    member this.TupleAdd(x,y) = 
        x + y

// test
let tc = new TupleAndCurriedMethodExample()
printfn "%i" <| tc.CurriedAdd 1 2
printfn "%i" <| tc.TupleAdd(1,2)

// use partial application
let addOne = tc.CurriedAdd 1  
printfn "%i" <| addOne 99

// So which approach should you use?
// The advantages of tuple form are:
// - Compatible with other .NET code
// - Supports named parameters and optional parameters
// - Supports method overloads (multiple methods with the same name that differ only in their function signature)
// On the other hand, the disadvantages of tuple form are:
// - Doesn’t support partial application
// - Doesn’t work well with higher order functions
// - Doesn’t work well with type inference
// For a more detailed discussion on tuple form vs. curried form see the post on type 
// extensions.
// http://fsharpforfunandprofit.com/posts/type-extensions/#tuple-form

// Let- bound functions in conjunction with class methods
// A common pattern is to create let-bound functions that do all the heavy lifting, and 
// then have the public methods call these internal functions directly. This has the 
// benefit that the type inference works much better with functional-style code than with 
// methods.
// Here’s an example:
type LetBoundFunctions() = 
    let listReduce reducer list = 
        list |> List.reduce reducer 

    let reduceWithSum sum elem = 
        sum + elem

    let sum list = 
        list |> listReduce reduceWithSum 

    // finally a public wrapper 
    member this.Sum  = sum
    
// test
let lbf = new LetBoundFunctions()
printfn "Sum is %i" <| lbf.Sum [1..10]

// For more details on how to do this, see this discussion.
// http://fsharpforfunandprofit.com/posts/type-extensions/#attaching-existing-functions

// Recursive methods
// Unlike normal let-bound functions, methods that are recursive do not need the special 
// rec keyword. Here’s the boringly familiar Fibonacci function as a method:
type MethodExample'() = 
        // recursive method without "rec" keyword
    member this.Fib x = 
        match x with
        | 0 | 1 -> 1
        | _ -> this.Fib (x-1) + this.Fib (x-2)

// test
let me' = new MethodExample'()
printfn "%i" <| me'.Fib 10

// Type annotation for methods
// As usual, the types for a method’s parameters and return value can normally be inferred 
// by the compiler, but if you need to specify them, you do so in the same way that you 
// would for a standard function:
type MethodExample''() = 
    // explicit type annotation
    member this.AddThree (x:int) :int = 
        x + 3

// Properties
// Properties can be divided into three groups:
// - Immutable properties, where there is a “get” but no “set”.
// - Mutable properties, where there is a “get” and also a (possibly private) “set”.
// - Write-only properties, where there is a “set” but no “get”. These are so unusual 
//   that I won’t discuss them here, but the MSDN documentation describes the syntax if 
//   you ever need it.
// The syntax for immutable and mutable properties is slightly different.
// For immutable properties, the syntax is simple. There is a “get” member that is similar to a standard “let” value binding. The expression on the right-hand side of the binding can be any standard expression, typically a combination of the constructor parameters, private let-bound fields, and private functions.
// Here’s an example:
type PropertyExample(seed) = 
    // immutable property 
    // using a constructor parameter
    member this.Seed = seed

// For mutable properties however, the syntax is more complicated. You need to provide two 
// functions, one to get and one to set. This is done by using the syntax:
// with get() = ...
// and set(value) = ...
// Here’s an example:
type PropertyExample'(seed) = 
    // private mutable value
    let mutable myProp = seed

    // mutable property
    // changing a private mutable value
    member this.MyProp 
        with get() = myProp 
        and set(value) =  myProp <- value

// To make the set function private, use the keywords private set instead.

// Automatic properties
// Starting in VS2012, F# supports automatic properties, which remove the requirement to 
// create a separate backing store for them.
// To create an immutable auto property, use the syntax:
// member val MyProp = initialValue
// To create a mutable auto property, use the syntax:
// member val MyProp = initialValue with get,set
// Note that in this syntax there is a new keyword val and the self-identifier has gone.

// Complete property example
// Here’s a complete example that demonstrates all the property types:
type CompletePropertyExample(seed) = 
    // private mutable value
    let mutable myProp = seed

    // private function
    let square x = x * x

    // immutable property 
    // using a constructor parameter
    member this.Seed = seed

    // immutable property 
    // using a private function
    member this.SeedSquared = square seed

    // mutable property
    // changing a private mutable value
    member this.MyProp 
        with get() = myProp 
        and set(value) =  myProp <- value

    // mutable property with private set
    member this.MyProp2 
        with get() = myProp 
        and private set(value) =  myProp <- value

    // automatic immutable property (in VS2012)
    member val ReadOnlyAuto = 1

    // automatic mutable property (in VS2012)
    member val ReadWriteAuto = 1 with get,set

// test 
let pe = new CompletePropertyExample(42)
printfn "%i" <| pe.Seed
printfn "%i" <| pe.SeedSquared
printfn "%i" <| pe.MyProp
printfn "%i" <| pe.MyProp2

// try calling set
pe.MyProp <- 43    // Ok
printfn "%i" <| pe.MyProp

// try calling private set
// pe.MyProp2 <- 43   // Error

// Properties vs. parameterless methods
// At this point you might be confused by the difference between properties and 
// parameterless methods. They look identical at first glance, but there is a subtle 
// difference – “parameterless” methods are not really parameterless; they always have a 
// unit parameter.
// Here’s an example of the difference in both definition and usage:
type ParameterlessMethodExample() = 
    member this.MyProp = 1    // No parens!
    member this.MyFunc() = 1  // Note the ()

// in use
let x = new ParameterlessMethodExample()
printfn "%i" <| x.MyProp      // No parens!
printfn "%i" <| x.MyFunc()    // Note the ()

// You can also tell the difference by looking at the signature of the class definition
// The class definition looks like this:
//type ParameterlessMethodExample =
//  class
//    new : unit -> ParameterlessMethodExample
//    member MyFunc : unit -> int
//    member MyProp : int
//  end
// The method has signature MyFunc : unit -> int and the property has signature 
// MyProp : int.
// This is very similar to what the signatures would be if the function and property were 
// declared standalone, outside of any class:
let MyFunc2() = 1 
let MyProp2 = 1 

// The signatures for these would look like:
// val MyFunc2 : unit -> int
// val MyProp2 : int = 1
// which is almost exactly the same.
// If you are unclear on the difference and why the unit parameter is needed for the 
// function, please read the discussion of parameterless methods.

// Secondary constructors
// In addition to the primary constructor embedded in its declaration, a class can have 
// additional constructors. These are indicated by the new keyword and must call the 
// primary constructor as their last expression.
type MultipleConstructors(param1, param2) =
    do printfn "Param1=%i Param12=%i" param1 param2

    // secondary constructor
    new(param1) = 
        MultipleConstructors(param1,-1) 

    // secondary constructor
    new() = 
        printfn "Constructing..."
        MultipleConstructors(13,17) 

// test
let mc1 = new MultipleConstructors(1,2)
let mc2 = new MultipleConstructors(42)
let mc3 = new MultipleConstructors()

// Static members
// Just as in C#, classes can have static members, and this is indicated with the static 
// keyword. The static modifier comes before the member keyword.
// Members which are static cannot have a self-identifier such as “this” because there is 
// no instance for them to refer to.
type StaticExample() = 
    member this.InstanceValue = 1
    static member StaticValue = 2  // no "this"
    
// test
let instance' = new StaticExample()
printf "%i" instance'.InstanceValue
printf "%i" StaticExample.StaticValue

// Static constructors
// There is no direct equivalent of a static constructor in F#, but you can create static 
// let-bound values and static do-blocks that are executed when the class is first used.
type StaticConstructor() =
    // static field
    static let rand = new System.Random()

    // static do
    static do printfn "Class initialization!"

    // instance member accessing static field
    member this.GetRand() = rand.Next()

// Accessibility of members
// You can control the accessibility of a member with the standard .NET keywords public, 
// private and internal. The accessibility modifiers come after the member keyword and 
// before the member name.
// Unlike C#, all class members are public by default, not private. This includes both 
// properties and methods. However, non-members (e.g. let declarations) are private and 
// cannot be made public.
// Here’s an example:
type AccessibilityExample() = 
    member this.PublicValue = 1
    member private this.PrivateValue = 2
    member internal this.InternalValue = 3

// test
let a = new AccessibilityExample();
printf "%i" a.PublicValue
// printf "%i" a.PrivateValue  // not accessible

// For properties, if the set and get have different accessibilities, you can tag each 
// part with a separate accessibility modifier.
type AccessibilityExample2() = 
    let mutable privateValue = 42
    member this.PrivateSetProperty
        with get() = 
            privateValue 
        and private set(value) = 
            privateValue <- value

// test
let a2 = new AccessibilityExample2();
printf "%i" a2.PrivateSetProperty  // ok to read
// a2.PrivateSetProperty <- 43        // not ok to write

// In practice, the “public get, private set” combination that is so common in C# is not 
// generally needed in F#, because immutable properties can be defined more elegantly, as 
// described earlier.