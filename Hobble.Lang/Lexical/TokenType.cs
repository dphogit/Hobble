namespace Hobble.Lang.Lexical;

public enum TokenType
{
    Plus,
    Minus,
    Star,
    Slash,
    LeftParen,
    RightParen,
    LeftBrace,
    RightBrace,
    Bang,
    Equal,
    LessThan,
    GreaterThan,
    LessThanEqual,
    GreaterThanEqual,
    EqualEqual,
    BangEqual,
    AmpAmp,
    PipePipe,
    SemiColon,
    
    String,
    Number,
    Identifier,
    
    True,
    False,
    Print,
    Var,
    If,
    Else,
    While,
    
    Error,
    Eof,
}