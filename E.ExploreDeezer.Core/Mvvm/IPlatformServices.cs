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


    /* As it says, a way for things to be run on the UI thread */
    public interface IMainThreadDispatcher
    {
        void ExecuteOnMainThread(Action action);
        Task ExecuteOnMainThreadAsync(Action action);
    }

    /* A platform specific way to securely store values.
     * This is used to keep the user's OAuth token safe! */
    public interface ISecureStorage
    {
        Task<string> ReadValue(string key);
        Task<bool> WriteValue(string key, string value);
    }
}
