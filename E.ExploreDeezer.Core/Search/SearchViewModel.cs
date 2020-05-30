using System;
using System.Collections.Generic;
using System.Text;

using E.ExploreDeezer.Core.Mvvm;
using E.ExploreDeezer.Core.Common;
using E.ExploreDeezer.Core.ViewModels;
using E.ExploreDeezer.Core.Collections;

namespace E.ExploreDeezer.Core.Search
{
    public interface ISearchViewModel : IViewModel
    {
        string CurrentQuery { get; set; }

        EFetchState AlbumResultFetchState { get; }
        IObservableCollection<IAlbumViewModel> Albums { get; }

        EFetchState TrackResultFetchState { get; }
        IObservableCollection<ITrackViewModel> Tracks { get; }

        EFetchState ArtistResultFetchState { get; }
        IObservableCollection<IArtistViewModel> Artists { get; }

        EFetchState PlaylistResultFetchState { get; }
        IObservableCollection<IPlaylistViewModel> Playlists { get; }
    }

    internal class SearchViewModel : ViewModelBase,
                                     ISearchViewModel
    {
        private readonly ISearchDataController dataController;
        private readonly MainThreadObservableCollectionAdapter<IAlbumViewModel> albums;
        private readonly MainThreadObservableCollectionAdapter<ITrackViewModel> tracks;
        private readonly MainThreadObservableCollectionAdapter<IArtistViewModel> artists;
        private readonly MainThreadObservableCollectionAdapter<IPlaylistViewModel> playlists;

        private string currentQuery;
        private EFetchState albumFetchState;
        private EFetchState trackFetchState;
        private EFetchState artistFetchState;
        private EFetchState playlistFetchState;

        public SearchViewModel(IPlatformServices platformServices)
            : base(platformServices)
        {

            this.dataController = ServiceRegistry.GetService<ISearchDataController>();

            this.albums = new MainThreadObservableCollectionAdapter<IAlbumViewModel>(this.dataController.AlbumResults,
                                                                                     PlatformServices.MainThreadDispatcher);

            this.tracks = new MainThreadObservableCollectionAdapter<ITrackViewModel>(this.dataController.TrackResults,
                                                                                     PlatformServices.MainThreadDispatcher);

            this.artists = new MainThreadObservableCollectionAdapter<IArtistViewModel>(this.dataController.ArtistResults,
                                                                                       PlatformServices.MainThreadDispatcher);

            this.playlists = new MainThreadObservableCollectionAdapter<IPlaylistViewModel>(this.dataController.PlaylistResults,
                                                                                           PlatformServices.MainThreadDispatcher);


            this.dataController.OnAlbumResultsFetchStateChanged += OnAlbumFetchStateChanged;
            this.dataController.OnTrackResultsFetchStateChanged += OnTrackFetchStateChanged;
            this.dataController.OnArtistResultsFetchStateChanged += OnArtistFetchStateChanged;
            this.dataController.OnPlaylistResultsFetchStateChanged += OnPlaylistFetchStateChanged;
        }


        // ISearchViewModel
        public string CurrentQuery
        {
            get => this.currentQuery;
            set
            {
                if (SetProperty(ref this.currentQuery, value))
                {
                    this.dataController.SearchAsync(value);
                }
            }
        }


        public EFetchState AlbumResultFetchState
        {
            get => this.albumFetchState;
            private set => SetProperty(ref this.albumFetchState, value);
        }

        public IObservableCollection<IAlbumViewModel> Albums => this.albums;


        public EFetchState TrackResultFetchState
        {
            get => this.trackFetchState;
            private set => SetProperty(ref this.trackFetchState, value);
        }

        public IObservableCollection<ITrackViewModel> Tracks => this.tracks;


        public EFetchState ArtistResultFetchState
        {
            get => this.artistFetchState;
            private set => SetProperty(ref this.artistFetchState, value);
        }

        public IObservableCollection<IArtistViewModel> Artists => this.artists;


        public EFetchState PlaylistResultFetchState
        {
            get => this.playlistFetchState;
            private set => SetProperty(ref this.playlistFetchState, value);
        }

        public IObservableCollection<IPlaylistViewModel> Playlists => this.playlists;


        private void OnAlbumFetchStateChanged(object sender, FetchStateChangedEventArgs args)
            => this.AlbumResultFetchState = args.NewValue;

        private void OnTrackFetchStateChanged(object sender, FetchStateChangedEventArgs args)
            => this.TrackResultFetchState = args.NewValue;

        private void OnArtistFetchStateChanged(object sender, FetchStateChangedEventArgs args)
            => this.ArtistResultFetchState = args.NewValue;

        private void OnPlaylistFetchStateChanged(object sender, FetchStateChangedEventArgs args)
            => this.PlaylistResultFetchState = args.NewValue;



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dataController.OnAlbumResultsFetchStateChanged -= OnAlbumFetchStateChanged;
                this.dataController.OnTrackResultsFetchStateChanged -= OnTrackFetchStateChanged;
                this.dataController.OnArtistResultsFetchStateChanged -= OnArtistFetchStateChanged;
                this.dataController.OnPlaylistResultsFetchStateChanged -= OnPlaylistFetchStateChanged;

                this.albums.Dispose();
                this.tracks.Dispose();
                this.artists.Dispose();
                this.playlists.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
