using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkRendle.CoffeeScriptLanguageService
{
    using Microsoft.VisualStudio.Package;

    public class CoffeeLanguageService : LanguageService
    {
        private LanguagePreferences _preferences;

        public override string GetFormatFilterList()
        {
            throw new NotImplementedException();
        }

        public override LanguagePreferences GetLanguagePreferences()
        {
            if (_preferences == null)
            {
                _preferences = new LanguagePreferences(this.Site, typeof(CoffeeLanguageService).GUID, this.Name);
                _preferences.Init();
            }

            return _preferences;
        }

        public override IScanner GetScanner(Microsoft.VisualStudio.TextManager.Interop.IVsTextLines buffer)
        {
            throw new NotImplementedException();
        }

        public override string Name
        {
            get { throw new NotImplementedException(); }
        }

        public override AuthoringScope ParseSource(ParseRequest req)
        {
            throw new NotImplementedException();
        }
    }

    class CoffeeScanner : IScanner
    {
        private enum ParseState
        {
            InText = 0,
            InSingleQuotes = 1,
            InDoubleQuotes = 2,
            InBlockComment = 3
        }

        private string _source;
        private int _offset;

        /// <summary>
        /// Used to (re)initialize the scanner before scanning a small portion of text, such as single source line for syntax coloring purposes
        /// </summary>
        /// <param name="source">The source text portion to be scanned</param><param name="offset">The index of the first character to be scanned</param>
        public void SetSource(string source, int offset)
        {
            _source = source;
            _offset = offset;
        }

        /// <summary>
        /// Scan the next token and fill in syntax coloring details about it in tokenInfo.
        /// </summary>
        /// <param name="tokenInfo">Keeps information about token.</param><param name="state">Keeps track of scanner state. In: state after last token. Out: state after current token.</param>
        /// <returns/>
        public bool ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int state)
        {
            throw new NotImplementedException();
        }

        private bool GetNextToken(TokenInfo tokenInfo, ref int state)
        {
            bool foundToken = false;
            var parseState = (ParseState) state;
            int index = _offset;
            if (index >= _source.Length) return false;

            int endIndex = -1;
            switch (parseState)
            {
                case ParseState.InSingleQuotes:
                    if (FindClosingQuote('\'', out endIndex))
                    {
                        foundToken = true;
                        state = (int)ParseState.InText;
                    }
                    break;
                case ParseState.InDoubleQuotes:
                    if (FindClosingQuote('"', out endIndex))
                    {
                        foundToken = true;
                        state = (int)ParseState.InText;
                    }
                    break;
                case ParseState.InBlockComment:
                    endIndex = _source.Length - 1;
                    _offset = _source.Length;
                    state = (int)ParseState.InText;
                    break;
                case ParseState.InText:
                    if (_source[index] == '\'')
                    {
                        if (FindClosingQuote('\'', out endIndex))
                        {
                            foundToken = true;
                            state = (int)ParseState.InText;
                        }
                        else
                        {
                            state = (int) ParseState.InSingleQuotes;
                        }
                    }
                    else if (_source[index] == '"')
                    {
                        if (FindClosingQuote('"', out endIndex))
                        {
                            foundToken = true;
                            state = (int)ParseState.InText;
                        }
                        else
                        {
                            state = (int)ParseState.InDoubleQuotes;
                        }
                    }
                    else if (_source[index] == '#')
                    {
                        _offset = _source.Length;
                        endIndex = _source.Length - 1;
                        _offset = _source.Length;
                        state = (int)ParseState.InText;
                    }
                    else
                    {
                        while (index < _source.Length && _source[index] != ' ')
                        {
                            index++;
                        }
                        
                    }
            }

            tokenInfo.EndIndex = endIndex;
            _offset = endIndex + 1;
            return foundToken;
        }

        private bool FindClosingQuote(char quote, out int endIndex)
        {
            bool backslash = false;
            int index = _offset;
            while (index < _source.Length)
            {
                switch (_source[index])
                {
                    case '\'':
                        if (!backslash)
                        {
                            endIndex = index;
                            return true;
                        }
                        break;
                    case '\\':
                        backslash = !backslash;
                        break;
                }
                index++;
            }

            endIndex = index - 1;
            return false;
        }
    }
}
