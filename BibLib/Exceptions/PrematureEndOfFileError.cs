namespace BibLib.Exceptions
{
    [Serializable]
    public class PrematureEndOfFileError : SyntaxError
    {
        public PrematureEndOfFileError(int position) : base(position)
        {
        }

        public PrematureEndOfFileError(int position, string message) : base(position, message)
        {
        }
    }
}