module _19_Functions_as_interfaces
// OO design patterns can be trivial when functions are used

// An important aspect of functional programming is that, in a sense, all functions are 
// “interfaces”, meaning that many of the roles that interfaces play in object-oriented 
// design are implicit in the way that functions work.
// In fact, one of the critical design maxims, "program to an interface, not an implementation", 
// is something you get for free in F#.

// Let’s say that we have a calculator interface in C#:
// interface ICalculator 
// {
//    int Calculate(int input);
// }

// And then a specific implementation:
// class AddingCalculator: ICalculator
// {
//    public int Calculate(int input) { return input + 1; }
// }

// And then if we want to add logging, we can wrap the core calculator implementation inside a 
// logging wrapper.
// class LoggingCalculator: ICalculator
// {
//    ICalculator _innerCalculator;

//    LoggingCalculator(ICalculator innerCalculator)
//    {
//       _innerCalculator = innerCalculator;
//    }

//    public int Calculate(int input) 
//    { 
//       Console.WriteLine("input is {0}", input);
//       var result  = _innerCalculator.Calculate(input);
//       Console.WriteLine("result is {0}", result);
//       return result; 
//    }
// }

// So far, so straightforward. But note that, for this to work, we must have defined an 
// interface for the classes. If there had been no ICalculator interface, it would be 
// necessary to retrofit the existing code.
// And here is where F# shines. In F#, you can do the same thing without having to define the 
// interface first. Any function can be transparently swapped for any other function as long 
// as the signatures are the same.
// Here is the equivalent F# code.
// The signature of the function is the interface.
let addingCalculator input = input + 1

let loggingCalculator innerCalculator input = 
   printfn "input is %A" input
   let result = innerCalculator input
   printfn "result is %A" result
   result

// Generic wrappers
// Even nicer is that by default, the F# logging code can be made completely generic so that it 
// will work for any function at all. Here are some examples:

let add1 input = input + 1
let times2 input = input * 2

let genericLogger anyFunc input = 
   printfn "input is %A" input   //log the input
   let result = anyFunc input    //evaluate the function
   printfn "result is %A" result //log the result
   result                        //return the result

// The new "wrapped" functions can be used anywhere the original functions could be used — no 
// one can tell the difference!
let add1WithLogging = genericLogger add1
let times2WithLogging = genericLogger times2

// test
add1WithLogging 3 |> ignore
times2WithLogging 3 |> ignore

[1..5] |> List.map add1WithLogging |> ignore

// Exactly the same generic wrapper approach can be used for other things. For example, here is 
// a generic wrapper for timing a function.

let genericTimer anyFunc input = 
   let stopwatch = System.Diagnostics.Stopwatch()
   stopwatch.Start() 
   let result = anyFunc input  //evaluate the function
   printfn "elapsed ms is %A" stopwatch.ElapsedMilliseconds
   result

let add1WithTimer = genericTimer add1WithLogging 

// test
add1WithTimer 3 |> ignore

// The ability to do this kind of generic wrapping is one of the great conveniences of the 
// function-oriented approach. You can take any function and create a similar function based 
// on it. As long as the new function has exactly the same inputs and outputs as the original 
// function, the new can be substituted for the original anywhere. Some more examples:
// - It is easy to write a generic caching wrapper for a slow function, so that the value is 
//   only calculated once.
// - It is also easy to write a generic “lazy” wrapper for a function, so that the inner 
// function is only called when a result is needed