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
        EFetchState AlbumChartFetchState { get; }
        IObservableCollection<IAlbumViewModel> AlbumChart { get; }

        EFetchState ArtistChartFetchState { get; }
        IObservableCollection<IArtistViewModel> ArtistChart { get; }

        EFetchState TrackChartFetchState { get; }
        IObservableCollection<ITrackViewModel> TrackChart { get; }

        EFetchState PlaylistChartFetchState { get; }
        IObservableCollection<IPlaylistViewModel> PlaylistChart { get; }

        EFetchState GenreListFetchState { get; }
        IObservableCollection<IGenreViewModel> GenreList { get; }

        int SelectedGenreIndex { get; set; }

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

        private int selectedGenreIndex;
        private EFetchState genreFetchState;
        private EFetchState albumFetchState;
        private EFetchState artistFetchState;
        private EFetchState trackFetchState;
        private EFetchState playlistFetchState;


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


            this.genreListDataController.OnFetchStateChanged += OnGenreListFetchStateChanged;

            this.chartsDataController.OnAlbumChartFetchStateChanged += OnAlbumChartFetchStatusChanged;
            this.chartsDataController.OnArtistChartFetchStateChanged += OnArtistChartFetchStatusChanged;
            this.chartsDataController.OnTrackChartFetchStateChanged += OnTrackChartFetchStatusChanged;
            this.chartsDataController.OnPlaylistChartFetchStateChanged += OnPlaylistChartFetchStatusChanged;


            //TODO: Need stop this 'Begin' call required before executing the below LINQ command stuff
            this.chartsDataController.BeginPopulateAsync();
            this.genreListDataController.RefreshGenreListAsync();

            this.selectedGenreIndex = this.genreList.Count == 0 ? 0
                                                                : this.genreList.Select((x, i) => new { Genre = x, Index = i })
                                                                                .Where(x => x.Genre.Id == this.chartsDataController.CurrentGenreFilter)
                                                                                .SingleOrDefault()
                                                                                .Index;


        }



        // IChartViewModel
        public IObservableCollection<IGenreViewModel> GenreList => this.genreList;

        public EFetchState GenreListFetchState
        {
            get => this.genreFetchState;
            private set => SetProperty(ref this.genreFetchState, value);
        }


        public IObservableCollection<IAlbumViewModel> AlbumChart => this.albums;

        public EFetchState AlbumChartFetchState
        {
            get => this.albumFetchState;
            private set => SetProperty(ref this.albumFetchState, value);
        }


        public IObservableCollection<IArtistViewModel> ArtistChart => this.artists;

        public EFetchState ArtistChartFetchState
        {
            get => this.artistFetchState;
            private set => SetProperty(ref this.artistFetchState, value);
        }


        public IObservableCollection<IPlaylistViewModel> PlaylistChart => this.playlists;

        public EFetchState PlaylistChartFetchState
        {
            get => this.playlistFetchState;
            private set => SetProperty(ref this.playlistFetchState, value);
        }


        public IObservableCollection<ITrackViewModel> TrackChart => this.tracks;

        public EFetchState TrackChartFetchState
        {
            get => this.trackFetchState;
            private set => SetProperty(ref this.trackFetchState, value);
        }


        public int SelectedGenreIndex
        {
            get => this.selectedGenreIndex;
            set
            {
                if (SetProperty(ref this.selectedGenreIndex, value))
                {
                    this.chartsDataController.SetGenreFilter(this.genreList[value].Id);
                }
            }
        }



        public TracklistViewModelParams GetTracklistViewModelParams(IAlbumViewModel albumViewModel)
            => ViewModelParamFactory.CreateTracklistViewModelParams(albumViewModel);

        public TracklistViewModelParams GetTracklistViewModelParams(IPlaylistViewModel playlistViewModel)
            => ViewModelParamFactory.CreateTracklistViewModelParams(playlistViewModel);

        public ArtistOverviewViewModelParams GetArtistOverviewViewModelParams(IArtistViewModel artistViewModel)
            => ViewModelParamFactory.CreateArtistOverviewViewModelParams(artistViewModel);



        private void OnAlbumChartFetchStatusChanged(object sender, FetchStateChangedEventArgs args)
            => this.AlbumChartFetchState = args.NewValue;

        private void OnArtistChartFetchStatusChanged(object sender, FetchStateChangedEventArgs args)
            => this.ArtistChartFetchState = args.NewValue;

        private void OnTrackChartFetchStatusChanged(object sender, FetchStateChangedEventArgs args)
            => this.TrackChartFetchState = args.NewValue;

        private void OnPlaylistChartFetchStatusChanged(object sender, FetchStateChangedEventArgs args)
            => this.PlaylistChartFetchState = args.NewValue;

        private void OnGenreListFetchStateChanged(object sender, FetchStateChangedEventArgs args)
            => this.GenreListFetchState = args.NewValue;


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.genreListDataController.OnFetchStateChanged -= OnGenreListFetchStateChanged;

                this.chartsDataController.OnAlbumChartFetchStateChanged -= OnAlbumChartFetchStatusChanged;
                this.chartsDataController.OnArtistChartFetchStateChanged -= OnArtistChartFetchStatusChanged;
                this.chartsDataController.OnTrackChartFetchStateChanged -= OnTrackChartFetchStatusChanged;
                this.chartsDataController.OnPlaylistChartFetchStateChanged -= OnPlaylistChartFetchStatusChanged;


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
