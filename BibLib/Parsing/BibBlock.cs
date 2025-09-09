namespace BibLib.Parsing
{
    public enum BibBlock
    {
        /// <summary>
        /// No block context (initial state)
        /// </summary>
        None,
        /// <summary>
        /// Inside the body of an entry (between the main braces)
        /// </summary>
        Body,
        /// <summary>
        /// Inside a field (after the '=' and between the field braces)
        /// </summary>
        Field,
        /// <summary>
        /// Inside an escaped string (inside field braces and enclosed in extra braces)
        /// </summary>
        Escape,
    }
}