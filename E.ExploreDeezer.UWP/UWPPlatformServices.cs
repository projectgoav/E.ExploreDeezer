using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.UI.Core;
using Windows.ApplicationModel;
using Windows.Security.Cryptography.DataProtection;
using System.Runtime.InteropServices.WindowsRuntime;

using E.ExploreDeezer.Core.Mvvm;
using Windows.UI.Xaml.Controls.Primitives;

namespace E.ExploreDeezer.UWP
{
    internal class UWPPlatformServices : IPlatformServices,
                                         ISecureStorage,
                                         IMainThreadDispatcher
    {
        private readonly CoreDispatcher dispatcher;


        public UWPPlatformServices(CoreDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }



        // IPlatformServices
        public ISecureStorage SecureStorage => this;

        public IMainThreadDispatcher MainThreadDispatcher => this;

        public IPresenter Presenter { get; } = null;


        // IMainThreadDispatcher
        public void ExecuteOnMainThread(Action action)
        {
            _ = this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
        }


        public Task ExecuteOnMainThreadAsync(Action action)
            => this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action())
                              .AsTask();


        // ISecureStorage
        public Task<string> ReadValue(string key)
        {
            var settings = GetSettings();

            var storedValue = settings.Values[key] as byte[];
            if (storedValue == null)
            {
                return Task.FromResult<string>(null);
            }

            var provider = new DataProtectionProvider();

            return provider.UnprotectAsync(storedValue.AsBuffer())
                           .AsTask()
                           .ContinueWith(t => Encoding.UTF8.GetString(t.Result.ToArray()));

        }

        public Task<bool> WriteValue(string key, string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);

            var provider = new DataProtectionProvider("LOCAL=user");

            if (bytes.Length == 0)
            {
                var settings = GetSettings();
                settings.Values.Remove(key);

                return Task.FromResult(true);
            }
           
            return provider.ProtectAsync(bytes.AsBuffer())
                           .AsTask()
                           .ContinueWith(t =>
                           {
                               var settings = GetSettings();
                               settings.Values[key] = t.Result.ToArray();

                               return true;
                           });
        }

        private static ApplicationDataContainer GetSettings()
        {
            var localSettings = ApplicationData.Current.LocalSettings;

            if (!localSettings.Containers.ContainsKey(Package.Current.Id.Name))
            {
                localSettings.CreateContainer(Package.Current.Id.Name, ApplicationDataCreateDisposition.Always);
            }

            return localSettings.Containers[Package.Current.Id.Name];
        }
    }
}
