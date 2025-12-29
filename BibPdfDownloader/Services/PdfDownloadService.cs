using BibLib.Daos;
using BibLib.DataModels.BibDownload;
using BibLib.Enums;
using BibLib.Extensions;
using BibLib.Parsing;
using BibLib.ServiceModels;
using BibLib.Utils;
using System.Text;

namespace BibPdfDownloader.Services
{
    public class PdfDownloadService: ServiceBase
    {
        private readonly string inputFile;

        public PdfDownloadService(string inputFile = "Indefinidos.bib")
        {
            this.inputFile = inputFile;
        }

        public override async Task<bool> RunAsync()
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
                return false;
            }

            ShowStatus(MessageTypeEnum.Warning, $"Carregando registros de histórico...");
            try
            {
                maps = BibDownloadMapDao.GetDictionary();
                notFoundMaps = BibNotFoundDao.GetNotDeletedDictionary ();
                ShowStatus($"Registros de arquivos baixados: {maps.Count}");
                ShowStatus($"Registros de arquivos não encontrados: {notFoundMaps.Count}");
            }
            catch (Exception ex)
            {
                ShowStatus(MessageTypeEnum.Error, "Erro ao carregar histórico de referências", ex.Message);
                return false;
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
                    element.Doi = element.TryGetValue("doi", out string doi) ? doi : null;
                    if (!(string.IsNullOrEmpty(element.Doi) || maps.ContainsKey(element.Doi) || notFoundMaps.ContainsKey(element.Doi)))
                    {
                        unknownReferences.Add(element);
                    }
                }

                ShowStatus($"Pendências restantes: {unknownReferences.Count}");
            }
            catch (Exception ex)
            {
                ShowStatus(MessageTypeEnum.Error, "Erro ao identificar referências com download pendente", ex.Message);
                return false;
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
                    if (!maps.ContainsKey(element.Doi))
                    {
                        try
                        {
                            var downloadData = await DoiUtils.FetchPdfAsync(element.Doi);
                            element.PageCount = downloadData == null ? -1 : PdfUtils.GetPageCount(downloadData.Bytes);

                            if (downloadData != null)
                            {
                                element["numpages"] = element.PageCount.ToString();
                                BibDownloadMap map = element;
                                map.FileName = downloadData.FileName;
                                maps.Add(element.Doi, map);
                                BibDownloadMapDao.Create(map);
                                File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "downloaded", downloadData.FileName), downloadData.Bytes);
                            }
                            else
                            {
                                BibNotFound map = element;
                                notFoundMaps.Add(element.Doi, map);
                                BibNotFoundDao.Create(map);
                                //ShowStatus(MessageTypeEnum.Warning, $"PDF indisponível: \"{element.DOI}\"");
                            }
                        }
                        catch (Exception ex)
                        {
                            ShowStatus(MessageTypeEnum.Error, $"Erro ao baixar documento \"{element.Doi}\"", ex.Message);
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowStatus(MessageTypeEnum.Error, "Erro ao baixar PDFs das referências pendentes", ex.Message);
                return false;
            }

            ShowStatus("Operação concluída.");
            return true;
        }

        public override string GetSumary()
        {
            var sb = new StringBuilder();
            ShowResultList(sb);
            //ShowFaillureList(sb);
            return sb.ToString();
        }

        private static void ShowResultList(StringBuilder sb)
        {
            var sbDownloaded = new StringBuilder();
            var sbNotDownloaded = new StringBuilder();

            var maps = BibDownloadMapDao.GetAll();
            var notFount = BibNotFoundDao.GetAll();
            sbDownloaded.AppendLine(BibPaeringExtensions.AsCsvTableHeader<BibDownloadMap>());
            sb.AppendLine(BibPaeringExtensions.AsTableHeader<BibDownloadMap>());
            foreach (var map in maps)
            {
                sb.AppendLine(map.AsTableRow());
                sbDownloaded.AppendLine(map.AsCsvRow());
            }
            sb.AppendLine(BibPaeringExtensions.AsTabFooter<BibDownloadMap>($"Total de registros: {maps.Count}"));
            File.WriteAllText("with_pdf.csv", sbDownloaded.ToString(), Encoding.UTF8);
            sb.AppendLine();
            sbNotDownloaded.AppendLine(BibPaeringExtensions.AsCsvTableHeader<BibNotFound>());
            sb.AppendLine(BibPaeringExtensions.AsTableHeader<BibNotFound>());
            foreach (var item in notFount)
            {
                sb.AppendLine(item.AsTableRow());
                sbNotDownloaded.AppendLine(item.AsCsvRow());
            }
            sb.AppendLine(BibPaeringExtensions.AsTabFooter<BibNotFound>($"PDFs indisponíveis: {notFount.Count}"));
            File.WriteAllText("without_pdf.csv", sbNotDownloaded.ToString(), Encoding.UTF8);
        }

        //private static void ShowFaillureList(StringBuilder sb)
        //{
        //    var maps = BibNotFoundDao.GetNotDeleted();
        //    var notFount = BibNotFoundDao.CountNotDeleted();
        //}
    }
}
