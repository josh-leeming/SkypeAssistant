using System;
using System.Speech.Synthesis;
using SkypeAssistant.Client.Extensions;
using SkypeAssistant.Client.Interfaces;
using SkypeAssistant.Client.Models;
using SKYPE4COMLib;

namespace TextToSpeech
{
    /// <summary>
    /// Announce Skype status and incoming callee
    /// </summary>
    public class TextToSpeechBehaviour : IClientLifecycleCallbackHandler
    {
        #region Dependencies
        public ILogHandler Logger { get; set; }
        #endregion

        #region Props
        /// <summary>
        /// True if the behaviour is running
        /// </summary>
        public bool IsRunning { get; private set; }

        private SpeechSynthesizer synthesizer;

        private Skype skypeHandle; 
        #endregion

        #region Ctor
        public TextToSpeechBehaviour(ILogHandler logger)
        {
            Logger = logger;
            Priority = Priority.Normal;
        } 
        #endregion

        #region IClientLifecycleCallbackHandler

        public Priority Priority { get; private set; }

        public void OnInitialise(Skype skype)
        {
            Logger.Debug("Initialised TextToSpeechBehaviour");

            skypeHandle = skype;

            synthesizer = new SpeechSynthesizer
            {
                Volume = 100,
                Rate = 1
            };
        }

        public void OnStartup()
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("TextToSpeechBehaviour is currently running");
            }

            IsRunning = true;
            Logger.Debug("Startup TextToSpeechBehaviour");

            skypeHandle.CallStatus += Skype_CallStatus;
            skypeHandle.UserStatus += Skype_UserStatus;
            skypeHandle.ContactsFocused += Skype_ContactsFocused;

            //if skype is already running then say the current status
            if (((ISkype)skypeHandle).AttachmentStatus == TAttachmentStatus.apiAttachSuccess)
            {
                var userStatus = skypeHandle.CurrentUserStatus.ToUserStatus();

                SpeakUserStatus(userStatus);
            }
        }

        public void OnShutdown()
        {
            if (IsRunning == false)
            {
                throw new InvalidOperationException("TextToSpeechBehaviour is currently stopped");
            }

            IsRunning = false;

            Logger.Debug("Shutdown TextToSpeechBehaviour");

            skypeHandle.CallStatus -= Skype_CallStatus;
            skypeHandle.UserStatus -= Skype_UserStatus;
            skypeHandle.ContactsFocused -= Skype_ContactsFocused;
        }

        #endregion

        #region Skype Callbacks

        private void Skype_CallStatus(Call pCall, TCallStatus status)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("TextToSpeechBehaviour Skype_CallStatus " + status);
            }

            if (status == TCallStatus.clsRinging)
            {
                synthesizer.SpeakAsync(string.Format("{0}", pCall.PartnerDisplayName)); 
            }
        }

        private void Skype_UserStatus(TUserStatus status)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("TextToSpeechBehaviour Skype_UserStatus " + status);
            }

            var userStatus = status.ToUserStatus();

            SpeakUserStatus(userStatus);
        }

        private void Skype_ContactsFocused(string username)
        {
            if (skypeHandle.ActiveCalls.Count > 0)
                return; 
            
            if (string.IsNullOrEmpty(username))
                return;

            var user = skypeHandle.User[username];
            var name = string.IsNullOrEmpty(user.DisplayName) ? user.FullName : user.DisplayName;
            var status = user.OnlineStatus.ToUserStatus();
            if (status != UserStatus.Online)
            {
                synthesizer.SpeakAsync(string.Format("{0} is {1}", name, status.ToString()));
            }
            else
            {
                synthesizer.SpeakAsync(name);
            }
        }

        #endregion

        #region Methods
        private void SpeakUserStatus(UserStatus status)
        {
            synthesizer.SpeakAsync("Skype status " + status);
        } 
        #endregion
    }
}
