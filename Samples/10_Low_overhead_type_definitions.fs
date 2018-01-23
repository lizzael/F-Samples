module _10_Low_overhead_type_definitions
// No penalty for making new types!!!

//In C#, there is a disincentive for creating new types — the lack of type inference means you 
// need to explicitly specify types in most places, resulting in brittleness and more visual 
// clutter. As a result, there is always a temptation to create monolithic classes rather than 
// modularizing them.

// In F# there is no penalty for making new types, so it is quite common to have hundreds if 
// not thousands of them. Every time you need to define a structure, you can create a special 
// type, rather than reusing (and overloading) existing types such as strings and lists.

// This means that your programs will be more type-safe, more self documenting, and more 
// maintainable (because when the types change you will immediately get compile-time errors 
// rather than runtime errors).

// Here are some examples of one-liner types in F#:

open System

// some "record" types
type Person = {FirstName:string; LastName:string; Dob:DateTime}
type Coord = {Lat:float; Long:float}

// some "union" (choice) types
type TimePeriod = Hour | Day | Week | Year
type Temperature = C of int | F of int
type Appointment = OneTime of DateTime 
                   | Recurring of DateTime list

let tp = Hour

let tempC = C 5
let tempF = F 5

let ap1 = OneTime DateTime.Now
let aps = Recurring [DateTime.Now]