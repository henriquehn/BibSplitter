namespace BibLib.Exceptions
{
    [Serializable]
    public class SyntaxError : Exception
    {
        public int StartPosition { get; }
        public int EndPosition { get; }

        public SyntaxError(int position): base($"Syntax error at char {position}")
        {
            this.StartPosition = position;
            this.EndPosition = position;
        }

        public SyntaxError(int position, string message): base($"Syntax error at char {position}: {message}")
        {
            this.StartPosition = position;
            this.EndPosition = position;
        }

        public SyntaxError(int start, int end, string message): base($"Syntax error at char range from {start} to {end}: {message}")
        {
            this.StartPosition = start;
            this.EndPosition = end;
        }
    }
}