# Specification for Tail

The language is composed of a ```Definition``` and a ```Template```.
You send an input ```# Title``` and it runs through the ```Definitions```, to match and return a ```Template```.

## Definition

The definition is composed of a simple idea: How little constructs are needed to create a powerful tool?
It is also important to be easy to read and debug visually.


| syntax | Name | Description| 
|---------|-------------|----------|
|!   | Priority |  Moves the definition to the top.    |
| [] | Prefix Capture | Capture the start of the input. Contains literal sequence of characters. Example ```[abc]``` to capture a string ```'abcdef'``` | 
| () | Suffix Capture | Captures the end of the input. Contains a literal sequence of characters.Example ```(def])``` to capture a string ```'abcdef'``` |
| {} | Contains capture | Captures a string, where it contains the characters. Example ```{c}``` to capture a string ```'abcdef'```|
| \ | Escape | Escapes any character ```\[``` or ```\!``` to escape key characters. |

### Examples

```
Definition: ![#]
Template: <h1>$1</h1>
Input: # Title

Output: <h1>Title</h1>

```
```
Definition: [---]
Template: <hr/>
Input: ---

Output: <hr/>

```

```
Definition: [<h1>](</h1>)
Template: # $1
Input: <h1>Title</h1>

Output: # Title

```
