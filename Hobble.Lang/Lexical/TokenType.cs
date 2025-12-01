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
    
    String,
    Number,
    
    True,
    False,
    
    Error,
    Eof,
}