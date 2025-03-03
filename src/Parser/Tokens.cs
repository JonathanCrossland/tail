namespace Tail.Parser;

public abstract class Token
{
    public Token(string value = "")
    {
        Value = value;
    }
    public string Value { get; set; }
}

public class PriorityToken : Token
{
    public PriorityToken() : base() { }
}

public class PrefixToken : Token
{
    public PrefixToken(string value) : base(value) { }
}

public class SuffixToken : Token
{
    public SuffixToken(string value) : base(value) { }
}

public class LiteralToken : Token
{
    public LiteralToken(string value) : base(value) { }
}

public class ContainsLiteralToken : Token
{
    public ContainsLiteralToken(string value) : base(value) { }
}

public class WhitespaceToken : Token
{
    public WhitespaceToken(string value) : base(value) { }
}


