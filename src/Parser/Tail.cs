public class Tail
{
    private List<Definition> _Definitions = new List<Definition>();

    public Tail(List<Definition> definitions)
    {
        if (definitions == null) return;
        _Definitions = definitions;

        ParseDefinitions(); 
    }

    public void ParseDefinitions()
    {
        TailParser p = new TailParser();

        foreach (var def in _Definitions)
        {
            def.Tokens = p.ParseKey(def.Key);
            def.IsHighPriority = def.Tokens.Any(t => t is TailParser.PriorityToken); // Set priority
        }

        _Definitions = _Definitions
            .OrderByDescending(d => d.IsHighPriority == true) 
            .ThenByDescending(d => d.Tokens.Any(t => t.GetType().Name == typeof(TailParser.ContainsLiteralToken).Name))
            .ThenByDescending(d => CountConstructs(d.Key))
            .ThenByDescending(d => d.Key.Length)
            .ToList();
    }

    public string Parse(string input = "")
    {
        input = input.Trim();
        return ParseRecursive(input, 0);
    }

   private bool Match(string input, List<TailParser.Token> keyTokens, out string capturedValue)
{
    capturedValue = string.Empty;
    List<string> capturedValues = new List<string>();
    int inputIndex = 0;
    int captureStart = -1;
    bool hasSuffix = keyTokens.Any(t => t is TailParser.SuffixToken);
    bool priorityTokenPresent = false;

    if (!CheckContainsLiterals(input, keyTokens))
    {
        return false;
    }

    foreach (var token in keyTokens)
    {
        if (token is TailParser.PriorityToken)
        {
            priorityTokenPresent = true;
            continue;
        }
        else if (token is TailParser.PrefixToken prefixToken)
        {
            if (!MatchPrefix(input, prefixToken, ref inputIndex)) return false;
            captureStart = inputIndex;
        }
        else if (token is TailParser.LiteralToken literalToken)
        {
            if (!MatchLiteral(input, literalToken, ref inputIndex)) return false;
            captureStart = inputIndex;
        }
        else if (token is TailParser.ContainsLiteralToken containsLiteralToken)
        {
            if (!MatchContainsLiteral(input, containsLiteralToken, ref inputIndex, capturedValues)) return false;
            captureStart = inputIndex;
        }
        else if (token is TailParser.SuffixToken suffixToken)
        {
            if (!MatchSuffix(input, suffixToken, ref captureStart, capturedValues)) return false;
            inputIndex = captureStart;
        }
    }

    capturedValue = CaptureFinalValue(input, captureStart, inputIndex, priorityTokenPresent);

    if (hasSuffix || capturedValues.Count > 0)
    {
        capturedValue = string.Join("\0", capturedValues);
    }

    if (keyTokens.Count == 1 && keyTokens[0] is TailParser.PrefixToken && string.IsNullOrEmpty(capturedValue))
    {
        capturedValue = input.Substring(inputIndex).TrimStart();
    }

    return true;
}

private bool MatchLiteral(string input, TailParser.Token token, ref int inputIndex)
{
    if (input.Substring(inputIndex).StartsWith(token.Value))
    {
        inputIndex += token.Value.Length;
        return true;
    }
    return false;
}

private bool MatchPrefix(string input, TailParser.Token token, ref int inputIndex)
{
    return MatchLiteral(input, token, ref inputIndex);
}

private bool MatchContainsLiteral(string input, TailParser.ContainsLiteralToken token, ref int inputIndex, List<string> capturedValues)
{
    int foundIndex = input.IndexOf(token.Value, inputIndex);
    if (foundIndex != -1)
    {
        if (foundIndex > inputIndex)
        {
            capturedValues.Add(input.Substring(inputIndex, foundIndex - inputIndex));
        }
        inputIndex = foundIndex + token.Value.Length;
        return true;
    }
    return false;
}

private bool MatchSuffix(string input, TailParser.SuffixToken token, ref int captureStart, List<string> capturedValues)
{
    int suffixIndex = input.IndexOf(token.Value, captureStart);
    if (suffixIndex != -1)
    {
        capturedValues.Add(input.Substring(captureStart, suffixIndex - captureStart));
        captureStart = suffixIndex + token.Value.Length;
        return true;
    }
    return false;
}

private bool CheckContainsLiterals(string input, List<TailParser.Token> keyTokens)
{
    foreach(var token in keyTokens)
    {
        if(token is TailParser.ContainsLiteralToken containsToken)
        {
            if(!input.Contains(containsToken.Value))
            {
                return false;
            }
        }
    }
    return true;
}

private string CaptureFinalValue(string input, int captureStart, int inputIndex, bool priorityTokenPresent)
{
    if (captureStart == -1) return "";

    if (priorityTokenPresent)
    {
        return input.Substring(captureStart);
    }
    else
    {
        if (captureStart == input.Length) return "";
        else return input.Substring(captureStart, inputIndex - captureStart);
    }
}

    private string ApplyTransformation(string definitionValue, string capturedValue)
    {
        string[] captured = capturedValue.Split('\0');
        string result = definitionValue;

        for (int i = 0; i < captured.Length; i++)
        {
            result = result.Replace($"${i + 1}", captured[i]);
        }

        return result;
    }

    private string ParseRecursive(string input, int recursionDepth)
    {
        if (recursionDepth > 100)
        {
            return input;
        }

        if (string.IsNullOrEmpty(input)) return "";

        foreach (var definition in _Definitions)
        {
            if (Match(input, definition.Tokens, out string capturedValue))
            {
                capturedValue = ParseRecursive(capturedValue, recursionDepth + 1);
                return ApplyTransformation(definition.Value, capturedValue);
            }
        }
        return input;
    }

    private int CountConstructs(string key)
    {
        int count = 0;
        for (int i = 0; i < key.Length; i++)
        {
            if (key[i] == '[' || key[i] == '(')
            {
                count++;
            }
        }
        return count;
    }
}