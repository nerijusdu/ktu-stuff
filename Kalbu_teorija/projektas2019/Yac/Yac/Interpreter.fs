namespace Yac

module Interpreter =
    open System.Collections.Generic
    open System.IO
    open AST
    
    exception NoVarFound of string * string
    exception VarExists of string * string
    exception UnknownType of string
    exception ParsingError of string
    exception RunTimeError of string
    
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
    
    let strValue (value: Value) : string =
        match value with
            | Value.Int x -> x.ToString()
            | Value.Bool x -> x.ToString()
            | Value.String x -> x.ToString()
            | Value.Float x -> x.ToString()
    
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
        
    let rec evalCondition (state: (VarLookup * FuncLookup), condition: Condition) =
            
        let compare a op b =
            match op with
            | Eq -> a.Equals b
            | Gt -> a > b
            | Lt -> a < b
            | Ge -> a >= b
            | Le -> a <= b
            
        match condition with
        | Cond(a, op, b) ->
            let A = evalExpresion(state, a)
            let B = evalExpresion(state, b)
            compare A op B
        | And(c1, c2) -> evalCondition(state, c1) && evalCondition(state, c2)
        | Or(c1, c2) -> evalCondition(state, c1) || evalCondition(state, c2)
        
    let evalThrow (state: (VarLookup * FuncLookup), throw: ThrowException, context: Option<Function>) =
        let unwrap = function | Throw cond -> cond
        let unwrapName = function | Identifier x -> x
        let cond = unwrap throw
        if cond.IsNone || evalCondition(state, cond.Value) then
            let msg =
                if context.IsSome then
                    if context.Value.error.IsSome then 
                        context.Value.error.Value.ToString() + " thrown at function: " + (unwrapName context.Value.name)
                    else
                        "thrown at function: " + (unwrapName context.Value.name)
                else
                    "thrown"
            raise(RunTimeError(msg))
            
    let evalConsole (state: (VarLookup * FuncLookup), write: Write) =
        match write with
        | ConsoleWrite expr ->
            evalExpresion(state, expr)
            |> strValue
            |> printfn "%s"
        | FileWrite(name, expr) ->
            let n = String.length name
            let fileName = name.[1..(n-2)]
            File.AppendAllText(fileName, (evalExpresion(state, expr) |> strValue) + "\n")
        
    let rec eval (state: (VarLookup * FuncLookup), stmt: Stmt, context: Option<Function>) =
        match stmt with
        | AST.Declare declaration -> evalDeclaration(state, declaration)
        | AST.FunctionCall fnCall -> evalFunction(state, fnCall) |> ignore // todo: ignore for testing
        | AST.ThrowStmt throw -> evalThrow(state, throw, context)
        | AST.WriteStmt console -> evalConsole(state, console)
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
        
        let vars = new VarLookup()
        let funcs = new FuncLookup(globalFuncs)
        let scopedState = vars, funcs
        
        if func.args.IsSome then
            for i = 0 to func.args.Value.Length - 1 do
                let id = unwrapArg func.args.Value.[i]
                let value = evalExpresion(state, args.Value.[i])
                setVar(scopedState, id, value)
                
        if stmts.IsSome then
            for stmt in stmts.Value do
                eval(scopedState, stmt, Some func) |> ignore
                
        let result =
            if rtrn.IsSome then
                evalExpresion(scopedState, (unwrapReturn rtrn.Value))
            else
                0 |> Int
        
//        printfn "%s result: %s" (func.name.ToString()) (result.ToString())
        result
        
    and declareConditionalConst (state: (VarLookup * FuncLookup), id: ID, ifExpr: IfExpr) =
        let unwrap = function | IfStmt(cond, block, elseBlock) -> (cond, block, elseBlock)
        let unwrapBlock = function | BlockStmts(stmts, rtrn) -> (stmts, rtrn)
        let unwrapReturn = function | ReturnExpr x -> x
        
        let (cond, block, elseBlock) = unwrap ifExpr
        
        if evalCondition(state, cond) then
            let (stmts, rtrn) = unwrapBlock block
            if stmts.IsSome then
                for stmt in stmts.Value do
                    eval(state, stmt, None) |> ignore
                    
            let result =
                if rtrn.IsSome then
                    evalExpresion(state, (unwrapReturn rtrn.Value))
                else
                    raise(ParsingError("No return statement in declarative IF statement"))
            setVar(state, id, result)
        else
            let (stmts, rtrn) = unwrapBlock elseBlock.Value
            if stmts.IsSome then
                for stmt in stmts.Value do
                    eval(state, stmt, None) |> ignore
                    
            let result =
                if rtrn.IsSome then
                    evalExpresion(state, (unwrapReturn rtrn.Value))
                else
                    raise(ParsingError("No return statement in declarative IF statement"))
            setVar(state, id, result)
            
    and evalDeclaration (state: (VarLookup * FuncLookup), declaration: AST.Declaration) =
        match declaration with
        | AST.DeclareConst (name, value) -> setVar(state, name, evalExpresion(state, value))
        | AST.FunctionDeclare (name, block, args, _type, error) -> setFunc(state, name, block, args, _type, error)
        | AST.DeclareCondConst (name, ifExpr) -> declareConditionalConst(state, name, ifExpr)
        | AST.DeclareConstFunc (name, fnCall) -> setVar(state, name, evalFunction(state, fnCall))
        
    let run statements =
        let vars = new VarLookup()
        let funcs = new FuncLookup()
        let state = vars, funcs
        
        try
            for stmt in (List.rev statements) do
                eval(state, stmt, None) |> ignore
        with
            | :? NoVarFound as ex -> printfn "%s not declared: %s" ex.Data1 ex.Data0
            | :? VarExists as ex -> printfn "%s already exists: %s" ex.Data1 ex.Data0
            | :? UnknownType as ex -> printfn "Unknown type: %s" ex.Data0
            | :? ParsingError as ex -> printfn "Parsing error: %s" ex.Data0
            | :? RunTimeError as ex -> printfn "Runtime error %s" ex.Data0
            
        state