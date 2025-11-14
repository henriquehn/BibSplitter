using BibAnalyzer.Utils;
using BibLib.Adapters;
using BibLib.DataModels;
using BibLib.Parsing;
using BibSettings;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Data;

namespace BibAnalyzer
{
    public partial class Form1 : Form
    {
        private BibTableAdapter adapter = new();
        private BibElementAdapter elementAdapter = new();
        private static readonly string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private readonly ClipboardMonitor clipboardMonitor;
        private readonly DataTable validEntriesTable = BuildBaseTable();
        private readonly DataView validEntriesView;
        private readonly object validEntriesLock = new();
        private readonly DataTable invalidEntriesTable = BuildBaseTable();
        private readonly DataView invalidEntriesView;
        private readonly object invalidEntriesLock = new();
        private readonly DataTable undefinedEntriesTable = BuildBaseTable();
        private readonly DataView undefinedEntriesView;
        private readonly object undefinedEntriesLock = new();
        private string lastClipboardContent = string.Empty;

        public Form1()
        {
            InitializeComponent();
            this.clipboardMonitor = new ClipboardMonitor();
            clipboardMonitor.ClipboardChanged += (s, e) =>
            {
                LastClipbardContent = e.Text;
            };
            validEntriesView = new(validEntriesTable);
            invalidEntriesView = new(invalidEntriesTable);
            undefinedEntriesView = new(undefinedEntriesTable);
        }

        ~Form1()
        {
            clipboardMonitor.Dispose();
        }

        private static DataTable BuildBaseTable()
        {
            var response = new DataTable() { CaseSensitive = false };
            response.Columns.Add("Key", typeof(string));
            response.Columns.Add("Type", typeof(string));
            response.Columns.Add("Title", typeof(string));
            response.Columns.Add("Author", typeof(string));

            return response;
        }

        public string LastClipbardContent
        {
            get => lastClipboardContent;
            set
            {
                if (!string.Equals(lastClipboardContent, value, StringComparison.OrdinalIgnoreCase))
                {
                    lastClipboardContent = value;
                    UpdateFilter();
                }
            }
        }

        private void UpdateFilter()
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action(UpdateFilter));
                return;
            }
            else
            {
                if (checkClipboard)
                {
                    txtFilter.Text = lastClipboardContent;
                }
            }
        }

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
        private bool checkClipboard;

        public string Filter
        {
            get => filter;
            set
            {
                filter = value?.Trim() ?? "";
                var filterString = DataUtils.BuildBibFilter(filter);
                validEntriesView.RowFilter = filterString;
                invalidEntriesView.RowFilter = filterString;
                undefinedEntriesView.RowFilter = filterString;

                validEntriesStatus.Text = $"Resultados: {validEntriesView.Count}";
                invalidEntriesStatus.Text = $"Resultados: {invalidEntriesView.Count}";
                undefinedEntriesStatus.Text = $"Resultados: {undefinedEntriesView.Count}";
            }
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

        private void BindDataSources()
        {
            validEntriesGrid.DataSource = validEntriesView;
            invalidEntriesGrid.DataSource = invalidEntriesView;
            undefinedEntriesGrid.DataSource = undefinedEntriesView;
        }

        private void UnbindDataSources()
        {
            validEntriesGrid.DataSource = null;
            invalidEntriesGrid.DataSource = null;
            undefinedEntriesGrid.DataSource = null;
        }

        private void BrowseFiles()
        {
            string currentFile = null;
            List<string> directories = [LastPath];
            directories.AddRange(Directory.GetDirectories(LastPath));
            BibElements entries = new();
            BibElements pendingEntries = new();

            UnbindDataSources();

            validEntriesTable.Rows.Clear();
            invalidEntriesTable.Rows.Clear();
            undefinedEntriesTable.Rows.Clear();


            foreach (var path in directories)
            {
                var files = Directory.GetFiles(path, "*.bib", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    currentFile = file;
                    var data = File.ReadAllText(file);
                    var newEntries = BibConverter<BibElement, BibElements, BibElementAdapter>.Deserialize(data);
                    entries.AddRange(newEntries);
                }
            }

            entries.Sort((x, y) => string.Compare(x.GetValueOrDefault("Author"), y.GetValueOrDefault("Author")));

            foreach (var entry in entries)
            {
                entry.Title = entry.GetValueOrDefault("title")?.Trim().TrimEnd('.', ' ');
                entry.Authors = entry.GetValueOrDefault("author")?.Trim().TrimEnd('.', ' ');
                entry.DOI = entry.GetValueOrDefault("doi")?.Trim().TrimEnd('.', ' ');

                if (entry.TryGetValue("numpages", out string value) && int.TryParse(value, out int numpages))
                {
                    entry.PageCount = numpages;
                    if (numpages >= 5)
                    {
                        adapter.AppendEntry(entry, validEntriesTable);
                    }
                    else
                    {
                        adapter.AppendEntry(entry, invalidEntriesTable);
                    }
                }
                else
                {
                    if (entry.TryGetValue("pages", out string value1))
                    {
                        var pages = value1?.Replace("--", "-").Split("-");
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
                                adapter.AppendEntry(entry, validEntriesTable);
                            }
                            else
                            {
                                adapter.AppendEntry(entry, invalidEntriesTable);
                            }
                        }
                        else
                        {
                            entry.PageCount = null;
                            if (string.IsNullOrEmpty(entry.DOI))
                            {
                                adapter.AppendEntry(entry, undefinedEntriesTable);
                            }
                            else
                            {
                                pendingEntries.Add(entry);
                            }
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(entry.DOI))
                        {
                            adapter.AppendEntry(entry, undefinedEntriesTable);
                        }
                        else
                        {
                            pendingEntries.Add(entry);
                        }
                    }
                }
            }

            if (pendingEntries.Count > 0)
            {
                var newTask = Task.Run(async () =>
                {
                    using (var client = new HttpClient())
                    {
                        foreach (var entry in pendingEntries)
                        {
                            var count = await CrossrefPageCounter.GetPageCountFromDoiAsync(entry.DOI, client).ConfigureAwait(false);
                            if (count.HasValue)
                            {
                                entry.PageCount = count.Value;
                                if (count.Value >= 5)
                                {
                                    lock (validEntriesLock)
                                    {
                                        adapter.AppendEntry(entry, validEntriesTable);
                                    }
                                }
                                else
                                {
                                    lock (invalidEntriesLock)
                                    {
                                        adapter.AppendEntry(entry, invalidEntriesTable);
                                    }
                                }
                            }
                            else
                            {
                                lock (undefinedEntriesLock)
                                {
                                    adapter.AppendEntry(entry, undefinedEntriesTable);
                                }
                            }
                        }
                    }
                });
                newTask.Wait();
            }

            Filter = txtFilter.Text;
            BindDataSources();
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            Filter = txtFilter.Text;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void chkClipboardMonitor_CheckedChanged(object sender, EventArgs e)
        {
            this.checkClipboard = chkClipboardMonitor.Checked;
        }

        private void cmdSaveValid_Click(object sender, EventArgs e)
        {
            Save(validEntriesTable);
        }

        private void cmdSaveInvalid_Click(object sender, EventArgs e)
        {
            Save(invalidEntriesTable);
        }

        private void cmdSaveUndefined_Click(object sender, EventArgs e)
        {
            Save(undefinedEntriesTable);
        }

        private void Save(DataTable entries)
        {
            try
            {
                var dlg = new SaveFileDialog();
                dlg.Filter = "BibTeX files (*.bib)|*.bib";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(dlg.FileName, BibConverter<DataRow, DataTable, BibTableAdapter>.Serialize(entries));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar o arquivo: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
