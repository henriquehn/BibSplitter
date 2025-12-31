using BibLib.DataModels;
using BibLib.Enums;
using BibLib.ServiceModels;
using BibLib.Utils;

namespace BibPdfDownloader.Services
{
    public partial class PaperManagerService : ServiceBase
    {
        private readonly List<ServiceBase> services = [
            new Stg1_IdentificationServiceService("F1","Coleta inicial"),
            new Stg2_MinimumLenghtService("F2.1","Número mínimo de 5 páginas"),
            new Stg3_DeduplicationService("F2.2","Remoção de duplicatas"),
            new Stg4_ScreeningService("F3","Triagem preliminar por palavras-chave"),
            new Stg5_MergeService("F4.1","Merge"),
            new Stg6_EligibilityService("F4.2","Aplicação de critérios de elegibilidade"),
            new Stg7_IncludedService("F5","Seleção final")
        ];
        private static readonly string rootPath = ConfigurationHelper.Get("RootPath");

        public PaperManagerService()
        {
            foreach (var service in services)
            {
                service.OnProgress += RaiseProgress;
                service.OnStatus += RaiseStatus;
            }
        }

        public override async Task<bool> RunAsync()
        {
            foreach (var service in services)
            {
                ShowStatus(@$"Iniciando serviço: ""{service.Name}: {service.Description}""");
                var result = await Run(service.Name, service);
                if (result)
                {
                    ShowStatus(@$"Serviço ""{service.Name}"" concluído com sucesso.");
                }
                else
                {
                    ShowStatus(MessageTypeEnum.Error, @$"O serviço ""{service.Name}"" falhou.", "O processo será interrompido.");
                    return false;
                }
            }
            return true;
        }

        private async Task<bool> Run(string serviceName, ServiceBase service)
        {
            try
            {
                return await service.RunAsync();
            }
            catch (Exception ex)
            {
                ShowStatus(MessageTypeEnum.Error, @$"O serviço ""{serviceName}"" encontrou um erro inesperado.", ex.Message);
                return false;
            }
        }
    }
}
