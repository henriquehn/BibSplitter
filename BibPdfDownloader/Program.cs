using BibLib.Daos;
using BibLib.DataModels;
using BibLib.Parsing;
using BibPdfDownloader.Services;
using System.Net.NetworkInformation;
using System.Xml.Linq;

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
            ShowResults();
        }

        private static void ShowResults()
        {
            var maps = BibDownloadMapDao.GetAll();
            var notFount = BibNotFoundDao.CountAll();
            Console.WriteLine(new string('-', 152));
            Console.WriteLine($"{"ID",5} | {"DOI",-28} | {"Arquivo",-88} | {"Paginas",7} | {"Criado em",-10}");
            Console.WriteLine(new string('-', 152));
            foreach (var map in maps)
            {
                Console.WriteLine($"{map.Id,5} | {map.Doi,-28} | {map.FileName,-88} | {map.PageCount,7} | {map.CreatedAt:dd/MM/yyyy}");
            }
            Console.WriteLine(new string('-', 152));
            Console.WriteLine($"Total de registros: {maps.Count}");
            Console.WriteLine($"PDFs indisponíveis: {notFount}");
            Console.WriteLine(new string('-', 152));
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
