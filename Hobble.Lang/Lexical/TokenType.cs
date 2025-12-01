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
    
    String,
    Number,
    
    True,
    False,
    
    Error,
    Eof,
}