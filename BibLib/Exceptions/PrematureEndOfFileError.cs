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
        public PrematureEndOfFileError(int start, int end, string message) : base(start, end, message)
        {
        }
    }
}