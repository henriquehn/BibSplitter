using BibLib.Exceptions;
using BibLib.Parsing;
using BibSplitter.DataModels;
using System.Text;

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
            var src = "E:\\Henrique\\Onedrive\\Documentos\\Henrique\\Acadêmicos\\Doutorado\\Revisão Sistemática\\Bases\\Google Schoolar";
            var dst = "E:\\Henrique\\Onedrive\\Documentos\\Henrique\\Acadêmicos\\Doutorado\\Revisão Sistemática\\Bases\\Google Schoolar\\Out";

            try
            {
                var files = Directory.GetFiles(src, "*.bib");
                var groups = GetGroups(files);
                var summary = GetSumary(groups);
                dataGridView1.DataSource = summary;

                foreach (var group in groups)
                {
                    File.WriteAllText(Path.Combine(dst, $"{group.Key ?? "sem_ano"}.bib"), BibConverter.Serialize(group), Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private IOrderedEnumerable<IGrouping<string, BibElement>> GetGroups(params string[] files)
        {
            var entries = new List<BibElement>();

            foreach (var file in files)
            {
                if (File.Exists(file))
                {
                    try
                    {
                        var data = File.ReadAllText(file);
                        entries.AddRange(BibConverter.Deserialize(data));
                    }
                    catch (PrematureEndOfFileError ex)
                    {
                        throw new Exception($"O arquivo \"{file}\" está incompleto ou corrompido:\r\n\r\n{ex.Message}", ex);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Falha procesaando o arquivo \"{file}\":\r\n\r\n{ex.Message}", ex);
                    }
                }
            }

            var groups = entries
                .GroupBy(x => x.GetValueOrDefault("year"))
                .OrderBy(g => g.Key);

            return groups;
        }

        private List<SummaryItem> GetSumary(IOrderedEnumerable<IGrouping<string, BibElement>> groups)
        {
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

        private void button2_Click(object sender, EventArgs e)
        {
            var data = BibConverter.FromCsvFile("E:\\Henrique\\Onedrive\\Documentos\\Henrique\\Acadêmicos\\Doutorado\\Revisão Sistemática\\Bases\\Springer Nature\\SpringerNature_EN_ALL_de_655.csv");
            var bib = BibConverter.Serialize(data);
            File.WriteAllText("E:\\Henrique\\Onedrive\\Documentos\\Henrique\\Acadêmicos\\Doutorado\\Revisão Sistemática\\Bases\\Springer Nature\\SpringerNature_EN_ALL_de_655.bib", bib);
        }

        private void CmdJoin_Click(object sender, EventArgs e)
        {
            string currentFile = null;
            ////try
            {
                const string root = "E:\\Henrique\\Onedrive\\Documentos\\Henrique\\Acadêmicos\\Doutorado\\Revisão Sistemática\\Bases";
                var directories = Directory.GetDirectories(root);
                foreach (var path in directories)
                {
                    var entries = new List<BibElement>();
                    var files = Directory.GetFiles(path, "*.bib", SearchOption.AllDirectories);

                    foreach (var file in files)
                    {
                        currentFile = file;
                        var data = File.ReadAllText(file);
                        entries.AddRange(BibConverter.Deserialize(data));
                    }

                    var outFile = Path.Combine(path, ".bib");

                    File.WriteAllText(outFile, BibConverter.Serialize(entries), Encoding.UTF8);
                }

                MessageBox.Show(this, "Concluído", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            //catch (Exception ex)
            //{
            //    throw new Exception($"Falha procesaando o arquivo \"{currentFile}\":\r\n\r\n{ex.Message}", ex);
            //}
        }
    }
}
