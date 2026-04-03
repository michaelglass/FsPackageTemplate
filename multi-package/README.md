# MyLib

<!-- sync:intro:start -->
A brief description of your library.
<!-- sync:intro:end -->

## Installation

```bash
dotnet add package MyLib
dotnet add package MyLib.Extension
```

## Usage

```fsharp
open MyLib
open MyLib.Extension

printfn "%s" (Say.hello "World")
printfn "%s" (Greet.formal "World")
```
