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
                int index = this.AlbumResultGrid.SelectedIndex;

                if (index == -1)
                    return;

                Navigation.ShowAlbumTracklist(this.SearchViewModel.Albums.GetItem(index));
            }
            else if (sender == this.ArtistResultGrid)
            {
                int index = this.ArtistResultGrid.SelectedIndex;

                if (index == -1)
                    return;

                Navigation.ShowArtistOverview(this.SearchViewModel.Artists.GetItem(index));
            }
            else if (sender == this.PlaylistsResultGrid)
            {
                int index = this.PlaylistsResultGrid.SelectedIndex;

                if (index == -1)
                    return;

                Navigation.ShowPlaylistTracklist(this.SearchViewModel.Playlists.GetItem(index));
            }
        }
    }
}
