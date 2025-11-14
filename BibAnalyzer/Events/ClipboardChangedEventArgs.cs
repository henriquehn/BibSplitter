namespace BibAnalyzer.Events
{
    public sealed class ClipboardChangedEventArgs : EventArgs
    {
        public string? Text { get; }
        public ClipboardChangedEventArgs(string? text) => Text = text;
    }
}
