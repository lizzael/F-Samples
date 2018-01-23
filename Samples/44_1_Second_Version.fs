module _44_SecondVersion
// Second version

// The code works, but there is something that's bugging me about it. The validation 
// logic seems very complicated. Surely the Romans didn't have to think about all of this?
// And also, I can think of examples that should fail validation, but pass, such as "VIV":
// "VIV" |> toRomanNumeral |> isValid |> printfn "%b --> validation fail!!!"

// We could try to tighten up our validation rules, but let's try another tack. 
// Complicated logic is often a sign that you don't quite understand the domain properly.
// In other words -- could we change the internal model to make everything simpler?
// What about if we stopped trying to map letters to digits, and created a domain that 
// mapped how the Romans thought it. In this model "I", "II", "III", "IV" and so on would 
// each be a separate digit.
// Let's run with it and see what happens.
// Here's the new types for the domain. I now have a digit type for every possible digit. 
// The RomanNumeral type stays the same.

type RomanDigit = 
    | I | II | III | IIII 
    | IV | V 
    | IX | X | XX | XXX | XXXX  
    | XL | L 
    | XC | C | CC | CCC | CCCC 
    | CD | D 
    | CM | M | MM | MMM | MMMM
type RomanNumeral = RomanNumeral of RomanDigit list 

// Output: second version
// Next, converting a single RomanDigit to an integer is the same as before, but 
// with more cases:
let digitToInt =
    function
    | I -> 1 | II -> 2 | III -> 3 | IIII -> 4 
    | IV -> 4 | V -> 5
    | IX -> 9 | X -> 10 | XX -> 20 | XXX -> 30 | XXXX -> 40 
    | XL -> 40 | L -> 50 
    | XC -> 90 | C -> 100 | CC -> 200 | CCC -> 300 | CCCC -> 400 
    | CD -> 400 | D -> 500 
    | CM -> 900 | M -> 1000 | MM -> 2000 | MMM -> 3000 | MMMM -> 4000

// tests
I  |> digitToInt |> printfn "%d"
III  |> digitToInt |> printfn "%d"
V  |> digitToInt |> printfn "%d"
CM  |> digitToInt |> printfn "%d"

// Calculating the sum of the digits is now trivial. No special cases needed:
/// converts a list of digits to an integer
let digitsToInt list = 
    list |> List.sumBy digitToInt 

// tests
[IIII]  |> digitsToInt  |> printfn "%d"
[IV]  |> digitsToInt  |> printfn "%d"
[V;I]  |> digitsToInt  |> printfn "%d"
[IX]  |> digitsToInt  |> printfn "%d"
[M;CM;L;X;X;IX]  |> digitsToInt  |> printfn "%d" // 1979
[M;CM;XL;IV] |> digitsToInt  |> printfn "%d" // 1944

// Finally, the top level function is identical:
/// converts a RomanNumeral to an integer
let toInt (RomanNumeral digits) = digitsToInt digits

// test
let x = RomanNumeral [M;CM;XL;X;IX]
x |> toInt |> printfn "%d"

// Input: second version
// For the input parsing, we'll keep the ParsedChar type. But this time, we have to match 
// 1,2,3, or 4 chars at a time. That means we can't just pull off one character like we did 
// in the first version -- we have to match in the main loop. This means the loop now has to 
// be recursive.
// Also, we want to convert IIII into a single IIII digit rather than 4 separate I digits, 
// so we put the longest matches at the front.
type ParsedChar = 
    | Digit of RomanDigit 
    | BadChar of char

let rec toRomanDigitListRec charList = 
    match charList with
    // match the longest patterns first
    // 4 letter matches
    | 'I'::'I'::'I'::'I'::ns -> 
        Digit IIII :: (toRomanDigitListRec ns)
    | 'X'::'X'::'X'::'X'::ns -> 
        Digit XXXX :: (toRomanDigitListRec ns)
    | 'C'::'C'::'C'::'C'::ns -> 
        Digit CCCC :: (toRomanDigitListRec ns)
    | 'M'::'M'::'M'::'M'::ns -> 
        Digit MMMM :: (toRomanDigitListRec ns)
    // 3 letter matches
    | 'I'::'I'::'I'::ns -> 
        Digit III :: (toRomanDigitListRec ns)
    | 'X'::'X'::'X'::ns -> 
        Digit XXX :: (toRomanDigitListRec ns)
    | 'C'::'C'::'C'::ns -> 
        Digit CCC :: (toRomanDigitListRec ns)
    | 'M'::'M'::'M'::ns -> 
        Digit MMM :: (toRomanDigitListRec ns)
    // 2 letter matches
    | 'I'::'I'::ns -> 
        Digit II :: (toRomanDigitListRec ns)
    | 'X'::'X'::ns -> 
        Digit XX :: (toRomanDigitListRec ns)
    | 'C'::'C'::ns -> 
        Digit CC :: (toRomanDigitListRec ns)
    | 'M'::'M'::ns -> 
        Digit MM :: (toRomanDigitListRec ns)
    | 'I'::'V'::ns -> 
        Digit IV :: (toRomanDigitListRec ns)
    | 'I'::'X'::ns -> 
        Digit IX :: (toRomanDigitListRec ns)
    | 'X'::'L'::ns -> 
        Digit XL :: (toRomanDigitListRec ns)
    | 'X'::'C'::ns -> 
        Digit XC :: (toRomanDigitListRec ns)
    | 'C'::'D'::ns -> 
        Digit CD :: (toRomanDigitListRec ns)
    | 'C'::'M'::ns -> 
        Digit CM :: (toRomanDigitListRec ns)
    // 1 letter matches
    | 'I'::ns -> 
        Digit I :: (toRomanDigitListRec ns)
    | 'V'::ns -> 
        Digit V :: (toRomanDigitListRec ns)
    | 'X'::ns -> 
        Digit X :: (toRomanDigitListRec ns)
    | 'L'::ns -> 
        Digit L :: (toRomanDigitListRec ns)
    | 'C'::ns -> 
        Digit C :: (toRomanDigitListRec ns)
    | 'D'::ns -> 
        Digit D :: (toRomanDigitListRec ns)
    | 'M'::ns -> 
        Digit M :: (toRomanDigitListRec ns)
    // bad letter matches
    | badChar::ns -> 
        BadChar badChar :: (toRomanDigitListRec ns)
    // 0 letter matches
    | [] -> 
        []

// Well, this is much longer than the first version, but otherwise basically the same.
// The top level functions are unchanged.
let toRomanDigitList (s:string) = 
    s.ToCharArray() 
    |> List.ofArray 
    |> toRomanDigitListRec

/// Convert a string to a RomanNumeral
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

// test good cases
"IIII"  |> toRomanNumeral |> printfn "%A"
"IV"  |> toRomanNumeral |> printfn "%A"
"VI"  |> toRomanNumeral |> printfn "%A"
"IX"  |> toRomanNumeral |> printfn "%A"
"MCMLXXIX"  |> toRomanNumeral |> printfn "%A"
"MCMXLIV" |> toRomanNumeral |> printfn "%A"
"" |> toRomanNumeral |> printfn "%A"

// error cases
"MC?I" |> toRomanNumeral |> printfn "%A"
"abc" |> toRomanNumeral |> printfn "%A"

// Validation: second version
// Finally, let's see how the new domain model affects the validation rules. Now, the rules 
// are much simpler. In fact, there is only one.
// Each digit must be smaller than the preceding digit
// check for validity
let rec isValidDigitList digitList =
    match digitList with
    // empty list is valid
    | [] -> true
    // a following digit that is equal or larger is an error
    | d1::d2::_ 
        when d1 <= d2  -> 
            false
    // A single digit is always allowed
    | _::ds -> 
        // check the remainder of the list
        isValidDigitList ds 

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
// Alas, after all that, we still didn't fix the bad case that triggered the rewrite!

"VIV" |> toRomanNumeral |> isValid  |> printfn "%b"
// There is a not-too-complicated fix for this, but I think it's time to leave it alone now!

// Comparing the two versions
// Which version did you like better? The second one is more longwinded because it has many 
// more cases, but on the other hand, the actual logic is the same or simpler in all areas, 
// with no special cases. And as a result, the total number of lines of code is about the 
// same for both versions.
// Overall, I prefer the second implementation because of the lack of special cases.
// As a fun experiment, try writing the same code in C# or your favorite imperative language!

// Making it object-oriented
// Finally, let's see how we might make this object oriented. We don't care about the helper 
// functions, so we probably just need three methods:
// - A static constructor
// - A method to convert to a int
// - A method to convert to a string
// - And here they are:
type RomanNumeral with

    static member FromString s = 
        toRomanNumeral s

    member this.ToInt() = 
        toInt this

    override this.ToString() = 
        sprintf "%A" this

// Note: you can ignore the compiler warning about deprecated overrides.
// Let's use this in an object oriented way now:

let r = RomanNumeral.FromString "XXIV"
let s = r.ToString()
let i = r.ToInt()

// Summary
// In this post we've seen lots and lots of pattern matching!
// But again, as with the last post, what's equally important is that we've seen how easy it 
// is to create a properly designed internal model for very trivial domains. And again, our 
// internal model used no primitive types -- there is no excuse not to create lots of little 
// types in order to represent the domain better. For example, the ParsedChar type -- would 
// you have bothered to create that in C#?
// And as should be clear, the choice of an internal model can make a lot of difference to 
// the complexity of the design. But if and when we do refactor, the compiler will almost 
// always warn us if we have forgotten something.