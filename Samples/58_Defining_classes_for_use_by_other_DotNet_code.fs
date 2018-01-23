// Tip: defining classes for use by other .NET code
// If you are defining classes that need to interop with other .NET code, do not define 
// them inside a module! Define them in a namespace instead, outside of any module.
// The reason for this is that F# modules are exposed as static classes, and any F# 
// classes defined inside a module are then defined as nested classes within the static 
// class, which can mess up your interop. For example, some unit test runners don’t like 
// static classes.
// F# classes which are defined outside a module are generated as normal top-level .NET 
// classes, which is probably what you want. But remember that (as discussed in a previous 
// post) if you don’t declare a namespace specifically, your class will be placed in an 
// automatically generated module, and will be nested without your knowledge.
// Here’s an example of two F# classes, one defined outside a module and one defined 
// inside:
// Note: this code will not work in an .FSX script, 
// only in an .FS source file.
namespace MyNamespace

type TopLevelClass() = 
    let nothing = 0

module MyModule = 
    
    type NestedClass() = 
        let nothing = 0

// And here’s how the same code might look in C#:
//namespace MyNamespace
//{
//  public class TopLevelClass
//  {
//  // code
//  }

//  public static class MyModule
//  {
//    public class NestedClass
//    {
//    // code
//    }
//  }
//}

