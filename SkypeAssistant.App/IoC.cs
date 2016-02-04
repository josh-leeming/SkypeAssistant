using System;
using System.Collections.Generic;
using SkypeAssistant.Client;
using SkypeAssistant.Client.Interfaces;
using TextToSpeech;
using TinyIoC;

namespace SkypeAssistant.App
{
    public static class IoC
    {
        public static void ConfigureContainer(TinyIoCContainer container)
        {
            container.Register(typeof(ISkypeClient), typeof(SkypeClient)).AsSingleton();
            container.Register(typeof(ILogHandler), typeof(Logger.Log4NetLogger)).AsSingleton();

            container.RegisterMultiple<IClientLifecycleCallbackHandler>(new List<Type>()
            {
                typeof (TextToSpeechBehaviour)
            });

        }
    }
}