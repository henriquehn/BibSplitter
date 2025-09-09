namespace BibLib.Parsing
{
    public enum BibTokenType
    {
        None,
        AtSymbol,
        EntryType,
        OpenBrace,
        CloseBrace,
        Comma,
        Equals,
        FieldName,
        FieldValue,
        EndOfFile
    }
}