using System;
using System.Collections.Generic;
using System.Text;

using System.Threading.Tasks;

namespace E.ExploreDeezer.Core.Mvvm
{
    public interface IPlatformServices
    {
        ISecureStorage SecureStorage { get; }
        IMainThreadDispatcher MainThreadDispatcher { get; }
    }


    public interface IMainThreadDispatcher
    {
        void ExecuteOnMainThread(Action action);
        Task ExecuteOnMainThreadAsync(Action action);
    }

    public interface ISecureStorage
    {
        Task<string> ReadValue(string key);
        Task<bool> WriteValue(string key, string value);
    }
}
