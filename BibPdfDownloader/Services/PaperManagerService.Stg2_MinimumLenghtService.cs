using BibLib.ServiceModels;

namespace BibPdfDownloader.Services
{
    public partial class PaperManagerService
    {
        private class Stg2_MinimumLenghtService : ServiceBase
        {
            public Stg2_MinimumLenghtService(string serviceName, string serviceLabel) : base(serviceName, serviceLabel)
            {
            }

            public override async Task<bool> RunAsync()
            {
                return true;
            }
        }
    }


}
