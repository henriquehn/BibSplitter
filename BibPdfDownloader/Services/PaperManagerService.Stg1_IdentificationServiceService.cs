using BibLib.Adapters;
using BibLib.Daos;
using BibLib.DataModels.BibDownload;
using BibLib.DataModels.PaperManager;
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
                Duplicate.ResetSequence();
                foreach (var folder in folders)
                {
                    var dirInfo = new DirectoryInfo(folder);
                    var folderName = dirInfo.Name;
                    var files = Directory.GetFiles(folder, "*.bib");
                    ShowStatus($"Arquivos encontrados em {folderName}: {files.Length}");
                    int currentCount = 0;
                    int folderEntries = 0;
                    foreach (var file in files)
                    {
                        var filename = Path.GetFileName(file);
                        currentCount++;
                        ShowStatus($"Processando arquivo {filename}({currentCount} de {files.Length}) em {folderName}");
                        ShowProgress(0);
                        List<Duplicate> duplicates = new();
                        var data = File.ReadAllText(file);
                        var newEntries = BibConverter<BibElement, BibElements, BibElementAdapter>.Deserialize(
                            data,
                            (value) => { ShowProgress(value); },
                            folderName,
                            file,
                            (entry) => { 
                                return !PaperDao.PaperStore.ContainsKey(entry.Hash); 
                            }
                        );
                        var papers = PaperDao.Create(newEntries, duplicates);
                        Thread.Sleep(100);
                        ShowProgress(100);
                        ShowStatus($"Trabalhos extraídos do arquivo {filename} em {folderName}: {newEntries.Count}\r\n" +
                            $"Trabalhos duplicados no arquivo {filename} em {folderName}: {duplicates.Count}\r\n" +
                            $"Salvando duplicatas...");
                        DuplicateDao.Create(duplicates);
                        folderEntries += newEntries.Count;
                    }
                    ShowStatus($"Trabalhos extraídos da pasta {folderName}: {folderEntries}");
                }

                return true;
            }
        }
    }


}
