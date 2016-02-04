using SKYPE4COMLib;
using SkypeAssistant.Client.Models;

namespace SkypeAssistant.Client.Interfaces
{
    public interface IClientLifecycleCallbackHandler
    {
        Priority Priority { get; }

        void OnInitialise(Skype skype);
        void OnStartup();
        void OnShutdown();
    }
}
