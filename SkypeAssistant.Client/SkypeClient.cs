using System;
using System.Collections.Generic;
using System.Linq;
using SKYPE4COMLib;
using SkypeAssistant.Client.Interfaces;

namespace SkypeAssistant.Client
{
    public class SkypeClient : ISkypeClient
    {
        #region Dependencies

        public ILogHandler Logger { get; set; }

        public Skype Skype { get; private set; }

        public List<IClientLifecycleCallbackHandler> ApplicationLifecycleCallbacks { get; set; }

        #endregion

        #region Props

        /// <summary>
        /// True if the client is running
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// True if the client has been initialised
        /// </summary>
        private bool isInitialised;

        #endregion

        #region Ctor

        public SkypeClient()
        {
            Skype = new Skype();
        } 

        #endregion

        #region Public Methods

        /// <summary>
        /// Start responding to Skype events
        /// </summary>
        public void StartClient()
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("Client is currently running");
            }

            IsRunning = true;

            if (isInitialised == false)
            {
                InitClient();
            }

            OnStartup();

            Logger.Info("Client started");
        }

        /// <summary>
        /// Reset Blynclight
        /// </summary>
        public void StopClient()
        {
            if (IsRunning == false)
            {
                throw new InvalidOperationException("Client is not running");
            }

            IsRunning = false;

            OnShutdown();

            Logger.Info("Client stopped");
        }

        #endregion

        #region Lifecycle

        protected void OnInitialise()
        {
            ApplicationLifecycleCallbacks.OrderByDescending(s => s.Priority)
                .ToList()
                .ForEach(handler => handler.OnInitialise(Skype));
        }

        protected void OnStartup()
        {
            ApplicationLifecycleCallbacks.OrderByDescending(s => s.Priority)
                .ToList()
                .ForEach(handler => handler.OnStartup());
        }

        protected void OnShutdown()
        {
            ApplicationLifecycleCallbacks.OrderByDescending(s => s.Priority)
                .ToList()
                .ForEach(handler => handler.OnShutdown());
        }

        #endregion

        #region Init

        protected void InitClient()
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Client initialising");
            }

            ((_ISkypeEvents_Event)Skype).AttachmentStatus += Skype_AttachmentStatus;

            //Attach async
            Skype.Attach(8, false);

            OnInitialise();

            isInitialised = true;

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Client initialised");
            }
        }

        #endregion

        #region Skype Callbacks

        private void Skype_AttachmentStatus(TAttachmentStatus Status)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Skype_AttachmentStatus " + Status);
            }

            if (Status == TAttachmentStatus.apiAttachSuccess)
            {
                Logger.Info("Skype Attached");
            }
            else if (Status == TAttachmentStatus.apiAttachAvailable)
            {
                Skype.Attach(8, false);
            }
            else if (Status == TAttachmentStatus.apiAttachNotAvailable)
            {
                Logger.Info("Skype Unavailable");
            }
        }

        #endregion

    }
}
