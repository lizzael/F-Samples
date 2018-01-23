module _44_Worked_example_Roman_numerals
// More pattern matching in practice

// Last time we looked at parsing a command line. This time we'll we'll look at another 
// pattern matching example, this time using Roman numerals.
// As before, we will try to have a "pure" internal model with separate stages to convert 
// the input to the internal model, and then another separate stage to convert from the 
// internal model to the output.

// Input 
// -> Transformation to internal model 
// -> Internal Model 
// -> Transformation from internal model 
// -> Output

// Requirements
// Let's start with the requirements:
// - Accept a string of letters like "MMMXCLXXIV" as a string and convert it to an 
//   integer. The conversions are: I=1; V=5; X=10; L=50; C=100; D=500; and M=1000;
// - If a lower letter comes before a higher one, the value of the higher is reduced 
//   accordingly, so IV=4; IX=9; XC=90; and so on.
// - As an additional step, validate the string of letters to see if it is a valid number. 
//   For example: "IIVVMM" is a not a valid roman numeral.

// First version
// As before we'll start by first creating the internal model, and then look at how we can 
// parse the input into the internal model.
// Here's a first stab at the model. We'll treat a RomanNumeral as a list of RomanDigits.
// type RomanDigit = int
// type RomanNumeral = RomanDigit list 

// No, stop right there! A RomanDigit is not just any digit, it has to be taken from a 
// limited set.
// Also RomanNumeral should not just be a type alias for a list of digits. It would be 
// better if it was its own special type as well. We can do this by creating a single case 
// union type.
// Here's a much better version:
type RomanDigit = I | V | X | L | C | D | M
type RomanNumeral = RomanNumeral of RomanDigit list 

// Output: Converting a numeral to an int
// Now let's do the output logic, converting a Roman numeral to an int.
// The digit conversion is easy:
/// Converts a single RomanDigit to an integer
let digitToInt =
    function
    | I -> 1
    | V -> 5
    | X -> 10
    | L -> 50
    | C -> 100
    | D -> 500
    | M -> 1000

// tests
I  |> digitToInt |> printfn "%d"
V  |> digitToInt |> printfn "%d"
M  |> digitToInt |> printfn "%d"

// Note that we're using the function keyword instead of the match..with expression.

// To convert a list of digits, we'll use a recursive loop again. There is a special case 
// when we need to look ahead to the next digit, and if it is larger than the current one, 
// then use their difference.
let rec digitsToInt =
    function
    // empty is 0
    | [] -> 0
    // special case when a smaller comes before larger
    // convert both digits and add the difference to the sum 
    // Example: "IV" and "CM"
    | smaller::larger::ns when smaller < larger -> 
        (digitToInt larger - digitToInt smaller)  + digitsToInt ns
    // otherwise convert the digit and add to the sum 
    | digit::ns -> 
        digitToInt digit + digitsToInt ns

// tests
[I;I;I;I]  |> digitsToInt |> printfn "%d --> Wrong!!!"
[I;V]  |> digitsToInt |> printfn "%d"
[V;I]  |> digitsToInt |> printfn "%d"
[I;X]  |> digitsToInt |> printfn "%d"
[M;C;M;L;X;X;I;X]  |> digitsToInt  |> printfn "%d" // 1979
[M;C;M;X;L;I;V] |> digitsToInt  |> printfn "%d" // 1944

/// converts a RomanNumeral to an integer
let toInt (RomanNumeral digits) = digitsToInt digits

// test
let x1 = RomanNumeral [I;I;I;I]
printfn "%d --> Wrong!!!" (toInt x1)

let x2 = RomanNumeral [M;C;M;L;X;X;I;X]
printfn "%d" (toInt x2)

// That takes care of the output.
// Input: Converting a string to an Roman Numeral
// Now let's do the input logic, converting a string to our internal model.
// First, let's handle a single character conversion. It seems straightforward.
// let charToRomanDigit =
//     function
//     | 'I' -> I
//     | 'V' -> V
//     | 'X' -> X
//     | 'L' -> L
//     | 'C' -> C
//     | 'D' -> D
//     | 'M' -> M
// The compiler doesn't like that! What happens if we get some other character?
// This is a great example of how exhaustive pattern matching can force you to think 
// about missing requirements.
// So, what should be done for bad input. How about printing an error message?
// Let's try again and add a case to handle all other characters:
// let charToRomanDigit =
//     function
//     | 'I' -> I
//     | 'V' -> V
//     | 'X' -> X
//     | 'L' -> L
//     | 'C' -> C
//     | 'D' -> D
//     | 'M' -> M
//     | ch -> eprintf "%c is not a valid character" ch
// The compiler doesn't like that either! The normal cases return a valid RomanDigit but 
// the error case returns unit. As we saw in the earlier post, every branch must return 
// the same type.
// How can we fix this? We could throw an exception, but that seems a bit excessive. If we 
// think about it some more, there's no way that charToRomanDigit can always return a 
// valid RomanDigit. Sometimes it can, and sometimes it can't. In other words, we need to 
// use something like an option type here.
// But on further consideration, we might also want the caller to know what the bad char 
// was. So we need to create our own little variant on the option type to hold both cases.

// Here's the fixed up version:
type ParsedChar = 
    | Digit of RomanDigit 
    | BadChar of char

let charToRomanDigit =
    function
    | 'I' -> Digit I
    | 'V' -> Digit V
    | 'X' -> Digit X
    | 'L' -> Digit L
    | 'C' -> Digit C
    | 'D' -> Digit D
    | 'M' -> Digit M
    | ch -> BadChar ch

// Note that I have removed the error message. Since the bad char is being returned, the 
// caller can print its own message for the BadChar case.
// Next, we should check the function signature to make sure it is what we expect:
// charToRomanDigit : char -> ParsedChar
// That looks good.
// Now, how can we convert a string into these digits? We convert the string to a char 
// array, convert that into a list, and then do a final conversion using charToRomanDigit.
// let toRomanDigitList s = 
//     s.ToCharArray() // error FS0072
//     |> List.ofArray 
//     |> List.map charToRomanDigit
// But the compiler complains again with "FS0072: Lookup on object of indeterminate type",
// That typically happens when you use a method rather than a function. Any object could 
// implement .ToCharArray() so the type inference cannot tell what type is meant.
// In this case, the solution is just to use an explicit type annotation on the parameter 
// -- our first so far!
let toRomanDigitList (s:string) = 
    s.ToCharArray() 
    |> List.ofArray 
    |> List.map charToRomanDigit
// But look at the signature:
// toRomanDigitList : string -> ParsedChar list
// It still has the pesky ParsedChar in it rather than RomanDigits. How do we want to 
// proceed? Answer, let's pass the buck again and let someone else deal with it!
// "Passing the buck" in this case is actually a good design principle. This function 
// doesn't know what its clients might want to do -- some might want to ignore errors, 
// while others might want to fail fast. So just pass back the information and let them 
// decide.
// In this case, the client is the top level function that creates a RomanNumeral type. 
// Here's our first attempt:
// convert a string to a RomanNumeral
// let toRomanNumeral s = 
//     toRomanDigitList s
//     |> RomanNumeral
// The compiler is not happy -- the RomanNumeral constructor requires a list of 
// RomanDigits, but the toRomanDigitList is giving us a list of ParsedChars instead.
// Now we finally do have to commit to an error handling policy. Let's choose to ignore 
// bad chars, but print out errors when they occur. We'll use the List.choose function for 
// this. It's similar to List.map, but in addition has a filter built into it. Elements 
// that are valid (Some something) are returned, but elements that are None are filtered 
// out.
// Our choose function should thus do the following:
// - For valid digits return Some digit
// - For the invalid BadChars, print the error message and return None.
// If we do this, the output of List.choose will be a list of RomanDigits, exactly as 
// needed as the input to the RomanNumeral constructor.
// Here is everything put together:
/// Convert a string to a RomanNumeral
/// Does not validate the input.E.g. "IVIV" would be valid
let toRomanNumeral s = 
    toRomanDigitList s
    |> List.choose (
        function 
        | Digit digit -> 
            Some digit 
        | BadChar ch -> 
            eprintfn "%c is not a valid character" ch
            None
        )
    |> RomanNumeral

// Let's test!

// test good cases
"IIII"  |> toRomanNumeral |> printfn "%A --> Wrong!!!"
"IV"  |> toRomanNumeral |> printfn "%A"
"VI"  |> toRomanNumeral |> printfn "%A"
"IX"  |> toRomanNumeral |> printfn "%A"
"MCMLXXIX"  |> toRomanNumeral |> printfn "%A"
"MCMXLIV" |> toRomanNumeral |> printfn "%A"
"" |> toRomanNumeral |> printfn "%A"

// error cases
"MC?I" |> toRomanNumeral |> printfn "%A --> Error case!!!"
"abc" |> toRomanNumeral |> printfn "%A --> Error case!!!"
// Ok, everything is good so far. Let's move on to validation.

// Validation rules
// The validation rules were not listed in the requirements, so let's put down our best 
// guess based on what we know about Roman numerals:
// - Five in a row of any digit is not allowed
// - Some digits are allowed in runs of up to 4. They are I,X,C, and M. The others (V,L,D) 
//   can only appear singly.
// - Some lower digits can come before a higher digit, but only if they appear singly. 
//   E.g. "IX" is ok but "IIIX" is not.
// - But this is only for pairs of digits. Three ascending numbers in a row is invalid. 
//   E.g. "IX" is ok but "IXC" is not.
// - A single digit with no runs is always allowed
// We can convert these requirements into a pattern matching function as follows:
let runsAllowed = 
    function 
    | I | X | C | M -> true
    | V | L | D -> false

let noRunsAllowed  = runsAllowed >> not 

// check for validity
let rec isValidDigitList digitList =
    match digitList with
    // empty list is valid
    | [] -> true
    // A run of 5 or more anything is invalid
    // Example:  XXXXX
    | d1::d2::d3::d4::d5::_ 
        when d1=d2 && d1=d3 && d1=d4 && d1=d5 -> 
            false
    // 2 or more non-runnable digits is invalid
    // Example:  VV
    | d1::d2::_ 
        when d1=d2 && noRunsAllowed d1 -> 
            false
    // runs of 2,3,4 in the middle are invalid if next digit is higher
    // Example:  IIIX
    | d1::d2::d3::d4::higher::ds 
        when d1=d2 && d1=d3 && d1=d4 
        && runsAllowed d1 // not really needed because of the order of matching
        && higher > d1 -> 
            false
    | d1::d2::d3::higher::ds 
        when d1=d2 && d1=d3 
        && runsAllowed d1 
        && higher > d1 -> 
            false
    | d1::d2::higher::ds 
        when d1=d2 
        && runsAllowed d1 
        && higher > d1 -> 
            false
    // three ascending numbers in a row is invalid
    // Example:  IVX
    | d1::d2::d3::_  when d1<d2 && d2<= d3 -> 
        false
    // A single digit with no runs is always allowed
    | _::ds -> 
        // check the remainder of the list
        isValidDigitList ds 

// Again, note that "equality" and "less than" did not need to be defined.
// And let's test the validation:
// test valid 
let validList = [
    [I;I;I;I]
    [I;V]
    [I;X]
    [I;X;V]
    [V;X]
    [X;I;V]
    [X;I;X]
    [X;X;I;I]
    ]

let testValid = validList |> List.map isValidDigitList
printfn "Valids: %A" testValid

let invalidList = [
    // Five in a row of any digit is not allowed
    [I;I;I;I;I]
    // Two in a row for V,L, D is not allowed
    [V;V] 
    [L;L] 
    [D;D]
    // runs of 2,3,4 in the middle are invalid if next digit is higher
    [I;I;V]
    [X;X;X;M]
    [C;C;C;C;D]
    // three ascending numbers in a row is invalid
    [I;V;X]
    [X;L;D]
    ]

let testInvalid = invalidList |> List.map isValidDigitList
printfn "Valids: %A" testInvalid

// Finally, we add a top level function to test validity of the RomanNumeral type itself.
// top level check for validity
let isValid (RomanNumeral digitList) =
    isValidDigitList digitList

// test good cases
"IIII"  |> toRomanNumeral |> isValid |> printfn "%b"
"IV"  |> toRomanNumeral |> isValid |> printfn "%b"
"" |> toRomanNumeral |> isValid |> printfn "%b"

// error cases
"IIXX" |> toRomanNumeral |> isValid |> printfn "%b"
"VV" |> toRomanNumeral |> isValid |> printfn "%b"

// grand finale
[ "IIII"; "XIV"; "MMDXC"; 
"IIXX"; "VV"; ]
|> List.map toRomanNumeral 
|> List.iter (function
    | n when isValid n ->
        printfn "%A is valid and its integer value is %i" n (toInt n) 
    | n ->
        printfn "%A is not valid" n
    )

// The entire code for the first version
// Here's all the code in one module:
module RomanNumeralsV1 =
    // ==========================================
    // Types
    // ==========================================
    type RomanDigit = I | V | X | L | C | D | M
    type RomanNumeral = RomanNumeral of RomanDigit list 

    // ==========================================
    // Output logic
    // ==========================================
    /// Converts a single RomanDigit to an integer
    let digitToInt =
        function
        | I -> 1
        | V -> 5
        | X -> 10
        | L -> 50
        | C -> 100
        | D -> 500
        | M -> 1000

    /// converts a list of digits to an integer
    let rec digitsToInt =
        function
        // empty is 0
        | [] -> 0
        // special case when a smaller comes before larger
        // convert both digits and add the difference to the sum 
        // Example: "IV" and "CM"
        | smaller::larger::ns when smaller < larger -> 
            (digitToInt larger - digitToInt smaller)  + digitsToInt ns
        // otherwise convert the digit and add to the sum 
        | digit::ns -> 
            digitToInt digit + digitsToInt ns

    /// converts a RomanNumeral to an integer
    let toInt (RomanNumeral digits) = digitsToInt digits

    // ==========================================
    // Input logic
    // ==========================================
    type ParsedChar = 
        | Digit of RomanDigit 
        | BadChar of char

    let charToRomanDigit =
        function
        | 'I' -> Digit I
        | 'V' -> Digit V
        | 'X' -> Digit X
        | 'L' -> Digit L
        | 'C' -> Digit C
        | 'D' -> Digit D
        | 'M' -> Digit M
        | ch -> BadChar ch

    let toRomanDigitList (s:string) = 
        s.ToCharArray() 
        |> List.ofArray 
        |> List.map charToRomanDigit

    /// Convert a string to a RomanNumeral
    /// Does not validate the input.E.g. "IVIV" would be valid
    let toRomanNumeral s = 
        toRomanDigitList s
        |> List.choose (
            function 
            | Digit digit -> 
                Some digit 
            | BadChar ch -> 
                eprintfn "%c is not a valid character" ch
                None
            )
        |> RomanNumeral

    // ==========================================
    // Validation logic
    // ==========================================
    let runsAllowed = 
        function 
        | I | X | C | M -> true
        | V | L | D -> false

    let noRunsAllowed  = runsAllowed >> not 

    // check for validity
    let rec isValidDigitList digitList =
        match digitList with
        // empty list is valid
        | [] -> true
        // A run of 5 or more anything is invalid
        // Example:  XXXXX
        | d1::d2::d3::d4::d5::_ 
            when d1=d2 && d1=d3 && d1=d4 && d1=d5 -> 
                false
        // 2 or more non-runnable digits is invalid
        // Example:  VV
        | d1::d2::_ 
            when d1=d2 && noRunsAllowed d1 -> 
                false
        // runs of 2,3,4 in the middle are invalid if next digit is higher
        // Example:  IIIX
        | d1::d2::d3::d4::higher::ds 
            when d1=d2 && d1=d3 && d1=d4 
            && runsAllowed d1 // not really needed because of the order of matching
            && higher > d1 -> 
                false
        | d1::d2::d3::higher::ds 
            when d1=d2 && d1=d3 
            && runsAllowed d1 
            && higher > d1 -> 
                false
        | d1::d2::higher::ds 
            when d1=d2 
            && runsAllowed d1 
            && higher > d1 -> 
                false
        // three ascending numbers in a row is invalid
        // Example:  IVX
        | d1::d2::d3::_  when d1<d2 && d2<= d3 -> 
            false
        // A single digit with no runs is always allowed
        | _::ds -> 
            // check the remainder of the list
            isValidDigitList ds 
    // top level check for validity
    let isValid (RomanNumeral digitList) =
        isValidDigitList digitList

"VIV" |> toRomanNumeral |> isValid |> printfn "%b --> validation fail!!!"