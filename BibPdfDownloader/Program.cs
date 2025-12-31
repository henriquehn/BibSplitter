using BibLib.DataModels;
using BibLib.Enums;
using BibLib.Interfaces;
using BibPdfDownloader.Services;

namespace BibPdfDownloader
{
    internal class Program
    {
        private static readonly object consoleLock = new();
        private static int lastPercent = -1;
        private static IService service;

        static Program()
        {
            //service = new PdfDownloadService();
            service = new PaperManagerService();
            //service = new AlternativePdfDownloadService();
            service.OnProgress += service_OnProgress;
            service.OnStatus += Service_OnStatus;
        }

        static async Task Main(string[] args)
        {
            if (await service.RunAsync())
            {
                lock (consoleLock)
                {
                    Console.Write(service.GetSumary());
                }
            }
            else
            {
                Console.WriteLine("Não foi possível processar as referências.");
            }
        }

        private static void service_OnProgress(object sender, int percent)
        {
            lock (consoleLock)
            {
                if (percent == lastPercent) return;
                lastPercent = percent;
                Console.Write($"\rProgresso: {percent}%   ");
            }
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
                Console.Write($"\r\n({status.MessageType}) {status.Title.Trim()}");
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine($" {status.Message}");
                lastPercent = -1;
            }
        }
    }
}
