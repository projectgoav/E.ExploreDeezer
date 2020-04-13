using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using E.Deezer;

using E.ExploreDeezer.Core.Util;
using E.ExploreDeezer.Core.ViewModels;
using E.ExploreDeezer.Core.Collections;

namespace E.ExploreDeezer.Core.Common
{
    internal struct OnGenreFilterChangedEventArgs
    {
        public OnGenreFilterChangedEventArgs(ulong genreId)
        {
            this.GenreId = genreId;
        }

        public ulong GenreId { get; }
    }

    internal delegate void OnGenreFilterChangedEventHandler(object sender, OnGenreFilterChangedEventArgs args);


    internal interface IGenreListDataController
    {
        EFetchState CurrentFetchState { get; }
        IObservableCollection<IGenreViewModel> TheList { get; }
        event FetchStateChangedEventHandler OnFetchStateChanged;

        Task RefreshGenreListAsync();
    }

    internal class GenreListDataController : IGenreListDataController
    {
        private readonly IDeezerSession session;
        private readonly UpdatableFetchState fetchState;
        private readonly ResetableCancellationTokenSource tokenSource;
        private readonly FixedSizeObservableCollection<IGenreViewModel> genreList;

        public GenreListDataController(IDeezerSession session)
        {
            this.session = session;

            this.fetchState = new UpdatableFetchState();
            this.tokenSource = new ResetableCancellationTokenSource();
            this.genreList = new FixedSizeObservableCollection<IGenreViewModel>();
        }


        // IGenreListDataController
        public EFetchState CurrentFetchState => this.fetchState.CurrentState;

        public IObservableCollection<IGenreViewModel> TheList => this.genreList;

        public event FetchStateChangedEventHandler OnFetchStateChanged
        {
            add => this.fetchState.OnFetchStateChanged += value;
            remove => this.fetchState.OnFetchStateChanged -= value;
        }



        public Task RefreshGenreListAsync()
        {
            if (this.genreList.Count > 0)
                return Task.CompletedTask;

            this.fetchState.SetLoading();

            return this.session.Genre.GetCommonGenre(this.tokenSource.Token)
                                        .ContinueWith(t =>
                                        {
                                            if (t.IsFaulted)
                                            {
                                                //TODO: Proper logging
                                                System.Diagnostics.Debug.WriteLine($"Failed to fetch genre list.\n{t.Exception.GetBaseException()}");
                                                this.fetchState.SetError();
                                                return;
                                            }

                                            this.genreList.SetContents(t.Result.Select(x => new GenreViewModel(x)));

                                            if (this.genreList.Count == 0)
                                            {
                                                this.fetchState.SetEmpty();
                                            }
                                            else
                                            {
                                                this.fetchState.SetAvailable();
                                            }

                                        }, this.tokenSource.Token, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
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
                this.genreList.ClearContents();

                this.tokenSource.Dispose();
                this.fetchState.Dispose();
            }
        }
    }

}
