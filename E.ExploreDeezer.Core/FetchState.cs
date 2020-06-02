using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;


namespace E.ExploreDeezer.Core
{
    /* FetchState
     * 
     * Since the majority of collections require some sort of
     * network request to be made in order to populate them we
     * assign a number of states to this interaction. 
     * 
     * The 4 states we have:
     *  - Loading
     *      Network request has been made. Waiting for response,
     *      parsing and collections updates
     *  - Error
     *      Network request failed or something went wrong when
     *      parsing the response
     *  - Empty
     *      The API returned no more data
     *  - Available
     *      Request was successful, response parsed and collection
     *      now has some contents to display. */
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


    /* UpdatableFetchState
     * 
     * Wrapper class around a FetchStateChanged event to help
     * manage the transitions between each state. 
     * 
     * This call will also automatically fire the event for any
     * new subscriber to avoid ViewModels having to query the
     * value once subscribed to avoid missed changes. */
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


        // Internal event. We create a wrapper so that anyone who registers an event handler is instantly called
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
}
