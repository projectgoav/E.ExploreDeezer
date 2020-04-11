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


namespace E.ExploreDeezer.Core.Charts
{
    internal interface IChartsDataController
    {
        EFetchState AlbumChartFetchState { get; }
        IObservableCollection<IAlbumViewModel> AlbumChart { get; }
        event FetchStateChangedEventHandler OnAlbumChartFetchStateChanged;

        EFetchState ArtistChartFetchState { get; }
        IObservableCollection<IArtistViewModel> ArtistChart { get; }
        event FetchStateChangedEventHandler OnArtistChartFetchStateChanged;

        EFetchState PlaylistChartFetchState { get; }
        IObservableCollection<IPlaylistViewModel> PlaylistChart { get; }
        event FetchStateChangedEventHandler OnPlaylistChartFetchStateChanged;

        EFetchState TrackChartFetchState { get; }
        IObservableCollection<ITrackViewModel> TrackChart { get; }
        event FetchStateChangedEventHandler OnTrackChartFetchStateChanged;

        ulong CurrentGenreFilter { get; }
        event OnGenreFilterChangedEventHandler OnGenreFilterChanged;

        void SetGenreFilter(ulong genreId);
        void BeginPopulateAsync();
    }


    internal class ChartsDataController : IChartsDataController,
                                          IDisposable
    {
        private const ulong DEFAULT_GENRE_ID = 0;

        private readonly IDeezerSession session;
        private readonly ResetableCancellationTokenSource tokenSource;

        private readonly UpdatableFetchState albumFetchState;
        private readonly UpdatableFetchState trackFetchState;
        private readonly UpdatableFetchState artistFetchState;
        private readonly UpdatableFetchState playlistFetchState;

        private readonly PagedObservableCollection<IAlbumViewModel> albums;
        private readonly PagedObservableCollection<ITrackViewModel> tracks;
        private readonly PagedObservableCollection<IArtistViewModel> artists;
        private readonly PagedObservableCollection<IPlaylistViewModel> playlists;



        public ChartsDataController(IDeezerSession session)
        {
            this.session = session;

            this.CurrentGenreFilter = ulong.MaxValue;
            this.tokenSource = new ResetableCancellationTokenSource();

            this.albumFetchState = new UpdatableFetchState();
            this.trackFetchState = new UpdatableFetchState();
            this.artistFetchState = new UpdatableFetchState();
            this.playlistFetchState = new UpdatableFetchState();

            this.albums = new PagedObservableCollection<IAlbumViewModel>();
            this.tracks = new PagedObservableCollection<ITrackViewModel>();
            this.artists = new PagedObservableCollection<IArtistViewModel>();
            this.playlists = new PagedObservableCollection<IPlaylistViewModel>();
        }


        // IChartsDataController
        public EFetchState AlbumChartFetchState => this.albumFetchState.CurrentState;

        public IObservableCollection<IAlbumViewModel> AlbumChart => this.albums;

        public event FetchStateChangedEventHandler OnAlbumChartFetchStateChanged
        {
            add => this.albumFetchState.OnFetchStateChanged += value;
            remove => this.albumFetchState.OnFetchStateChanged -= value;
        }


        public EFetchState ArtistChartFetchState => this.artistFetchState.CurrentState;

        public IObservableCollection<IArtistViewModel> ArtistChart => this.artists;

        public event FetchStateChangedEventHandler OnArtistChartFetchStateChanged
        {
            add => this.artistFetchState.OnFetchStateChanged += value;
            remove => this.artistFetchState.OnFetchStateChanged -= value;
        }


        public EFetchState PlaylistChartFetchState => this.playlistFetchState.CurrentState;

        public IObservableCollection<IPlaylistViewModel> PlaylistChart => this.playlists;

        public event FetchStateChangedEventHandler OnPlaylistChartFetchStateChanged
        {
            add => this.playlistFetchState.OnFetchStateChanged += value;
            remove => this.playlistFetchState.OnFetchStateChanged -= value;
        }


        public EFetchState TrackChartFetchState => this.trackFetchState.CurrentState;

        public IObservableCollection<ITrackViewModel> TrackChart => this.tracks;

        public event FetchStateChangedEventHandler OnTrackChartFetchStateChanged
        {
            add => this.trackFetchState.OnFetchStateChanged += value;
            remove => this.trackFetchState.OnFetchStateChanged -= value;
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

            this.albumFetchState.SetLoading();
            this.trackFetchState.SetLoading();
            this.artistFetchState.SetLoading();
            this.playlistFetchState.SetLoading();

            this.CurrentGenreFilter = genreId;
            this.OnGenreFilterChanged?.Invoke(this, new OnGenreFilterChangedEventArgs(this.CurrentGenreFilter));


            this.albums.SetFetcher((start, count, ct) => this.session.Charts.GetAlbumChartForGenre(this.CurrentGenreFilter, ct, (uint)start, (uint)count)
                                                                            .ContinueWith<IEnumerable<IAlbumViewModel>>(t =>
                                                                            {
                                                                                if (t.IsFaulted)
                                                                                {
                                                                                    this.albumFetchState.SetError();

                                                                                    var ex = t.Exception.GetBaseException();
                                                                                    System.Diagnostics.Debug.WriteLine($"Failed to fetch album chart. {ex}");

                                                                                    throw ex;
                                                                                }

                                                                                var items = t.Result.Select(x => new AlbumViewModel(x));

                                                                                bool hasContents = this.albums.Count > 0 || items.Any();
                                                                                if (hasContents)
                                                                                {
                                                                                    this.albumFetchState.SetAvailable();
                                                                                }
                                                                                else
                                                                                {
                                                                                    this.albumFetchState.SetEmpty();
                                                                                }

                                                                                return items;

                                                                            }, ct, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default));


            this.artists.SetFetcher((start, count, ct) => this.session.Charts.GetArtistChartForGenre(this.CurrentGenreFilter, ct, (uint)start, (uint)count)
                                                                            .ContinueWith<IEnumerable<IArtistViewModel>>(t =>
                                                                            {
                                                                                if (t.IsFaulted)
                                                                                {
                                                                                    this.artistFetchState.SetError();
 
                                                                                    var ex = t.Exception.GetBaseException();
                                                                                    System.Diagnostics.Debug.WriteLine($"Failed to fetch artists chart. {ex}");

                                                                                    throw ex;
                                                                                }

                                                                                var items = t.Result.Select(x => new ArtistViewModel(x));

                                                                                bool hasContents = this.artists.Count > 0 || items.Any();
                                                                                if (hasContents)
                                                                                {
                                                                                    this.artistFetchState.SetAvailable();
                                                                                }
                                                                                else
                                                                                {
                                                                                    this.artistFetchState.SetEmpty();
                                                                                }
                                                                                 
                                                                                return items;

                                                                           }, ct, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default));


            this.tracks.SetFetcher((start, count, ct) => this.session.Charts.GetTrackChartForGenre(this.CurrentGenreFilter, ct, (uint)start, (uint)count)
                                                                            .ContinueWith<IEnumerable<ITrackViewModel>>(t =>
                                                                            {
                                                                                if (t.IsFaulted)
                                                                                {
                                                                                    this.trackFetchState.SetError();

                                                                                    var ex = t.Exception.GetBaseException();
                                                                                    System.Diagnostics.Debug.WriteLine($"Failed to fetch tracks chart. {ex}");

                                                                                    throw ex;
                                                                                }

                                                                                var items = t.Result.Select(x => new TrackViewModel(x));

                                                                                bool hasContents = this.tracks.Count > 0 || items.Any();
                                                                                if (hasContents)
                                                                                {
                                                                                    this.trackFetchState.SetAvailable();
                                                                                }
                                                                                else
                                                                                {
                                                                                    this.trackFetchState.SetEmpty();
                                                                                }

                                                                                return items;

                                                                           }, ct, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default));


            this.playlists.SetFetcher((start, count, ct) => this.session.Charts.GetPlaylistChartForGenre(this.CurrentGenreFilter, ct, (uint)start, (uint)count)
                                                                           .ContinueWith<IEnumerable<IPlaylistViewModel>>(t =>
                                                                           {
                                                                               if (t.IsFaulted)
                                                                               {
                                                                                   this.playlistFetchState.SetError();

                                                                                   var ex = t.Exception.GetBaseException();
                                                                                   System.Diagnostics.Debug.WriteLine($"Failed to fetch playlists chart. {ex}");

                                                                                   throw ex;
                                                                               }

                                                                               var items = t.Result.Select(x => new PlaylistViewModel(x));

                                                                               bool hasContents = this.playlists.Count > 0 || items.Any();
                                                                               if (hasContents)
                                                                               {
                                                                                   this.playlistFetchState.SetAvailable();
                                                                               }
                                                                               else
                                                                               {
                                                                                   this.playlistFetchState.SetEmpty();
                                                                               }

                                                                               return items;

                                                                           }, ct, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default));
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

            }
        }
    }
}
