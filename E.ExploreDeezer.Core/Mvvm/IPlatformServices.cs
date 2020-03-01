using System;
using System.Collections.Generic;
using System.Text;

using System.Threading.Tasks;

namespace E.ExploreDeezer.Core.Mvvm
{
    public interface IPlatformServices
    {
        IMainThreadDispatcher MainThreadDispatcher { get; }
    }


    public interface IMainThreadDispatcher
    {
        void ExecuteOnMainThread(Action action);
        Task ExecuteOnMainThreadAsync(Action action);
    }


}
