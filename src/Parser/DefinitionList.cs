using Tail.Parser;

public class Definition
{
    public string Key { get; }
    public string Value { get; }
    public List<Token> Tokens { get; set; } 
    public bool IsHighPriority { get; set; } 
     

    public Definition(string key, string value)
    {
        Key = key;
        Value = value;
        Tokens = new List<Token>();
        
    }
}

public class DefinitionList
{
    public Dictionary<Definition, List<Token>> List = new Dictionary<Definition, List<Token>>();
}