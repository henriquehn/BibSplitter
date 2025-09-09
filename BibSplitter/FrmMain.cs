using BibLib.Parsing;
using BibSplitter.DataModels;

namespace BibSplitter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = GetSumary("GoogleSchoolar.bib", "Web_Of_Science.bib");
            //dataGridView1.DataSource = GetSumary("Web_Of_Science.bib");

        }

        private List<SummaryItem> GetSumary(params string[] files)
        {
            var entries = new List<BibElement>();

            foreach (var file in files)
            {
                if (File.Exists(file))
                {
                    var data = File.ReadAllText(file);
                    entries.AddRange(BibConverter.Deserialize(data));
                }
            }

            var groups = entries
                .GroupBy(x => x.GetValueOrDefault("year"))
                .OrderBy(g => g.Key);

            var summary = new List<SummaryItem>();
            foreach (var group in groups)
            {
                summary.Add(new SummaryItem
                {
                    Year = group.Key ?? "Undefined",
                    Count = group.Count()
                });
            }
            return summary;
        }
    }
}
