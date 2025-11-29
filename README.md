# Hobble - A Hobby Programming Language

Hobble is my own dynamically typed programming language that I develop for my own education and enjoyment.
I called it Hobble because it is a hobby, and it's probably awkward/slow.

Read the [Language Reference](./docs/reference.md) for an informal reference/specification on how the language works.

## Example

```hob
// fib.hob

/* Calculate the nth number in the Fibonacci sequence. */
fn fib(n) {
    if (n <= 1) return n;
    return fib(n-1) + fib(n-2); 
}

var x = fib(10);

print x;  // Output: 55
```