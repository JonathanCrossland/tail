
using System.Text;


public class Definition
{
    public string Key { get; }
    public string Value { get; }
    public List<TailParser.Token> Tokens { get; set; } 
    public bool IsHighPriority { get; set; } 
     

    public Definition(string key, string value)
    {
        Key = key;
        Value = value;
        Tokens = new List<TailParser.Token>();
        
    }
}

public class DefList
{
    public Dictionary<Definition, List<TailParser.Token>> List = new Dictionary<Definition, List<TailParser.Token>>();


}