using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using E.Deezer;
using E.ExploreDeezer.Core.Mvvm;
using E.ExploreDeezer.Core.Services;
using E.ExploreDeezer.Core.Collections;

namespace E.ExploreDeezer.Core.ViewModels
{
    public interface IChartsViewModel : INotifyPropertyChanged,
                                        IDisposable
    {
        IObservableCollection<IGenreViewModel> GenreList { get; }
        IObservableCollection<IAlbumViewModel> Albums { get; }
        IObservableCollection<IArtistViewModel> Artists { get; }
        IObservableCollection<IPlaylistViewModel> Playlists { get; }
        IObservableCollection<ITrackViewModel> Tracks { get; }

        void SetSelectedGenre(IGenreViewModel genre);


        TracklistViewModelParams GetTracklistViewModelParams(IAlbumViewModel albumViewModel);
        TracklistViewModelParams GetTracklistViewModelParams(IPlaylistViewModel playlistViewModel);

        ArtistOverviewViewModelParams GetArtistOverviewViewModelParams(IArtistViewModel artistViewModel);
    }

    internal class ChartsViewModel : ViewModelBase,
                                     IChartsViewModel,
                                     IDisposable
    {
        private const ulong DEFAULT_GENRE_ID = 0;

        private readonly IGenreListService genreService;
        private readonly ChartsDataController dataController;

        private readonly MainThreadObservableCollectionAdapter<IGenreViewModel> genreList;
        private readonly MainThreadObservableCollectionAdapter<IAlbumViewModel> albums;
        private readonly MainThreadObservableCollectionAdapter<IArtistViewModel> artists;
        private readonly MainThreadObservableCollectionAdapter<IPlaylistViewModel> playlists;
        private readonly MainThreadObservableCollectionAdapter<ITrackViewModel> tracks;

        public ChartsViewModel(IPlatformServices platformServices)
            : base(platformServices)
        {
            this.genreService = ServiceRegistry.GetService<IGenreListService>();

            this.genreList = new MainThreadObservableCollectionAdapter<IGenreViewModel>(genreService.GenreList,
                                                                                        PlatformServices.MainThreadDispatcher);

            this.dataController = ServiceRegistry.GetService<ChartsDataController>();

            this.albums = new MainThreadObservableCollectionAdapter<IAlbumViewModel>(dataController.Albums,
                                                                                     PlatformServices.MainThreadDispatcher);

            this.artists = new MainThreadObservableCollectionAdapter<IArtistViewModel>(dataController.Artists,
                                                                                       PlatformServices.MainThreadDispatcher);

            this.tracks = new MainThreadObservableCollectionAdapter<ITrackViewModel>(dataController.Tracks,
                                                                                     PlatformServices.MainThreadDispatcher);

            this.playlists = new MainThreadObservableCollectionAdapter<IPlaylistViewModel>(dataController.Playlists,
                                                                                           PlatformServices.MainThreadDispatcher);

            dataController.SetGenreId(DEFAULT_GENRE_ID);
        }



        // IChartViewModel
        public IObservableCollection<IGenreViewModel> GenreList => this.genreList;
        public IObservableCollection<IAlbumViewModel> Albums => this.albums;
        public IObservableCollection<IArtistViewModel> Artists => this.artists;
        public IObservableCollection<IPlaylistViewModel> Playlists => this.playlists;
        public IObservableCollection<ITrackViewModel> Tracks => this.tracks;


        public void SetSelectedGenre(IGenreViewModel genre)
        {
            Assert.That(genre != null);
            this.dataController.SetGenreId(genre.Id);
        }

        public TracklistViewModelParams GetTracklistViewModelParams(IAlbumViewModel albumViewModel)
            => ViewModelParamFactory.CreateTracklistViewModelParams(albumViewModel);

        public TracklistViewModelParams GetTracklistViewModelParams(IPlaylistViewModel playlistViewModel)
            => ViewModelParamFactory.CreateTracklistViewModelParams(playlistViewModel);

        public ArtistOverviewViewModelParams GetArtistOverviewViewModelParams(IArtistViewModel artistViewModel)
            => ViewModelParamFactory.CreateArtistOverviewViewModelParams(artistViewModel);



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.albums.Dispose();
                this.artists.Dispose();
                this.playlists.Dispose();
                this.tracks.Dispose();
                this.genreList.Dispose();
            }

            base.Dispose(disposing);
        }

    }
}
