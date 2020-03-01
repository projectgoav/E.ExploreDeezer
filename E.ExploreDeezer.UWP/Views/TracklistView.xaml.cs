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

using E.ExploreDeezer.Core.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace E.ExploreDeezer.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TracklistView : Page
    {
        private UserControl pageHeader;

        public TracklistView()
        {
            this.InitializeComponent();
        }


        public ITracklistViewModel ViewModel => this.DataContext as ITracklistViewModel;


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.DataContext = new TracklistViewModel(ServiceRegistry.DeezerSession,
                                                      ServiceRegistry.PlatformServices,
                                                      e.Parameter as ITracklistViewModelParams);

            SetupUI();

            this.ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            TearDownUI();

            this.ViewModel.PropertyChanged -= ViewModel_PropertyChanged;

            this.ViewModel.Dispose();

            this.DataContext = null;
        }


        private void SetupUI()
        {
            switch(this.ViewModel.Type)
            {
                case ETracklistViewModelType.Album:
                    this.pageHeader = new Controls.AlbumTracklistHeader();
                    this.pageHeader.DataContext = this.ViewModel.AlbumViewModel;

                    this.HeaderFrame.Content = this.pageHeader;

                    break;

                case ETracklistViewModelType.Playlist:
                    this.pageHeader = new Controls.PlaylistTracklistHeader();
                    this.pageHeader.DataContext = this.ViewModel.PlaylistViewModel;

                    this.HeaderFrame.Content = this.pageHeader;

                    break;
            }
        }

        private void TearDownUI()
        {
            this.HeaderFrame.Content = null;

            if (this.pageHeader != null)
            {
                this.pageHeader.DataContext = null;
            }
        }



        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ITracklistViewModel.AlbumViewModel):

                    if (this.ViewModel.Type == ETracklistViewModelType.Album && this.pageHeader != null)
                    {
                        this.pageHeader.DataContext = this.ViewModel.AlbumViewModel;
                    }

                    break;

                case nameof(ITracklistViewModel.PlaylistViewModel):

                    if (this.ViewModel.Type == ETracklistViewModelType.Playlist && this.pageHeader != null)
                    {
                        this.pageHeader.DataContext = this.ViewModel.PlaylistViewModel;
                    }

                    break;
            }
        }
    }
}
