using BibLib.ServiceModels;

namespace BibPdfDownloader.Services
{
    public partial class PaperManagerService
    {
        private class Stg5_MergeService : ServiceBase
        {
            public Stg5_MergeService(string serviceName, string serviceLabel) : base(serviceName, serviceLabel)
            {
            }

            public override async Task<bool> RunAsync()
            {
                return true;
            }
        }
    }


}
