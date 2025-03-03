using System.Text;
using Tail.Parser;

namespace Tail;
public class TailLang
{
    private List<Definition> _Definitions = new List<Definition>();

    public TailLang(List<Definition> definitions)
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
            def.IsHighPriority = def.Tokens.Any(t => t is PriorityToken); // Set priority
        }

        _Definitions = _Definitions
            .OrderByDescending(d => d.IsHighPriority == true)
            .ThenByDescending(d => d.Tokens.Any(t => t.GetType().Name == typeof(ContainsLiteralToken).Name))
            .ThenByDescending(d => CountConstructs(d.Key))
            .ThenByDescending(d => d.Key.Length)
            .ToList();
    }

    public string Parse(string input = "")
    {
        input = input.Trim();

        WriteDefinitionsToFile();

        return ParseRecursive(input, 0);
    }

    

    private bool Match(string input, List<Token> keyTokens, out string capturedValue)
    {
        capturedValue = string.Empty;
        List<string> capturedValues = new List<string>();
        int inputIndex = 0;
        int captureStart = -1;
        bool hasSuffix = keyTokens.Any(t => t is SuffixToken);
        bool priorityTokenPresent = false;

        WriteMatchToFile(input, keyTokens);

        if (!CheckContainsLiterals(input, keyTokens))
        {
            return false;
        }

        foreach (var token in keyTokens)
        {
            if (token is PriorityToken)
            {
                priorityTokenPresent = true;
                continue;
            }
            else if (token is PrefixToken prefixToken)
            {
                if (!MatchPrefix(input, prefixToken, ref inputIndex)) return false;
                captureStart = inputIndex;
            }
            else if (token is LiteralToken literalToken)
            {
                if (!MatchLiteral(input, literalToken, ref inputIndex)) return false;
                captureStart = inputIndex;
            }
            else if (token is ContainsLiteralToken containsLiteralToken)
            {
                if (!MatchContainsLiteral(input, containsLiteralToken, ref inputIndex, capturedValues)) return false;
                captureStart = inputIndex;
            }
            else if (token is SuffixToken suffixToken)
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

        if (keyTokens.Count == 1 && keyTokens[0] is PrefixToken && string.IsNullOrEmpty(capturedValue))
        {
            capturedValue = input.Substring(inputIndex).TrimStart();
        }

        return true;
    }

    private bool MatchLiteral(string input, Token token, ref int inputIndex)
    {
        if (input.Substring(inputIndex).StartsWith(token.Value))
        {
            inputIndex += token.Value.Length;
            return true;
        }
        return false;
    }

    private bool MatchPrefix(string input, Token token, ref int inputIndex)
    {
        return MatchLiteral(input, token, ref inputIndex);
    }

    private bool MatchContainsLiteral(string input, ContainsLiteralToken token, ref int inputIndex, List<string> capturedValues)
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

    private bool MatchSuffix(string input, SuffixToken token, ref int captureStart, List<string> capturedValues)
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

    private bool CheckContainsLiterals(string input, List<Token> keyTokens)
    {
        foreach (var token in keyTokens)
        {
            if (token is ContainsLiteralToken containsToken)
            {
                if (!input.Contains(containsToken.Value))
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

    private void WriteDefinitionsToFile()
    {
        string filePath = "TailDefinitions.txt";
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("Tail Definitions:");
        foreach (var def in _Definitions)
        {
            sb.AppendLine($"Definition: {def.Key}");
            sb.AppendLine($"Template: {def.Value}");
            sb.AppendLine("Tokens:");
            foreach (var token in def.Tokens)
            {
                sb.AppendLine($"  {token.GetType().Name}: {token.Value}");
            }
            sb.AppendLine($"IsHighPriority: {def.IsHighPriority}");
            sb.AppendLine();
        }

        File.WriteAllText(filePath, sb.ToString());
    }
    private void WriteMatchToFile(string input, List<Token> keyTokens, string capturedValue = null)
    {
        string filePath = "TailMatches.txt";
        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"Match Input: {input}");
        sb.AppendLine("Tokens:");
        foreach (var token in keyTokens)
        {
            sb.AppendLine($"  {token.GetType().Name}: {token.Value}");
        }

        if (capturedValue != null)
        {
            sb.AppendLine($"Captured Value: {capturedValue}");
        }

        sb.AppendLine();

        File.AppendAllText(filePath, sb.ToString());
    }
}