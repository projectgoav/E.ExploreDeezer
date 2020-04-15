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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace E.ExploreDeezer.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ArtistOverviewView : Page
    {
        public ArtistOverviewView()
        {
            this.InitializeComponent();
        }


        public IArtistOverviewViewModel ViewModel => this.DataContext as IArtistOverviewViewModel;


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Assert.ObjectOfType<ArtistOverviewViewModelParams>(e.Parameter);

            this.DataContext = ServiceRegistry.ViewModelFactory.CreateArtistOverviewViewModel((ArtistOverviewViewModelParams)e.Parameter);

            this.AlbumGrid.SelectionChanged += OnGridSelectionChanged;
            this.RelatedArtistsGrid.SelectionChanged += OnGridSelectionChanged;
            this.FeaturedPlaylistsGrid.SelectionChanged += OnGridSelectionChanged;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            this.AlbumGrid.SelectionChanged -= OnGridSelectionChanged;
            this.RelatedArtistsGrid.SelectionChanged -= OnGridSelectionChanged;
            this.FeaturedPlaylistsGrid.SelectionChanged += OnGridSelectionChanged;

            (this.ViewModel as IDisposable)?.Dispose();
            this.DataContext = null;
        }

        private void OnGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == this.AlbumGrid)
            {
                int index = this.AlbumGrid.SelectedIndex;

                if (index == -1)
                    return;

                var album = this.ViewModel.Albums.GetItem(this.AlbumGrid.SelectedIndex);
                var p = this.ViewModel.CreateTracklistViewModelParams(album);

                ServiceRegistry.GetService<Frame>()
                               .ShowNewPage(typeof(TracklistView), p);
            }
            else if (sender == this.RelatedArtistsGrid)
            {
                int index = this.RelatedArtistsGrid.SelectedIndex;

                if (index == -1)
                    return;

                var artist = this.ViewModel.RelatedArtists.GetItem(this.RelatedArtistsGrid.SelectedIndex);
                var p = this.ViewModel.CreateArtistOverviewViewModelParams(artist);

                ServiceRegistry.GetService<Frame>()
                               .ShowNewPage(typeof(ArtistOverviewView), p);
            }
            else if (sender == this.FeaturedPlaylistsGrid)
            {
                int index = this.FeaturedPlaylistsGrid.SelectedIndex;

                if (index == -1)
                    return;

                var playlist = this.ViewModel.FeaturedPlaylists.GetItem(this.FeaturedPlaylistsGrid.SelectedIndex);
                var p = this.ViewModel.CreateTracklistViewModelParams(playlist);

                ServiceRegistry.GetService<Frame>()
                               .ShowNewPage(typeof(TracklistView), p);
            }
        }
    }
}
