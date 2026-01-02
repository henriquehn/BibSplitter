using BibLib.Exceptions;
using System.Text;

namespace BibLib.Parsing
{
    internal class BibScanner
    {
        private readonly string data;
        private BibTokenType lastToken = BibTokenType.None;
        private BibBlock currentBlock = BibBlock.None;
        private int position = 0;

        public BibScanner(string data)
        {
            this.data = data;
        }

        public char CurrentChar { get => data[position]; }
        private HashSet<char> ScapedChars = ['{', '}'];
        private int lastTokenPos;

        public char NextChar { get => data.Length > position + 1 ? data[position + 1] : '\0'; }
        public int Position { get { return this.position; } }

        private char? NextValidChar()
        {
            while (position < data.Length && char.IsWhiteSpace(CurrentChar))
            {
                position++;
            }
            return position < data.Length ? CurrentChar : null;
        }

        public BibToken NextToken()
        {
            var nextChar = NextValidChar();
            var tokenPos = position;
            BibToken response = null;
            int increment = 0;
            switch (nextChar)
            {
                case null:
                    response = CreateToken(BibTokenType.EndOfFile, tokenPos);
                    break;
                case '@':
                    switch (currentBlock)
                    {
                        case BibBlock.None:
                            /* It is a new entry */
                            response = CreateToken(BibTokenType.AtSymbol, tokenPos, "@");
                            increment = 1;
                            break;
                        case BibBlock.Field:
                            response = ScanFieldValue();
                            increment = 0;
                            break;
                        default:
                            /* It is wrong */
                            throw new SyntaxError(position);
                    }
                    break;
                case '{':
                    if (currentBlock == BibBlock.Field)
                    {
                        response = ScanFieldValue();
                        increment = 0;
                    }
                    else
                    {
                        response = CreateToken(BibTokenType.OpenBrace, tokenPos, "{");
                        switch (currentBlock)
                        {
                            case BibBlock.None:
                                currentBlock = BibBlock.Body;
                                break;
                            case BibBlock.Body:
                                currentBlock = BibBlock.Field;
                                break;
                            case BibBlock.Escape:
                                throw new SyntaxError(position);
                        }
                        increment = 1;

                    }
                    break;
                case '}':
                    response = CreateToken(BibTokenType.CloseBrace, tokenPos, "}");
                    switch (currentBlock)
                    {
                        case BibBlock.None:
                            throw new SyntaxError(position);
                        case BibBlock.Body:
                            currentBlock = BibBlock.None;
                            break;
                        case BibBlock.Field:
                            currentBlock = BibBlock.Body;
                            break;
                        case BibBlock.Escape:
                            currentBlock = BibBlock.Field;
                            break;
                    }
                    increment = 1;
                    break;
                case '=':
                    if (currentBlock == BibBlock.Field)
                    {
                        response = ScanFieldValue();
                        increment = 0;
                    }
                    else
                    {
                        response = CreateToken(BibTokenType.Equals, tokenPos, "=");
                        increment = 1;
                    }
                    break;
                case ',':
                    if (currentBlock == BibBlock.Field)
                    {
                        response = ScanFieldValue();
                        increment = 0;
                    }
                    else
                    {
                        response = CreateToken(BibTokenType.Comma, tokenPos, ",");
                        increment = 1;
                    }
                    break;
                default:
                    switch (lastToken)
                    {
                        case BibTokenType.AtSymbol:
                            response = ScanFieldName(BibTokenType.EntryType);
                            lastToken = BibTokenType.EntryType;
                            break;
                        case BibTokenType.Equals:
                            response = ScanFieldValue();
                            break;
                        case BibTokenType.OpenBrace:
                            switch (currentBlock)
                            {
                                case BibBlock.Body:
                                    response = ScanFieldName(BibTokenType.FieldName);
                                    break;
                                case BibBlock.Field:
                                    response = ScanFieldValue();
                                    break;
                                default:
                                    throw new SyntaxError(position, $"The char \"{CurrentChar}\" is unexpected in current context");
                            }
                            break;
                        case BibTokenType.Comma:
                            response = ScanFieldName(BibTokenType.FieldName);
                            break;
                        default:
                            response = ScanComment();
                            break;
                            ///throw new SyntaxError(position, $"The char \"{CurrentChar}\" is unexpected in current context");
                    }
                    break;
            }

            position += increment;
            return response;
        }

        private BibToken ScanComment()
        {
            var sb = new StringBuilder();
            var tokenPos = position;
            while (position < data.Length && (CurrentChar != '@'))
            {
                sb.Append(CurrentChar);
                position++;
            }
            return CreateToken(BibTokenType.Comment, tokenPos, sb.ToString());
        }

        private BibToken ScanFieldValue()
        {
            var sb = new StringBuilder();
            var tokenPos = position;
            this.lastTokenPos = position;
            if (lastToken == BibTokenType.OpenBrace)
            {
                while (position < data.Length && (CurrentChar != '}'))
                {
                    if (CurrentChar == '\\' && ScapedChars.Contains(NextChar))
                    {
                        /* é um caractere de escape */
                        sb.Append(CurrentChar);
                        position++;
                        /* passa direto pelo caractere seguinte */
                        sb.Append(CurrentChar);
                        position++;
                    }
                    else if (CurrentChar == '{')
                    {
                        sb.Append(ScanEscape());
                    }
                    else
                    {
                        sb.Append(CurrentChar);
                        position++;
                    }
                }
            }
            else
            {
                while (position < data.Length && (CurrentChar != ','))
                {
                    sb.Append(CurrentChar);
                    position++;
                }
            }
            return CreateToken(BibTokenType.FieldValue, tokenPos, sb.ToString());
        }

        private string ScanEscape()
        {
            int level = 0;
            var sb = new StringBuilder();

            /* skip initial brace */
            sb.Append(CurrentChar);
            position++;
            while (position < data.Length && (level > 0 || CurrentChar != '}'))
            {
                sb.Append(CurrentChar);
                if (CurrentChar == '\\' && ScapedChars.Contains(NextChar))
                {
                    /* é um caractere de escape, pula para o caractere seguinte*/
                    position++;
                    sb.Append(CurrentChar);
                }
                else if (CurrentChar == '{')
                {
                    level++;
                }
                else if (CurrentChar == '}')
                {
                    level--;
                }
                position++;
            }
            if (position < data.Length && CurrentChar == '}')
            {
                sb.Append(CurrentChar);
                position++;
            }
            else
            {
                throw new PrematureEndOfFileError(position,"Premature end of file");
            }
            return sb.ToString();
        }

        private BibToken ScanFieldName(BibTokenType type)
        {
            var tokenPos = position;
            var sb = new StringBuilder();
            while (position < data.Length && CurrentChar != '{' && CurrentChar != ',' && CurrentChar != '=' && !char.IsWhiteSpace(CurrentChar))
            {
                sb.Append(CurrentChar);
                position++;
            }
            return CreateToken(type, tokenPos, sb.ToString());
        }

        private BibToken CreateToken(BibTokenType type, int tokenPos, string value = null)
        {
            lastToken = type;
            return new BibToken { Type = type, Position = tokenPos, Value = value };
        }

        public BibToken PeekNextToken()
        {
            var previousPos = position;
            var previousLastToken = lastToken;
            var previousCurrentBlock = currentBlock;
            var result = NextToken();
            position = previousPos;
            lastToken = previousLastToken;
            currentBlock = previousCurrentBlock;
            return result;
        }

        public IList<BibToken> PeekNextTokens(int count)
        {
            var result = new List<BibToken>();
            var previousPos = position;
            var previousLastToken = lastToken;
            var previousCurrentBlock = currentBlock;
            for (int index = 0; index < count; index++)
            {
                result.Add(NextToken());
            }
            position = previousPos;
            lastToken = previousLastToken;
            currentBlock = previousCurrentBlock;
            return result;
        }

        public string Inspect(int position, int previous, int count)
        {
            try
            {
                int start;
                start = (position < previous) ? 0 : position - previous;
                count = start + count > data.Length ? data.Length - start : count;
                return data.Substring(start, count);
            }
            catch
            {
                return string.Empty;
            }
        }

        public string Inspect(int previous, int count)
        {
            try
            {
                int start;
                start = (this.position < previous) ? 0 : this.position - previous;
                count = start + count > data.Length ? data.Length - start : count;
                return data.Substring(start, count);
            }
            catch
            {
                return string.Empty;
            }
        }

        public string Inspect(int count)
        {
            try
            {
                int start = this.position;
                count = start + count > data.Length ? data.Length - start : count;
                return data.Substring(start, count);
            }
            catch
            {
                return string.Empty;
            }
        }

        public int GetProgress()
        {
            return this.data == null || this.data.Length < 1 ? 100 : (int)((double)this.position / this.data.Length * 100);
        }
    }
}
