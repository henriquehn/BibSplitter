using BibLib.Parsing;
using BibSettings;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Text;

namespace BibAnalyzer
{
    public partial class Form1 : Form
    {
        private static readonly string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private List<BibElement> validEntries = [];
        private List<BibElement> invalidEntries = [];
        private List<BibElement> undefinedEntries = [];

        private static string LastPath
        {
            get
            {
                return BibSettingsHolder.Load(DefaultBibSettings.LastBrowsingPath, defaultPath, "BibAnalyzer");
            }
            set
            {
                BibSettingsHolder.Save(DefaultBibSettings.LastBrowsingPath, value, "BibAnalyzer");
            }
        }

        private string filter = string.Empty;
        public string Filter
        {
            get => filter;
            set
            {
                filter = value?.Trim() ?? "";
                if (string.IsNullOrEmpty(filter))
                {
                    validEntriesGrid.DataSource = validEntries;
                    validEntriesStatus.Text = $"Resultados: {validEntries.Count}";
                    invalidEntriesGrid.DataSource = invalidEntries;
                    invalidEntriesStatus.Text = $"Resultados: {invalidEntries.Count}";
                    undefinedEntriesGrid.DataSource = undefinedEntries;
                    undefinedEntriesStatus.Text = $"Resultados: {undefinedEntries.Count}";
                }
                else
                {
                    List<BibElement> entries = FilterEntries(validEntries, filter);
                    validEntriesGrid.DataSource = entries;
                    validEntriesStatus.Text = $"Resultados: {entries.Count}";
                    entries = FilterEntries(invalidEntries, filter);
                    invalidEntriesGrid.DataSource = entries;
                    invalidEntriesStatus.Text = $"Resultados: {entries.Count}";
                    entries = FilterEntries(undefinedEntries, filter);
                    undefinedEntriesGrid.DataSource = entries;
                    undefinedEntriesStatus.Text = $"Resultados: {entries.Count}";
                }
            }
        }

        private List<BibElement> FilterEntries(List<BibElement> entries, string filterText)
        {
            var response = new List<BibElement>();
            foreach (var entry in entries)
            {
                if ((entry.Title?.Contains(filterText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                   (entry.Authors?.Contains(filterText, StringComparison.OrdinalIgnoreCase) ?? false))
                {
                    response.Add(entry);
                }
            }
            return response;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void cmdBrowseFolder_Click(object sender, EventArgs e)
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                dialog.Title = "Selecione uma pasta";
                dialog.InitialDirectory = LastPath;

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    LastPath = dialog.FileName;
                    BrowseFiles();
                }
            }
        }

        private void BrowseFiles()
        {
            string currentFile = null;
            var directories = Directory.GetDirectories(LastPath);
            var entries = new List<BibElement>();

            if (directories == null || directories.Length == 0)
            {
                directories = new string[] { LastPath };
            }

            validEntriesGrid.DataSource = null;
            invalidEntriesGrid.DataSource = null;
            undefinedEntriesGrid.DataSource = null;

            validEntries.Clear();
            invalidEntries.Clear();
            undefinedEntries.Clear();

            foreach (var path in directories)
            {
                var files = Directory.GetFiles(path, "*.bib", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    currentFile = file;
                    var data = File.ReadAllText(file);
                    entries.AddRange(BibConverter.Deserialize(data));
                }
            }

            entries.Sort((x, y) => string.Compare(x.GetValueOrDefault("Author"), y.GetValueOrDefault("Author")));

            foreach (var entry in entries)
            {
                entry.Title = entry.GetValueOrDefault("title")?.Trim().TrimEnd('.', ' ');
                entry.Authors = entry.GetValueOrDefault("author")?.Trim().TrimEnd('.', ' ');

                if (entry.ContainsKey("numpages") && int.TryParse(entry["numpages"], out int numpages))
                {
                    entry.PageCount = numpages;
                    if (numpages >= 5)
                    {
                        validEntries.Add(entry);
                    }
                    else
                    {
                        invalidEntries.Add(entry);
                    }
                }
                else
                {
                    if (entry.ContainsKey("pages"))
                    {
                        var pages = entry["pages"]?.Replace("--", "-").Split("-");
                        if (pages != null &&
                            pages.Length == 2 &&
                            int.TryParse(pages[0], out int startPage) &&
                            int.TryParse(pages[1], out int endPage) &&
                            startPage > 0 && endPage >= startPage
                            )
                        {
                            entry.PageCount = (endPage - startPage + 1);
                            entry.Add("numpages", (endPage - startPage + 1).ToString());
                            if (endPage - startPage >= 4)
                            {
                                validEntries.Add(entry);
                            }
                            else
                            {
                                invalidEntries.Add(entry);
                            }
                        }
                        else
                        {
                            entry.PageCount = null;
                            undefinedEntries.Add(entry);
                        }
                    }
                    else
                    {
                        undefinedEntries.Add(entry);
                    }
                }
            }

            Filter = txtFilter.Text;
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            Filter = txtFilter.Text;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
