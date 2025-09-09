namespace BibLib.Exceptions
{
    [Serializable]
    public class SyntaxError : Exception
    {
        public int Position { get; }

        public SyntaxError(int position): base($"Syntax arror at char {position}")
        {
            this.Position = position;
        }

        public SyntaxError(int position, string message): base($"Syntax arror at char {position}: {message}")
        {
            this.Position = position;
        }
    }
}