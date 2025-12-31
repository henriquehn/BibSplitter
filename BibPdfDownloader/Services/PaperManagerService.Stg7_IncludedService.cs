using BibLib.ServiceModels;

namespace BibPdfDownloader.Services
{
    public partial class PaperManagerService
    {
        private class Stg7_IncludedService : ServiceBase
        {
            public Stg7_IncludedService(string serviceName, string serviceLabel) : base(serviceName, serviceLabel)
            {
            }

            public override async Task<bool> RunAsync()
            {
                return true;
            }
        }
    }


}
