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

namespace E.ExploreDeezer.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserOverviewView : Page
    {
        public UserOverviewView()
        {
            this.InitializeComponent();
        }


        public IUserOverviewViewModel ViewModel => this.DataContext as IUserOverviewViewModel;


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Assert.ObjectOfType<UserOverviewViewModelParams>(e.Parameter);

            this.DataContext = ServiceRegistry.ViewModelFactory.CreateUserOverviewViewModel((UserOverviewViewModelParams)e.Parameter);

            this.PlaylistGrid.SelectionChanged += OnPlaylistSelectionChanged;

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            this.PlaylistGrid.SelectionChanged -= OnPlaylistSelectionChanged;

            (this.ViewModel as IDisposable)?.Dispose();
        }


        private void OnPlaylistSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.PlaylistGrid.SelectedIndex >= 0)
            {
                var playlist = this.ViewModel.Playlists.GetItem(this.PlaylistGrid.SelectedIndex);
                var p = this.ViewModel.CreateTracklistViewModelParams(playlist);

                ServiceRegistry.GetService<Frame>()
                               .Navigate(typeof(TracklistView), p);
            }
        }
    }
}
