using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using E.ExploreDeezer.Core.Mvvm;
using E.ExploreDeezer.Core.Common;
using E.ExploreDeezer.Core.ViewModels;
using E.ExploreDeezer.Core.Collections;


namespace E.ExploreDeezer.Core.Charts
{
    public interface IChartsViewModel : INotifyPropertyChanged,
                                        IDisposable
    {
        //EFetchState AlbumChartFetchState { get; }
        IObservableCollection<IAlbumViewModel> AlbumChart { get; }

        //EFetchState ArtistChartFetchState { get; }
        IObservableCollection<IArtistViewModel> ArtistChart { get; }

        //EFetchState TrackChartFetchState { get; }
        IObservableCollection<ITrackViewModel> TrackChart { get; }

        //EFetchState PlaylistFetchState { get; }
        IObservableCollection<IPlaylistViewModel> PlaylistChart { get; }

        //EFetchState GenreListFetchState { get; }
        IObservableCollection<IGenreViewModel> GenreList { get; }

        //int SelectedGenreIndex { get; set; }

        TracklistViewModelParams GetTracklistViewModelParams(IAlbumViewModel albumViewModel);
        TracklistViewModelParams GetTracklistViewModelParams(IPlaylistViewModel playlistViewModel);

        ArtistOverviewViewModelParams GetArtistOverviewViewModelParams(IArtistViewModel artistViewModel);
    }

    internal class ChartsViewModel : ViewModelBase,
                                     IChartsViewModel,
                                     IDisposable
    {
        private readonly IChartsDataController chartsDataController;
        private readonly IGenreListDataController genreListDataController;

        private readonly MainThreadObservableCollectionAdapter<IGenreViewModel> genreList;
        private readonly MainThreadObservableCollectionAdapter<IAlbumViewModel> albums;
        private readonly MainThreadObservableCollectionAdapter<IArtistViewModel> artists;
        private readonly MainThreadObservableCollectionAdapter<IPlaylistViewModel> playlists;
        private readonly MainThreadObservableCollectionAdapter<ITrackViewModel> tracks;

        public ChartsViewModel(IPlatformServices platformServices)
            : base(platformServices)
        {
            this.chartsDataController = ServiceRegistry.GetService<IChartsDataController>();
            this.genreListDataController = ServiceRegistry.GetService<IGenreListDataController>();


            this.genreList = new MainThreadObservableCollectionAdapter<IGenreViewModel>(this.genreListDataController.TheList,
                                                                                        PlatformServices.MainThreadDispatcher);



            this.albums = new MainThreadObservableCollectionAdapter<IAlbumViewModel>(this.chartsDataController.AlbumChart,
                                                                                     PlatformServices.MainThreadDispatcher);

            this.artists = new MainThreadObservableCollectionAdapter<IArtistViewModel>(this.chartsDataController.ArtistChart,
                                                                                       PlatformServices.MainThreadDispatcher);

            this.tracks = new MainThreadObservableCollectionAdapter<ITrackViewModel>(this.chartsDataController.TrackChart,
                                                                                     PlatformServices.MainThreadDispatcher);

            this.playlists = new MainThreadObservableCollectionAdapter<IPlaylistViewModel>(this.chartsDataController.PlaylistChart,
                                                                                           PlatformServices.MainThreadDispatcher);


            this.chartsDataController.BeginPopulateAsync();
            this.genreListDataController.RefreshGenreListAsync();
        }



        // IChartViewModel
        public IObservableCollection<IGenreViewModel> GenreList => this.genreList;
        public IObservableCollection<IAlbumViewModel> AlbumChart => this.albums;
        public IObservableCollection<IArtistViewModel> ArtistChart => this.artists;
        public IObservableCollection<IPlaylistViewModel> PlaylistChart => this.playlists;
        public IObservableCollection<ITrackViewModel> TrackChart => this.tracks;


        public void SetSelectedGenre(IGenreViewModel genre)
        {
            Assert.That(genre != null);
            this.chartsDataController.SetGenreFilter(genre.Id);
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
