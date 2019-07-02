"sample.nm" in 4:6: Type: ImportKeyword
"sample.nm" in 4:15: Type: Identifier. Value: "systypes"
"sample.nm" in 4:16: Type: Semicolon
"sample.nm" in 5:6: Type: ImportKeyword
"sample.nm" in 5:9: Type: Identifier. Value: "io"
"sample.nm" in 5:10: Type: Semicolon
"sample.nm" in 7:10: Type: EntrypointKeyword
"sample.nm" in 7:19: Type: FunctionKeyword
"sample.nm" in 7:27: Type: Identifier. Value: "integer"
"sample.nm" in 7:32: Type: Identifier. Value: "main"
"sample.nm" in 7:33: Type: BracketOpen
"sample.nm" in 7:34: Type: BracketClosed
"sample.nm" in 8:1: Type: BraceOpened
"sample.nm" in 9:7: Type: VarKeyword
"sample.nm" in 9:9: Type: Identifier. Value: "a"
"sample.nm" in 9:10: Type: Colon
"sample.nm" in 9:18: Type: Identifier. Value: "integer"
"sample.nm" in 9:20: Type: EqualSign
"sample.nm" in 9:22: Type: Number. Value: "4"
"sample.nm" in 9:23: Type: Semicolon
"sample.nm" in 10:7: Type: VarKeyword
"sample.nm" in 10:9: Type: Identifier. Value: "b"
"sample.nm" in 10:10: Type: Colon
"sample.nm" in 10:18: Type: Identifier. Value: "integer"
"sample.nm" in 10:20: Type: EqualSign
"sample.nm" in 10:22: Type: Number. Value: "6"
"sample.nm" in 10:24: Type: PlusSign
"sample.nm" in 10:26: Type: Number. Value: "4"
"sample.nm" in 10:27: Type: Semicolon
"sample.nm" in 11:7: Type: VarKeyword
"sample.nm" in 11:9: Type: Identifier. Value: "c"
"sample.nm" in 11:10: Type: Colon
"sample.nm" in 11:18: Type: Identifier. Value: "integer"
"sample.nm" in 11:20: Type: EqualSign
"sample.nm" in 11:22: Type: Identifier. Value: "a"
"sample.nm" in 11:24: Type: PlusSign
"sample.nm" in 11:26: Type: Number. Value: "2"
"sample.nm" in 11:28: Type: MultiplySign
"sample.nm" in 11:30: Type: Number. Value: "9"
"sample.nm" in 11:31: Type: Semicolon
"sample.nm" in 12:1: Type: BraceClosed
 - Lexeme[Type: Import. import <identifier:systypes>]
 - Lexeme[Type: Import. import <identifier:io>]
 - Lexeme[Type: Function. entrypoint function <identifier:integer> <identifier:main> ( )]
    - Lexeme[Type: Var. var <identifier:a> : <identifier:integer> = <number:4>]
       - Expression[<number:4>]
    - Lexeme[Type: Var. var <identifier:b> : <identifier:integer> = <number:6> + <number:4>]
       - Expression[<number:6>++<number:4>]
    - Lexeme[Type: Var. var <identifier:c> : <identifier:integer> = <identifier:a> + <number:2> * <number:9>]
       - Expression[<identifier:a>++<number:2>**<number:9>]
(-) %0 <- <number:6> + <number:4>
(-) %0 <- <number:2> * <number:9>
(-) %1 <- <identifier:a> + %0
