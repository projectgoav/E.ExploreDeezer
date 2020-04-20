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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace E.ExploreDeezer.UWP.Views
{ 
    public interface IWrappedTrackViewModel : ITrackViewModel
    {
        Action<ulong> OnArtistSelected { get; }
    }


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TracklistView : Page
    {
        private class WrappedTrackViewModel : IWrappedTrackViewModel
        {
            public WrappedTrackViewModel(ITrackViewModel track,
                                         Action<ulong> onArtistSelected)
            {
                this.TrackViewModel = track;
                this.OnArtistSelected = onArtistSelected;
            }


            // ITrackViewModel
            public bool IsPresent => this.TrackViewModel.IsPresent;

            public ETrackLHSMode LHSMode => this.TrackViewModel.LHSMode;

            public string Title => this.TrackViewModel.Title;

            public string Artist => this.TrackViewModel.Artist;

            public ulong ArtistId => this.TrackViewModel.ArtistId;

            public string TrackNumber => this.TrackViewModel.TrackNumber;

            public string ArtworkUri => this.TrackViewModel.ArtworkUri;


            // IWrappedTrackViewModel
            public Action<ulong> OnArtistSelected { get; }


            // Helpers
            public ITrackViewModel TrackViewModel { get; }
        }



        private ItemConvertingObservableCollection<ITrackViewModel, IWrappedTrackViewModel> wrappedCollection;


        public TracklistView()
        {
            this.InitializeComponent();
        }


        public ITracklistViewModel ViewModel => this.DataContext as ITracklistViewModel;


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Assert.ObjectOfType<TracklistViewModelParams>(e.Parameter);
            this.DataContext = ServiceRegistry.ViewModelFactory.CreateTracklistViewModel((TracklistViewModelParams)e.Parameter);

            this.wrappedCollection
                = new ItemConvertingObservableCollection<ITrackViewModel, IWrappedTrackViewModel>(this.ViewModel.Tracklist,
                                                                                                  ServiceRegistry.PlatformServices.MainThreadDispatcher,
                                                                                                  x => new WrappedTrackViewModel(x, OnArtistSelected),
                                                                                                  x => (x as WrappedTrackViewModel)?.TrackViewModel ?? null);

            this.Tracklist.ItemsSource = this.wrappedCollection;

            this.SubtitleButton.Click += OnSubtitleClicked;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            this.Tracklist.ItemsSource = null;

            this.SubtitleButton.Click -= OnSubtitleClicked;

            this.wrappedCollection.Dispose();

            (this.ViewModel as IDisposable)?.Dispose();
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


        private void OnArtistSelected(ulong artistId)
        {
            var p = this.ViewModel.CreateArtistOverviewViewModelParams(artistId);

            ServiceRegistry.GetService<Frame>()
                           .Navigate(typeof(ArtistOverviewView), p);

        }

    }

}
