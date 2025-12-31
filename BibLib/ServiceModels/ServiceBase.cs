using BibLib.Collections;
using BibLib.DataModels;
using BibLib.Enums;
using BibLib.Interfaces;

namespace BibLib.ServiceModels
{
    public abstract partial class ServiceBase : IService
    {
        public event EventHandler<int> OnProgress;
        public event EventHandler<StatusMessage> OnStatus;
        /// <summary>
        /// Mantém um estado compartilhado entre todos os serviços.
        /// </summary>
        protected static ThreadSafeStateMachine SharedState { get; } = new();
        /// <summary>
        /// Mantém um estado local para cada instância do serviço.
        /// </summary>
        protected ThreadSafeStateMachine LocalState { get; } = new();
        public string Name { get; }
        public string Description { get; }

        public ServiceBase(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }
        public ServiceBase()
        {
            this.Name = this.GetType().Name;
            this.Description = this.Name;
        }

        public bool Run()
        {
            return RunAsync().Result;
        }

        public abstract Task<bool> RunAsync();

        public virtual string GetSumary()
        {
            return "";
        }

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

        protected void RaiseProgress(object sender, int percent)
        {
            try
            {
                OnProgress?.Invoke(sender, percent);
            }
            catch { }
        }

        protected void RaiseStatus(object sender, StatusMessage status)
        {
            try
            {
                OnStatus?.Invoke(sender, status);
            }
            catch { }
        }
    }
}
