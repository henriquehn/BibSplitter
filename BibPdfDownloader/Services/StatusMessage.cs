namespace BibPdfDownloader.Services
{
    public class StatusMessage
    {
        public MessageTypeEnum MessageType { get; set; } = MessageTypeEnum.Info;
        public string Message { get; }
        public string Title { get; }
        public StatusMessage(MessageTypeEnum messageType, string title, string message)
        {
            MessageType = messageType;
            Title = title;
            Message = message;
        }
    }
}