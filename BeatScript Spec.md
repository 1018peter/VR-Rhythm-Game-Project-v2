# Technical Specifications for BeatScript

The BeatScript (~~BS for short~~) is a command-based domain-specific scripting language.

...Though it really is just a kind of (virtual) Assembly language, with a custom set of instructions.

## Format

BeatScript is interpreted **line by line**, and tokens are separated by spaces or periods (,). They can be mixed, but it is not recommended.

Every line begins with the name of the instruction, followed by a sequence of parameters associated with it.

`{INSTRUCTION} {PARAMETERS...}`

Comments are also supported. Lines that begin with `//` (C-style) or `#` (Perl, Python) are treated as comments and are ignored by the interpreter. Empty lines are also ignored.

Yep. That's it. Simple, isn't it?

## Rules

Right now, aside from the typical syntax rules, the only extra rule is that every script file should at least specify BPM once with the `BPM` instruction. It doesn't have to be at the top of the file, but it is recommended that it is put there.

## Expanding the Instruction Set

In SongManager.cs, look for the `LoadBeatmapScript` public method.
![](https://i.gyazo.com/910e6b8f24a1f85feaa97ab0325a9735.png)

You can insert new instructions by adding new case statements.

A rule of thumb is that you should assert the number of tokens in the line before you do anything with the parameters, and log the `lineCount` variable if an error occurs. In the picture above, the `BPM` instruction expects one parameter, so a line with `BPM` as its first token should have two tokens in total. That's why we assert that `tokens.Length == 2`.

Instructions can be associated with a timestamp (like the `AIRNOTE` instruction), or they can be configuration commands (like the `BPM` instruction).