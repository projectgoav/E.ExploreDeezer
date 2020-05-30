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
using E.ExploreDeezer.Core.Charts;
using E.ExploreDeezer.UWP.ViewModels;
using E.ExploreDeezer.Core.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace E.ExploreDeezer.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChartsView : Page
    {
        private ContentUserControlViewModel<ITrackViewModel> trackChartViewModel;
        private ContentUserControlViewModel<IAlbumViewModel> albumChartViewModel;
        private ContentUserControlViewModel<IArtistViewModel> artistChartViewModel;
        private ContentUserControlViewModel<IPlaylistViewModel> playlistChartViewModel;


        public ChartsView()
        {
            this.InitializeComponent();
        }


        public IChartsViewModel ViewModel { get; private set; }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.ViewModel = ServiceRegistry.ViewModelFactory.CreateChartsViewModel();
            this.DataContext = this.ViewModel;

            this.trackChartViewModel = new ContentUserControlViewModel<ITrackViewModel>(this.ViewModel,
                                                                                        nameof(IChartsViewModel.TrackChart),
                                                                                        nameof(IChartsViewModel.TrackChartFetchState),
                                                                                        () => this.ViewModel.TrackChart,
                                                                                        () => this.ViewModel.TrackChartFetchState,
                                                                                        _ => { },
                                                                                        ServiceRegistry.PlatformServices);

            this.albumChartViewModel = new ContentUserControlViewModel<IAlbumViewModel>(this.ViewModel,
                                                                                        nameof(IChartsViewModel.AlbumChart),
                                                                                        nameof(IChartsViewModel.AlbumChartFetchState),
                                                                                        () => this.ViewModel.AlbumChart,
                                                                                        () => this.ViewModel.AlbumChartFetchState,
                                                                                        OnAlbumSelected,
                                                                                        ServiceRegistry.PlatformServices);

            this.artistChartViewModel = new ContentUserControlViewModel<IArtistViewModel>(this.ViewModel,
                                                                                          nameof(IChartsViewModel.ArtistChart),
                                                                                          nameof(IChartsViewModel.ArtistChartFetchState),
                                                                                          () => this.ViewModel.ArtistChart,
                                                                                          () => this.ViewModel.ArtistChartFetchState,
                                                                                          OnArtistSelected,
                                                                                          ServiceRegistry.PlatformServices);

            this.playlistChartViewModel = new ContentUserControlViewModel<IPlaylistViewModel>(this.ViewModel,
                                                                                              nameof(IChartsViewModel.PlaylistChart),
                                                                                              nameof(IChartsViewModel.PlaylistChartFetchState),
                                                                                              () => this.ViewModel.PlaylistChart,
                                                                                              () => this.ViewModel.PlaylistChartFetchState,
                                                                                              OnPlaylistSelected,
                                                                                              ServiceRegistry.PlatformServices);


            this.TrackList.DataContext = this.trackChartViewModel;
            this.AlbumGrid.DataContext = this.albumChartViewModel;
            this.ArtistGrid.DataContext = this.artistChartViewModel;
            this.PlaylistGrid.DataContext = this.playlistChartViewModel;
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            this.TrackList.DataContext = null;
            this.AlbumGrid.DataContext = null;
            this.ArtistGrid.DataContext = null;
            this.PlaylistGrid.DataContext = null;

            this.trackChartViewModel.Dispose();
            this.albumChartViewModel.Dispose();
            this.artistChartViewModel.Dispose();
            this.playlistChartViewModel.Dispose();

            this.ViewModel.Dispose();

            this.DataContext = null;
        }


        private void OnAlbumSelected(int index)
        {
            if (index == -1)
                return;

            Navigation.ShowAlbumTracklist(this.ViewModel.AlbumChart.GetItem(index));
        }

        private void OnPlaylistSelected(int index)
        {
            if (index == -1)
                return;

            Navigation.ShowPlaylistTracklist(this.ViewModel.PlaylistChart.GetItem(index));
        }

        private void OnArtistSelected(int index)
        {
            if (index == -1)
                return;

            Navigation.ShowArtistOverview(this.ViewModel.ArtistChart.GetItem(index));
        }
    }
}
