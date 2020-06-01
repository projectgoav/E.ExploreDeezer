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
using E.ExploreDeezer.Core.ViewModels;
using E.ExploreDeezer.Core.Collections;

namespace E.ExploreDeezer.UWP.Views
{ 
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TracklistView : Page
    {
        public TracklistView()
        {
            this.InitializeComponent();
        }


        public ITracklistViewModel ViewModel { get; private set; }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Assert.ObjectOfType<TracklistViewModelParams>(e.Parameter);
            this.ViewModel = ServiceRegistry.ViewModelFactory.CreateTracklistViewModel((TracklistViewModelParams)e.Parameter);
            this.DataContext = this.ViewModel;

            this.SubtitleButton.Click += OnSubtitleClicked;
            this.FavouriteButton.Click += OnFavouriteButtonClicked;
        }

        private void OnFavouriteButtonClicked(object sender, RoutedEventArgs e)
            => this.ViewModel.ToggleFavourited();

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            this.SubtitleButton.Click -= OnSubtitleClicked;
            this.FavouriteButton.Click -= OnFavouriteButtonClicked;

            this.ViewModel.Dispose();
            this.DataContext = null;
        }


        private void OnSubtitleClicked(object sender, RoutedEventArgs e)
        {
            switch(this.ViewModel.Type)
            {
                case ETracklistViewModelType.Album:
                    {
                        var p = this.ViewModel.CreateArtistOverviewViewModelParams();

                        ServiceRegistry.GetService<Frame>()
                                       .Navigate(typeof(ArtistOverviewView), p);

                        break;
                    }

                case ETracklistViewModelType.Playlist:
                    {
                        var p = this.ViewModel.CreateUserOverviewViewModelParams();

                        ServiceRegistry.GetService<Frame>()
                                       .Navigate(typeof(UserOverviewView), p);

                        break;
                    }
            }
        }
    }

}
