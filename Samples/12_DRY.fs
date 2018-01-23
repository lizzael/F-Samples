module _12_DRY

// Using functions to extract boilerplate code
// The functional approach to the DRY principle

// public static int Product(int n)
// {
//     int product = 1;
//     for (int i = 1; i <= n; i++)
//     {
//         product *= i;
//     }
//     return product;
// }

// List.fold. This is a powerful, general purpose function which starts with the initial 
// value and then runs the action function for each element in the list in turn.
// Can you use the "fold" approach in C#? Yes. LINQ does have an equivalent to fold, called 
// Aggregate.
let product n = 
    let initialValue = 1
    let action productSoFar x = productSoFar * x
    [1..n] |> List.fold action initialValue

//test
product 10 |> printf "%d\n"

// public static int SumOfOdds(int n)
// {
//     int sum = 0;
//     for (int i = 1; i <= n; i++)
//     {
//         if (i % 2 != 0) { sum += i; }
//     }
//     return sum;
// }

let sumOfOdds n = 
    let initialValue = 0
    let action sumSoFar x = if x%2=0 then sumSoFar else sumSoFar+x 
    [1..n] |> List.fold action initialValue

//test
sumOfOdds 10 |> printf "%d\n"

// public static int AlternatingSum(int n)
// {
//     int sum = 0;
//     bool isNeg = true;
//     for (int i = 1; i <= n; i++)
//     {
//         if (isNeg)
//         {
//             sum -= i;
//             isNeg = false;
//         }
//         else
//         {
//             sum += i;
//             isNeg = true;
//         }
//     }
//     return sum;
// }

// it use a tuple (pair of values) for the initial value and the result of the action. This is 
// because both the running total and the isNeg flag must be passed to the next iteration of 
// the loop -- there are no "global" values that can be used. The final result of the fold is 
// also a tuple so we have to use the "snd" (second) function to extract the final total that 
// we want.
let alternatingSum n = 
    let initialValue = (true,0)
    let action (isNeg,sumSoFar) x = if isNeg then (false,sumSoFar-x)
                                             else (true ,sumSoFar+x)
    [1..n] |> List.fold action initialValue |> snd

//test
alternatingSum 100 |> printf "%d\n"

// By using List.fold and avoiding any loop logic at all, the F# code gains a number of 
// benefits:
// - The key program logic is emphasized and made explicit. The important differences between 
//   the functions become very clear, while the commonalities are pushed to the background.
// - The boilerplate loop code has been eliminated, and as a result the code is more condensed 
//   than the C# version (4-5 lines of F# code vs. at least 9 lines of C# code)
// - There can never be a error in the loop logic (such as off-by-one) because that logic is 
//   not exposed to us.

let sumOfSquaresWithFold n = 
    let initialValue = 0
    let action sumSoFar x = sumSoFar + (x*x)
    [1..n] |> List.fold action initialValue 

//test
sumOfSquaresWithFold 100 |> printf "%d\n"

//public static int ProductWithAggregate(int n)
//{
//    var initialValue = 1;
//    Func<int, int, int> action = (productSoFar, x) => 
//        productSoFar * x;
//    return Enumerable.Range(1, n)
//            .Aggregate(initialValue, action);
//}

//public static int SumOfOddsWithAggregate(int n)
//{
//    var initialValue = 0;
//    Func<int, int, int> action = (sumSoFar, x) =>
//        (x % 2 == 0) ? sumSoFar : sumSoFar + x;
//    return Enumerable.Range(1, n)
//        .Aggregate(initialValue, action);
//}

//public static int AlternatingSumsWithAggregate(int n)
//{
//    var initialValue = Tuple.Create(true, 0);
//    Func<Tuple<bool, int>, int, Tuple<bool, int>> action =
//        (t, x) => t.Item1
//            ? Tuple.Create(false, t.Item2 - x)
//            : Tuple.Create(true, t.Item2 + x);
//    return Enumerable.Range(1, n)
//        .Aggregate(initialValue, action)
//        .Item2;
//}

// Well, in some sense these implementations are simpler and safer than the original C# 
// versions, but all the extra noise from the generic types makes this approach much less 
// elegant than the equivalent code in F#. You can see why most C# programmers prefer to stick 
// with explicit loops.

// A more relevant example
// A slightly more relevant example that crops up frequently in the real world is how to get 
// the "maximum" element of a list when the elements are classes or structs. The LINQ method 
// 'max' only returns the maximum value, not the whole element that contains the maximum value.

//Here's a solution using an explicit loop:

//public class NameAndSize
//{
//    public string Name;
//    public int Size;
//}

//public static NameAndSize MaxNameAndSize(IList<NameAndSize> list)
//{
//    if (list.Count() == 0)
//    {
//        return default(NameAndSize);
//    }

//    var maxSoFar = list[0];
//    foreach (var item in list)
//    {
//        if (item.Size > maxSoFar.Size)
//        {
//            maxSoFar = item;
//        }
//    }
//    return maxSoFar;
//}

// Doing this in LINQ seems hard to do efficiently (that is, in one pass), and has come up as 
// a Stack Overflow question: 
// http://stackoverflow.com/questions/1101841/linq-how-to-perform-max-on-a-property-of-all-objects-in-a-collection-and-ret
// Jon Skeet event wrote an article about it:
// http://codeblog.jonskeet.uk/2005/10/02/a-short-case-study-in-linq-efficiency/

//And here's the C# code using Aggregate:

//public class NameAndSize
//{
//    public string Name;
//    public int Size;
//}

//public static NameAndSize MaxNameAndSize(IList<NameAndSize> list)
//{
//    if (!list.Any())
//    {
//        return default(NameAndSize);
//    }

//    var initialValue = list[0];
//    Func<NameAndSize, NameAndSize, NameAndSize> action =
//        (maxSoFar, x) => x.Size > maxSoFar.Size ? x : maxSoFar;
//    return list.Aggregate(initialValue, action);
//}

// Note that this C# version returns null for an empty list. That seems dangerous -- so what 
// should happen instead? Throwing an exception? That doesn't seem right either.

//Here's the F# code using fold:

type NameAndSize= {Name:string;Size:int}
 
// Similar to what we have seen before.
let maxNameAndSize list = 
    
    let innerMaxNameAndSize initialValue rest = 
        let action maxSoFar x = if maxSoFar.Size < x.Size then x else maxSoFar
        rest |> List.fold action initialValue 

    // With an empty list, it returns a None, and in the non-empty case, it returns a Some. 
    // Doing this guarantees that the caller of the function has to handle both cases.
    match list with
    | [] -> 
        None
    | first::rest -> 
        let max = innerMaxNameAndSize first rest
        Some max

//test
let list = [
    {Name="Alice"; Size=10}
    {Name="Bob"; Size=1}
    {Name="Carol"; Size=12}
    {Name="David"; Size=5}
    ]    

maxNameAndSize list |> printf "%A\n"
maxNameAndSize []  |> printf "%A\n"

let maxNameAndSize2 list = 
    match list with
    | [] -> 
        None
    | _ -> 
        let max = list |> List.maxBy (fun item -> item.Size)
        Some max

list |> maxNameAndSize2 |> printf "%A\n"
[] |> maxNameAndSize2 |> printf "%A\n"