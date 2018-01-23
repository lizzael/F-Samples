module _03_DownloadingWebPage

// The use of "open" at the top allows us to write "WebRequest" 
// rather than "System.Net.WebRequest". It is similar to a 
// "using System.Net" header in C#.
open System.Net
open System
open System.IO

// Next, we define the fetchUrl function, which takes two arguments, 
// a callback to process the stream, and the url to fetch.
let fetchUrl callback url =
    // We next wrap the url string in a Uri. F# has strict type-checking, 
    // so if instead we had written: let req = WebRequest.Create(url) the 
    // compiler would have complained that it didn't know which version of 
    // WebRequest.Create to use.
    let req = WebRequest.Create(Uri(url)) 

    // When declaring the response, stream and reader values, the "use" 
    // keyword is used instead of "let". This can only be used in conjunction 
    // with classes that implement IDisposable. It tells the compiler to 
    // automatically dispose of the resource when it goes out of scope. This 
    // is equivalent to the C# "using" keyword.
    use resp = req.GetResponse() 
    use stream = resp.GetResponseStream() 
    use reader = new StreamReader(stream) 

    // The last line calls the callback function with the StreamReader and url 
    // as parameters. Note that the type of the callback does not have to be 
    // specified anywhere.
    callback reader url

let myCallback (reader:StreamReader) url = 
    let html = reader.ReadToEnd()
    let html1000 = html.Substring(0,1000)
    printfn "Downloaded %s. First 1000 is %s" url html1000
    html      // return all the html

//test
let google1 = fetchUrl myCallback "http://google.com"

// A very useful feature of F# is that you can "bake in" parameters in a function 
// so that they don’t have to be passed in every time. This is why the url parameter 
// was placed last rather than first, as in the C# version. The callback can be setup 
// once, while the url varies from call to call.

// build a function with the callback "baked in"
let fetchUrl2 = fetchUrl myCallback 

// test
let google2 = fetchUrl2 "http://www.google.com"
let bbc    = fetchUrl2 "http://news.bbc.co.uk"

// test with a list of sites
let sites = ["http://www.bing.com";
             "http://www.google.com";
             "http://www.yahoo.com"]

// process each site in the list
sites |> List.map fetchUrl2 |> printf "%A"