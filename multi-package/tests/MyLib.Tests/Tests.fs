module MyLib.Tests.Tests

open Xunit
open Swensen.Unquote
open MyLib
open MyLib.Extension

[<Fact>]
let ``hello returns greeting`` () =
    test <@ Say.hello "World" = "Hello, World!" @>

[<Fact>]
let ``formal returns formal greeting`` () =
    test <@ Greet.formal "World" = "Good day, World. Hello, World!" @>
