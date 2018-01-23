//C# code:
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

// And the equivalent F# code:
namespace PortingToFsharp

open System
open System.Collections.Generic

type Squarer() =  
    let Square input = 
        let result = input * input
        result

    let PrintSquare input = 
        let result = Square input
        printf "Input=%i. Result=%i" input result