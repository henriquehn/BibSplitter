namespace BibLib.Parsing
{
    public enum BibTokenType
    {
        None,
        Comment,
        AtSymbol,
        EntryType,
        OpenBrace,
        CloseBrace,
        Comma,
        Equals,
        FieldName,
        FieldValue,
        EndOfFile,
    }
}