module _59_Constructing_and_using_a_class
// Now that we have defined the class, how do we go about using it?

// One way to create an instance of a class is straightfoward and just like C# -- use the 
// new keyword and pass in the arguments to the constructor.
type MyClass(intParam:int, strParam:string) = 
    member this.Two = 2
    member this.Square x = x * x

let myInstance = new MyClass(1,"hello")

// However, in F#, the constructor is considered to be just another function, so you can 
// normally eliminate the new and call the constructor function on its own, like this:
let myInstance2 = MyClass(1,"hello")
let point = System.Drawing.Point(1,2)   // works with .NET classes too!

// In the case when you are creating a class that implements IDisposible, you will get a 
// compiler warning if you do not use new.
let sr1 = System.IO.StringReader("")      // Warning
let sr2 = new System.IO.StringReader("")  // OK

// This can be a useful reminder to use the use keyword instead of the let keyword for 
// disposables. See the post on use for more.
// http://fsharpforfunandprofit.com/posts/let-use-do/#use

// Calling methods and properties
// And once you have an instance, you can “dot into” the instance and use any methods and 
// properties in the standard way.
myInstance.Two
myInstance.Square 2

// We have seen many examples of member usage in the above discussion, and there's not 
// too much to say about it.
// Remember that, as discussed above, tuple-style methods and curried-style methods can 
// be called in distinct ways:
type TupleAndCurriedMethodExample() = 
    member this.TupleAdd(x,y) = x + y
    member this.CurriedAdd x y = x + y

let tc = TupleAndCurriedMethodExample()
tc.TupleAdd(1,2)      // called with parens
tc.CurriedAdd 1 2     // called without parens
2 |> tc.CurriedAdd 1  // partial application