using System;
using System.Collections.Generic;
using System.Text;

using System.Threading.Tasks;

namespace E.ExploreDeezer.Core.Mvvm
{
    public interface IPlatformServices
    {
        IPresenter Presenter { get; }
        IMainThreadDispatcher MainThreadDispatcher { get; }
    }


    public interface IMainThreadDispatcher
    {
        void ExecuteOnMainThread(Action action);
        Task ExecuteOnMainThreadAsync(Action action);
    }


    public interface IPresenter
    {
        void ShowViewModel(IViewModel viewModelToShow);
    }
}
