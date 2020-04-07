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
using E.ExploreDeezer.Core.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace E.ExploreDeezer.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChartsView : Page
    {
        public ChartsView()
        {
            this.InitializeComponent();
        }


        public IChartsViewModel ViewModel => this.DataContext as IChartsViewModel;


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.DataContext = ServiceRegistry.ViewModelFactory.CreateChartsViewModel();

            SetupEvents();
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            RemoveEvents();

            this.ViewModel.Dispose();

            this.DataContext = null;
        }


        private void SetupEvents()
        {
            this.AlbumChartGrid.SelectionChanged += OnGridSelectionChanged;
            this.PlaylistsChartGrid.SelectionChanged += OnGridSelectionChanged;
            this.ArtistChartGrid.SelectionChanged += OnGridSelectionChanged;
        }

        private void RemoveEvents()
        {
            this.AlbumChartGrid.SelectionChanged -= OnGridSelectionChanged;
            this.PlaylistsChartGrid.SelectionChanged -= OnGridSelectionChanged;
            this.ArtistChartGrid.SelectionChanged -= OnGridSelectionChanged;
        }



        private void OnGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == this.AlbumChartGrid)
            {
                var selectedItem = this.ViewModel.Albums.ElementAt(this.AlbumChartGrid.SelectedIndex);
                var p = this.ViewModel.GetTracklistViewModelParams(selectedItem);

                ServiceRegistry.GetService<Frame>()
                               .ShowNewPage(typeof(TracklistView), p);
                return;
            }
            else if (sender == this.PlaylistsChartGrid)
            {
                var selectedItem = this.ViewModel.Playlists.ElementAt(this.PlaylistsChartGrid.SelectedIndex);
                var p = this.ViewModel.GetTracklistViewModelParams(selectedItem);

                ServiceRegistry.GetService<Frame>()
                               .ShowNewPage(typeof(TracklistView), p);
                return;

            }
            else if (sender == this.ArtistChartGrid)
            {
                var selectedItem = this.ViewModel.Artists.ElementAt(this.ArtistChartGrid.SelectedIndex);
                var p = this.ViewModel.GetArtistOverviewViewModelParams(selectedItem);

                ServiceRegistry.GetService<Frame>()
                               .ShowNewPage(typeof(ArtistOverviewView), p);
            }

            else
                return; // TODO
        }
    }
}
