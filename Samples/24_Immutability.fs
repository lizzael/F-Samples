module _24_Immutability
// Making your code predictable

// Here’s some simple C# code that processes a list of numbers.
// public List<int> MakeList() 
// {
//    return new List<int> {1,2,3,4,5,6,7,8,9,10};
// }

// public List<int> OddNumbers(List<int> list) 
// { 
//    // some code
// }

// public List<int> EvenNumbers(List<int> list) 
// { 
//    // some code
// }

// Now let me test it:
// public void Test() 
// { 
//    var odds = OddNumbers(MakeList()); 
//    var evens = EvenNumbers(MakeList());
//    // assert odds = 1,3,5,7,9 -- OK!
//    // assert evens = 2,4,6,8,10 -- OK!
// }

// Everything works great, and the test passes, but I notice that I am creating the list 
// twice — surely I should refactor this out? So I do the refactoring, and here’s the new 
// improved version:
// public void RefactoredTest() 
// { 
//    var list = MakeList();
//    var odds = OddNumbers(list); 
//    var evens = EvenNumbers(list);
//    // assert odds = 1,3,5,7,9 -- OK!
//    // assert evens = 2,4,6,8,10 -- FAIL!
// }

// But now the test suddenly fails! Why would a refactoring break the test? Can you tell 
// just by looking at the code?
// The answer is, of course, that the list is mutable, and it is probable that the 
// OddNumbers function is making destructive changes to the list as part of its filtering 
// logic. Of course, in order to be sure, we would have to examine the code inside the 
// OddNumbers function.
// In other words, when I call the OddNumbers function, I am unintentionally creating 
// undesirable side effects.
// Is there a way to ensure that this cannot happen? Yes – if the functions had used 
// IEnumerable instead:
// public IEnumerable<int> MakeList() {}
// public List<int> OddNumbers(IEnumerable<int> list) {} 
// public List<int> EvenNumbers(IEnumerable <int> list) {}

// In this case we can be confident that calling the OddNumbers function could not 
// possibly have any effect on the list, and EvenNumbers would work correctly. What’s 
// more, we can know this just by looking at the signatures, without having to examine 
// the internals of the functions. And if you try to make one of the functions misbehave 
// by assigning to the list then you will get an error straight away, at compile time.

// So IEnumerable can help in this case, but what if I had used a type such as 
// IEnumerable<Person> instead of IEnumerable<int>? Could I still be as confident that 
// the functions wouldn’t have unintentional side effects?

// How F# does immutability
// Immutable values and types are the default in F#:

// immutable list
let list = [1;2;3;4]    

type PersonalName = {FirstName:string; LastName:string}
// immutable person
let john = {FirstName="John"; LastName="Doe"}

// Because of this, F# has a number of tricks to make life easier and to optimize the 
// underlying code.
// First, since you can’t modify a data structure, you must copy it when you want to 
// change it. F# makes it easy to copy another data structure with only the changes you 
// want:
let alice = {john with FirstName="Alice"}

// And complex data structures are implemented as linked lists or similar, so that common 
// parts of the structure are shared.
// create an immutable list
let list1 = [1;2;3;4]   

// prepend to make a new list
let list2 = 0::list1    

// get the last 4 of the second list 
let list3 = list2.Tail

// the two lists are the identical object in memory!
System.Object.ReferenceEquals(list1,list3) |> printfn "%b"

// This technique ensures that, while you might appear to have hundreds of copies of a 
// list in your code, they are all sharing the same memory behind the scenes.