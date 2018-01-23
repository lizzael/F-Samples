module _15_Mini_languages

// Domain-specific languages (DSLs) are well recognized as a technique to create more readable 
// and concise code. The functional approach is very well suited for this.
// If you need to, you can go the route of having a full “external” DSL with its own lexer, 
// parser, and so on, and there are various toolsets for F# that make this quite 
// straightforward.
// But in many cases, it is easier to stay within the syntax of F#, and just design a set of 
// “verbs” and “nouns” that encapsulate the behavior we want.
// The ability to create new types concisely and then match against them makes it very easy to 
// set up fluent interfaces quickly. For example, here is a little function that calculates 
// dates using a simple vocabulary. Note that two new enum-style types are defined just for 
// this one function.

// set up the vocabulary
// one “verb”, using lots of types for the “nouns”.
type DateScale = Hour | Hours | Day | Days | Week | Weeks
type DateDirection = Ago | Hence

// define a function that matches on the vocabulary
let getDate interval scale direction =
    let absHours = match scale with
                   | Hour | Hours -> 1 * interval
                   | Day | Days -> 24 * interval
                   | Week | Weeks -> 24 * 7 * interval
    let signedHours = match direction with
                      | Ago -> -1 * absHours 
                      | Hence ->  absHours 
    System.DateTime.Now.AddHours(float signedHours)

// test some examples
let example1 = getDate 5 Days Ago
let example2 = getDate 1 Hour Hence

printfn "%A" example1
printfn "%A" example2

// the C# equivalent would probably be more like this:
// getDate().Interval(5).Days().Ago()
// getDate().Interval(1).Hour().Hence()

// The following example demonstrates how you might build the functional equivalent of a 
// fluent interface with many “verbs”.
// Say that we are creating a drawing program with various shapes. Each shape has a color, 
// size, label and action to be performed when clicked, and we want a fluent interface to 
// configure each shape.

// Here is an example of what a simple method chain for a fluent interface in C# might 
// look like:
// FluentShape.Default
//    .SetColor("red")
//    .SetLabel("box")
//    .OnClick( s => Console.Write("clicked") );

// Now the concept of "fluent interfaces" and "method chaining" is really only relevant for 
// object-oriented design. In a functional language like F#, the nearest equivalent would be 
// the use of the pipeline operator to chain a set of functions together.

// create an underlying type
type FluentShape = {
    label : string; 
    color : string; 
    onClick : FluentShape->FluentShape // a function type
    }

let defaultShape = 
    {label=""; color=""; onClick=fun shape->shape}

let click shape = 
    shape.onClick shape

// For "method chaining" to work, every function should return an object that can be used next 
// in the chain. So you will see that the "display" function returns the shape, rather than 
// nothing.
let display shape = 
    printfn "My label=%s and my color=%s" shape.label shape.color
    shape   //return same shape

// Next we create some helper functions which we expose as the "mini-language", and will be 
// used as building blocks by the users of the language.

let setLabel label shape = 
   {shape with FluentShape.label = label}

let setColor color shape = 
   {shape with FluentShape.color = color}

// add a click action to what is already there
// Notice that appendClickAction takes a function as a parameter and composes it with the 
// existing click action. As you start getting deeper into the functional approach to reuse, 
// you start seeing many more "higher order functions" like this, that is, functions that act 
// on other functions. Combining functions like this is one of the keys to understanding the 
// functional way of programming.
let appendClickAction action shape = 
   {shape with FluentShape.onClick = shape.onClick >> action}

// Now as a user of this "mini-language", I can compose the base helper functions into more 
// complex functions of my own, creating my own function library. (In C# this kind of thing 
// might be done using extension methods.)

// Compose two "base" functions to make a compound function.
let setRedBox = setColor "red" >> setLabel "box" 

// Create another function by composing with previous function.
// It overrides the color value but leaves the label alone.
let setBlueBox = setRedBox >> setColor "blue"  

// Make a special case of appendClickAction
let changeColorOnClick color = appendClickAction (setColor color)   

// I can then combine these functions together to create objects with the desired behavior.

//setup some test values
let redBox = defaultShape |> setRedBox
let blueBox = defaultShape |> setBlueBox 

// create a shape that changes color when clicked
let changeColor = 
    redBox 
    |> display 
    |> changeColorOnClick "green" 
    |> click 
    |> display  // new version after the click

// create a shape that changes label and color when clicked
let changeLabelAndColor = 
    blueBox 
        |> display
        |> appendClickAction (setLabel "box2" >> setColor "green")  
        |> click
        |> display  // new version after the click

// Here is a more complex example. We will create a function "showRainbow" that, for each color 
// in the rainbow, sets the color and displays the shape.
let rainbow =
    ["red";"orange";"yellow";"green";"blue";"indigo";"violet"]

let showRainbow = 
    let setColorAndDisplay color = setColor color >> display 
    rainbow 
    |> List.map setColorAndDisplay 
    |> List.reduce (>>)

// test the showRainbow function
defaultShape |> showRainbow |> ignore

// Notice that the functions are getting more complex, but the amount of code is still quite 
// small. One reason for this is that the function parameters can often be ignored when doing 
// function composition, which reduces visual clutter. For example, the "showRainbow" function 
// does take a shape as a parameter, but it is not explicitly shown! This elision of parameters 
// is called "point-free" style:
// http://fsharpforfunandprofit.com/series/thinking-functionally.html