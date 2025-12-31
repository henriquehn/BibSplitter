using BibLib.ServiceModels;

namespace BibPdfDownloader.Services
{
    public partial class PaperManagerService
    {
        private class Stg3_DeduplicationService : ServiceBase
        {
            public Stg3_DeduplicationService(string serviceName, string serviceLabel) : base(serviceName, serviceLabel)
            {
            }

            public override async Task<bool> RunAsync()
            {
                return true;
            }
        }
    }


}
