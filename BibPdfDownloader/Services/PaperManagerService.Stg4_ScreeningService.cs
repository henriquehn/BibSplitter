using BibLib.ServiceModels;

namespace BibPdfDownloader.Services
{
    public partial class PaperManagerService
    {
        private class Stg4_ScreeningService : ServiceBase
        {
            public Stg4_ScreeningService(string serviceName, string serviceLabel) : base(serviceName, serviceLabel)
            {
            }

            public override async Task<bool> RunAsync()
            {
                return true;
            }
        }
    }


}
