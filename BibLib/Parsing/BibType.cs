using System.ComponentModel;
using System.Runtime.Serialization;

namespace BibLib.Parsing
{
    public enum BibType
    {
        Article=1,
        Book=2,
        Booklet=3,
        Conference=4,
        InBook=5,
        InCollection=6,
        InProceedings=7,
        Manual=8,
        MasterThesis=9,
        MastersThesis=10,
        Misc=11,
        PhDThesis=12,
        Proceedings=13,
        TechReport=14,
        Unpublished=15,
        [Description("Revisão")]
        Review=16,
        [Description("book-chapter")]
        Book_chapter=17,
        Editorial=18
    }
}