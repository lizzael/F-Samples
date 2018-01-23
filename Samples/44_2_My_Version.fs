module _44_2_My_Version

let RomanToInt (str: string) =
    let list = List.ofArray (str.ToCharArray())

    let Return s r =
        match r with
        | Some v -> Some (s + v)
        | None -> None

    let Units list = 
        match list with
        | [] -> Some 0
        | ['I'; 'X'] -> Some 9
        | ['V'; 'I'; 'I'; 'I'] -> Some 8
        | ['V'; 'I'; 'I'] -> Some 7
        | ['V';'I'] -> Some 6
        | ['V'] -> Some 5
        | ['I';'V'] -> Some 4
        | ['I';'I';'I'] -> Some 3
        | ['I';'I'] -> Some 2
        | ['I'] -> Some 1
        | _ -> None

    let Tens list = 
        match list with
        | [] -> Some 0
        | 'X'::'C'::rest -> Return 90 (Units rest)
        | 'L'::'X'::'X'::'X'::rest -> Return 80 (Units rest)
        | 'L'::'X'::'X'::rest -> Return 70 (Units rest)
        | 'L'::'X'::rest -> Return 60 (Units rest)
        | 'L'::rest -> Return 50 (Units rest)
        | 'X'::'L'::rest -> Return 40 (Units rest)
        | 'X'::'X'::'X'::rest -> Return 30 (Units rest)
        | 'X'::'X'::rest -> Return 20 (Units rest)
        | 'X'::rest -> Return 10 (Units rest)
        | rest -> Return 0 (Units rest)

    let Hundreds list =
        match list with
        | [] -> Some 0
        | 'C'::'M'::rest -> Return 900 (Tens rest)
        | 'D'::'C'::'C'::'C'::rest -> Return 800 (Tens rest)
        | 'D'::'C'::'C'::rest -> Return 700 (Tens rest)
        | 'D'::'C'::rest -> Return 600 (Tens rest)
        | 'D'::rest -> Return 500 (Tens rest)
        | 'C'::'D'::rest -> Return 400 (Tens rest)
        | 'C'::'C'::'C'::rest -> Return 300 (Tens rest)
        | 'C'::'C'::rest -> Return 200 (Tens rest)
        | 'C'::rest -> Return 100 (Tens rest)
        | rest -> Return 0 (Tens rest)

    match list with
    | [] -> Some 0
    | 'M'::'M'::'M'::rest -> Return 3000 (Hundreds rest)
    | 'M'::'M'::rest -> Return 2000 (Hundreds rest)
    | 'M'::rest -> Return 1000 (Hundreds rest)
    | rest -> Return 0 (Hundreds rest)

let IntToRoman x =
    let list = List.ofArray (x.ToString().ToCharArray()) |> List.rev

    let Return s r =
        match r with
        | Some v -> Some (v + s)
        | None -> None

    let Thousands list =
        match list with
        | [] -> Some ""
        | ['1'] -> Some "M"
        | ['2'] -> Some "MM"
        | ['3'] -> Some "MMM"
        | _ -> None

    let Hundreds list =
        match list with
        | [] -> Some ""
        | '0'::rest -> Return "" (Thousands rest)
        | '1'::rest -> Return "C" (Thousands rest)
        | '2'::rest -> Return "CC" (Thousands rest)
        | '3'::rest -> Return "CCC" (Thousands rest)
        | '4'::rest -> Return "CD" (Thousands rest)
        | '5'::rest -> Return "D" (Thousands rest)
        | '6'::rest -> Return "DC" (Thousands rest)
        | '7'::rest -> Return "DCC" (Thousands rest)
        | '8'::rest -> Return "DCCC" (Thousands rest)
        | '9'::rest -> Return "CM" (Thousands rest)
        | _ -> None // Never happens!

    let Tens list =
        match list with
        | [] -> Some ""
        | '0'::rest -> Return "" (Hundreds rest)
        | '1'::rest -> Return "X" (Hundreds rest)
        | '2'::rest -> Return "XX" (Hundreds rest)
        | '3'::rest -> Return "XXX" (Hundreds rest)
        | '4'::rest -> Return "XL" (Hundreds rest)
        | '5'::rest -> Return "L" (Hundreds rest)
        | '6'::rest -> Return "LX" (Hundreds rest)
        | '7'::rest -> Return "LXX" (Hundreds rest)
        | '8'::rest -> Return "LXXX" (Hundreds rest)
        | '9'::rest -> Return "XC" (Hundreds rest)
        | _ -> None // Never happens!

    match list with
    | [] -> Some ""
    | '0'::rest -> Return "" (Tens rest)
    | '1'::rest -> Return "I" (Tens rest)
    | '2'::rest -> Return "II" (Tens rest)
    | '3'::rest -> Return "III" (Tens rest)
    | '4'::rest -> Return "IV" (Tens rest)
    | '5'::rest -> Return "V" (Tens rest)
    | '6'::rest -> Return "VI" (Tens rest)
    | '7'::rest -> Return "VII" (Tens rest)
    | '8'::rest -> Return "VIII" (Tens rest)
    | '9'::rest -> Return "IX" (Tens rest)
    | _ -> None // Never happens!

[0..4000]
|> Seq.iter (fun p -> 
    let x = p // System.Console.ReadLine() |> int
    printfn "X: %d" x
    
    let roman = IntToRoman x
    match roman with
    | Some v1 -> 
        printfn "Roman: %s" v1
        
        let y = RomanToInt v1
        match y with
        | Some v2 -> 
            printfn "Int: %d" v2
            printfn ""

            if x <> v2 then
                System.Console.ReadLine() |> ignore

        | None ->
            ()

    | None -> 
        ()
    )

printfn ""
let r = RomanToInt "IIII"
printfn "%A" r
printfn "%A" (RomanToInt "VV")
printfn "%A" (RomanToInt "XXXX")
printfn "%A" (RomanToInt "LL")
printfn "%A" (RomanToInt "CCCC")
printfn "%A" (RomanToInt "DD")
printfn "%A" (RomanToInt "MMMM")