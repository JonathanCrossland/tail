namespace Tail.Parser;

using System.Text;
using System.Xml.Linq;

internal partial class TailParser
{
    private string _Input;
    private int _Position;
    private List<Token> _Tokens;

    private const char EscapeChar = '\\';
    private const char HighPriorityChar = '!';
    private const char PrefixStartChar = '[';
    private const char PrefixEndChar = ']';
    private const char SuffixStartChar = '(';
    private const char SuffixEndChar = ')';
    private const char ContainsLiteralStartChar = '{';
    private const char ContainsLiteralEndChar = '}';

    internal List<Token> ParseKey(string key = "")
    {
        _Input = key ?? "";
        _Position = 0;
        _Tokens = new List<Token>();

        EatHighPriority();

        while (_Position < _Input.Length)
        {
            EatEscape();
            EatPrefix();
            EatSuffix();
            EatContainsLiteral();
            EatWhitespace();
            EatLiteral();
        }

        EatFinalLiteral();
        return _Tokens;
    }

    protected void EatHighPriority()
    {
        if (_Position < _Input.Length && PeekChar() == HighPriorityChar && _Tokens.Count == 0 && _Position == 0)
        {
            _Tokens.Add(new PriorityToken());
            _Position++;
        }
    }

    private void EatEscape()
    {
        if (_Position < _Input.Length && PeekChar() == EscapeChar)
        {
            _Position++;
            if (_Position < _Input.Length)
            {
                AppendLiteral(ReadChar());
                _Position++;
            }
        }
    }

    private void EatPrefix()
    {
        if (_Position < _Input.Length && PeekChar() == PrefixStartChar)
        {
            _Position++;
            StringBuilder prefixValue = new StringBuilder();
            bool escaped = false;
            while (_Position < _Input.Length && (PeekChar() != PrefixEndChar || escaped))
            {
                if (PeekChar() == EscapeChar && !escaped)
                {
                    escaped = true;
                    _Position++;
                    continue;
                }
                prefixValue.Append(ReadChar());
                _Position++;
                escaped = false;
            }
            if (_Position < _Input.Length && PeekChar() == PrefixEndChar)
            {
                _Position++;
                _Tokens.Add(new PrefixToken(prefixValue.ToString()));
            }
            else
            {
                _Tokens.Add(new LiteralToken(PrefixStartChar.ToString()));
                AppendLiteral(prefixValue.ToString());
            }
        }
    }

    private void EatSuffix()
    {
        if (_Position < _Input.Length && PeekChar() == SuffixStartChar)
        {
            _Position++;
            StringBuilder suffixValue = new StringBuilder();
            bool escaped = false;
            while (_Position < _Input.Length && (PeekChar() != SuffixEndChar || escaped))
            {
                if (PeekChar() == EscapeChar && !escaped)
                {
                    escaped = true;
                    _Position++;
                    continue;
                }
                suffixValue.Append(ReadChar());
                _Position++;
                escaped = false;
            }
            if (_Position < _Input.Length && PeekChar() == SuffixEndChar)
            {
                _Position++;
                _Tokens.Add(new SuffixToken(suffixValue.ToString()));
            }
            else
            {
                _Tokens.Add(new LiteralToken(SuffixStartChar.ToString()));
                AppendLiteral(suffixValue.ToString());
            }
        }
    }

    private void EatContainsLiteral()
    {
        if (_Position < _Input.Length && PeekChar() == ContainsLiteralStartChar)
        {
            _Position++;
            int endIndex = _Input.IndexOf(ContainsLiteralEndChar, _Position);
            if (endIndex != -1)
            {
                string containsLiteralValue = _Input.Substring(_Position, endIndex - _Position);
                _Tokens.Add(new ContainsLiteralToken(containsLiteralValue));
                _Position = endIndex + 1;
            }
            else
            {
                _Tokens.Add(new LiteralToken(ContainsLiteralStartChar.ToString()));
            }
        }
    }

    private void EatLiteral()
    {
        if (_Position < _Input.Length && !IsSpecialCharacter(PeekChar()) && !char.IsWhiteSpace(PeekChar()))
        {
            StringBuilder literalValue = new StringBuilder();
            while (_Position < _Input.Length && !IsSpecialCharacter(PeekChar()) && !char.IsWhiteSpace(PeekChar()))
            {
                literalValue.Append(ReadChar());
                _Position++;
            }
            if (literalValue.Length > 0)
            {
                AppendLiteral(literalValue.ToString());
            }
        }
    }

    private void EatWhitespace()
    {
        if (_Position < _Input.Length && char.IsWhiteSpace(PeekChar()))
        {
            StringBuilder whitespaceValue = new StringBuilder();
            while (_Position < _Input.Length && char.IsWhiteSpace(PeekChar()))
            {
                whitespaceValue.Append(ReadChar());
                _Position++;
            }
            _Tokens.Add(new WhitespaceToken(whitespaceValue.ToString()));
        }
    }

    private void EatFinalLiteral()
    {
        if (_Position > 0 && _Position == _Input.Length)
        {
            return;
        }

        if (_Position > 0 && _Input.Length > 0 && !IsSpecialCharacter(_Input[_Input.Length - 1]) && _Position == _Input.Length)
        {
            StringBuilder literalValue = new StringBuilder();
            int tempPos = _Position - 1;
            while (tempPos >= 0 && !IsSpecialCharacter(_Input[tempPos]))
            {
                literalValue.Insert(0, _Input[tempPos]);
                tempPos--;
            }
            if (literalValue.Length > 0)
            {
                AppendLiteral(literalValue.ToString());
            }
        }
    }

    private bool IsSpecialCharacter(char c)
    {
        return c == EscapeChar || c == HighPriorityChar || c == PrefixStartChar || c == PrefixEndChar || c == SuffixStartChar || c == SuffixEndChar || c == ContainsLiteralStartChar || c == ContainsLiteralEndChar;
    }

    private void AppendLiteral(string value)
    {
        if (_Tokens.Count > 0 && _Tokens[_Tokens.Count - 1] is LiteralToken literalToken)
        {
            literalToken.Value += value;
        }
        else
        {
            _Tokens.Add(new LiteralToken(value));
        }
    }

    private string ReadChar()
    {
        return _Input[_Position].ToString();
    }

    private char PeekChar()
    {
        return _Input[_Position];
    }

   
}