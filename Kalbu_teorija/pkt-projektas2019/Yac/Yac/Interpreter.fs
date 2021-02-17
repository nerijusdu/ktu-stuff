namespace Yac

module Interpreter =
    open System.Collections.Generic
    open AST
    
    exception NoVarFound of string * string
    exception VarExists of string * string
    exception UnknownType of string
    exception ParsingError of string
    
    type Value =
    | Int of int
    | Float of float
    | String of string
    | Bool of bool
    type Function =
        struct
            val name: ID
            val block: Block
            val args: Option<Arg list>
            val returnType: Option<Type>
            val error: Option<Error>
            new(name, block, args, returnType, error) = {
                name = name;
                block = block;
                args = args;
                returnType = returnType;
                error = error
            }
        end
    type VarLookup = Dictionary<ID,Value>
    type FuncLookup = Dictionary<ID, Function>
    
    let getVar (state: (VarLookup * FuncLookup), id: ID) =
        let (vars, funcs) = state
        if not(vars.ContainsKey(id)) then
            raise(NoVarFound(id.ToString(), "variable"))
            
        vars.[id]            
            
    let setVar (state: (VarLookup * FuncLookup), id: ID, value: Value) =
        let (vars, funcs) = state
        if vars.ContainsKey(id) then
            raise(VarExists(id.ToString(), "variable"))
            
        vars.Add(id, value)
        
    let setFunc (state: (VarLookup * FuncLookup), name, block, args, _type, error) =
        let (vars, funcs) = state
        if funcs.ContainsKey(name) then
            raise(VarExists(name.ToString(), "function"))
        
        let func = new Function(name, block, args, _type, error)
        funcs.Add(name, func)
        
    let getFunc (state: (VarLookup * FuncLookup), id: ID) =
        let (vars, funcs) = state
        if not(funcs.ContainsKey(id)) then
            raise(NoVarFound(id.ToString(), "function"))
            
        funcs.[id]    
        
    let aritEval (state: (VarLookup * FuncLookup), expr: AritExpr) : Value =
        let numToVal x =
            match x with
            | Integer y -> y |> Int
            | NumericExpr.Float y -> y |> Float
            | NumId y -> getVar(state, y)
            
        let add x y =
            match (x, y) with
            | (Int intX, Int intY) -> intX + intY |> Int
            | (Float floatX, Float floatY) -> floatX + floatY |> Float
            | _ -> raise(ParsingError("Unsupported operations between different types"))
                
        let sub x y =
            match (x, y) with
            | (Int intX, Int intY) -> intX - intY |> Int
            | (Float floatX, Float floatY) -> floatX - floatY |> Float
            | _ -> raise(ParsingError("Unsupported operations between different types"))
        
        let mult x y =
            match (x, y) with
            | (Int intX, Int intY) -> intX * intY |> Int
            | (Float floatX, Float floatY) -> floatX * floatY |> Float
            | _ -> raise(ParsingError("Unsupported operations between different types"))
            
        let div x y =
            match (x, y) with
            | (Int intX, Int intY) -> intX / intY |> Int
            | (Float floatX, Float floatY) -> floatX / floatY |> Float
            | _ -> raise(ParsingError("Unsupported operations between different types"))
            
        match expr with
        | AST.Add(x, y) -> add (numToVal x) (numToVal y)
        | AST.Sub(x, y) -> sub (numToVal x) (numToVal y)
        | AST.Mult(x, y) -> mult (numToVal x) (numToVal y)
        | AST.Div(x, y) -> div (numToVal x) (numToVal y)
        
    let evalExpresion (state: (VarLookup * FuncLookup), expr: Expr) : Value =
        match expr with
        | AST.Num num ->
            match num with
            | NumId id -> getVar(state, id)
            | Integer value -> value |> Value.Int
            | NumericExpr.Float value -> value |> Value.Float
        | AST.Bool b ->
            let unwrap = function | Boolean boolExpr -> boolExpr
            unwrap b |> Value.Bool
        | AST.Str str ->
            let unwrap = function | AST.String boolExpr -> boolExpr
            let string = unwrap str
            let n = String.length string
            // trim "
            string.[1..(n-2)] |> Value.String
        | AST.Arit arit -> aritEval(state, arit)
        | AST.Id id -> getVar(state, id)
    
    let evalDeclaration (state: (VarLookup * FuncLookup), declaration: AST.Declaration) =
        match declaration with
        | AST.DeclareConst (name, value) -> setVar(state, name, evalExpresion(state, value))
        | AST.FunctionDeclare (name, block, args, _type, error) -> setFunc(state, name, block, args, _type, error)
        | _ -> ()
        
    let rec eval (state: (VarLookup * FuncLookup), stmt: Stmt) =
        match stmt with
        | AST.Declare declaration -> evalDeclaration(state, declaration)
        | AST.FunctionCall fnCall -> evalFunction(state, fnCall) |> ignore // todo: ignore for testing
        | _ -> ()
        |> ignore
        state
        
    and evalFunction (state: (VarLookup * FuncLookup), fnCall: FnCall): Value =
        let (globalVars, globalFuncs) = state
        
        let unwrap = function | Call(id, args) -> (id, args)
        let unwrapBlock = function | BlockStmts(stmts, rtrn) -> (stmts, rtrn)
        let unwrapReturn = function | ReturnExpr x -> x
        let unwrapArg = function | Argument x -> x
        
        let (id, args) = unwrap fnCall
        let func = getFunc(state, id)
        let (stmts, rtrn) = unwrapBlock func.block
        
        let vars = new VarLookup(globalVars)
        let funcs = new FuncLookup(globalFuncs)
        let scopedState = vars, funcs
        
        if func.args.IsSome then
            for i = 0 to func.args.Value.Length - 1 do
                let id = unwrapArg func.args.Value.[i]
                let value = evalExpresion(state, args.Value.[i])
                setVar(scopedState, id, value)
                
        if stmts.IsSome then
            for stmt in stmts.Value do
                eval(scopedState, stmt) |> ignore
                
        let result =
            if rtrn.IsSome then
                evalExpresion(scopedState, (unwrapReturn rtrn.Value))
            else
                0 |> Int
        
        printfn "%s result: %s" (func.name.ToString()) (result.ToString())
        result
        
    let run statements =
        let vars = new VarLookup()
        let funcs = new FuncLookup()
        let state = vars, funcs
        printfn "----------------------"
        try
            for stmt in (List.rev statements) do
                printfn "%s" (stmt.ToString())
                eval(state, stmt) |> ignore
        with
            | :? NoVarFound as ex -> printfn "%s not declared: %s" ex.Data1 ex.Data0
            | :? VarExists as ex -> printfn "%s already exists: %s" ex.Data1 ex.Data0
            | :? UnknownType as ex -> printfn "Unknown type: %s" ex.Data0
            | :? ParsingError as ex -> printfn "Parsing error: %s" ex.Data0
            
        state