module MDB

open AST
open System.IO
open Microsoft.FSharp.Text.Lexing
open Yac
open Yac.Interpreter

let parseFromFile (fileName:string) = 
    use textReader = new System.IO.StreamReader(fileName)
    let lexbuf = LexBuffer<char>.FromTextReader textReader
    try
        let tree = Parser.start Lexer.tokenstream lexbuf
        Some tree
    with e ->
        let pos = lexbuf.EndPos
        printf "Error near line %d, character %d\n%s\n" (pos.pos_lnum + 1) (pos.pos_cnum - pos.pos_bol) (e.ToString())
        None

let compareNumeric (expected:string) (tree:string) = 
    use sr = new System.IO.StreamReader (expected)
    let expectedText = sr.ReadToEnd()
    if tree.Contains(expectedText)
    then "Test passed."
    else "Test failed. " + tree

let print (str: string) = 
    printfn "%s" str
   
let printState (state: VarLookup * FuncLookup) =
    let (vars, funcs) = state
    let unwrapId = function | Identifier x -> x
    
    printfn "Variables:"
    for key in vars.Keys do
        let var = vars.[key]
        let value =
            match var with
            | Value.Int x -> x.ToString()
            | Value.Bool x -> x.ToString()
            | Value.String x -> x.ToString()
            | Value.Float x -> x.ToString()
        let id = unwrapId key
        printfn "%s: %s" id value
    
    printfn "Functions:"
    for key in funcs.Keys do
        let id = unwrapId key
        printfn "%s" id

let testCase file expect =
    let tree = Path.Combine(__SOURCE_DIRECTORY__, file) |> parseFromFile
    if tree.IsSome then
        tree.Value.ToString()
        |> compareNumeric (Path.Combine(__SOURCE_DIRECTORY__, expect))
        |> print
        
        let result = Interpreter.run tree.Value.Statements
        printState result

// Note: line endings in result files should be LF to match console
//testCase "TestCases/Assign.txt" "TestCases/Assign.Res.txt"
//testCase "TestCases/Arithmetics.txt" "TestCases/Arithmetics.Res.txt"
//testCase "TestCases/EmptyFunction.txt" "TestCases/EmptyFunction.Res.txt"
//testCase "TestCases/Function.txt" "TestCases/Function.Res.txt"
//testCase "TestCases/FunctionParams.txt" "TestCases/FunctionParams.Res.txt"
testCase "TestCases/FunctionCall.txt" "TestCases/FunctionCall.Res.txt"
//testCase "TestCases/If.txt" "TestCases/If.Res.txt"
//testCase "TestCases/ThrowIf.txt" "TestCases/ThrowIf.Res.txt"
//testCase "TestCases/FunctionType.txt" "TestCases/FunctionType.Res.txt"

printfn "Press any key to continue..."
//System.Console.ReadLine() |> ignore