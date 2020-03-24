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

            this.DataContext = ServiceRegistry.ViewModelFactory.CreateArtistOverviewViewModel(e.Parameter as IArtistOverviewViewModelParams);

            this.AlbumGrid.SelectionChanged += OnGridSelectionChanged;             
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            this.AlbumGrid.SelectionChanged -= OnGridSelectionChanged;
        }

        private void OnGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == this.AlbumGrid)
            {
                var album = this.ViewModel.Albums.ElementAt(this.AlbumGrid.SelectedIndex);
                var p = this.ViewModel.CreateTracklistViewModelParams(album);

                ServiceRegistry.GetService<Frame>()
                               .Navigate(typeof(TracklistView), p);
            }
        }
    }
}
