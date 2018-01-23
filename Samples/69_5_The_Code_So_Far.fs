module _69_5_The_Code_So_Far

// Let's refactor the Contact code now, with the new wrapper types and modules added.

module EmailAddress = 
    type T = EmailAddress of string

    // create with continuation
    let createWithCont success failure (s:string) = 
        if System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$") 
            then success (EmailAddress s)
            else failure "Email address must contain an @ sign"

    // create directly
    let create s = 
        let success e = Some e
        let failure _  = None
        createWithCont success failure s

    // unwrap with continuation
    let apply f (EmailAddress e) = f e

    // unwrap directly
    let value e = apply id e

module ZipCode = 
    type T = ZipCode of string

    // create with continuation
    let createWithCont success failure  (s:string) = 
        if System.Text.RegularExpressions.Regex.IsMatch(s,@"^\d{5}$") 
            then success (ZipCode s) 
            else failure "Zip code must be 5 digits"
    
    // create directly
    let create s = 
        let success e = Some e
        let failure _  = None
        createWithCont success failure s

    // unwrap with continuation
    let apply f (ZipCode e) = f e

    // unwrap directly
    let value e = apply id e

module StateCode = 
    type T = StateCode of string

    // create with continuation
    let createWithCont success failure  (s:string) = 
        let s' = s.ToUpper()
        let stateCodes = ["AZ";"CA";"NY"] //etc
        if stateCodes |> List.exists ((=) s')
            then success (StateCode s') 
            else failure "State is not in list"
    
    // create directly
    let create s = 
        let success e = Some e
        let failure _  = None
        createWithCont success failure s

    // unwrap with continuation
    let apply f (StateCode e) = f e

    // unwrap directly
    let value e = apply id e

type PersonalName = 
    {
    FirstName: string;
    MiddleInitial: string option;
    LastName: string;
    }

type EmailContactInfo = 
    {
    EmailAddress: EmailAddress.T;
    IsEmailVerified: bool;
    }

type PostalAddress = 
    {
    Address1: string;
    Address2: string;
    City: string;
    State: StateCode.T;
    Zip: ZipCode.T;
    }

type PostalContactInfo = 
    {
    Address: PostalAddress;
    IsAddressValid: bool;
    }

type Contact = 
    {
    Name: PersonalName;
    EmailContactInfo: EmailContactInfo;
    PostalContactInfo: PostalContactInfo;
    }

// By the way, notice that we now have quite a lot of duplicate code in the three wrapper 
// type modules. What would be a good way of getting rid of it, or at least making it 
// cleaner?

// Summary
// To sum up the use of discriminated unions, here are some guidelines:
// - Do use single case discriminated unions to create types that represent the domain 
//   accurately.
// - If the wrapped value needs validation, then provide constructors that do the 
//   validation and enforce their use.
// - Be clear what happens when validation fails. In simple cases, return option types. 
//   In more complex cases, let the caller pass in handlers for success and failure.
// - If the wrapped value has many associated functions, consider moving it into its own 
//   module.
// - If you need to enforce encapsulation, use signature files.
// We're still not done with refactoring. We can alter the design of types to enforce 
// business rules at compile time -- making illegal states unrepresentable.

// Update
// Many people have asked for more information on how to ensure that constrained types 
// such as EmailAddress are only created through a special constructor that does the 
// validation. So I have created a gist here 
// https://gist.github.com/swlaschin/54cfff886669ccab895a
// that has some detailed examples of other ways of doing it.