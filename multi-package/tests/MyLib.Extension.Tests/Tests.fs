module MyLib.Extension.Tests.Tests

open Xunit
open Swensen.Unquote
open MyLib.Extension

[<Fact>]
let ``formal returns formal greeting`` () =
    test <@ Greet.formal "World" = "Good day, World. Hello, World!" @>
