﻿module _65_Getting_started_with_direct_porting
// F# equivalents to C#

// Basic syntax conversion guidelines
// Before starting a port, you need to understand how F# syntax is different from C# 
// syntax.
// (For a quick overview of F# syntax as a whole, see "F# syntax in 60 seconds")
// http://fsharpforfunandprofit.com/posts/fsharp-in-60-seconds/

// Curly braces and indentation
// C# uses curly braces to indicate the start and end of a block of code. F# generally 
// just uses indentation.
// Curly braces are used in F#, but not for blocks of code. Instead, you will see them 
// used:
// - For definitions and usage of "record" types.
// - In conjunction with computation expressions, such as seq and async. In general, you 
//   will not be using these expressions for basic ports anyway.
// For details on the indentation rules, see this post.
// http://fsharpforfunandprofit.com/posts/fsharp-syntax

// Semicolons
// Unlike C#'s semicolon, F# does not require any kind of line or statement terminator.

// Commas
// F# does not use commas for separating parameters or list elements, so remember not to 
// use commas when porting!
// For separating list elements, use semicolons rather than commas.
// C# example
// var list = new int[] { 1,2,3}
// F# example
let list = [1;2;3] // semicolons

// For separating parameters for native F# functions, use white space.
// C# example 
// int myFunc(int x, int y, int z) { ... function body ...}
// F# example 
// let myFunc (x:int) (y:int) (z:int) :int = ... function body ...
// let myFunc x y z = ... function body ...
// Commas are generally only used for tuples, or for separating parameters when calling 
// .NET library functions. (See this post for more on tuples vs multiple parameters)
// http://fsharpforfunandprofit.com/posts/defining-functions/#tuples

// Defining variables, functions and types
// In F#, definitions of both variables and functions use the form:
// let someName = // the definition
// Definitions for all types (classes, structures, interfaces, etc.) use the form:
// type someName = // the definition
// The use of the = sign is an important difference between F# and C#. Where C# uses 
// curly braces, F# uses the = and then the following block of code must be indented.

// Mutable values
// In F#, values are immutable by default. If you are doing a direct imperative port, you 
// probably need to make some of the values mutable, using the mutable keyword. Then to 
// assign to the values, use the <- operator, not the equals sign.

// C# example 
//var variableName = 42
//variableName = variableName + 1
// F# example 
let mutable variableName = 42 // Bound to 42 on declaration
variableName <- variableName + 1 // Mutated (reassigned)

// Assignment vs. testing for equality
// In C#, the equals sign is used for assignment, and the double equals == is used for 
// testing equality.
// However in F#, the equals sign is used for testing equality, and is also used to 
// initially bind values to other values when declared,
variableName <- variableName + 1  // Mutated (reassigned)
variableName = variableName + 1   // Comparison not assignment! 

// To test for inequality, use SQL-style <> rather than !=
variableName <> 43                // Comparison will return true.
// variableName != 43                // Error FS0020.
// If you accidentally use != you will probably get an error FS0020.
// http://fsharpforfunandprofit.com/troubleshooting-fsharp/#FS0020

// Conversion example #1
// With these basic guidelines in place, let's look at some real code examples, and do a 
// direct port for them.
// This first example has some very simple code, which we will port line by line. Here's 
// the C# code.
//using System;
//using System.Collections.Generic;

//namespace PortingToFsharp
//{
//    public class Squarer
//    {
//        public int Square(int input)
//        {
//            var result = input * input;
//            return result;
//        }

//        public void PrintSquare(int input)
//        {
//            var result = this.Square(input);
//            Console.WriteLine("Input={0}. Result={1}", 
//              input, result);
//        }
//    }
//}

// Converting "using" and "namespace"
// These keywords are straightforward:
// - using becomes open
// - namespace with curly braces becomes just namespace.
// Unlike C#, F# files do not generally declare namespaces unless they need to interop 
// with other .NET code. The filename itself acts as a default namespace.
// Note that the namespace, if used, must come before anything else, such as "open". This 
// the opposite order from most C# code.

// Converting the class
// To declare a simple class, use:
//type myClassName() = 
//  ... code ...  
// Note that there are parentheses after the class name. These are required for class 
// definitions.
// More complicated class definitions will be shown in the next example, and you read the 
// complete discussion of classes.
// http://fsharpforfunandprofit.com/posts/classes/

// Converting function/method signatures
// For function/method signatures:
// - Parentheses are not needed around the parameter list
// - Whitespace is used to separate the parameters, not commas
// - Rather than curly braces, an equals sign signals the start of the function body
// - The parameters don't normally need types but if you do need them:
//   - The type name comes after the value or parameter
//   - The parameter name and type are separated by colons
//   - When specifying types for parameters, you should probably wrap the pair in 
//     parentheses to avoid unexpected behavior.
//   - The return type for the function as a whole is prefixed by a colon, and comes 
//     after all the other parameters
// Here's a C# function signature:
// int Square(int input) { ... code ...}
// and here's the corresponding F# function signature with explicit types:
// let Square (input:int) :int =  ... code ...
// However, because F# can normally infer the parameter and return types, you rarely 
// need to specify them explicitly.
// Here's a more typical F# signature, with inferred types:
// let Square input =  ... code ...

// void
// The void keyword in C# is generally not needed, but if required, would be converted to 
// unit
// So the C# code:
// void PrintSquare(int input) { ... code ...}
// could be converted to the F# code:
// let PrintSquare (input:int) :unit =  ... code ...
// but again, the specific types are rarely needed, and so the F# version is just:
// let PrintSquare input =  ... code ...

// Converting function/method bodies
// In a function body, you are likely to have a combination of:
// - Variable declarations and assignments
// - Function calls
// - Control flow statements
// - Return values
// We'll have a quick look at porting each of these in turn, except for control flow, 
// which we'll discuss later.

// Converting variable declarations
// Almost always, you can use let on its own, just like var in C#:

// C# variable declaration
// var result = input * input;
// F# value declaration
// let result = input * input
// Unlike C#, you must always assign ("bind") something to an F# value as part of its 
// declaration.

// C# example 
// int unassignedVariable; //valid
// F# example 
// let unassignedVariable // not valid
// As noted above, if you need to change the value after its declaration, you must use 
// the "mutable" keyword.
// If you need to specify a type for a value, the type name comes after the value or 
// parameter, preceded by a colon.
// C# example 
// int variableName = 42;
// F# example 
// let variableName:int = 42

// Converting function calls
// When calling a native F# function, there is no need for parentheses or commas. In 
// other words, the same rules apply for calling a function as when defining it.
// Here's C# code for defining a function, then calling it:
// define a method/function 
// int Square(int input) { ... code  ...}
// call it
// var result = Square(input);
// However, because F# can normally infer the parameter and return types, you rarely need to specify them explicitly So here's typical F# code for defining a function and then calling it:
// define a function 
// let Square input = ... code ...
// call it
// let result = Square input

// Return values
// In C#, you use the return keyword. But in F#, the last value in the block is 
// automatically the "return" value.
// Here's the C# code returning the result variable.
//public int Square(int input)
//{
//    var result = input * input;
//    return result;   //explicit "return" keyword
//}
// And here's the F# equivalent.
let Square input = 
    let result = input * input
    result        // implicit "return" value
// This is because F# is expression-based. Everything is an expression, and the value of 
// a block expression as a whole is just the value of the last expression in the block.
// For more details on expression-oriented code, see "expressions vs statements".

// Printing to the console
// To print output in C#, you generally use Console.WriteLine or similar. In F#, you 
// generally use printf or similar, which is typesafe. (More details on using "printf" 
// family).
// http://fsharpforfunandprofit.com/posts/printf