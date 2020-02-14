using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using System.Threading;

namespace E.ExploreDeezer.Mvvm
{
    internal abstract class ViewModelBase : INotifyPropertyChanged,
                                            IDisposable
    {
        private static CancellationToken PRE_CANCELLED_TOKEN = new CancellationToken(canceled: true);

        private readonly object lockObject;
        private readonly IPlatformServices platformServices;
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


        public void SetProperty<T>(ref T storage, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (!storage.Equals(newValue))
            {
                storage = newValue;
                RaisePropertyChangedSafe(propertyName);
            }
        }


        private void RaisePropertyChangedSafe(string propertyName)
        {
            this.platformServices.MainThreadDispatcher.ExecuteOnMainThread(() =>
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
