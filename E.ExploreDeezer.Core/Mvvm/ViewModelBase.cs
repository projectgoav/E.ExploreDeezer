using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using System.Threading;
using E.ExploreDeezer.Core.Util;

namespace E.ExploreDeezer.Core.Mvvm
{
    /* ViewModels
     * 
     * A very basic ViewModel implementation. Provides the
     * INotifyPropertyChanged interface and will fire that
     * event from the UI thread. */
    public interface IViewModel : INotifyPropertyChanged
    { }


    public abstract class ViewModelBase : IViewModel,
                                          IDisposable
    {
        private static CancellationToken PRE_CANCELLED_TOKEN = new CancellationToken(canceled: true);

        private readonly object lockObject;
        private readonly ResetableCancellationTokenSource cancellationTokenSource;


        public ViewModelBase(IPlatformServices platformServices)
        {
            this.PlatformServices = platformServices;

            this.lockObject = new object();
            this.cancellationTokenSource = new ResetableCancellationTokenSource();
        }

        // ViewModelBase
        protected IPlatformServices PlatformServices { get; }


        public bool SetProperty<T>(ref T storage, T newValue, [CallerMemberName] string propertyName = null)
        {
            bool changed = storage == null || !storage.Equals(newValue);

            if (changed)
            {
                storage = newValue;
                RaisePropertyChangedSafe(propertyName);
            }

            return changed;
        }


        protected void RaisePropertyChangedSafe(string propertyName)
        {
            this.PlatformServices
                .MainThreadDispatcher
                .ExecuteOnMainThread(() =>
            {
                if (!this.cancellationTokenSource.Token.IsCancellationRequested)
                {
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
            });
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        // IDisposable
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        { 
            if (disposing)
            {
                this.cancellationTokenSource.Cancel();
                this.cancellationTokenSource.Dispose();
            }
        }
    }
}
