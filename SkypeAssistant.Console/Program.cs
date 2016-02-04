using System;
using System.Runtime.InteropServices;
using SkypeAssistant.App;
using SkypeAssistant.Client.Interfaces;
using TinyIoC;

namespace Skype
{
    class Program
    {
        private static ISkypeClient skypeClient = null;

        private static ConsoleEventDelegate consoleEventCallbackHandler;   
        private delegate bool ConsoleEventDelegate(int eventType);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

        static void Main(string[] args)
        {
            Start(args);

            consoleEventCallbackHandler = ConsoleEventCallback;
            SetConsoleCtrlHandler(consoleEventCallbackHandler, true);

            System.Console.WriteLine(@"Press any key to stop...");
            System.Console.ReadKey(true);

            Stop();
        }

        private static void Start(string[] args)
        {
            var container = TinyIoCContainer.Current;
            try
            {
                IoC.ConfigureContainer(container);

                skypeClient = container.Resolve<ISkypeClient>();
                container.BuildUp(skypeClient);

                skypeClient.StartClient();
            }
            catch (Exception e)
            {
                if (skypeClient != null && skypeClient.IsRunning)
                {
                    skypeClient.StopClient();
                }
                System.Console.WriteLine(e.Message);
            }
        }

        private static void Stop()
        {
            try
            {
                skypeClient.StopClient();
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }

        private static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2)
            {
                Stop();
            }
            return false;
        }
    }
}
