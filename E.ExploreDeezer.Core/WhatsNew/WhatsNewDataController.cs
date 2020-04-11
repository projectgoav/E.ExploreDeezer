using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using E.Deezer;

using E.ExploreDeezer.Core.Util;
using E.ExploreDeezer.Core.Common;
using E.ExploreDeezer.Core.ViewModels;
using E.ExploreDeezer.Core.Collections;

namespace E.ExploreDeezer.Core.WhatsNew
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

    internal interface IWhatsNewDataController
    {
        EFetchState NewReleaseFetchState { get; }
        IObservableCollection<IAlbumViewModel> NewReleases { get; }
        event FetchStateChangedEventHandler OnNewReleaseFetchStateChanged;

        EFetchState DeezerPicksFetchState { get; }
        IObservableCollection<IAlbumViewModel> DeezerPicks { get; }
        event FetchStateChangedEventHandler OnDeezerPicksFetchStateChanged;

        ulong CurrentGenreFilter { get; }
        event OnGenreFilterChangedEventHandler OnGenreFilterChanged;

        void SetGenreFilter(ulong genreId);
        void BeginPopulateAsync();
    }
    
    
    internal class WhatsNewDataController : IWhatsNewDataController,
                                            IDisposable
    {
        private const ulong DEFAULT_GENRE_ID = 0;

        private readonly IDeezerSession session;
        private readonly UpdatableFetchState newReleaseFetchState;
        private readonly UpdatableFetchState deezerPicksFetchState;
        private readonly ResetableCancellationTokenSource tokenSource;
        private readonly PagedObservableCollection<IAlbumViewModel> newReleases;
        private readonly PagedObservableCollection<IAlbumViewModel> deezerPicks;


        public WhatsNewDataController(IDeezerSession session)
        {
            this.session = session;

            this.CurrentGenreFilter = ulong.MaxValue;
            this.tokenSource = new ResetableCancellationTokenSource();

            this.newReleaseFetchState = new UpdatableFetchState();
            this.deezerPicksFetchState = new UpdatableFetchState();

            this.newReleases = new PagedObservableCollection<IAlbumViewModel>();
            this.deezerPicks = new PagedObservableCollection<IAlbumViewModel>();
        }


        // IWhatsNewDataController
        public EFetchState NewReleaseFetchState => this.newReleaseFetchState.CurrentState;

        public IObservableCollection<IAlbumViewModel> NewReleases => this.newReleases;

        public event FetchStateChangedEventHandler OnNewReleaseFetchStateChanged
        {
            add => this.newReleaseFetchState.OnFetchStateChanged += value;
            remove => this.newReleaseFetchState.OnFetchStateChanged -= value;
        }


        public EFetchState DeezerPicksFetchState => this.deezerPicksFetchState.CurrentState;

        public IObservableCollection<IAlbumViewModel> DeezerPicks => this.deezerPicks;

        public event FetchStateChangedEventHandler OnDeezerPicksFetchStateChanged
        {
            add => this.deezerPicksFetchState.OnFetchStateChanged += value;
            remove => this.deezerPicksFetchState.OnFetchStateChanged -= value;
        }


        public ulong CurrentGenreFilter { get; private set; }

        public event OnGenreFilterChangedEventHandler OnGenreFilterChanged;


        public void BeginPopulateAsync()
        {
            if (this.CurrentGenreFilter == ulong.MaxValue)
            {
                SetGenreFilter(DEFAULT_GENRE_ID);
            }
        }

        public void SetGenreFilter(ulong genreId)
        {
            if (this.CurrentGenreFilter == genreId)
                return;

            this.tokenSource.Reset();

            this.newReleaseFetchState.SetLoading();
            this.deezerPicksFetchState.SetLoading();

            this.CurrentGenreFilter = genreId;
            this.OnGenreFilterChanged?.Invoke(this, new OnGenreFilterChangedEventArgs(this.CurrentGenreFilter));


            this.newReleases.SetFetcher((start, count, ct) => this.session.Genre.GetNewReleasesForGenre(this.CurrentGenreFilter, ct, (uint)start, (uint)count)
                                                                                .ContinueWith<IEnumerable<IAlbumViewModel>>(t =>
                                                                                {
                                                                                    if (t.IsFaulted)
                                                                                    {
                                                                                        this.newReleaseFetchState.SetError();
                                                                                        //TODO: Log
                                                                                        throw t.Exception.GetBaseException();
                                                                                    }

                                                                                    var items = t.Result.Select(x => new AlbumViewModel(x));

                                                                                    bool hasContents = this.newReleases.Count > 0 || items.Any();
                                                                                    if (hasContents)
                                                                                    {
                                                                                        this.newReleaseFetchState.SetAvailable();
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        this.newReleaseFetchState.SetEmpty();
                                                                                    }

                                                                                    return items;

                                                                                }, ct, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default));

            this.deezerPicks.SetFetcher((start, count, ct) => this.session.Genre.GetDeezerSelectionForGenre(this.CurrentGenreFilter, ct, (uint)start, (uint)count)
                                                                                .ContinueWith<IEnumerable<IAlbumViewModel>>(t =>
                                                                                {
                                                                                    if (t.IsFaulted)
                                                                                    {
                                                                                        this.deezerPicksFetchState.SetError();
                                                                                        //TODO: Log
                                                                                        throw t.Exception.GetBaseException();
                                                                                    }

                                                                                    var items = t.Result.Select(x => new AlbumViewModel(x));

                                                                                    bool hasContents = this.deezerPicks.Count > 0 || items.Any();
                                                                                    if (hasContents)
                                                                                    {
                                                                                        this.deezerPicksFetchState.SetAvailable();
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        this.deezerPicksFetchState.SetEmpty();
                                                                                    }

                                                                                    return items;

                                                                                }, ct, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default));
        }



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
                this.tokenSource.Dispose();

                this.newReleaseFetchState.Dispose();
                this.deezerPicksFetchState.Dispose();
            }
        }
    }
}
