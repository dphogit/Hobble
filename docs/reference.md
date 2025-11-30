# Hobble Programming Language Reference

This document provides an informal reference to the Hobble Programming Language. It can be referred to for syntax,
idioms and how the language works.

## Types

Hobble has the following built-in value types:

- **Number**: A single type is to cover all numeric types. e.g. `-5`, `67`, `42.69`
- **String**: A sequence of characters. e.g. `"Hello, World!"`

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
expression = additive ;

additive = multiplicative ( ( "+" | "-" ) multiplicative )* ;

multiplicative = primary ( ( "*" | "/" ) primary )* ;

primary = NUMBER | STRING ;
```

### Lexical Grammar

The lexical grammar groups a stream of characters (string) into tokens for downstream parsing.

```ebnf
NUMBER = DIGIT+ ( "." DIGIT+ )? ;

STRING = " \" <any character except \"> \" " ;

ALPHA = "a" | ... | "z" | "A" | ... | "Z" | UNDER ;

UNDER = "_" ;

DIGIT = "0" | ... | "9" ;
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

These operators perform arithmetic with Number type operands. These operators are categorized as either **unary** (one
operand) or **binary** (two operands).

#### Addition Operator +

The addition operator `+` computes the sum of its operands. It can also be used to
concatenate two string operands, if they are both String types. Mixing operand types is not allowed for the addition
operator, unlike other dynamically typed languages. i.e. Adding a String and Number will result in an error.

#### Subtraction Operator -

The subtraction operator `-` subtracts the left operand by the right operand.

#### Multiplication Operator *

The multiplication operator `*` computes the product of its operands.

#### Division Operator /

The division operator `*` divides the left operands by the right operand.

### Operator Precedence

With expressions that contain multiple operators, the operators with higher precedence are evaluated before
the operators with lower precedence.

The following table contains the operators in descending precedence.

| Precedence | Operators | Name           |
|:----------:|:----------|----------------|
|     1      | * /       | Multiplicative |
|     2      | + -       | Additive       |
