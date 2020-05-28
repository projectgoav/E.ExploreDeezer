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
using E.ExploreDeezer.Core.ViewModels;
using E.ExploreDeezer.UWP.ViewModels;
using Windows.ApplicationModel.Chat;

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


            this.TrackList.DataContext = this.trackChartViewModel;
            this.AlbumGrid.DataContext = this.albumChartViewModel;
            this.ArtistGrid.DataContext = this.artistChartViewModel;

            SetupEvents();
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            RemoveEvents();

            this.trackChartViewModel.Dispose();
            this.albumChartViewModel.Dispose();
            this.artistChartViewModel.Dispose();

            this.ViewModel.Dispose();

            this.DataContext = null;
        }


        private void SetupEvents()
        {
            this.PlaylistsChartGrid.SelectionChanged += OnGridSelectionChanged;
        }

        private void RemoveEvents()
        {
            this.PlaylistsChartGrid.SelectionChanged -= OnGridSelectionChanged;
        }



        private void OnGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == this.PlaylistsChartGrid)
            {
                int index = this.PlaylistsChartGrid.SelectedIndex;

                if (index == -1)
                    return;

                Navigation.ShowPlaylistTracklist(this.ViewModel.PlaylistChart.GetItem(index));
            }
            else
                return;
        }


        private void OnAlbumSelected(int index)
        {
            if (index == -1)
                return;

            Navigation.ShowAlbumTracklist(this.ViewModel.AlbumChart.GetItem(index));
        }

        private void OnArtistSelected(int index)
        {
            if (index == -1)
                return;

            Navigation.ShowArtistOverview(this.ViewModel.ArtistChart.GetItem(index));
        }
    }
}
