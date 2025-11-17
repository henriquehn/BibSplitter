
using BibLib.DataModels;
using BibLib.Exceptions;
using BibLib.Extensions;
using BibLib.Interfaces;

namespace BibLib.Parsing
{
    public class BibParser<T, L, A> where A: IBibAdapter<T, L>, new() where L: new()
    {
        private readonly A adapter;

        private static readonly Dictionary<string, string> SpecialEntries = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Revisão", "Review" },
            { "book-chapter", "Book_chapter" },
        };

        private readonly BibScanner scanner;
        private readonly Action<int> progressCallback;

        public BibParser(string data, Action<int> progressCallback = null)
        {
            this.adapter = new();
            this.scanner = new BibScanner(data);
            this.progressCallback ??= ((_) => { });
        }

        public BibParser(string data, A adapter, Action<int> progressCallback = null)
        {
            this.adapter = adapter;
            this.scanner = new BibScanner(data);
            this.progressCallback ??= ((_) => { });
        }

        public L Parse()
        {
            var response = adapter.CreateList();
            BibElement entry;
            this.progressCallback.InvokeAsync(0);
            while ((entry = ParseEntry()) != null)
            {
                adapter.AppendEntry(entry, response);
                int progress = scanner.GetProgress();
                progressCallback.InvokeAsync(progress);
            }

            return response;
        }

        private BibElement ParseEntry()
        {
            BibToken currentToken;
            do
            {
                currentToken = Parse(BibTokenType.AtSymbol, BibTokenType.EndOfFile, BibTokenType.Comment);

                if (currentToken.Type == BibTokenType.EndOfFile)
                {
                    return null;
                }
            } while (currentToken.Type != BibTokenType.AtSymbol);

            var entryType = Parse(BibTokenType.EntryType);
            if (SpecialEntries.TryGetValue(entryType.Value, out var mappedValue))
            {
                entryType.Value = mappedValue;
            }

            var type = Enum.Parse<BibType>(entryType.Value, ignoreCase: true);
            Parse(BibTokenType.OpenBrace);
            var key = Parse(BibTokenType.FieldName);

            var result = new BibElement
            {
                Type = type,
                Key = key.Value
            };

            while (Peek(BibTokenType.Comma))
            {
                Parse(BibTokenType.Comma);
                if (Peek(BibTokenType.FieldName))
                {
                    var fieldName = Parse(BibTokenType.FieldName, BibTokenType.CloseBrace);

                    Parse(BibTokenType.Equals);
                    var nextToken = Parse(BibTokenType.OpenBrace, BibTokenType.FieldValue, BibTokenType.Comma);
                    string fieldValue;
                    if (nextToken.Type != BibTokenType.Comma)
                    {
                        if (nextToken.Type == BibTokenType.OpenBrace)
                        {
                            if (Peek(BibTokenType.CloseBrace))
                            {
                                fieldValue = string.Empty;
                            }
                            else
                            {
                                fieldValue = Parse(BibTokenType.FieldValue).Value;
                            }
                            Parse(BibTokenType.CloseBrace);
                        }
                        else
                        {
                            fieldValue = nextToken.Value;
                        }
                        if (!result.ContainsKey(fieldName.Value))
                        {
                            result.Add(fieldName.Value, fieldValue);
                        }
                    }
                }
            }
            Parse(BibTokenType.CloseBrace);

            return result;
        }

        private BibToken Parse(params BibTokenType[] expectedTokens)
        {
            var lastPosition = this.scanner.Position;
            var token = scanner.NextToken();
            if (!expectedTokens.Contains(token.Type))
            {
                if (token.Type == BibTokenType.EndOfFile)
                {
                    throw new PrematureEndOfFileError(token.Position, this.scanner.Position, $"Unexpected end of file: {expectedTokens.Join(" or ")} was expected but {token.Type} was found");
                }
                else
                {
                    throw new SyntaxError(token.Position, this.scanner.Position, $"{expectedTokens.Join(" or ")} was expected but {token.Type} was found");
                }
            }
            return token;
        }

        private bool Peek(params BibTokenType[] expectedTokens)
        {
            var token = scanner.PeekNextToken();
            return expectedTokens.Contains(token.Type);
        }

        private bool PeekMany(params BibTokenType[] expectedTokens)
        {
            var tokens = scanner.PeekNextTokens(expectedTokens.Length);

            for (int index = 0;index < expectedTokens.Length; index++)
            {
                if (tokens[index].Type != expectedTokens[index])
                {
                    return false;
                }
            }
            return true;
        }
    }
}