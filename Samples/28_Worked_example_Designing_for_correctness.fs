﻿module _28_Worked_example_Designing_for_correctness
// Worked example: Designing for correctness

// In this post, we'll see how you can design for correctness (or at least, for the 
// requirements as you currently understand them), by which I mean that a client of a 
// well designed model will not be able to put the system into an illegal state — a 
// state that doesn't meet the requirements. You literally cannot create incorrect code 
// because the compiler will not let you.
// For this to work, we do have to spend some time up front thinking about design and 
// making an effort to encode the requirements into the types that you use. If you just 
// use strings or lists for all your data structures, you will not get any benefit from 
// the type checking.
// We'll use a simple example. Let's say that you are designing an e-commerce site which 
// has a shopping cart and you are given the following requirements.
// - You can only pay for a cart once.
// - Once a cart is paid for, you cannot change the items in it.
// - Empty carts cannot be paid for.

// A bad design in C#
// In C#, we might think that this is simple enough and dive straight into coding. Here 
// is a straightforward implementation in C# that seems OK at first glance.
//public class NaiveShoppingCart<TItem> {
//   private List<TItem> items;
//   private decimal paidAmount;

//   public NaiveShoppingCart() {
//      this.items = new List<TItem>();
//      this.paidAmount = 0;
//   }

//   /// Is cart paid for?
//   public bool IsPaidFor { get { return this.paidAmount > 0; } }

//   /// Readonly list of items
//   public IEnumerable<TItem> Items { get {return this.items; } }

//   /// add item only if not paid for
//   public void AddItem(TItem item) {
//      if (!this.IsPaidFor) 
//         this.items.Add(item);
//   }

//   /// remove item only if not paid for
//   public void RemoveItem(TItem item)
//   {
//      if (!this.IsPaidFor)
//         this.items.Remove(item);
//   }

//   /// pay for the cart
//   public void Pay(decimal amount)
//   {
//      if (!this.IsPaidFor)
//         this.paidAmount = amount;
//   }
//}

// Unfortunately, it's actually a pretty bad design:
// One of the requirements is not even met. Can you see which one?
// It has a major design flaw, and a number of minor ones. Can you see what they are?
// So many problems in such a short piece of code!
// What would happen if we had even more complicated requirements and the code was 
// thousands of lines long? For example, the fragment that is repeated everywhere:
// if (!this.IsPaidFor) { do something }

// looks like it will be quite brittle if requirements change in some methods but not 
// others.
// Before you read the next section, think for a minute how you might better implement 
// the requirements above in C#, with these additional requirements:
// - If you try to do something that is not allowed in the requirements, you will get a 
//   compile time error, not a run time error. For example, you must create a design such 
//   that you cannot even call the RemoveItem method from an empty cart.
// - The contents of the cart in any state should be immutable. The benefit of this is 
//   that if I am in the middle of paying for a cart, the cart contents can't change even 
//   if some other process is adding or removing items at the same time.

// A correct design in F#
// Let's step back and see if we can come up with a better design. Looking at these 
// requirements, it's obvious that we have a simple state machine with three states and 
// some state transitions:
// - A Shopping Cart can be Empty, Active or PaidFor
// - When you add an item to an Empty cart, it becomes Active
// - When you remove the last item from an Active cart, it becomes Empty
// - When you pay for an Active cart, it becomes PaidFor
// And now we can add the business rules to this model:
// - You can add an item only to carts that are Empty or Active
// - You can remove an item only from carts that are Active
// - You can only pay for carts that are Active

// It's worth noting that these kinds of state-oriented models are very common in business 
// systems. Product development, customer relationship management, order processing, and 
// other workflows can often be modeled this way.
// Now we have the design, we can reproduce it in F#:
type CartItem = string    // placeholder for a more complicated type

type EmptyState = NoItems // don't use empty list! We want to
                          // force clients to handle this as a 
                          // separate case. E.g. "you have no 
                          // items in your cart"

type ActiveState = { UnpaidItems : CartItem list; }
type PaidForState = { PaidItems : CartItem list; 
                      Payment : decimal}

type Cart = 
    | Empty of EmptyState 
    | Active of ActiveState 
    | PaidFor of PaidForState 

// Next we can create the operations for each state. The main thing to note is each 
// operation will always take one of the States as input and return a new Cart. That is, 
// you start off with a particular known state, but you return a Cart which is a wrapper 
// for a choice of three possible states.

// =============================
// operations on empty state
// =============================

let addToEmptyState item = 
   // returns a new Active Cart
   Cart.Active {UnpaidItems=[item]}

// =============================
// operations on active state
// =============================

let addToActiveState state itemToAdd = 
   let newList = itemToAdd :: state.UnpaidItems
   Cart.Active {state with UnpaidItems=newList }

let removeFromActiveState state itemToRemove = 
   let newList = state.UnpaidItems 
                 |> List.filter (fun i -> i<>itemToRemove)
                
   match newList with
   | [] -> Cart.Empty NoItems
   | _ -> Cart.Active {state with UnpaidItems=newList} 

let payForActiveState state amount = 
   // returns a new PaidFor Cart
   Cart.PaidFor {PaidItems=state.UnpaidItems; Payment=amount}

// Next, we attach the operations to the states as methods
type EmptyState with
   member this.Add = addToEmptyState 

type ActiveState with
   member this.Add = addToActiveState this 
   member this.Remove = removeFromActiveState this 
   member this.Pay = payForActiveState this 

// And we can create some cart level helper methods as well. At the cart level, we have to 
// explicitly handle each possibility for the internal state with a match..with expression.
let addItemToCart cart item =  
   match cart with
   | Empty state -> state.Add item
   | Active state -> state.Add item
   | PaidFor state ->  
       printfn "ERROR: The cart is paid for"
       cart   

let removeItemFromCart cart item =  
   match cart with
   | Empty state -> 
      printfn "ERROR: The cart is empty"
      cart   // return the cart 
   | Active state -> 
      state.Remove item
   | PaidFor state ->  
      printfn "ERROR: The cart is paid for"
      cart   // return the cart

let displayCart cart  =  
   match cart with
   | Empty state -> 
      printfn "The cart is empty"   // can't do state.Items
   | Active state -> 
      printfn "The cart contains %A unpaid items"
                                                state.UnpaidItems
   | PaidFor state ->  
      printfn "The cart contains %A paid items. Amount paid: %f"
                                    state.PaidItems state.Payment

type Cart with
   static member NewCart = Cart.Empty NoItems
   member this.Add = addItemToCart this 
   member this.Remove = removeItemFromCart this 
   member this.Display = displayCart this 

// Testing the design
let emptyCart = Cart.NewCart
printf "emptyCart="; emptyCart.Display

let cartA = emptyCart.Add "A"
printf "cartA="; cartA.Display

// We now have an active cart with one item in it. Note that “cartA” is a completely 
// different object from “emptyCart” and is in a different state.
let cartAB = cartA.Add "B"
printf "cartAB="; cartAB.Display

let cartB = cartAB.Remove "A"
printf "cartB="; cartB.Display

let emptyCart2 = cartB.Remove "B"
printf "emptyCart2="; emptyCart2.Display

// So far, so good. Again, all these are distinct objects in different states,
// Let’s test the requirement that you cannot remove items from an empty cart:
let emptyCart3 = emptyCart2.Remove "B"    //error
printf "emptyCart3="; emptyCart3.Display

// An error — just what we want!
// Now say that we want to pay for a cart. We didn't create this method at the Cart level, 
// because we didn't want to tell the client how to handle all the cases. This method only 
// exists for the Active state, so the client will have to explicitly handle each case and 
// only call the Pay method when an Active state is matched.
// First we'll try to pay for cartA.
//  try to pay for cartA
let cartAPaid = 
    match cartA with
    | Empty _ | PaidFor _ -> cartA 
    | Active state -> state.Pay 100m
printf "cartAPaid="; cartAPaid.Display

// The result was a paid cart.
// Now we'll try to pay for the emptyCart.
//  try to pay for emptyCart
let emptyCartPaid = 
    match emptyCart with
    | Empty _ | PaidFor _ -> emptyCart
    | Active state -> state.Pay 100m
printf "emptyCartPaid="; emptyCartPaid.Display

// Nothing happens. The cart is empty, so the Active branch is not called. We might want 
// to raise an error or log a message in the other branches, but no matter what we do we 
// cannot accidentally call the Pay method on an empty cart, because that state does not 
// have a method to call!
// The same thing happens if we accidentally try to pay for a cart that is already paid.
//  try to pay for cartAB 
let cartABPaid = 
    match cartAB with
    | Empty _ | PaidFor _ -> cartAB // return the same cart
    | Active state -> state.Pay 100m

//  try to pay for cartAB again
let cartABPaidAgain = 
    match cartABPaid with
    | Empty _ | PaidFor _ -> cartABPaid  // return the same cart
    | Active state -> state.Pay 100m

// You might argue that the client code above might not be representative of code in the 
// real world — it is well-behaved and already dealing with the requirements.
// So what happens if we have badly written or malicious client code that tries to force 
// payment:
// match cartABPaid with
// | Empty state -> state.Pay 100m
// | PaidFor state -> state.Pay 100m
// | Active state -> state.Pay 100m
// If we try to force it like this, we will get compile errors. There is no way the client 
// can create code that does not meet the requirements.

// Summary
// We have designed a simple shopping cart model which has many benefits over the C# 
// design.
// It maps to the requirements quite clearly. It is impossible for a client of this API 
// to call code that doesn't meet the requirements.
// Using states means that the number of possible code paths is much smaller than the C# 
// version, so there will be many fewer unit tests to write.
// Each function is simple enough to probably work the first time, as, unlike the C# 
// version, there are no conditionals anywhere.