module _25_Mutable_Data

// F# is not dogmatic about immutability; it does support mutable data with the mutable 
// keyword. But turning on mutability is an explicit decision, a deviation from the 
// default, and it is generally only needed for special cases such as optimization, 
// caching, etc, or when dealing with the .NET libraries.
// In practice, a serious application is bound to have some mutable state if it deals 
// with messy world of user interfaces, databases, networks and so on. But F# encourages 
// the minimization of such mutable state. You can generally still design your core 
// business logic to use immutable data, with all the corresponding benefits.