using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using System.Threading;

namespace E.ExploreDeezer.Core.Mvvm
{
    public interface IViewModel : INotifyPropertyChanged
    {

    }


    internal abstract class ViewModelBase : IViewModel,
                                            IDisposable
    {
        private static CancellationToken PRE_CANCELLED_TOKEN = new CancellationToken(canceled: true);

        private readonly object lockObject;
        private readonly CancellationTokenSource cancellationTokenSource;


        public ViewModelBase(IPlatformServices platformServices)
        {
            this.PlatformServices = platformServices;

            this.lockObject = new object();
            this.cancellationTokenSource = new CancellationTokenSource();
        }

        // ViewModelBase
        protected IPlatformServices PlatformServices { get; }

        protected CancellationToken CancellationToken => this.cancellationTokenSource.IsCancellationRequested ? PRE_CANCELLED_TOKEN
                                                                                                              : this.cancellationTokenSource.Token;


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
                if (!this.CancellationToken.IsCancellationRequested)
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
