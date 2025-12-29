using BibLib.Daos;
using BibLib.DataModels.BibDownload;
using BibLib.Enums;
using BibLib.Extensions;
using BibLib.ServiceModels;
using BibLib.Utils;
using System.Text;

namespace BibPdfDownloader.Services
{
    public class AlternativePdfDownloadService : ServiceBase
    {
        public override async Task<bool> RunAsync()
        {
            IDictionary<string, BibDownloadMap> maps;
            IList<BibNotFound> elements;
            int newDownloads = 0;

            ShowStatus($"Carregando referências baixadas da base de dados...");
            try
            {
                maps = BibDownloadMapDao.GetDictionary();
                ShowStatus($"Referências baixadas: {maps.Count}");
            }
            catch (Exception ex)
            {
                ShowStatus(MessageTypeEnum.Error, "Erro ao carregar referências baixadas", ex.Message);
                return false;
            }

            ShowStatus($"Carregando referências pendentes da base de dados...");
            try
            {
                elements = BibNotFoundDao.GetNotDeleted();
                ShowStatus($"Referências pendentes: {elements.Count}");
            }
            catch (Exception ex)
            {
                ShowStatus(MessageTypeEnum.Error, "Erro ao carregar referências pendentes", ex.Message);
                return false;
            }

            ShowStatus("Baixando referências pendentes...");
            try
            {
                int count = elements.Count;
                int currentCount = 0;
                foreach (var element in elements)
                {
                    currentCount++;
                    ShowProgress(currentCount, count);
                    if (!maps.ContainsKey(element.Doi))
                    {
                        BibDownloadMap map = null;
                        try
                        {
                            var downloadData = DoiUtils.FetchAlternativePdf(element.Doi);
                            Thread.Sleep(DoiUtils.MilissecondsBetweenFiles);

                            if (downloadData != null)
                            {
                                map = element;
                                map.FileName = downloadData.FileName;
                                map.PageCount = downloadData == null ? -1 : PdfUtils.GetPageCount(downloadData.Bytes) ?? -1;
                                maps.Add(element.Doi, map);
                                BibDownloadMapDao.Create(map);
                                element.Deleted = true;
                                BibNotFoundDao.Replace(element);
                                File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "downloaded", downloadData.FileName), downloadData.Bytes);
                                newDownloads++;
                            }
                        }
                        catch (Exception ex)
                        {
                            ShowStatus(MessageTypeEnum.Error, $"Erro ao baixar documento \"{element.Doi}\"", ex.Message);
                            return false;
                        }
                    }
                    else
                    {
                        element.Deleted = true;
                        BibNotFoundDao.Replace(element);
                    }
                }
                ShowStatus(MessageTypeEnum.Info, $"Total de novos downloads: {newDownloads}");
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
    }
}
