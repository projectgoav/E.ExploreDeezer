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
using E.ExploreDeezer.Core.Search;
using E.ExploreDeezer.UWP.ViewModels;
using E.ExploreDeezer.Core.ViewModels;

namespace E.ExploreDeezer.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchView : Page
    {
        private ContentUserControlViewModel<IAlbumViewModel> albumResultViewModel;
        private ContentUserControlViewModel<ITrackViewModel> trackResultViewModel;
        private ContentUserControlViewModel<IArtistViewModel> artistResultViewModel;
        private ContentUserControlViewModel<IPlaylistViewModel> playlistResultViewModel;

        public SearchView()
        {
            this.InitializeComponent();
        }


        public ISearchViewModel ViewModel { get; private set; }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.ViewModel = e.Parameter as ISearchViewModel;
            this.DataContext = this.ViewModel;

            this.albumResultViewModel = new ContentUserControlViewModel<IAlbumViewModel>(this.ViewModel,
                                                                                         nameof(ISearchViewModel.Albums),
                                                                                         nameof(ISearchViewModel.AlbumResultFetchState),
                                                                                         () => this.ViewModel.Albums,
                                                                                         () => this.ViewModel.AlbumResultFetchState,
                                                                                         OnAlbumSelected,
                                                                                         ServiceRegistry.PlatformServices);

            this.trackResultViewModel = new ContentUserControlViewModel<ITrackViewModel>(this.ViewModel,
                                                                                         nameof(ISearchViewModel.Tracks),
                                                                                         nameof(ISearchViewModel.TrackResultFetchState),
                                                                                         () => this.ViewModel.Tracks,
                                                                                         () => this.ViewModel.TrackResultFetchState,
                                                                                         _ => { },
                                                                                         ServiceRegistry.PlatformServices);

            this.artistResultViewModel = new ContentUserControlViewModel<IArtistViewModel>(this.ViewModel,
                                                                                           nameof(ISearchViewModel.Artists),
                                                                                           nameof(ISearchViewModel.ArtistResultFetchState),
                                                                                           () => this.ViewModel.Artists,
                                                                                           () => this.ViewModel.ArtistResultFetchState,
                                                                                           OnArtistSelected,
                                                                                           ServiceRegistry.PlatformServices);

            this.playlistResultViewModel = new ContentUserControlViewModel<IPlaylistViewModel>(this.ViewModel,
                                                                                               nameof(ISearchViewModel.Playlists),
                                                                                               nameof(ISearchViewModel.PlaylistResultFetchState),
                                                                                               () => this.ViewModel.Playlists,
                                                                                               () => this.ViewModel.PlaylistResultFetchState,
                                                                                               OnPlaylistSelected,
                                                                                               ServiceRegistry.PlatformServices);


            this.AlbumGrid.DataContext = this.albumResultViewModel;
            this.TrackList.DataContext = this.trackResultViewModel;
            this.ArtistGrid.DataContext = this.artistResultViewModel;
            this.PlaylistGrid.DataContext = this.playlistResultViewModel;
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            this.AlbumGrid.DataContext = null;
            this.TrackList.DataContext = null;
            this.ArtistGrid.DataContext = null;
            this.PlaylistGrid.DataContext = null;

            this.albumResultViewModel.Dispose();
            this.trackResultViewModel.Dispose();
            this.artistResultViewModel.Dispose();
            this.playlistResultViewModel.Dispose();

            // Don't dispose the SearchViewModel as
            // is owned by the MainView 
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

            Navigation.ShowArtistOverview(this.ViewModel.Artists.GetItem(index));
        }

        private void OnPlaylistSelected(int index)
        {
            if (index == -1)
                return;

            Navigation.ShowPlaylistTracklist(this.ViewModel.Playlists.GetItem(index));
        }
    }
}
