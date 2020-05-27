using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using E.Deezer.Api;
using E.ExploreDeezer.Core.Collections;
using E.ExploreDeezer.Core.Mvvm;
using E.ExploreDeezer.Core.ViewModels;

namespace E.ExploreDeezer.Core.MyDeezer
{
    public interface IMyDeezerViewModel : IViewModel,
                                          IDisposable
    {
        bool IsLoggedIn { get; }
        bool IsLoggedOut { get; }


        EFetchState AlbumFetchState { get; }
        IObservableCollection<IAlbumViewModel> FavouriteAlbums { get; }

        EFetchState TrackFetchState { get; }
        IObservableCollection<ITrackViewModel> FavouriteTracks { get; }

        EFetchState ArtistFetchState { get; }
        IObservableCollection<IArtistViewModel> FavouriteArtists { get; }

    }
     
    internal class MyDeezerViewModel : ViewModelBase,
                                       IMyDeezerViewModel
    {
        private readonly IAuthenticationService authService;
        private readonly IMyDeezerDataController dataController;
        private readonly MainThreadObservableCollectionAdapter<ITrackViewModel> favouriteTracks;
        private readonly MainThreadObservableCollectionAdapter<IAlbumViewModel> favouriteAlbums;
        private readonly MainThreadObservableCollectionAdapter<IArtistViewModel> favouriteArtists;

        private bool isLoggedIn;
        private EFetchState trackFetchState;
        private EFetchState albumFetchState;
        private EFetchState artistFetchState;


        public MyDeezerViewModel(IAuthenticationService authService,
                                 IMyDeezerDataController dataController,
                                 IPlatformServices platformServices)
            : base(platformServices)
        {
            this.authService = authService;
            this.dataController = dataController;

            this.authService.OnAuthenticationStatusChanged += OnAuthenticationStatusChanged;

            this.favouriteTracks = new MainThreadObservableCollectionAdapter<ITrackViewModel>(this.dataController.FavouriteTracks,
                                                                                              PlatformServices.MainThreadDispatcher);

            this.favouriteAlbums = new MainThreadObservableCollectionAdapter<IAlbumViewModel>(this.dataController.FavouriteAlbums,
                                                                                              PlatformServices.MainThreadDispatcher);

            this.favouriteArtists = new MainThreadObservableCollectionAdapter<IArtistViewModel>(this.dataController.FavouriteArtists,
                                                                                                PlatformServices.MainThreadDispatcher);

            this.dataController.OnFavouriteAlbumFetchStateChanged += OnFavouriteAlbumsFetchStateChanged;
            this.dataController.OnFavouriteTracksFetchStateChanged += OnFavouriteTracksFetchStateChanged;
            this.dataController.OnFavouriteArtistsFetchStateChanged += OnFavouriteArtistFetchStateChanged;
        }


        // IMyDeezerViewModel
        private bool LoggedIn
        {
            get => this.isLoggedIn;
            set
            {
                if (SetProperty(ref this.isLoggedIn, value))
                {
                    RaisePropertyChangedSafe(nameof(IsLoggedIn));
                    RaisePropertyChangedSafe(nameof(IsLoggedOut));
                }
            }
        }

        public bool IsLoggedIn => this.LoggedIn;
        public bool IsLoggedOut => !this.LoggedIn;


        public EFetchState TrackFetchState
        {
            get => this.trackFetchState;
            private set => SetProperty(ref this.trackFetchState, value);
        }

        public IObservableCollection<ITrackViewModel> FavouriteTracks => this.favouriteTracks;


        public EFetchState AlbumFetchState
        {
            get => this.albumFetchState;
            private set => SetProperty(ref this.albumFetchState, value);
        }

        public IObservableCollection<IAlbumViewModel> FavouriteAlbums => this.favouriteAlbums;


        public EFetchState ArtistFetchState
        {
            get => this.artistFetchState;
            private set => SetProperty(ref this.artistFetchState, value);
        }

        public IObservableCollection<IArtistViewModel> FavouriteArtists => this.favouriteArtists;



        private void OnAuthenticationStatusChanged(object sender, OnAuthenticationStatusChangedEventArgs e)
        {
            this.LoggedIn = e.IsAuthenticated;
        }



        private void OnFavouriteArtistFetchStateChanged(object sender, FetchStateChangedEventArgs e)
            => this.ArtistFetchState = e.NewValue;

        private void OnFavouriteTracksFetchStateChanged(object sender, FetchStateChangedEventArgs e)
            => this.TrackFetchState = e.NewValue;

        private void OnFavouriteAlbumsFetchStateChanged(object sender, FetchStateChangedEventArgs e)
            => this.AlbumFetchState = e.NewValue;




        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.authService.OnAuthenticationStatusChanged -= OnAuthenticationStatusChanged;

                this.dataController.OnFavouriteAlbumFetchStateChanged -= OnFavouriteAlbumsFetchStateChanged;
                this.dataController.OnFavouriteTracksFetchStateChanged -= OnFavouriteTracksFetchStateChanged;
                this.dataController.OnFavouriteArtistsFetchStateChanged -= OnFavouriteArtistFetchStateChanged;

                this.favouriteTracks.Dispose();
                this.favouriteAlbums.Dispose();
                this.favouriteArtists.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
