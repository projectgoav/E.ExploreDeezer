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

namespace E.ExploreDeezer.Core.Charts
{
    internal interface IChartsDataController
    {
        IObservableCollection<IAlbumViewModel> AlbumChart { get; }
        event FetchStateChangedEventHandler OnAlbumChartFetchStateChanged;

        IObservableCollection<IArtistViewModel> ArtistChart { get; }
        event FetchStateChangedEventHandler OnArtistChartFetchStateChanged;

        IObservableCollection<IPlaylistViewModel> PlaylistChart { get; }
        event FetchStateChangedEventHandler OnPlaylistChartFetchStateChanged;

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
        public IObservableCollection<IAlbumViewModel> AlbumChart => this.albums;

        public event FetchStateChangedEventHandler OnAlbumChartFetchStateChanged
        {
            add => this.albumFetchState.OnFetchStateChanged += value;
            remove => this.albumFetchState.OnFetchStateChanged -= value;
        }


        public IObservableCollection<IArtistViewModel> ArtistChart => this.artists;

        public event FetchStateChangedEventHandler OnArtistChartFetchStateChanged
        {
            add => this.artistFetchState.OnFetchStateChanged += value;
            remove => this.artistFetchState.OnFetchStateChanged -= value;
        }


        public IObservableCollection<IPlaylistViewModel> PlaylistChart => this.playlists;

        public event FetchStateChangedEventHandler OnPlaylistChartFetchStateChanged
        {
            add => this.playlistFetchState.OnFetchStateChanged += value;
            remove => this.playlistFetchState.OnFetchStateChanged -= value;
        }


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
                                                                            .ContinueWhenNotCancelled<IEnumerable<IAlbum>, IEnumerable<IAlbumViewModel>>(t =>
                                                                            {
                                                                                (bool faulted, Exception ex) = t.CheckIfFailed();

                                                                                if (faulted)
                                                                                { 
                                                                                    this.albumFetchState.SetError();
                                                                                    System.Diagnostics.Debug.WriteLine($"Failed to fetch album chart. {ex}");
                                                                                    return null;
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

                                                                            }, ct));


            this.artists.SetFetcher((start, count, ct) => this.session.Charts.GetArtistChartForGenre(this.CurrentGenreFilter, ct, (uint)start, (uint)count)
                                                                             .ContinueWhenNotCancelled<IEnumerable<IArtist>, IEnumerable<IArtistViewModel>>(t =>
                                                                             {
                                                                                 (bool faulted, Exception ex) = t.CheckIfFailed();

                                                                                 if (faulted)
                                                                                 {
                                                                                     this.artistFetchState.SetError();
                                                                                     System.Diagnostics.Debug.WriteLine($"Failed to fetch artist chart. {ex}");
                                                                                     return null;
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

                                                                           }, ct));


            this.tracks.SetFetcher((start, count, ct) => this.session.Charts.GetTrackChartForGenre(this.CurrentGenreFilter, ct, (uint)start, (uint)count)
                                                                            .ContinueWhenNotCancelled<IEnumerable<ITrack>, IEnumerable<ITrackViewModel>>(t =>
                                                                            {
                                                                                (bool faulted, Exception ex) = t.CheckIfFailed();

                                                                                if (faulted)
                                                                                {
                                                                                    this.trackFetchState.SetError();
                                                                                    System.Diagnostics.Debug.WriteLine($"Failed to fetch track chart. {ex}");
                                                                                    return null;
                                                                                }

                                                                                var items = t.Result.Select(x => new TrackViewModel(x, 
                                                                                                                                    ETrackLHSMode.Artwork, 
                                                                                                                                    ETrackArtistMode.NameWithLink));

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

                                                                           }, ct));


            this.playlists.SetFetcher((start, count, ct) => this.session.Charts.GetPlaylistChartForGenre(this.CurrentGenreFilter, ct, (uint)start, (uint)count)
                                                                           .ContinueWhenNotCancelled<IEnumerable<IPlaylist>, IEnumerable<IPlaylistViewModel>>(t =>
                                                                           {
                                                                               (bool faulted, Exception ex) = t.CheckIfFailed();

                                                                               if (faulted)
                                                                               {
                                                                                   this.playlistFetchState.SetError();
                                                                                   System.Diagnostics.Debug.WriteLine($"Failed to fetch playlist chart. {ex}");
                                                                                   return null;
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

                                                                           }, ct));
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
