namespace BibSplitter.DataModels
{
    public class SummaryItem: IComparable<SummaryItem>
    {
        public string Year { get; internal set; }
        public int Count { get; internal set; }

        public int CompareTo(SummaryItem other)
        {
            return string.Compare(Year, other.Year);
        }
    }
}
