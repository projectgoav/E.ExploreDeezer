using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using E.ExploreDeezer.Core;
using E.ExploreDeezer.Core.Common;
using E.ExploreDeezer.UWP.ViewModels;
using E.ExploreDeezer.Core.ViewModels;
using E.Deezer.Api;
using Windows.ApplicationModel.Chat;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace E.ExploreDeezer.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ArtistOverviewView : Page
    {
        private ContentUserControlViewModel<IAlbumViewModel> albumsViewModel;
        private ContentUserControlViewModel<ITrackViewModel> topTrackViewModel;
        private ContentUserControlViewModel<IPlaylistViewModel> featuredOnViewModel;
        private ContentUserControlViewModel<IArtistViewModel> relatedArtistViewModel;

        public ArtistOverviewView()
        {
            this.InitializeComponent();
        }


        public IArtistOverviewViewModel ViewModel { get; private set; }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);


            Assert.ObjectOfType<ArtistOverviewViewModelParams>(e.Parameter);
            this.ViewModel = ServiceRegistry.ViewModelFactory.CreateArtistOverviewViewModel((ArtistOverviewViewModelParams)e.Parameter);
            this.DataContext = ViewModel;

            this.albumsViewModel = new ContentUserControlViewModel<IAlbumViewModel>(this.ViewModel,
                                                                                    nameof(IArtistOverviewViewModel.Albums),
                                                                                    nameof(IArtistOverviewViewModel.AlbumFetchState),
                                                                                    () => this.ViewModel.Albums,
                                                                                    () => this.ViewModel.AlbumFetchState,
                                                                                    OnAlbumSelected,
                                                                                    ServiceRegistry.PlatformServices);

            this.topTrackViewModel = new ContentUserControlViewModel<ITrackViewModel>(this.ViewModel,
                                                                                      nameof(IArtistOverviewViewModel.TopTracks),
                                                                                      nameof(IArtistOverviewViewModel.TopTrackFetchState),
                                                                                      () => this.ViewModel.TopTracks,
                                                                                      () => this.ViewModel.TopTrackFetchState,
                                                                                      _ => { },
                                                                                      ServiceRegistry.PlatformServices);

            this.featuredOnViewModel = new ContentUserControlViewModel<IPlaylistViewModel>(this.ViewModel,
                                                                                           nameof(IArtistOverviewViewModel.FeaturedPlaylists),
                                                                                           nameof(IArtistOverviewViewModel.FeaturedPlaylistFetchState),
                                                                                           () => this.ViewModel.FeaturedPlaylists,
                                                                                           () => this.ViewModel.FeaturedPlaylistFetchState,
                                                                                           OnPlaylistSelected,
                                                                                           ServiceRegistry.PlatformServices);

            this.relatedArtistViewModel = new ContentUserControlViewModel<IArtistViewModel>(this.ViewModel,
                                                                                            nameof(IArtistOverviewViewModel.RelatedArtists),
                                                                                            nameof(IArtistOverviewViewModel.RelatedArtistFetchState),
                                                                                            () => this.ViewModel.RelatedArtists,
                                                                                            () => this.ViewModel.RelatedArtistFetchState,
                                                                                            OnArtistSelected,
                                                                                            ServiceRegistry.PlatformServices);

            this.AlbumGrid.DataContext = this.albumsViewModel;
            this.TopTrackList.DataContext = this.topTrackViewModel;
            this.ArtistGrid.DataContext = this.relatedArtistViewModel;
            this.PlaylistGrid.DataContext = this.featuredOnViewModel;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            this.AlbumGrid.DataContext = null;
            this.TopTrackList.DataContext = null;
            this.ArtistGrid.DataContext = null;
            this.PlaylistGrid.DataContext = null;

            this.albumsViewModel.Dispose();
            this.topTrackViewModel.Dispose();
            this.featuredOnViewModel.Dispose();
            this.relatedArtistViewModel.Dispose();

            this.ViewModel.Dispose();
            this.DataContext = null;
        }

        
        private void OnAlbumSelected(int index)
        {
            if (index == -1)
                return;

            Navigation.ShowAlbumTracklist(this.ViewModel.Albums.GetItem(index));
        }

        private void OnArtistSelected(int index)
        {
            if (index == -1)
                return;

            Navigation.ShowArtistOverview(this.ViewModel.RelatedArtists.GetItem(index));
        }

        private void OnPlaylistSelected(int index)
        {
            if (index == -1)
                return;

            Navigation.ShowPlaylistTracklist(this.ViewModel.FeaturedPlaylists.GetItem(index));
        }
    }
}
