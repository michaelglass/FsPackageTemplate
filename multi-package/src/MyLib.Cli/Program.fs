open MyLib

[<EntryPoint>]
let main argv =
    let name = if argv.Length > 0 then argv[0] else "World"
    printfn "%s" (Say.hello name)
    0
