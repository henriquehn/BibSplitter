using BibLib.Collections;
using DodFramework.DataLibrary.DAO.Attributes.ItemAttributes;

namespace BibLib.DataModels.PaperManager
{
    [SourceTable("duplicates")]
    public class Duplicate : PaperBase
    {
        private static SequenceKeeper sequenceKeeper = new();

        [SourceField("abstract")]
        public string Abstract { get; set; }
        [SourceField("paper_hash")]
        public string PaperHash { get; set; }
        [SourceField("sequence")]
        public int? Sequence { get; set; }

        public void GenerateSequence()
        {
            this.Sequence ??= ComputeHash();
        }

        private int ComputeHash()
        {
            var sequence = sequenceKeeper.Next(this.PaperHash);
            var hashData = System.Text.Encoding.UTF8.GetBytes($"{this.PaperHash}|{sequence}");
            this.Hash = DodFramework.Security.Algorithms.Encription.ComputeSHA256String(hashData);
            return sequence;
        }

        public static void ResetSequence()
        {
            sequenceKeeper.Reset();
        }
    }
}
