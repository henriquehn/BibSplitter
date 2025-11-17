using BibLib.Daos;
using BibLib.DataModels;
using BibLib.Parsing;
using BibLib.Utils;

namespace BibPdfDownloader
{
    internal class Program
    {
        private static readonly object consoleLock = new();
        private static int lastPercent = -1;

        static void Main(string[] args)
        {
            const string InputFile = "Indefinidos.bib";
            var maps = BibDownloadMapDao.GetAll();

            //var filename = ConfigurationHelper.Get("MainDataSourceName");
            //var configPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), InputFile));
            //Console.WriteLine($"Caminho = {configPath}");

            var elements = (BibElements)BibConverter.DeserializeFile(InputFile, ShowProgress);
            Thread.Sleep(500);
            Console.WriteLine();
            Console.WriteLine($"Resultados encontrados: {elements.Count}");
            Console.WriteLine($"Mapeamentos encontrados: {maps.Count}");
            Console.WriteLine("Pressione alguma tecla para continuar...");
            Console.ReadKey();
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
