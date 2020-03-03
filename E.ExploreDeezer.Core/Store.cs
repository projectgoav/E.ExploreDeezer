﻿using System;
using System.Collections.Generic;
using System.Text;

using System.Reactive.Linq;

namespace E.ExploreDeezer.Core
{
    public interface IAction { }

    internal delegate TState Reducer<TState>(TState state, IAction action);

    // Returns true if action has been consumed by middleware,
    // false if 
    internal delegate bool Middleware(IAction action);


    public delegate void OnActionDispatchedHandler(IAction action);
    public delegate void OnStateChangedHandler<TState>(TState newState);

    public interface IStore<TState> : IObservable<TState>
    {
        TState CurrentState { get; }

        void Dispatch(IAction action);

        event OnActionDispatchedHandler OnActionDispatched;
        event OnStateChangedHandler<TState> OnStateChanged;
    }

    internal class Store<TState> : IStore<TState>
    {
        private readonly Reducer<TState> reducer;
        private readonly object lockObject;
        private readonly IObservable<TState> storeObservable;
        private readonly IEnumerable<Middleware> middlewares;


        public Store(TState initialState, Reducer<TState> reducer, IEnumerable<Middleware> middlewares)
        {
            this.reducer = reducer;
            this.middlewares = middlewares;

            this.CurrentState = initialState;

            this.lockObject = new object();
            this.storeObservable = Observable.FromEventPattern<OnStateChangedHandler<TState>, TState>(h => this.OnStateChanged += h,
                                                                                                      h => this.OnStateChanged -= h)
                                             .Select(x => x.EventArgs);
        }


        // IStore
        public TState CurrentState { get; private set; }

        public void Dispatch(IAction action)
        {
            OnActionDispatched?.Invoke(action);

            if (!ProcessMiddleware(action))
            {
                UpdateState(action);
            }
        }


        // IObserable
        public IDisposable Subscribe(IObserver<TState> observer)
            => this.storeObservable.Subscribe(observer);


        public event OnActionDispatchedHandler OnActionDispatched;

        public event OnStateChangedHandler<TState> OnStateChanged;


        private bool ProcessMiddleware(IAction action)
        {
            foreach(var m in this.middlewares)
            {
                if (m(action))
                    return true;
            }

            return false;
        }

        private void UpdateState(IAction action)
        {
            lock(this.lockObject)
            {
                this.CurrentState = this.reducer(this.CurrentState, action);
            }

            // Execute outside the lock to allow progress to continue
            OnStateChanged?.Invoke(this.CurrentState);
        }
    }
}
