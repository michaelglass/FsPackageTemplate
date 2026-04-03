module MyLib.Tests.Tests

open Xunit
open Swensen.Unquote
open MyLib

[<Fact>]
let ``hello returns greeting`` () =
    test <@ Say.hello "World" = "Hello, World!" @>
