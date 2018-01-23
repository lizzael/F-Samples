module _07_Pattern_matching

// A typical matching expression that replaces if-then-else looks like this:
let booleanExpression = true
match booleanExpression with
| false -> printf "%d" 0// false branch
| true -> printf "%d" 1 // true branch

// And the replacement of switch might look like this:
let aDigit = 1
match aDigit with
| 1 -> printf "%d" 1 // Case when digit=1
| 2 -> printf "%d" 2 // Case when digit=2
| _ -> printf "Something else" // Case otherwise


// Finally, loops are generally done using recursion, and typically look something like this:
let aList = []
match aList with
| [] -> printf "\n" // Empty case 
| first::rest -> 
     // Case with at least one element.
     // Process first element, 
     printf "%A" first
     // and then call 
     // recursively with the rest of the list
     
// In the following example, we create a Shape type representing four different shapes 
// and then define a draw function with different behavior for each kind of shape. This 
// is similar to polymorphism in an object oriented language, but based on functions.
type Shape =        // define a "union" of alternative structures
    | Circle of radius:int 
    // The int * int is a tuple, a pair of ints. 
    // If you're wondering why the types are "multiplied", see:
    // http://fsharpforfunandprofit.com/posts/tuples/
    | Rectangle of height:int * width:int
    | Point of x:int * y:int 
    | Polygon of pointList:(int * int) list

// As usual, we didn't have to specify any types. The compiler correctly determined that 
//the shape parameter for the "draw" function was of type Shape.
let draw shape =    // define a function "draw" with a shape param
  // You can see that the match..with logic not only matches against the internal structure 
  // of the shape, but also assigns values based on what is appropriate for the shape.
  match shape with
  | Circle radius -> 
      printfn "The circle has a radius of %d" radius
  | Rectangle (height,width) -> 
      printfn "The rectangle is %d high by %d wide" height width
  | Polygon points -> 
      printfn "The polygon is made of these points %A" points
  // The underscore is similar to the "default" branch in a switch statement, except that 
  // in F# it is required – every possible case must always be handled. If you comment out 
  // this line see what happens when you compile!
  | _ -> printfn "I don't recognize this shape"

let circle = Circle(10)
let rect = Rectangle(4,5)
let point = Point(2,3)
let polygon = Polygon( [(1,1); (2,2); (3,3)])

[circle; rect; polygon; point] |> List.iter draw

// Behaviour-oriented design vs data-oriented design
// You might be wondering if this kind of pattern matching is a good idea? In an 
// object-oriented design, checking for a particular class is an anti-pattern because 
// you should only care about behavior, not about the class that implements it.

// But in a pure functional design there are no objects and no behavior. There are functions 
// and there are "dumb" data types. Data types do not have any behavior associated with them, 
// and functions do not contain data -- they just transform data types into other data types.

// In this case, Circle and Rectangle are not actually types. The only type is Shape -- a 
// choice, a discriminated union -- and these are various cases of that type.

// In order to work with the Shape type, a function needs to handle each case of the Shape, 
// which it does with pattern matching.