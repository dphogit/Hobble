# Hobble Programming Language Reference

This document provides an informal reference to the Hobble Programming Language. It can be referred to for syntax,
idioms and how the language works.

<!-- TOC -->
* [Hobble Programming Language Reference](#hobble-programming-language-reference)
  * [Types](#types)
  * [Grammar](#grammar)
    * [Syntax Grammar](#syntax-grammar)
      * [Declarations and Statements](#declarations-and-statements)
      * [Expressions](#expressions)
      * [Utility Rules](#utility-rules)
    * [Lexical Grammar](#lexical-grammar)
      * [Comments](#comments)
  * [Operators and Expressions](#operators-and-expressions)
    * [Arithmetic Operators](#arithmetic-operators)
      * [Addition Operator +](#addition-operator-)
      * [Subtraction Operator -](#subtraction-operator--)
      * [Multiplication Operator *](#multiplication-operator-)
      * [Division Operator /](#division-operator-)
      * [Unary Minus Operator](#unary-minus-operator)
    * [Relational Operators](#relational-operators)
      * [Less Than Operator <](#less-than-operator-)
      * [Greater Than Operator >](#greater-than-operator-)
      * [Less Than Or Equal Operator <=](#less-than-or-equal-operator-)
      * [Greater Than Or Equal Operator <=](#greater-than-or-equal-operator-)
      * [Equality Operator `==`](#equality-operator-)
      * [Inequality Operator `!=`](#inequality-operator-)
    * [Logical Operators](#logical-operators)
      * [Logical Negation Operator `!`](#logical-negation-operator-)
      * [Logical And Operator `&&`](#logical-and-operator-)
      * [Logical Or Operator `||`](#logical-or-operator-)
    * [Assignment Operators](#assignment-operators)
      * [Simple Assignment](#simple-assignment)
    * [Operator Precedence](#operator-precedence)
  * [Statements](#statements)
    * [Variable Declaration Statements](#variable-declaration-statements)
    * [Expression Statements](#expression-statements)
    * [Print Statements](#print-statements)
    * [Selection Statements](#selection-statements)
      * [If Statements](#if-statements)
    * [Iterative Statements](#iterative-statements)
      * [While Statement](#while-statement)
      * [For Statement](#for-statement)
  * [Variables](#variables-)
    * [Local Variables and Lexical Scope](#local-variables-and-lexical-scope)
  * [Functions](#functions)
    * [Return Values](#return-values)
<!-- TOC -->

## Types

Hobble has the following built-in types:

- **Number**: A single type is to cover all numeric types. e.g. `-5`, `67`, `42.69`
- **String**: A sequence of characters. e.g. `"Hello, World!"`.
- **Bool**: A Boolean value, which is either `true`, or `false`.
- **Null**: A null reference, does not refer to any value.

## Grammar

An [Extended Backus Naur Form (EBNF)](https://en.wikipedia.org/wiki/Extended_Backus%E2%80%93Naur_form#:~:text=In%20computer%20science%2C%20extended%20Backus,as%20a%20computer%20programming%20language.)
like notation is used to help describe the Hobble Programming Language.

We include the description of some of the special notation used.

| Notation | Description                          |
|:--------:|:-------------------------------------|
|    A?    | A can be repeated zero or one time.  |
|    A*    | A can be repeated one or more times. |
|  A \| B  | A or B.                              |

### Syntax Grammar

The syntax grammar parses a sequence of tokens into a tree structure the downstream to compute on. The first entrypoint
production rule matches the Hobble program, which is a list of one or more declarations statements.

```ebnf
program = declaration* EOF;
```

#### Declarations and Statements

Declaration statements introduce binding to identifiers.

```ebnf
declaration = varDecl | fnDecl | statement ;

fnDecl      = "fn" IDENTIFIER "(" params? ")" block ;

varDecl     = "var" IDENTIFIER ( "=" expression )? ";" ;
```

The remaining statements which aren't declarations, produce side effects without creating bindings.

```ebnf
statement  = exprStmt | printStmt | ifStmt | whileStmt | forStmt | returnStmt | block ;

exprStmt   = expression ";" ;

printStmt  = "print" expression ";" ;

ifStmt     = "if" "(" expression ")" statement ( "else" statement )? ;

whileStmt  = "while" "(" expression ")" statement ;

forStmt    =  "for" "(" ( varDecl | exprStmt | ";" )
                        expression? ";"
                        expression?
                    ")" statement ;

returnStmt = "return" expression? ";" ;

block     = "{" declaration* } ;
```

#### Expressions

Expressions produce values. A separate rule is used for each precedence to make it explicit, which is complemented
by the [operator precedence table](#operator-precedence). We can see that as we progress down the production rules for
expressions, the precedence is increasing.

```ebnf
expression     = assignment ;

assignment     = ( IDENTIFIER "=" assignment )
               | logicalOr ;

logicalOr      = logicalAnd ( "||" | logicalAnd )* ;

logicalAnd     = equality ( "&&" | equality )* ;

equality       = relational ( ( "==" | "!=" ) relational )* ;

relational     = additive ( ( "<" | ">" | "<=" | ">=" ) additive )* ;

additive       = multiplicative ( ( "+" | "-" ) multiplicative )* ;

multiplicative = unary ( ( "*" | "/" ) unary )* ;

unary          = ( ( "-" | "!" ) unary ) | call ;

call           = primary ( "(" arguments? ")" )* ;

primary        = NUMBER | STRING | grouping | IDENTIFIER | "false" | "true" ;

grouping       = "(" expression ")" ;
```

#### Utility Rules

Reusable production rules to keep the syntax grammar more readable and tidy.

```ebnf
params    = IDENTIFIER ( "," IDENTIFIER )* ;

arguments = expression ( "," expression )* ;
```

### Lexical Grammar

The lexical grammar groups a stream of characters (string) into tokens for downstream parsing.

```ebnf
NUMBER     = DIGIT+ ( "." DIGIT+ )? ;

STRING     = " \" <any character except \"> \" " ;

ALPHA      = "a" | ... | "z" | "A" | ... | "Z" | UNDER ;

UNDER      = "_" ;

DIGIT      = "0" | ... | "9" ;

IDENTIFIER = ALPHA ( ALPHA | DIGIT ) * ;
```

#### Comments

Comments are used to annotate the program for helping the developer's understanding. They are not compiled.

Two types of comments are supported:
1. **Single-Line Comments:** Begin with the characters `//` and extends to the end of the line.
2. **Block Comments:** Begin with the characters `/*` and ends with the characters `*/`. They can occupy a portion of a
line or span over multiple lines. [TODO - Implementation]

Comments do not nested, and do not hold any meaning within each other regardless of comment type.

Examples:
```hob
/* 
 A block comment can span multiple lines.
 */

// Single-line comment

var x = 1; // Inline comment
```

## Operators and Expressions

Operators allow the user to perform basic operations with the built-in value types. Expressions are constructed from
a combination of operators and operands.

### Arithmetic Operators

These operators perform arithmetic with Number type operands. These operators are categorised as either **unary** (one
operand) or **binary** (two operands).

#### Addition Operator +

The addition operator `+` computes the sum of its operands. It can also be used to
concatenate two string operands, if they are both String types. Mixing operand types is not allowed for the addition
operator, unlike other dynamically typed languages. i.e. Adding a String and Number will result in an error.

```hob
print 1 + 2;             // output: 3
print "foo" + "bar";     // output: "foobar"
print 1 + "1";           // RuntimeError
```

#### Subtraction Operator -

The subtraction operator `-` subtracts the left operand by the right operand.

```hob
print 2 - 1;    // output: 1
```

#### Multiplication Operator *

The multiplication operator `*` computes the product of its operands.

```hob
print 2 * 3;    // output: 6
```

#### Division Operator /

The division operator `*` divides the left operands by the right operand.

```hob
print 2 / 1;    // output: 2
print 2 / 0;    // DivideByZeroError
```

#### Unary Minus Operator

The unary `-` operator computes the numeric negation of it's single operand.

```hob
print -2;       // output: -2
print -(-2);    // output: 2
```

### Relational Operators

Relational operators, also known as comparison operators, compare their operands, returning Boolean values.

#### Less Than Operator <

The `<` operator returns `true` if the left operand is strictly less than the right operand, else `false`.

```hob
print 5 < 7;       // output: true
print 7 < 7;       // output: false 
print 7.1 < 7.2;   // output: true
```

#### Greater Than Operator >

The `>` operator returns `true` if the right operand is strictly greater than the right operand, else `false`.

```hob
print 3.1 > 3.0;   // output: true
print 2.1 > 3.0;   // output: false 
print 3 > 3;       // output: false
```

#### Less Than Or Equal Operator <=

The `<=` operator returns `true` if the left operand is less than or equal than the right operand, else `false`.

```hob
print 3.1 <= 3.2;  // output: true
print 3.0 <= 3.0;  // output: true
print 3.6 <= 3.2;  // output: false
```

#### Greater Than Or Equal Operator <=

The `>=` operator returns `true` if the right operand is greater than or equal than the left operand, else `false`.

```hob
print 3.1 >= 3.2;  // output: false
print 3.0 >= 3.0;  // output: true
print 3.6 >= 3.2;  // output: true 
```

#### Equality Operator `==`

The `==` operator returns `true` if its operands are equal, else `false`.

<!-- TODO: When introducing objects, need to distinguish between value and reference types, as well as string being a
special case of a reference type which is used so often we implement value type operations on it such as equality. -->

```hob
print 1 + 2 == 3;   // output: true
print "x" == "X"    // output: false
```

#### Inequality Operator `!=`

The `!=` operator returns `true` if its operands are not equal, else `false`. For builtin types, the expression `x != y`
is equivalent to the expression `!(x == y)`.

```hob
print 1 + 2 != 3;   // output: false
print "x" != "X"    // output: true
```

### Logical Operators

Logical operators perform logical operations on Bool typed operands.

#### Logical Negation Operator `!`

The unary prefix `!` operator computes the logical negation of its operand. The `!` operator results in `true`, if the
operand evaluates to `false`. Accordingly, it results in `false` if the operand evaluates to `true`.

```hob
var x = true;
print x;    // output: true
print !x;   // output: false
print !!x;  // output: true
```

#### Logical And Operator `&&`

The `&&` operator evaluates to `true` if both of its operands evaluate to `true`. This operator short-circuits if its
left operand evaluates to `false`, i.e. it does not evaluate the right operand if the left operand evaluates `false`.

```hob
// In this example, the `sayHello` function will only be called if the left operand of the `&&` operator is `true`.

fn sayHello() {
  print("Hello");
  return true; 
}

print false && sayHello();
// output:
// false

print true && sayHello();
// output:
// "Hello"
// true
```

#### Logical Or Operator `||`

The `||` operator evaluates to `true` if either of its operands evaluate to `true`. This operator short-circuits if its
left operand evaluates to `true`, i.e. it does not evaluate the right operand if the left operand evaluates `true`.

```hob
// In this example, the `sayHello` function will only be called if the left operand of the `||` operator is `false`.

fn sayHello() {
  print("Hello");
  return true; 
}

print false || sayHello();
// output:
// "Hello"
// true

print true && sayHello();
// output:
// true
```

### Assignment Operators

#### Simple Assignment

The simple `=` assignment operator assigns the value of the right operand to the left operand variable. The assignment
expression result itself is the value that was assigned to the left operand.

```hob
var a;
print a = 5;  // output: 5
```

The `=` assignment operator is **right-associative** unlike many other operators, that is:

```hob
a = b = c;
a = (b = c);
```

### Operator Precedence

With expressions that contain multiple operators, the operators with higher precedence are evaluated before
the operators with lower precedence. The following table contains the operators in descending precedence.

| Precedence | Operators                            | Name           |
|:----------:|:-------------------------------------|----------------|
|     1      | `x()`,                               | Call           |
|     1      | `-x`, `!x`                           | Unary          |
|     2      | `x * y`, `x / y`                     | Multiplicative |
|     3      | `x + y`, `x - y`                     | Additive       |
|     4      | `x > y`, `x < y`, `x >= y`, `x <= y` | Relational     |
|     5      | `x == y`, `x != y`                   | Equality       |
|     6      | `x && y`                             | Logical And    |
|     7      | `x \|\| y`                           | Logical Or     |
|     8      | `x = y`                              | Assignment     |

Parentheses can be used to control the precedence of how an expression evaluates. Enclose an expression with
parentheses to evaluate the enclosing expression before any other evaluation happens outside the parentheses.

```hob
print 6 * 2 + 1     // output: 13
print 6 * (2 + 1)   // output: 18
```

## Statements

### Variable Declaration Statements

Variables are declared through a user-defined unique identifier, which can be optionally initialised.

```hob
var width;        // Without initialisation
var length = 10;  // With initialisation
```

If a variable is not explicitly initialised by the user, it is implicitly assigned `null`.

```hob
var a;
print a;  // output: "nil"
```

Variables once defined, cannot be redefined in the same scope.

```hob
var a = "hello";
var a = "world";  // error: variable 'a' already defined.
```

### Expression Statements

Expression statements allow users to place expressions where a statement is expected. The evaluation result of the
expression is discarded, but expression statements are useful for producing side effects e.g. function calls.

```hob
fn sayHello() {
  print "Hello";
}

sayHello(); // "Hello";
```

### Print Statements

Evaluates an expression and outputs the result to the user.

```hob
print 1 + 2;  // output: 3
print 5 * 9;  // output: 45
```

### Selection Statements

Selection statements select certain statements to execute based on some value of a provided expression.

#### If Statements

The `if` statement executes a statement if the Boolean expression evaluates to `true`.

```hob
fn calculateCircleArea(radius) {
  if (radius < 0) {
    print "Warning: Radius is negative."; 
  }
  
  print 3.14 * radius * radius;
}

calculateCircleArea(-10);
// output:
// Warning: Radius is negative.
// 314
```

An `if-else` statement allows one of two code paths to be chosen based on the evaluation of the Boolean expression.

```hob
fn isWaterFreezing(tempCelsius) {
  if (tempCelsius <= 0) {
    print true; 
  } else {
    print false; 
  }
}

isWaterFreezing(-10);   // output: true
isWaterFreezing(10);    // output: false
```

### Iterative Statements

Iterative statements repeatedly execute a statement or a block of statements.

#### While Statement

The `while` statement executes a statement or a block of statements when it's Boolean condition expression evaluates to
`true`. The condition is evaluated before each iteration of the loop, meaning the body of the `while` loop can be
executed zero or more times.

```hob
var i = 0;
while (i < 3) {
  print i;
  i = i + 1;
}
// output:
// 0
// 1
// 2
```

#### For Statement

The `for` statement evaluates an initialisation expression, then while a condition is true it repeatedly executes a
statement or block of statements. At the end of each iteration, an increment expression is evaluated.

Within the parenthesis of the `for` statement contains three clauses to implement the above described behaviour of the
`for` loop:

1. The first clause is the *initialiser*. The optional initialiser can be used to evaluate an expression, commonly used
to assign to a variable. The initialiser can alternatively be used to define a local variable scoped to the loop's body.
The initialiser is executed once before anything else in the loop.
2. The second clause is the *condition*. This condition when evaluated to `true` executes the loop's body. The condition
must evaluate to a Bool type otherwise a runtime error occurs. If a condition is not given, then it defaults to `true`.
3. The third clause is the *increment*. It is an optional arbitrary expression that evaluates at the end of each
iteration. In practice, when executed it would usually have a side effect to be impactful to the loop, for example
updating the value of a variable that is used in the condition clause so that the loop guarantees to end.

```hob
for (var i = 0; i < 3; i = i + 1) {
  print i;
}
// output:
// 0
// 1
// 2
```

## Variables 

Variables are used to store values which are referred to or associated by user defined identifiers. These can then be
later referred to throughout the program or scope it is declared/defined in.

```hob
var x = 10;   // The variable 'x' has the value 10
print x;      // output: 10
```

### Local Variables and Lexical Scope

The lifetime of a local variable is the portion of the program which storage is guaranteed to be reserved for it. The
lifetime spans from the entry of the current scope it is in, until the end of the execution of that scope.

Lexical scoping is used, which basically where a scope begins and end can be read from the program's source. The
left curly brace `{` and right curly brace `}` are commonly used to indicate the beginning and closing of a scope
respectively. Though these are not always required to be used (e.g. `if` statements).

```hob
// global scope

var a = 10;

{ // local scope 1
  var a = 20;
  print a; // output: 20
} // end local scope 1

{ // local scope 2
  var a = 30;
  print a; // output: 30
} // end local scope 1

print a;  // output: 10
```

A local variable is instantiated each time the scope is entered.

```hob
fn sayHello() {
  var x = "Hello";
  print x;
}

// Each call will have their own localised scope instantiation with each their own 'x' variable.
sayHello(); // output: "Hello"
sayHello(); // output: "Hello"
```

## Functions

Functions allow for reusable blocks of code.

The `fn` keyword is used to declare a function. The function requires a name, an optional comma-separated list of
parameters and the body, which is a block containing the statements to be executed when the function is invoked/called.

```hob
// Declare the function
fn printSum(a, b) {
  print a + b;
}

// Call the function
printSum(1, 2); // output: 3
```

### Return Values

Functions can return a value to the caller using the `return` statement. The value followed by the `return` keyword will
be returned to the caller, if the value is omitted or the `return` keyword is omitted from the function then it will
implicitly return `null`.

```hob
fn sum(a, b) {
  a + b;
}

var x = sum(1, 2);
print x;  // output: "null"

fn sum2(a, b) {
  return a + b;
}

var y = sum(1, 2);
print y;  // output: 3
```