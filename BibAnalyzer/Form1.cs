using BibSettings;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace BibAnalyzer
{
    public partial class Form1 : Form
    {
        private static readonly string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
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
            throw new NotImplementedException();
        }
    }
}
