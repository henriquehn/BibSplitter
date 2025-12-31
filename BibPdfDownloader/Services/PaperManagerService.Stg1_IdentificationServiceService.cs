using BibLib.Adapters;
using BibLib.Daos;
using BibLib.DataModels.BibDownload;
using BibLib.Parsing;
using BibLib.ServiceModels;

namespace BibPdfDownloader.Services
{
    public partial class PaperManagerService
    {
        private class Stg1_IdentificationServiceService : ServiceBase
        {

            public Stg1_IdentificationServiceService(string serviceName, string serviceLabel) : base(serviceName, serviceLabel)
            {
            }

            public override async Task<bool> RunAsync()
            {
                ShowStatus(@$"Pesquisando em ""{rootPath}""");
                var folders = Directory.GetDirectories(rootPath);
                ShowStatus(@$"Pastas encontradas: {folders.Length}");
                foreach (var folder in folders)
                {
                    var dirInfo = new DirectoryInfo(folder);
                    var folderName = dirInfo.Name;
                    var files = Directory.GetFiles(folder, "*.bib");
                    ShowStatus($"Arquivos encontrados em {folderName}: {files.Length}");
                    int currentCount = 0;
                    ShowProgress(0);
                    int folderEntries = 0;
                    foreach (var file in files)
                    {
                        currentCount++;
                        ShowStatus($"Processando arquivo {currentCount} de {files.Length} em {folderName}");
                        var data = File.ReadAllText(file);
                        var newEntries = BibConverter<BibElement, BibElements, BibElementAdapter>.Deserialize(
                            data,
                            (value) => { ShowProgress(value); },
                            folderName,
                            (entry) => { return true; }
                        );
                        var papers = PaperDao.Create(newEntries);
                        ShowProgress(100);
                        ShowStatus($"Trabalhos extraídos do arquivo {Path.GetFileName(file)} na pasta {folderName}: {newEntries.Count}");
                        folderEntries+= newEntries.Count;
                    }
                    ShowStatus($"Trabalhos extraídos da pasta {folderName}: {folderEntries}");
                }
                return true;
            }
        }
    }


}
