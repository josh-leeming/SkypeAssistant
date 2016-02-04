namespace SkypeAssistant.Client.Interfaces
{
    public interface ISkypeClient
    {
        bool IsRunning { get; }
        void StartClient();
        void StopClient();
    }
}