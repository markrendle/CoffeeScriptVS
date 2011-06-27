using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkRendle.CoffeeScriptLanguageService
{
    using Microsoft.VisualStudio.Package;

    class CoffeeScriptParser
    {
        public enum ParseState
        {
            InText = 0,
            InSingleQuotes = 1,
            InDoubleQuotes = 2,
            InBlockComment = 3,
            InSingleQuoteHeredoc = 4,
            InDoubleQuoteHeredoc = 5
        }

        private string _source;
        private int _offset;

        public void SetSource(string source, int offset)
        {
            _source = source;
            _offset = offset;
        }

        public bool GetNextToken(TokenInfo tokenInfo, ref ParseState state)
        {
            bool foundToken = false;
            int index = _offset;
            if (index >= _source.Length) return false;

            int endIndex = -1;
            switch (state)
            {
                case ParseState.InSingleQuotes:
                    state = HandleSingleQuotes(out endIndex, ref foundToken);
                    break;
                case ParseState.InDoubleQuotes:
                    state = HandleDoubleQuotes(out endIndex, ref foundToken);
                    break;
                case ParseState.InSingleQuoteHeredoc:
                    state = HandleHeredoc('\'', state, out endIndex);
                    break;
                case ParseState.InDoubleQuoteHeredoc:
                    state = HandleHeredoc('"', state, out endIndex);
                    break;
                case ParseState.InBlockComment:
                    state = HandleBlockComment(out endIndex);
                    break;
                case ParseState.InText:
                    state = HandleToken(ref foundToken, ref endIndex);
                    break;
            }

            tokenInfo.EndIndex = endIndex;
            _offset = endIndex + 1;
            return foundToken;
        }

        private ParseState HandleToken(ref bool foundToken, ref int endIndex)
        {
            if (_source[_offset] == '\'')
            {
                _offset++;
                return HandleSingleQuotes(out endIndex, ref foundToken);
            }
            if (_source[_offset] == '"')
            {
                _offset++;
                return HandleDoubleQuotes(out endIndex, ref foundToken);
            }
            if (_source[_offset] == '#')
            {
                return HandleComment(out endIndex);
            }

            int startIndex = _offset;
            while (_offset < _source.Length && _source[_offset] != ' ')
            {
                _offset++;
            }
            endIndex = _offset - 1;
            string token = _source.Substring(startIndex, (endIndex - startIndex) + 1);

            return ParseState.InText;
        }

        private ParseState HandleComment(out int endIndex)
        {
            if (_offset == 0 && _source.TrimEnd(' ') == "###")
            {
                endIndex = 2;
                return ParseState.InBlockComment;
            }
            _offset = _source.Length;
            endIndex = _source.Length - 1;
            _offset = _source.Length;
            return ParseState.InText;
        }

        private ParseState HandleHeredoc(char quote, ParseState state, out int endIndex)
        {
            if (_source.Trim() == string.Concat(quote,quote,quote))
            {
                endIndex = 2;
                return ParseState.InText;
            }
            endIndex = _source.Length - 1;
            return state;
        }

        private ParseState HandleBlockComment(out int endIndex)
        {
            if (_source.TrimEnd(' ') == "###")
            {
                endIndex = 2;
                return ParseState.InText;
            }
            endIndex = _source.Length - 1;
            return ParseState.InBlockComment;
        }

        private ParseState HandleDoubleQuotes(out int endIndex, ref bool foundToken)
        {
            if (_source[_offset] == '"' && _source.Length > _offset + 1 && _source[_offset + 1] == '"')
            {
                endIndex = _offset + 1;
                return ParseState.InDoubleQuoteHeredoc;
            }
            if (FindClosingQuote('"', out endIndex))
            {
                foundToken = true;
                return ParseState.InText;
            }
            return ParseState.InDoubleQuotes;
        }

        private ParseState HandleSingleQuotes(out int endIndex, ref bool foundToken)
        {
            if (_source[_offset] == '\'' && _source.Length > _offset + 1 && _source[_offset + 1] == '\'')
            {
                endIndex = _offset + 1;
                return ParseState.InSingleQuoteHeredoc;
            }
            if (FindClosingQuote('\'', out endIndex))
            {
                foundToken = true;
                return ParseState.InText;
            }
            return ParseState.InSingleQuotes;
        }

        private bool FindClosingQuote(char quote, out int endIndex)
        {
            bool backslash = false;
            int index = _offset;
            while (index < _source.Length)
            {
                if (_source[index] == quote)
                {
                    endIndex = index;
                    return true;
                }
                if (_source[index] == '\\')
                    backslash = !backslash;
                index++;
            }

            endIndex = index - 1;
            return false;
        }
    }
}
