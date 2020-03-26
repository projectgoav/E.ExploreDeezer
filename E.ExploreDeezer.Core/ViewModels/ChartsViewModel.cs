using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using E.Deezer;
using E.ExploreDeezer.Core.Mvvm;

namespace E.ExploreDeezer.Core.ViewModels
{
    public interface IChartsViewModel : INotifyPropertyChanged,
                                        IDisposable
    {
        EContentFetchStatus AlbumsFetchStatus { get; }
        EContentFetchStatus ArtistFetchStatus { get; }
        EContentFetchStatus PlaylistsFetchStatus { get; }
        EContentFetchStatus TracksFetchStatus { get; }

        IEnumerable<IAlbumViewModel> Albums { get; }
        IEnumerable<IArtistViewModel> Artists { get; }
        IEnumerable<IPlaylistViewModel> Playlists { get; }
        IEnumerable<ITrackViewModel> Tracks { get; }


        TracklistViewModelParams GetTracklistViewModelParams(IAlbumViewModel albumViewModel);
        TracklistViewModelParams GetTracklistViewModelParams(IPlaylistViewModel playlistViewModel);

        ArtistOverviewViewModelParams GetArtistOverviewViewModelParams(IArtistViewModel artistViewModel);
    }

    internal class ChartsViewModel : ViewModelBase,
                                     IChartsViewModel,
                                     IDisposable
    {
        private const uint MAX_ITEM_COUNT = 50;


        private readonly IDeezerSession session;

        private EContentFetchStatus albumsStatus;
        private EContentFetchStatus artistStatus;
        private EContentFetchStatus playlistStatus;
        private EContentFetchStatus tracksStatus;

        private IEnumerable<IAlbumViewModel> albums;
        private IEnumerable<IArtistViewModel> artists;
        private IEnumerable<IPlaylistViewModel> playlists;
        private IEnumerable<ITrackViewModel> tracks;

        public ChartsViewModel(IDeezerSession session,
                               IPlatformServices platformServices)
            : base(platformServices)
        {
            this.session = session;

            FetchContents();
        }



        // IChartViewModel
        public EContentFetchStatus AlbumsFetchStatus
        {
            get => this.albumsStatus;
            private set => SetProperty(ref this.albumsStatus, value);
        }

        public EContentFetchStatus ArtistFetchStatus
        {
            get => this.artistStatus;
            private set => SetProperty(ref this.artistStatus, value);
        }

        public EContentFetchStatus PlaylistsFetchStatus
        {
            get => this.playlistStatus;
            private set => SetProperty(ref this.playlistStatus, value);
        }

        public EContentFetchStatus TracksFetchStatus
        {
            get => this.tracksStatus;
            private set => SetProperty(ref this.tracksStatus, value);
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


        public TracklistViewModelParams GetTracklistViewModelParams(IAlbumViewModel albumViewModel)
            => ViewModelParamFactory.CreateTracklistViewModelParams(albumViewModel);

        public TracklistViewModelParams GetTracklistViewModelParams(IPlaylistViewModel playlistViewModel)
            => ViewModelParamFactory.CreateTracklistViewModelParams(playlistViewModel);

        public ArtistOverviewViewModelParams GetArtistOverviewViewModelParams(IArtistViewModel artistViewModel)
            => ViewModelParamFactory.CreateArtistOverviewViewModelParams(artistViewModel);



        private void FetchContents()
        {
            this.AlbumsFetchStatus = EContentFetchStatus.Loading;
            this.ArtistFetchStatus = EContentFetchStatus.Loading;

            this.session.Charts.GetCharts(this.CancellationToken, 0, MAX_ITEM_COUNT)
                              .ContinueWith(t =>
                              {
                                  if (t.IsFaulted || t.IsCanceled)
                                  {
                                      this.Albums = Array.Empty<IAlbumViewModel>();
                                      this.AlbumsFetchStatus = EContentFetchStatus.Error;
                                  }
                                  else
                                  {
                                      var chart = t.Result;

                                      var albums = chart.Albums.Select(x => new AlbumViewModel(x))
                                                               .ToList();

                                      var artists = chart.Artists.Select(x => new ArtistViewModel(x))
                                                                 .ToList();

                                      var playlists = chart.Playlists.Select(x => new PlaylistViewModel(x))
                                                                     .ToList();

                                      var tracks = chart.Tracks.Select(x => new TrackViewModel(x))
                                                               .ToList();

                                      this.Albums = albums;
                                      this.Artists = artists;
                                      this.Playlists = playlists;
                                      this.Tracks = tracks;

                                      this.AlbumsFetchStatus = albums.Count == 0 ? EContentFetchStatus.Empty
                                                                                 : EContentFetchStatus.Available;

                                      this.ArtistFetchStatus = artists.Count == 0 ? EContentFetchStatus.Empty
                                                                                  : EContentFetchStatus.Available;

                                      this.PlaylistsFetchStatus = playlists.Count == 0 ? EContentFetchStatus.Empty
                                                                                      : EContentFetchStatus.Available;

                                      this.TracksFetchStatus = tracks.Count == 0 ? EContentFetchStatus.Empty
                                                                                 : EContentFetchStatus.Available;
                                  }
                              },
                              this.CancellationToken,
                              TaskContinuationOptions.ExecuteSynchronously,
                              TaskScheduler.Default);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Albums = Array.Empty<IAlbumViewModel>();
                this.Artists = Array.Empty<IArtistViewModel>();
                this.Playlists = Array.Empty<IPlaylistViewModel>();
                this.Tracks = Array.Empty<ITrackViewModel>();
            }

            base.Dispose(disposing);
        }

    }
}
