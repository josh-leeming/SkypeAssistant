using System;
using System.Windows.Forms;
using SkypeAssistant.App.Properties;
using SkypeAssistant.Client.Interfaces;
using TinyIoC;

namespace SkypeAssistant.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = TinyIoCContainer.Current;
            IoC.ConfigureContainer(container);

            var skypeClient = container.Resolve<ISkypeClient>();
            container.BuildUp(skypeClient);

            skypeClient.StartClient();

            using (var icon = new ProcessIcon(skypeClient))
            {
                icon.Display();

                Application.Run();
            }
        }

        public class ProcessIcon : IDisposable
        {
            private readonly ISkypeClient _skypeClient;
            private readonly NotifyIcon notifyIcon;

            public ProcessIcon(ISkypeClient skypeClient)
            {
                this._skypeClient = skypeClient;
                this.notifyIcon = new NotifyIcon();
            }

            public void Display()
            {
                notifyIcon.Icon = Resources.TrayIcon_Running;
                notifyIcon.Text = Resources.TrayIcon_Running_Text;
                notifyIcon.Visible = true;

                notifyIcon.ContextMenuStrip = new ContextMenuStrip();

                var start = new ToolStripMenuItem("Start") { Enabled = false };
                var stop = new ToolStripMenuItem("Stop") { Enabled = true };
                var exit = new ToolStripMenuItem("Exit");

                start.Click += (sender, args) =>
                {
                    if (_skypeClient.IsRunning == false)
                    {
                        _skypeClient.StartClient();
                        notifyIcon.Icon = Resources.TrayIcon_Running;
                        notifyIcon.Text = Resources.TrayIcon_Running_Text;
                        start.Enabled = false;
                        stop.Enabled = true;
                    }
                };

                stop.Click += (sender, args) =>
                {
                    if (_skypeClient.IsRunning)
                    {
                        _skypeClient.StopClient();
                        notifyIcon.Icon = Resources.TrayIcon_Stopped;
                        notifyIcon.Text = Resources.TrayIcon_Stopped_Text;
                        start.Enabled = true;
                        stop.Enabled = false;
                    }
                };

                exit.Click += (sender, args) =>
                {
                    if (_skypeClient.IsRunning)
                    {
                        _skypeClient.StopClient();
                    }
                    Application.Exit();
                };

                notifyIcon.ContextMenuStrip.Items.Add(start);
                notifyIcon.ContextMenuStrip.Items.Add(stop);
                notifyIcon.ContextMenuStrip.Items.Add(exit);
            }

            public void Dispose()
            {
                notifyIcon.Dispose();
            }
        }
    }
}
