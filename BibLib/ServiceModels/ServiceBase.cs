using BibLib.DataModels;
using BibLib.Enums;
using BibLib.Interfaces;

namespace BibLib.ServiceModels
{
    public abstract class ServiceBase : IService
    {
        public event EventHandler<int> OnProgress;
        public event EventHandler<StatusMessage> OnStatus;

        public void Run()
        {
            RunAsync().Wait();
        }

        public abstract Task<bool> RunAsync();
        public abstract string GetSumary();

        protected void ShowProgress(int currentCount, int count)
        {
            ShowProgress((int)((currentCount / (double)count) * 100));
        }

        protected void ShowProgress(int percent)
        {
            try
            {
                OnProgress?.Invoke(this, percent);
            }
            catch { }
        }

        protected void ShowStatus(string message)
        {
            ShowStatus(MessageTypeEnum.Info, "", message);
        }

        protected void ShowStatus(MessageTypeEnum messageType, string message)
        {
            ShowStatus(messageType, "", message);
        }

        protected void ShowStatus(MessageTypeEnum messageType, string title, string message)
        {
            try
            {
                OnStatus?.Invoke(this, new StatusMessage(messageType, title, message));
            }
            catch { }
        }
    }
}
