using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using E.ExploreDeezer.Mvvm;
using Windows.UI.Core;

namespace E.ExploreDeezer.UWP
{
    internal class UWPPlatformServices : IPlatformServices,
                                         IMainThreadDispatcher
    {
        private readonly CoreDispatcher dispatcher;


        public UWPPlatformServices(CoreDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }



        // IPlatformServices
        public IMainThreadDispatcher MainThreadDispatcher => this;


        // IMainThreadDispatcher
        public void ExecuteOnMainThread(Action action)
        {
            _ = this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
        }


        public Task ExecuteOnMainThreadAsync(Action action)
            => this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action())
                              .AsTask();


    }
}
