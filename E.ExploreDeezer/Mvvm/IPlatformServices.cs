using System;
using System.Collections.Generic;
using System.Text;

using System.Threading.Tasks;

namespace E.ExploreDeezer.Mvvm
{
    internal interface IPlatformServices
    {
        IMainThreadDispatcher MainThreadDispatcher { get; }
    }


    internal interface IMainThreadDispatcher
    {
        void ExecuteOnMainThread(Action action);
        Task ExecuteOnMainThreadAsync(Action action);
    }


}
