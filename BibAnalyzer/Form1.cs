using BibAnalyzer.Enums;
using BibAnalyzer.Utils;
using BibLib.Adapters;
using BibLib.DataModels;
using BibLib.Extensions;
using BibLib.Interfaces;
using BibLib.Parsing;
using BibSettings;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Data;
using System.Diagnostics;
using System.Numerics;

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

        private string filter = string.Empty;
        private bool checkClipboard;
        private StatusEnum status;
        private int progress = 100;
        private string step;

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

        public StatusEnum Status
        {
            get => this.status;
            private set
            {
                if (this.status != value)
                {
                    this.status = value;
                    ShowStatus(this.status);
                }
            }
        }

        private void ShowStatus(StatusEnum value)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<StatusEnum>(ShowStatus), value);
            }
            else
            {
                lblStatus.Text = value.GetDescription();
                lblStatus.ForeColor = value.GetStatusColor();
                Application.DoEvents();
            }
        }

        public string Step
        {
            get => this.step;
            private set
            {
                if (!string.Equals(this.step, value))
                {
                    this.step = value;
                    ShowStep(this.step);
                }
            }
        }

        private void ShowStep(string value)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(ShowStep), value);
            }
            else
            {
                lblStep.Text = value;
                Application.DoEvents();
            }
        }

        public int Progress
        {
            get => this.progress;
            set
            {
                value = value < 0 ? 0 : value > 100 ? 100 : value;
                
                if (this.progress != value)
                {
                    this.progress = value;
                    ShowProgress(this.progress);
                }
            }
        }

        private void ShowProgress(int value)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<int>(ShowProgress),value);
            }
            else
            {
                progressBar.Value = value;
                progressBar.Visible = value < 100;
            }
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
                UpdateCount();
            }
        }

        private void UpdateCount()
        { 
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action(UpdateCount));
                return;
            }
            validEntriesStatus.Text = $"Resultados: {validEntriesView.Count}";
            invalidEntriesStatus.Text = $"Resultados: {invalidEntriesView.Count}";
            undefinedEntriesStatus.Text = $"Resultados: {undefinedEntriesView.Count}";
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
                    BrowseFiles(this);
                }
            }
        }

        private void BindDataSources()
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action(BindDataSources));
                return;
            }
            validEntriesGrid.DataSource = validEntriesView;
            invalidEntriesGrid.DataSource = invalidEntriesView;
            undefinedEntriesGrid.DataSource = undefinedEntriesView;
            UpdateCount();
        }

        private void UnbindDataSources()
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action(UnbindDataSources));
                return;
            }
            validEntriesGrid.DataSource = null;
            invalidEntriesGrid.DataSource = null;
            undefinedEntriesGrid.DataSource = null;
            validEntriesTable.Rows.Clear();
            invalidEntriesTable.Rows.Clear();
            undefinedEntriesTable.Rows.Clear();
            UpdateCount();

        }

        private void AppendValidEntry(BibElement element)
        {
            AppendEntry(element, validEntriesTable);
        }

        private void AppendInvalidEntry(BibElement element)
        {
            AppendEntry(element, invalidEntriesTable);
        }

        private void AppendUndefined(BibElement element)
        {
            AppendEntry(element, undefinedEntriesTable);
        }

        private void AppendEntry(BibElement element, DataTable table)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<BibElement, DataTable>(AppendEntry), element, table);
                return;
            }
            adapter.AppendEntry(element, table);
        }

        private static void BrowseFiles(Form1 sourceForm)
        {
            Task.Run(() => {
                try
                {
                    sourceForm.ShorError("");
                    sourceForm.Status = StatusEnum.SearchingFiles;
                    sourceForm.Step = "";
                    sourceForm.Progress = 100;

                    string currentFile = null;
                    List<string> directories = [LastPath];
                    directories.AddRange(Directory.GetDirectories(LastPath));
                    BibElements entries = new();
                    BibElements pendingEntries = new();

                    sourceForm.UnbindDataSources();


                    sourceForm.Progress = 100;
                    sourceForm.Status = StatusEnum.ParsingFiles;

                    int stepCount = directories.Count;
                    int currentStep = 0;

                    foreach (var path in directories)
                    {
                        currentStep++;
                        sourceForm.Step = $"Arquivo {currentStep} de {stepCount}";
                        var files = Directory.GetFiles(path, "*.bib", SearchOption.AllDirectories);
                        int current = 0;
                        sourceForm.Progress = 0;
                        foreach (var file in files)
                        {
                            currentFile = file;
                            var data = File.ReadAllText(file);
                            var newEntries = BibConverter<BibElement, BibElements, BibElementAdapter>.Deserialize(data, (value) => { sourceForm.Progress = value; });
                            entries.AddRange(newEntries);
                            current++;
                        }
                    }
                    sourceForm.Step = "";
                    sourceForm.Progress = 100;
                    sourceForm.Status = StatusEnum.CountingPages;

                    entries.Sort((x, y) => string.Compare(x.GetValueOrDefault("Author"), y.GetValueOrDefault("Author")));

                    stepCount = entries.Count;
                    currentStep = 0;
                    foreach (var entry in entries)
                    {
                        currentStep++;
                        sourceForm.Progress = (int)((currentStep / (double)stepCount) * 100);
                        entry.Title = entry.GetValueOrDefault("title")?.Trim().TrimEnd('.', ' ');
                        entry.Authors = entry.GetValueOrDefault("author")?.Trim().TrimEnd('.', ' ');
                        entry.DOI = entry.GetValueOrDefault("doi")?.Trim().TrimEnd('.', ' ');

                        if (entry.TryGetValue("numpages", out string value) && int.TryParse(value, out int numpages))
                        {
                            entry.PageCount = numpages;
                            if (numpages >= 5)
                            {
                                sourceForm.AppendValidEntry(entry);
                            }
                            else
                            {
                                sourceForm.AppendInvalidEntry(entry);
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
                                        sourceForm.AppendValidEntry(entry);
                                    }
                                    else
                                    {
                                        sourceForm.AppendInvalidEntry(entry);
                                    }
                                }
                                else
                                {
                                    entry.PageCount = null;
                                    if (string.IsNullOrEmpty(entry.DOI))
                                    {
                                        sourceForm.AppendUndefined(entry);
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
                                    sourceForm.AppendUndefined(entry);
                                }
                                else
                                {
                                    pendingEntries.Add(entry);
                                }
                            }
                        }
                    }

                    sourceForm.Step = "";
                    sourceForm.Progress = 100;

                    if (pendingEntries.Count > 0)
                    {
                        stepCount = pendingEntries.Count;
                        currentStep = 0;
                        sourceForm.Progress = 0;

                        sourceForm.Status = StatusEnum.SearchingPageInfoOnline;
                        var newTask = Task.Run(async () =>
                        {
                            using (var client = new HttpClient())
                            {
                                foreach (var entry in pendingEntries)
                                {
                                    currentStep++;
                                    sourceForm.Progress = (int)((currentStep / (double)stepCount) * 100);
                                    var count = await DoiUtils.GetPageCountAsync(entry.DOI, client).ConfigureAwait(false);
                                    if (count.HasValue)
                                    {
                                        entry.PageCount = count.Value;
                                        if (count.Value >= 5)
                                        {
                                            sourceForm.AppendValidEntry(entry);
                                        }
                                        else
                                        {
                                            sourceForm.AppendInvalidEntry(entry);
                                        }
                                    }
                                    else
                                    {
                                        sourceForm.AppendUndefined(entry);
                                    }
                                }
                            }
                        });
                        newTask.Wait();
                    }
                    sourceForm.Step = "";
                    sourceForm.Progress = 100;
                    sourceForm.Status = StatusEnum.Done;

                    sourceForm.BindDataSources();
                }
                catch(Exception ex)
                {
                    sourceForm.Status = StatusEnum.Error;
                    sourceForm.Progress = 100;
                    sourceForm.Step = "";
                    sourceForm.ShorError($"{ex.Message}\r\n\r\n{ex.StackTrace}");
                }
            });
        }

        private void ShorError(string message)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(ShorError), message);
            }
            else
            {
                txtLastError.Text = message;
            }
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
            Save(validEntriesView.ToTable(), adapter);
        }

        private void cmdSaveInvalid_Click(object sender, EventArgs e)
        {
            Save(invalidEntriesView.ToTable(), adapter);
        }

        private void cmdSaveUndefined_Click(object sender, EventArgs e)
        {
            Save(undefinedEntriesView.ToTable(), adapter);
        }

        private static void Save(DataTable entries, BibTableAdapter adapter)
        {
            var dlg = new SaveFileDialog();
            dlg.Filter = "BibTeX files (*.bib)|*.bib";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var records = adapter.GetRecords(entries);
                Task.Run(() => {
                    try
                    {
                        File.WriteAllText(dlg.FileName, BibConverter.Serialize(records));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao salvar o arquivo: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                });
            }
        }

        private void CmdTestDownload_Click(object sender, EventArgs e)
        {
            var response = DoiUtils.FetchPdfAsync("10.1186/s13638-017-0957-5");
            response.Wait();
            var result = response.Result;
            //File.WriteAllBytes(result.FileName, result.Bytes);
            var pages = PdfUtils.GetPageCount(result.Bytes);
            Debugger.Break();
        }
    }
}
