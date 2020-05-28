using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using E.ExploreDeezer.Core;
using E.ExploreDeezer.Core.MyDeezer;
using E.ExploreDeezer.Core.ViewModels;
using E.ExploreDeezer.UWP.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace E.ExploreDeezer.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MyDeezerView : Page
    {
        private ContentUserControlViewModel<ITrackViewModel> favouriteTracksViewModel;
        private ContentUserControlViewModel<IAlbumViewModel> favouriteAlbumsViewModel;
        private ContentUserControlViewModel<IArtistViewModel> favouriteArtistsViewModel;

        public MyDeezerView()
        {
            this.InitializeComponent();
        }

        public IMyDeezerViewModel ViewModel { get; private set; }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var vm = ServiceRegistry.ViewModelFactory.CreateMyDeezerViewModel();

            this.ViewModel = vm;
            this.DataContext = vm;

            this.favouriteTracksViewModel = new ContentUserControlViewModel<ITrackViewModel>(this.ViewModel,
                                                                                             nameof(IMyDeezerViewModel.FavouriteTracks),
                                                                                             nameof(IMyDeezerViewModel.TrackFetchState),
                                                                                             () => this.ViewModel.FavouriteTracks,
                                                                                             () => this.ViewModel.TrackFetchState,
                                                                                             _ => { },
                                                                                             ServiceRegistry.PlatformServices);

            this.favouriteAlbumsViewModel = new ContentUserControlViewModel<IAlbumViewModel>(this.ViewModel,
                                                                                             nameof(IMyDeezerViewModel.FavouriteAlbums),
                                                                                             nameof(IMyDeezerViewModel.AlbumFetchState),
                                                                                             () => this.ViewModel.FavouriteAlbums,
                                                                                             () => this.ViewModel.AlbumFetchState,
                                                                                             OnAlbumSelected,
                                                                                             ServiceRegistry.PlatformServices);

            this.favouriteArtistsViewModel = new ContentUserControlViewModel<IArtistViewModel>(this.ViewModel,
                                                                                               nameof(IMyDeezerViewModel.FavouriteArtists),
                                                                                               nameof(IMyDeezerViewModel.ArtistFetchState),
                                                                                               () => this.ViewModel.FavouriteArtists,
                                                                                               () => this.ViewModel.ArtistFetchState,
                                                                                               OnArtistSelected,
                                                                                               ServiceRegistry.PlatformServices);

            this.FavouriteTrackList.DataContext = this.favouriteTracksViewModel;
            this.FavouriteAlbumGrid.DataContext = this.favouriteAlbumsViewModel;
            this.FavouriteArtistGrid.DataContext = this.favouriteArtistsViewModel;


            this.LoginButton.Click += OnLoginButtonClicked;
            this.LogoutButton.Click += OnLogoutButtonClicked;
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            this.DataContext = null;

            this.favouriteTracksViewModel.Dispose();
            this.favouriteAlbumsViewModel.Dispose();
            this.favouriteArtistsViewModel.Dispose();

            this.ViewModel.Dispose();
        }


        private void OnLoginButtonClicked(object sender, RoutedEventArgs e)
        {
            var loginDialog = new OAuthLoginDialog();

            loginDialog.ShowAsync();
        }

        private void OnLogoutButtonClicked(object sender, RoutedEventArgs e)
            => this.ViewModel.LogoutUser();


        private void OnAlbumSelected(int index)
        {
            if (index == -1)
                return;

            var album = this.ViewModel.FavouriteAlbums.GetItem(index);
            Navigation.ShowAlbumTracklist(album);
        }

        private void OnArtistSelected(int index)
        {
            if (index == -1)
                return;

            var artist = this.ViewModel.FavouriteArtists.GetItem(index);
            Navigation.ShowArtistOverview(artist);
        }
    }
}
