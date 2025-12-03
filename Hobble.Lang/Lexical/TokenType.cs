namespace Hobble.Lang.Lexical;

public enum TokenType
{
    Plus,
    Minus,
    Star,
    Slash,
    LeftParen,
    RightParen,
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
    
    Error,
    Eof,
}