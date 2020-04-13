using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using E.Deezer;

using E.ExploreDeezer.Core.Mvvm;
using E.ExploreDeezer.Core.Common;

namespace E.ExploreDeezer.Core.ViewModels
{
    public interface ISearchViewModel
    {
        string CurrentQuery { get; }

        IEnumerable<IAlbumViewModel> Albums { get; }
        IEnumerable<IArtistViewModel> Artists { get; }
        IEnumerable<IPlaylistViewModel> Playlists { get; }
        IEnumerable<ITrackViewModel> Tracks { get; }

        void SetQuery(string query);

        TracklistViewModelParams CreateTracklistViewModelParams(IAlbumViewModel albumViewModel);
        TracklistViewModelParams CreateTracklistViewModelParams(IPlaylistViewModel playlistViewModel);

        ArtistOverviewViewModelParams CreateArtistOverviewViewModelParams(IArtistViewModel artistViewModel);
    }

    internal class SearchViewModel : ViewModelBase,
                                     ISearchViewModel
    {
        private const uint kMaxResults = 50;

        private readonly IDeezerSession session;

        private string currentQuery;
        private IEnumerable<IAlbumViewModel> albums;
        private IEnumerable<IArtistViewModel> artists;
        private IEnumerable<IPlaylistViewModel> playlists;
        private IEnumerable<ITrackViewModel> tracks;
        private CancellationTokenSource searchTokenSource;

        public SearchViewModel(IDeezerSession session,
                               IPlatformServices platformServices)
            : base(platformServices)
        {
            this.session = session;

            this.albums = Array.Empty<IAlbumViewModel>();
            this.artists = Array.Empty<IArtistViewModel>();
            this.playlists = Array.Empty<IPlaylistViewModel>();
            this.tracks = Array.Empty<ITrackViewModel>();
        }


        // ISearchViewModel
        public string CurrentQuery
        {
            get => this.currentQuery;
            private set => SetProperty(ref this.currentQuery, value);
        }


        public IEnumerable<IAlbumViewModel> Albums
        {
            get => this.albums;
            private set => SetProperty(ref this.albums, value);
        }

        public IEnumerable<IArtistViewModel> Artists
        {
            get => this.artists;
            private set => SetProperty(ref this.artists, value);
        }

        public IEnumerable<IPlaylistViewModel> Playlists
        {
            get => this.playlists;
            private set => SetProperty(ref this.playlists, value);
        }

        public IEnumerable<ITrackViewModel> Tracks
        {
            get => this.tracks;
            private set => SetProperty(ref this.tracks, value);
        }


        public void SetQuery(string query)
        {
            if (this.CurrentQuery != query)
            {
                this.CurrentQuery = query;
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                UpdateResults();
            }
        }


        public TracklistViewModelParams CreateTracklistViewModelParams(IAlbumViewModel albumViewModel)
            => ViewModelParamFactory.CreateTracklistViewModelParams(albumViewModel);

        public TracklistViewModelParams CreateTracklistViewModelParams(IPlaylistViewModel playlistViewModel)
            => ViewModelParamFactory.CreateTracklistViewModelParams(playlistViewModel);

        public ArtistOverviewViewModelParams CreateArtistOverviewViewModelParams(IArtistViewModel artistViewModel)
            => ViewModelParamFactory.CreateArtistOverviewViewModelParams(artistViewModel);


        private void UpdateResults()
        {
            if (this.searchTokenSource != null)
            {
                this.searchTokenSource.Cancel();
                this.searchTokenSource.Dispose();
                this.searchTokenSource = null;
            }

            this.searchTokenSource = new CancellationTokenSource();

            this.session.Search.FindAlbums(this.CurrentQuery, this.searchTokenSource.Token, 0, kMaxResults)
                               .ContinueWith(t =>
                               {
                                   if (t.IsFaulted)
                                       return; //TODO

                                   this.Albums = t.Result.Select(x => new AlbumViewModel(x))
                                                         .ToList();

                               }, this.searchTokenSource.Token, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

            this.session.Search.FindArtists(this.CurrentQuery, this.searchTokenSource.Token, 0, kMaxResults)
                               .ContinueWith(t =>
                               {
                                   if (t.IsFaulted)
                                       return; //TODO

                                   this.Artists = t.Result.Select(x => new ArtistViewModel(x))
                                                          .ToList();

                               }, this.searchTokenSource.Token, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

            this.session.Search.FindPlaylists(this.CurrentQuery, this.searchTokenSource.Token, 0, kMaxResults)
                               .ContinueWith(t =>
                               {
                                   if (t.IsFaulted)
                                       return; //TODO

                                   this.Playlists = t.Result.Select(x => new PlaylistViewModel(x))
                                                            .ToList();

                               }, this.searchTokenSource.Token, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

            this.session.Search.FindTracks(this.CurrentQuery, this.searchTokenSource.Token, 0, kMaxResults)
                               .ContinueWith(t =>
                               {
                                   if (t.IsFaulted)
                                       return; //TODO

                                   this.Tracks = t.Result.Select(x => new TrackViewModel(x))
                                                         .ToList();

                               }, this.searchTokenSource.Token, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.searchTokenSource != null)
                {
                    this.searchTokenSource.Cancel();
                    this.searchTokenSource.Dispose();
                    this.searchTokenSource = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}
