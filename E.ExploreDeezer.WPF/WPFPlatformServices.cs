using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;


using E.ExploreDeezer.Core.Mvvm;

namespace E.ExploreDeezer.WPF
{
    internal class WPFPlatformServices : IPlatformServices,
                                         IMainThreadDispatcher
    {
        private readonly Dispatcher dispatcher;


        public WPFPlatformServices(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }



        // IPlatformServices
        public IMainThreadDispatcher MainThreadDispatcher => this;

        public IPresenter Presenter { get; } = null;


        // IMainThreadDispatcher
        public void ExecuteOnMainThread(Action action)
            => this.dispatcher.Invoke(action);

        public Task ExecuteOnMainThreadAsync(Action action)
            => this.dispatcher.BeginInvoke(action)
                              .Task;
    }
}
