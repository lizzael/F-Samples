module _20_The_strategy_pattern

open System.Threading

// We can apply this same approach to another common design pattern, the “strategy pattern.”
// Let’s use the familiar example of inheritance: an Animal superclass with Cat and Dog 
// subclasses, each of which overrides a MakeNoise() method to make different noises.
// In a true functional design, there are no subclasses, but instead the Animal class would 
// have a NoiseMaking function that would be passed in with the constructor. This approach is 
// exactly the same as the “strategy” pattern in OO design.

type Animal(noiseMakingStrategy) = 
   member this.MakeNoise = 
      noiseMakingStrategy() |> printfn "Making noise %s" 
   
// now create a cat 
let meowing() = "Meow"
let cat = Animal(meowing)
cat.MakeNoise

// .. and a dog
let woofOrBark() = if (System.DateTime.Now.Second % 2 = 0) 
                   then "Woof" else "Bark"
let dog = Animal(woofOrBark)
dog.MakeNoise
Thread.Sleep(1000);
dog.MakeNoise  //try again a second later

// Note that again, we do not have to define any kind of INoiseMakingStrategy interface first. 
// Any function with the right signature will work. As a consequence, in the functional model, 
// the standard .NET “strategy” interfaces such as IComparer, IFormatProvider, and 
// IServiceProvider become irrelevant.
// Many other design patterns can be simplified in the same way.