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

namespace E.ExploreDeezer.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchView : Page
    {
        public SearchView()
        {
            this.InitializeComponent();
        }

        private ISearchViewModel SearchViewModel => this.DataContext as ISearchViewModel;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.DataContext = e.Parameter;

            this.AlbumResultGrid.SelectionChanged += OnGridSelectionChanged;
            this.ArtistResultGrid.SelectionChanged += OnGridSelectionChanged;
            this.PlaylistsResultGrid.SelectionChanged += OnGridSelectionChanged;
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            this.AlbumResultGrid.SelectionChanged -= OnGridSelectionChanged;
            this.ArtistResultGrid.SelectionChanged -= OnGridSelectionChanged;
            this.PlaylistsResultGrid.SelectionChanged -= OnGridSelectionChanged;
        }


        private void OnGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == this.AlbumResultGrid)
            {
                var album = this.SearchViewModel.Albums.ElementAt(this.AlbumResultGrid.SelectedIndex);
                var p = this.SearchViewModel.CreateTracklistViewModelParams(album);

                ServiceRegistry.GetService<Frame>()
                               .Navigate(typeof(TracklistView), p);
            }
            else if (sender == this.ArtistResultGrid)
            {
                var artist = this.SearchViewModel.Artists.ElementAt(this.ArtistResultGrid.SelectedIndex);
                var p = this.SearchViewModel.CreateArtistOverviewViewModelParams(artist);

                ServiceRegistry.GetService<Frame>()
                               .Navigate(typeof(ArtistOverviewView), p);
            }
            else if (sender == this.PlaylistsResultGrid)
            {
                var playlist = this.SearchViewModel.Playlists.ElementAt(this.PlaylistsResultGrid.SelectedIndex);
                var p = this.SearchViewModel.CreateTracklistViewModelParams(playlist);

                ServiceRegistry.GetService<Frame>()
                               .Navigate(typeof(TracklistView), p);
            }
        }
    }
}
