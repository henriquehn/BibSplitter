using BibLib.Daos;
using BibLib.DataModels;
using BibLib.Parsing;
using BibLib.Utils;
using System.Diagnostics;

namespace BibPdfDownloader.Services
{
    public class BibService
    {
        private readonly string inputFile;

        public event EventHandler<int> OnProgress;
        public event EventHandler<StatusMessage> OnStatus;

        public BibService(string inputFile = "Indefinidos.bib")
        {
            this.inputFile = inputFile;
        }

        public void Run()
        {
            RunAsync().Wait();
        }

        public async Task RunAsync()
        {
            BibElements elements;
            IDictionary<string, BibDownloadMap> maps;
            IDictionary<string, BibNotFound> notFoundMaps;
            var newMaps = new List<BibDownloadMap>();
            var unknownReferences = new BibElements();

            ShowStatus($"Carregando referências pendentes em \"{inputFile}\"...");
            try
            {
                elements = (BibElements)BibConverter.DeserializeFile(inputFile, ShowProgress);
                ShowStatus($"Referências encontradas: {elements.Count}");
            }
            catch (Exception ex)
            {
                ShowStatus(MessageTypeEnum.Error, "Erro ao carregar referências", ex.Message);
                return;
            }

            ShowStatus(MessageTypeEnum.Warning, $"Carregando registros de histórico...");
            try
            {
                maps = BibDownloadMapDao.GetDictionary();
                notFoundMaps = BibNotFoundDao.GetDictionary();
                ShowStatus($"Registros de arquivos baixados: {maps.Count}");
                ShowStatus($"Registros de arquivos não encontrados: {notFoundMaps.Count}");
            }
            catch (Exception ex)
            {
                ShowStatus(MessageTypeEnum.Error, "Erro ao carregar histórico de referências", ex.Message);
                return;
            }
            
            ShowStatus("Identificando referências com download pendente...");
            try
            {
                int count = elements.Count;
                int currentCount = 0;
                foreach (var element in elements)
                {
                    currentCount++;
                    ShowProgress(currentCount, count);
                    element.Title = element.TryGetValue("title", out string title) ? title : null;
                    element.DOI = element.TryGetValue("doi", out string doi) ? doi : null;
                    if (!(string.IsNullOrEmpty(element.DOI) || maps.ContainsKey(element.DOI) || notFoundMaps.ContainsKey(element.DOI)))
                    {
                        unknownReferences.Add(element);
                    }
                }

                ShowStatus($"Pendências restantes: {unknownReferences.Count}");
            }
            catch (Exception ex)
            {
                ShowStatus(MessageTypeEnum.Error, "Erro ao identificar referências com download pendente", ex.Message);
                return;
            }
            
            ShowStatus("Baixando referências pendentes...");
            try
            {
                int count = elements.Count;
                int currentCount = 0;
                foreach (var element in unknownReferences)
                {
                    currentCount++;
                    ShowProgress(currentCount, count);
                    if (!maps.ContainsKey(element.DOI))
                    {
                        try
                        {
                            var downloadData = await DoiUtils.FetchPdfAsync(element.DOI);
                            element.PageCount = downloadData == null ? -1 : PdfUtils.GetPageCount(downloadData.Bytes);

                            if (downloadData != null)
                            {
                                element["numpages"] = element.PageCount.ToString();
                                BibDownloadMap map = element;
                                map.FileName = downloadData.FileName;
                                maps.Add(element.DOI, map);
                                BibDownloadMapDao.Create(map);
                                File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "downloaded", downloadData.FileName), downloadData.Bytes);
                            }
                            else
                            {
                                BibNotFound map = element;
                                notFoundMaps.Add(element.DOI, map);
                                BibNotFoundDao.Create(map);
                                //ShowStatus(MessageTypeEnum.Warning, $"PDF indisponível: \"{element.DOI}\"");
                            }
                        }
                        catch (Exception ex)
                        {
                            ShowStatus(MessageTypeEnum.Error, $"Erro ao baixar documento \"{element.DOI}\"", ex.Message);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowStatus(MessageTypeEnum.Error, "Erro ao baixar PDFs das referências pendentes", ex.Message);
                return;
            }

            ShowStatus("Operação concluída.");
        }

        private void ShowProgress(int currentCount, int count)
        {
            ShowProgress((int)((currentCount / (double)count) * 100));
        }

        private void ShowProgress(int percent)
        {
            try
            {
                OnProgress?.Invoke(this, percent);
            }
            catch { }
        }

        private void ShowStatus(string message)
        {
            ShowStatus(MessageTypeEnum.Info, "", message);
        }

        private void ShowStatus(MessageTypeEnum messageType, string message)
        {
            ShowStatus(messageType, "", message);
        }

        private void ShowStatus(MessageTypeEnum messageType, string title, string message)
        {
            try
            {
                OnStatus?.Invoke(this, new StatusMessage(messageType, title, message));
            }
            catch { }
        }
    }
}
