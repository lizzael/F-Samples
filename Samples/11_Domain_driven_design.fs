module _11_Domain_driven_design

// The conciseness of the type system in F# is particularly useful when doing domain driven 
// design (DDD). In DDD, for each real world entity and value object, you ideally want to have 
// a corresponding type. This can mean creating hundreds of “little” types, which can be 
// tedious in C#.

// Furthermore, “value” objects in DDD should have structural equality, meaning that two 
// objects containing the same data should always be equal. In C# this can mean more tedium in 
// overriding IEquatable<T>, but in F#, you get this for free by default.

// To show how easy it is to create DDD types in F#, here are some example types that might be 
// created for a simple “customer” domain.

type PersonalName = {FirstName:string; LastName:string}

// Addresses
type StreetAddress = {Line1:string; Line2:string; Line3:string }

type ZipCode =  ZipCode of string   
type StateAbbrev =  StateAbbrev of string
type ZipAndState =  {State:StateAbbrev; Zip:ZipCode }
type USAddress = {Street:StreetAddress; Region:ZipAndState}

type UKPostCode =  PostCode of string
type UKAddress = {Street:StreetAddress; Region:UKPostCode}

type InternationalAddress = {
    Street:StreetAddress; Region:string; CountryName:string}

// choice type  -- must be one of these three specific types
type Address = USAddress | UKAddress | InternationalAddress

// Email
type Email = Email of string

// Phone
type CountryPrefix = Prefix of int
type Phone = {CountryPrefix:CountryPrefix; LocalNumber:string}

type Contact = 
    {
    PersonalName: PersonalName;
    // "option" means it might be missing
    Address: Address option;
    Email: Email option;
    Phone: Phone option;
    }

// Put it all together into a CustomerAccount type
type CustomerAccountId  = AccountId of string
type CustomerType  = Prospect | Active | Inactive

// override equality and deny comparison
[<CustomEquality; NoComparison>]
type CustomerAccount = 
    {
    CustomerAccountId: CustomerAccountId;
    CustomerType: CustomerType;
    ContactInfo: Contact;
    }

    override this.Equals(other) =
        match other with
        | :? CustomerAccount as otherCust -> 
          (this.CustomerAccountId = otherCust.CustomerAccountId)
        | _ -> false

    override this.GetHashCode() = hash this.CustomerAccountId 