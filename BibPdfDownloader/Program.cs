using BibLib.Daos;
using BibLib.DataModels;
using BibLib.Extensions;
using BibPdfDownloader.Services;
using System.Text;

namespace BibPdfDownloader
{
    internal class Program
    {
        private static readonly object consoleLock = new();
        private static int lastPercent = -1;
        private static BibService service;


        static Program()
        {
            service = new BibService();
            service.OnProgress += service_OnProgress;
            service.OnStatus += Service_OnStatus;
        }

        static void Main(string[] args)
        {
            service.Run();
            ShowResultList();
            ShowFaillureList();
        }

        private static void ShowResultList()
        {
            var sbDownloaded = new System.Text.StringBuilder();
            var sbNotDownloaded = new System.Text.StringBuilder();

            var maps = BibDownloadMapDao.GetAll();
            var notFount = BibNotFoundDao.GetAll();
            sbDownloaded.AppendLine(BibPaeringExtensions.AsCsvTableHeader<BibDownloadMap>());
            Console.WriteLine(BibPaeringExtensions.AsTableHeader<BibDownloadMap>());
            foreach (var map in maps)
            {
                Console.WriteLine(map.AsTableRow());
                sbDownloaded.AppendLine(map.AsCsvRow());
            }
            Console.WriteLine(BibPaeringExtensions.AsTabFooter<BibDownloadMap>($"Total de registros: {maps.Count}"));
            File.WriteAllText("with_pdf.csv", sbDownloaded.ToString(), Encoding.UTF8);
            Console.WriteLine();
            sbNotDownloaded.AppendLine(BibPaeringExtensions.AsCsvTableHeader<BibNotFound>());
            Console.WriteLine(BibPaeringExtensions.AsTableHeader<BibNotFound>());
            foreach (var item in notFount)
            {
                Console.WriteLine(item.AsTableRow());
                sbNotDownloaded.AppendLine(item.AsCsvRow());
            }
            Console.WriteLine(BibPaeringExtensions.AsTabFooter<BibNotFound>($"PDFs indisponíveis: {notFount.Count}"));
            File.WriteAllText("without_pdf.csv", sbNotDownloaded.ToString(), Encoding.UTF8);
        }

        private static void ShowFaillureList()
        {
            var maps = BibNotFoundDao.GetAll();
            var notFount = BibNotFoundDao.CountAll();
        }

        private static void service_OnProgress(object sender, int percent)
        {
            ShowProgress(percent);
        }

        private static void Service_OnStatus(object sender, StatusMessage status)
        {
            lock (consoleLock)
            {
                switch (status.MessageType)
                {
                    case MessageTypeEnum.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.BackgroundColor = ConsoleColor.Black;
                        break;
                    case MessageTypeEnum.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.BackgroundColor = ConsoleColor.Black;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                        break;
                }
                Console.WriteLine($"\r\n({status.MessageType}) {status.Title}");
                Console.ResetColor();
                Console.WriteLine($" {status.Message}");
                lastPercent = -1;
            }
        }

        private static void ShowProgress(int percent)
        {
            lock(consoleLock)
            {
                if (percent == lastPercent) return;
                lastPercent = percent;
                Console.Write($"\rProgresso: {percent}%   ");
            }
        }
    }
}
