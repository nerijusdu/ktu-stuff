// https://uva.onlinejudge.org/index.php?option=com_onlinejudge&Itemid=8&category=448&page=show_problem&problem=4331

open System
open System.IO

let readLines filePath = File.ReadLines(filePath)

let print (line: string) = printf "%s\n" line
   
let startsWithCount (path: string, lines: seq<string>) =
    lines 
    |> Seq.filter(fun (y: string) -> y.StartsWith(path))
    |> Seq.length

let replaceFirst (text: string, search: string, replace: string) =
    let pos = text.IndexOf(search)
    if pos < 0 then
        text
    else
        text.Substring(0, pos) + replace + text.Substring(pos + search.Length)
        
let hasDepth (lines: seq<string>) = Seq.exists(fun x -> String.exists(fun c -> c.Equals '\\') x) lines

let rec handleThings (lines: seq<string>, spaces: string) : (seq<string>) =
    if hasDepth lines then
        let transformedLines =
            lines
            |> Seq.filter(fun x -> startsWithCount(x, lines).Equals 1)
            |> Seq.map(fun x -> replaceFirst(x, "\\", "\n" + spaces))
        Seq.iter print transformedLines
        printf "\n"
        handleThings(transformedLines, (spaces + "  "))
    else
        lines

[<EntryPoint>]
let main argv =
    let lines = readLines "data.txt" |> Seq.sort
    let result = 
        handleThings(lines, "  ") 
        |> Seq.map(fun x -> x.Split('\n'))
        |> Seq.concat
        |> Seq.distinct

    Seq.iter print result

    Console.ReadKey() |> ignore
    0 // return an integer exit code
