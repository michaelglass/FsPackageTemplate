namespace MyLib.Extension

open MyLib

module Greet =
    /// Returns a formal greeting
    let formal name = sprintf "Good day, %s. %s" name (Say.hello name)
