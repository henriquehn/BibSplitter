using System.ComponentModel;
using System.Runtime.Serialization;

namespace BibLib.Parsing
{
    public enum BibType
    {
        Article,
        Book,
        Booklet,
        Conference,
        InBook,
        InCollection,
        InProceedings,
        Manual,
        MasterThesis,
        MastersThesis,
        Misc,
        PhDThesis,
        Proceedings,
        TechReport,
        Unpublished,
        [Description("Revisão")]
        Review,
        [Description("book-chapter")]
        Book_chapter,
        Editorial
    }
}