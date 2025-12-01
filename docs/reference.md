# Hobble Programming Language Reference

This document provides an informal reference to the Hobble Programming Language. It can be referred to for syntax,
idioms and how the language works.

<!-- TOC -->
* [Hobble Programming Language Reference](#hobble-programming-language-reference)
  * [Types](#types)
  * [Grammar](#grammar)
    * [Syntax Grammar](#syntax-grammar)
      * [Expressions](#expressions)
    * [Lexical Grammar](#lexical-grammar)
      * [Comments](#comments)
  * [Operators](#operators)
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
    * [Operator Precedence](#operator-precedence)
<!-- TOC -->

## Types

Hobble has the following built-in types:

- **Number**: A single type is to cover all numeric types. e.g. `-5`, `67`, `42.69`
- **String**: A sequence of characters. e.g. `"Hello, World!"`.
- **Bool**: A Boolean value, which is either `true`, or `false`.

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

The syntax grammar parses a sequence of tokens into a structure the downstream to compute on.

#### Expressions

Expressions produce values. A separate rule is used for each precedence to make it explicit, which is complemented
by the [operator precedence table](#operator-precedence). We can see that as we progress down the production rules for
expressions, the precedence is increasing.

```ebnf
expression     = logicalOr;

logicalOr      = logicalAnd ( "||" | logicalAnd )* ;

logicalAnd     = equality ( "&&" | equality )* ;

equality       = relational ( ( "==" | "!=" ) relational )* ;

relational     = additive ( ( "<" | ">" | "<=" | ">=" ) additive )* ;

additive       = multiplicative ( ( "+" | "-" ) multiplicative )* ;

multiplicative = unary ( ( "*" | "/" ) unary )* ;

unary          = ( ( "-" | "!" ) unary ) | primary ;

primary        = NUMBER | STRING | grouping ;

grouping       = "(" expression ")" ;
```

### Lexical Grammar

The lexical grammar groups a stream of characters (string) into tokens for downstream parsing.

```ebnf
NUMBER = DIGIT+ ( "." DIGIT+ )? ;

STRING = " \" <any character except \"> \" " ;

ALPHA  = "a" | ... | "z" | "A" | ... | "Z" | UNDER ;

UNDER  = "_" ;

DIGIT  = "0" | ... | "9" ;
```

#### Comments

Comments are used to annotate the program for helping the developer's understanding. They are not compiled.

Two types of comments are supported:
1. **Single-Line Comments:** Begin with the characters `//` and extends to the end of the line.
2. **Block Comments:** Begin with the characters `/*` and ends with the characters `*/`. They can occupy a portion of a
line or span over multiple lines.

Comments do not nested, and do not hold any meaning within each other regardless of comment type.

Examples:
```hob
/* 
 A block comment can span multiple lines.
 */

// Single-line comment

var x = 1; // Inline comment
```

## Operators

Operators allow the user to perform basic operations with the built-in value types.

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

### Operator Precedence

With expressions that contain multiple operators, the operators with higher precedence are evaluated before
the operators with lower precedence. The following table contains the operators in descending precedence.

| Precedence | Operators                            | Name           |
|:----------:|:-------------------------------------|----------------|
|     1      | `-x`, `!x`                           | Unary          |
|     2      | `x * y`, `x / y`                     | Multiplicative |
|     3      | `x + y`, `x - y`                     | Additive       |
|     4      | `x > y`, `x < y`, `x >= y`, `x <= y` | Relational     |
|     5      | `x == y`, `x != y`                   | Equality       |
|     6      | `x && y`                             | Logical And    |
|     7      | `x \|\| y`                           | Logical Or     |

Parentheses can be used to control the precedence of how an expression evaluates. Enclose an expression with
parentheses to evaluate the enclosing expression before any other evaluation happens outside the parentheses.

```hob
print 6 * 2 + 1     // output: 13
print 6 * (2 + 1)   // output: 18
```