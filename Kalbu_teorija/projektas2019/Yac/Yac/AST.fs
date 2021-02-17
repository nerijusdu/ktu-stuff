module AST

// Primitives
type Op = Eq | Gt | Ge | Lt | Le
type Type =
  | TInt
  | TFloat
  | TString
  | TBool
type Error = TError
type ID = Identifier of string
type BooleanExpr = Boolean of bool
type StringExpr = String of string
type NumericExpr = 
  | Integer of int
  | Float of float
  | NumId of ID

// Expressions

type AritExpr =
  | Add of NumericExpr * NumericExpr
  | Sub of NumericExpr * NumericExpr
  | Mult of NumericExpr * NumericExpr
  | Div of NumericExpr * NumericExpr 

type Expr =
  | Bool of BooleanExpr
  | Str of StringExpr
  | Num of NumericExpr
  | Arit of AritExpr
  | Id of ID
  
type Write =
  | ConsoleWrite of Expr
  | FileWrite of string * Expr

// Statements
type Return = ReturnExpr of Expr

type Arg = 
  | Argument of ID

type FnCall =
  | Call of ID * Option<Expr list>
 // | Console of Expr
 
type  Block =
  | BlockStmts of Option<Stmt list> * Option<Return>

and Stmt = 
  | Declare of Declaration
  | FunctionCall of FnCall
  | ThrowStmt of ThrowException
  | WriteStmt of Write

and Declaration =
  | DeclareConst of ID * Expr
  | DeclareConstFunc of ID * FnCall
  | DeclareCondConst of ID * IfExpr
  | FunctionDeclare of ID * Block * Option<Arg list> * Option<Type> * Option<Error>

and IfExpr =
  | IfStmt of Condition * Block * Option<Block>

and Condition =
  | Cond of Expr * Op * Expr
  | And of Condition * Condition
  | Or of Condition * Condition

and ThrowException =
  | Throw of Option<Condition>


// Start
type START = { Statements : Stmt list }
