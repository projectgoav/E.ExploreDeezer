using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using E.Deezer;

using E.ExploreDeezer.Core.Util;

namespace E.ExploreDeezer.Core.Services
{
    // Fetch Status
    public enum EFetchState
    {
        Loading,
        Available,
        Empty,
        Error,
    }

    internal struct FetchStateChangedEventArgs
    {
        public FetchStateChangedEventArgs(EFetchState oldValue,
                                           EFetchState newValue)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }

        public EFetchState OldValue { get; }
        public EFetchState NewValue { get; }
    }


    internal delegate void FetchStateChangedEventHandler(object sender, FetchStateChangedEventArgs e);


    internal class UpdatableFetchState : IDisposable
    {
        public UpdatableFetchState(EFetchState initialStatus = EFetchState.Loading)
        {
            this.CurrentState = new EFetchState();
        }


        public EFetchState CurrentState { get; private set; }

        // Event
        public event FetchStateChangedEventHandler OnFetchStateChanged
        {
            add
            {
                OnFetchStateChangedInternal += value;
                value(this, new FetchStateChangedEventArgs(this.CurrentState, this.CurrentState));
            }
            remove => OnFetchStateChangedInternal -= value;
        }


        // Internal event. We create a wrapper so that anyone whom registers an event handler is instantly called
        // with the most up-to-date value
        private event FetchStateChangedEventHandler OnFetchStateChangedInternal;


        // Helpers
        public void SetLoading()
            => MoveToState(EFetchState.Loading);

        public void SetError()
            => MoveToState(EFetchState.Error);

        public void SetEmpty()
            => MoveToState(EFetchState.Empty);

        public void SetAvailable()
            => MoveToState(EFetchState.Available);


        private void MoveToState(EFetchState incomingState)
        {
            if (this.CurrentState == incomingState)
                return;

            EFetchState oldState = this.CurrentState;
            EFetchState newState = incomingState;

            this.CurrentState = incomingState;

            this.OnFetchStateChangedInternal?.Invoke(this, new FetchStateChangedEventArgs(oldState, newState));
        }



        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Assert.That(this.OnFetchStateChangedInternal == null, "Dangling event handlers on fetch status.");
            }
        }
    }



    internal interface IDataFetchingService : IDisposable
    {
        string Id { get; }
        EFetchState FetchState { get; }

        event FetchStateChangedEventHandler OnFetchStateChanged;
    }
}
