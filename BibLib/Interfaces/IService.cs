using BibLib.DataModels;

namespace BibLib.Interfaces
{
    public interface IService
    {
        event EventHandler<int> OnProgress;
        event EventHandler<StatusMessage> OnStatus;
        bool Run();
        Task<bool> RunAsync();
        string GetSumary();
    }
}
