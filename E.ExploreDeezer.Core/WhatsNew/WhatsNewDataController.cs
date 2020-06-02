using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using E.Deezer;
using E.Deezer.Api;

using E.ExploreDeezer.Core.Util;
using E.ExploreDeezer.Core.Common;
using E.ExploreDeezer.Core.ViewModels;
using E.ExploreDeezer.Core.Collections;
using E.ExploreDeezer.Core.Extensions;

namespace E.ExploreDeezer.Core.WhatsNew
{
    internal interface IWhatsNewDataController
    {
        IObservableCollection<IAlbumViewModel> NewReleases { get; }
        event FetchStateChangedEventHandler OnNewReleaseFetchStateChanged;

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
        public IObservableCollection<IAlbumViewModel> NewReleases => this.newReleases;

        public event FetchStateChangedEventHandler OnNewReleaseFetchStateChanged
        {
            add => this.newReleaseFetchState.OnFetchStateChanged += value;
            remove => this.newReleaseFetchState.OnFetchStateChanged -= value;
        }


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
                                                                                .ContinueWhenNotCancelled<IEnumerable<IAlbum>, IEnumerable<IAlbumViewModel>>(t =>
                                                                                {
                                                                                    (bool faulted, Exception ex) = t.CheckIfFailed();

                                                                                    if (faulted)
                                                                                    {
                                                                                        this.newReleaseFetchState.SetError();
                                                                                        System.Diagnostics.Debug.WriteLine($"Failed to fetch new releases. {ex}");
                                                                                        return null;
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

                                                                                }, ct));

            this.deezerPicks.SetFetcher((start, count, ct) => this.session.Genre.GetDeezerSelectionForGenre(this.CurrentGenreFilter, ct, (uint)start, (uint)count)
                                                                                .ContinueWhenNotCancelled<IEnumerable<IAlbum>, IEnumerable<IAlbumViewModel>>(t =>
                                                                                {
                                                                                    (bool faulted, Exception ex) = t.CheckIfFailed();

                                                                                    if (faulted)
                                                                                    {
                                                                                        this.deezerPicksFetchState.SetError();
                                                                                        System.Diagnostics.Debug.WriteLine($"Failed to fetch deezer picks. {ex}");
                                                                                        return null;
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

                                                                                }, ct));
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

                this.newReleases.Dispose();
                this.deezerPicks.Dispose();

                this.newReleaseFetchState.Dispose();
                this.deezerPicksFetchState.Dispose();
            }
        }
    }
}
