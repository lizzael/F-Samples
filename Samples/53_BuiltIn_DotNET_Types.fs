module _53_BuiltIn_DotNET_Types
// Ints, strings, bools, etc

// In this post we'll take a quick look at how F# handles the standard types that are 
// built into .NET.
// http://msdn.microsoft.com/en-us/library/hfa3fa08%28VS.80%29.aspx

// Literals
// F# uses the same syntax for literals that C# does, with a few exceptions.
// I'll divide the built-in types into the following groups:
// - miscellaneous types (bool, char, etc. )
// - string types
// - integer types (int, uint and byte, etc)
// - float types (float, decimal, etc)
// - pointer types (IntPtr, etc)

// The usual special characters can be used inside normal strings, such as \n, \t, \\, 
// etc. Quotes must be escaped with a backslash: \' and \".
// In verbatim strings, backslashes are ignored (good for Windows filenames and regex 
// patterns). But quotes need to be doubled.
// Triple-quoted strings are new in VS2012. They are useful because special characters do 
// not need to be escaped at all, and so they can handle embedded quotes nicely (great 
// for XML).

// BigInteger is available in all versions of F#. From .NET 4 it is included as part of 
// the .NET base library.
// Integer types can also be written in hex and octal.
// - The hex prefix is 0x. So 0xFF is hex for 255.
// - The octal prefix is 0o. So 0o377 is octal for 255.

// F# natively uses float instead of double, but both can be used.

// Casting between built-in primitive types
// There is no direct "cast" syntax in F#, but there are helper functions to cast between 
// types. These helper functions have the same name as the type (you can see them in the 
// Microsoft.FSharp.Core namespace).
// So for example, in C# you might write:
// var x = (int)1.23
// var y = (double)1   
// In F# the equivalent would be:
let x = int 1.23
let y = float 1

// In F# there are only casting functions for numeric types. In particular, there is no 
// cast for bool, and you must use Convert or similar.
// let x' = bool 1  //error
let y' = System.Convert.ToBoolean(1)  // ok

// Boxing and unboxing
// Just as in C# and other .NET languages, the primitive int and float types are value 
// objects, not classes. Although this is normally transparent, there are certain 
// occasions where it can be an issue.
// First, lets look at the transparent case. In the example below, we define a function 
// that takes a parameter of type Object, and simply returns it. If we pass in an int, it 
// is silently boxed into an object, as can be seen from the test code, which returns an 
// object not an int.

// create a function with parameter of type Object
let objFunction (o:obj) = o

// test: call with an integer
let result = objFunction 1

// result is
// val result : obj = 1
// The fact that result is an object, not an int, can cause type errors if you are not 
// careful. For example, the result cannot be directly compared with the original value:
// let resultIsOne = (result = 1)
// error FS0001: This expression was expected to have type obj 
// but here has type int    
// To work with this situation, and other similar ones, you can convert a primitive type 
// to an object directly, by using the box keyword:

let o = box 1

// retest the comparison example above, but with boxing
let result' = objFunction 1
let resultIsOne = (result = box 1)  // OK

// To convert an object back to an primitive type, use the unbox keyword, but unlike box, 
// you must either supply a specific type to unbox to, or be sure that the compiler has 
// enough information to make an accurate type inference.

// box an int
let o' = box 1

// type known for target value
let i:int = unbox o'  // OK 

// explicit type given in unbox
let j = unbox<int> o'  // OK 

// type inference, so no type annotation needed
let k = 1 + unbox o'  // OK 

// So the comparison example above could also be done with unbox. No explicit type 
// annotation is needed because it is being compared with an int.
let result'' = objFunction 1
let resultIsOne'' = (unbox result = 1)  // OK

// A common problem occurs if you do not specify enough type information -- you will 
// encounter the infamous "Value restriction" error, as shown below:
let o'' = box 1

// no type specified
// let i = unbox o  // FS0030: Value restriction error
// The solution is to reorder the code to help the type inference, or when all else fails, 
// add an explicit type annotation. See the post on type inference for more tips.

// Boxing in combination with type detection
// Let's say that you want to have a function that matches based on the type of the 
// parameter, using the :? operator:
// let detectType v =
//     match v with
//         | :? int -> printfn "this is an int"
//         | _ -> printfn "something else"
// Unfortunately, this code will fail to compile, with the following error:
// error FS0008: This runtime coercion or type test from type 'a to int    
// involves an indeterminate type based on information prior to this program point. 
// Runtime type tests are not allowed on some types. Further type annotations are needed.

// The message tells you the problem: "runtime type tests are not allowed on some types".
// The answer is to "box" the value which forces it into a reference type, and then you 
// can type check it:
let detectTypeBoxed v =
    match box v with      // used "box v" 
        | :? int -> printfn "this is an int"
        | _ -> printfn "something else"

//test
detectTypeBoxed 1
detectTypeBoxed 3.14