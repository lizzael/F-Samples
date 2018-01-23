module _18_Out_of_the_box_behavior_for_types
// Immutability and built-in equality with no coding

// One nice thing about F# is that most types immediately have some useful “out-of-the-box” 
// behavior such as immutability and built-in equality, functionality that often has to be 
// explicitly coded for in C#.
// By "most" F# types, I mean the core "structural" types such as tuples, records, unions, 
// options, lists, etc. Classes and some other types have been added to help with .NET 
// integration, but lose some of the power of the structural types.
// This built-in functionality for these core types includes:
// - Immutability
// - Pretty printing when debugging
// - Equality
// - Comparisons

// Here is an immutable type in F#:
type PersonalName = {FirstName:string; LastName:string}

//And here is how the same type is typically coded in C#:
// class ImmutablePersonalName
// {
//     public ImmutablePersonalName(string firstName, string lastName)
//     {
//         this.FirstName = firstName;
//         this.LastName = lastName;
//     }

//     public string FirstName { get; private set; }
//     public string LastName { get; private set; }
// }

// Most F# types have built-in pretty printing
// In F#, you don't have to override ToString() for most types -- you get pretty printing for 
// free!
type USAddress = 
   {Street:string; City:string; State:string; Zip:string}
type UKAddress = 
   {Street:string; Town:string; PostCode:string}
type Address = US of USAddress | UK of UKAddress
type Person = 
   {Name:string; Address:Address}

let alice = {
   Name="Alice"; 
   Address=US {Street="123 Main";City="LA";State="CA";Zip="91201"}}
let bob = {
   Name="Bob"; 
   Address=UK {Street="221b Baker St";Town="London";PostCode="NW1 6XE"}} 

printfn "Alice is %A" alice
printfn "Bob is %A" bob

// Most F# types have built-in structural equality
// In C#, you often have to implement the IEquatable interface so that you can test for 
// equality between objects. This is needed when using objects for Dictionary keys, for 
// example.
// In F#, you get this for free with most F# types. For example, using the PersonalName type 
// from above, we can compare two names straight away.
let alice1 = {FirstName="Alice"; LastName="Adams"}
let alice2 = {FirstName="Alice"; LastName="Adams"}
let bob1 = {FirstName="Bob"; LastName="Bishop"}

//test
printfn "alice1=alice2 is %A" (alice1=alice2)
printfn "alice1=bob1 is %A" (alice1=bob1)

// Most F# types are automatically comparable
// In C#, you often have to implement the IComparable interface so that you can sort objects.
// Again, in F#, you get this for free with most F# types. For example, here is a simple 
// definition of a deck of cards.
type Suit = Club | Diamond | Spade | Heart
type Rank = Two | Three | Four | Five | Six | Seven | Eight 
            | Nine | Ten | Jack | Queen | King | Ace

//We can write a function to test the comparison logic:
let compareCard card1 card2 = 
    if card1 < card2 
    then printfn "%A is greater than %A" card2 card1 
    else printfn "%A is greater than %A" card1 card2 

let aceHearts = Heart, Ace
let twoHearts = Heart, Two
let aceSpades = Spade, Ace

compareCard aceHearts twoHearts 
compareCard twoHearts aceSpades

// Note that the Ace of Hearts is automatically greater than the Two of Hearts, because the 
// "Ace" rank value comes after the "Two" rank value.

// Here's an example of a hand of cards:

let hand = [ Club,Ace; Heart,Three; Heart,Ace; 
             Spade,Jack; Diamond,Two; Diamond,Ace ]

//instant sorting!
List.sort hand |> printfn "sorted hand is (low to high) %A"

// And as a side benefit, you get min and max for free too!
List.max hand |> printfn "high card is %A"
List.min hand |> printfn "low card is %A"
