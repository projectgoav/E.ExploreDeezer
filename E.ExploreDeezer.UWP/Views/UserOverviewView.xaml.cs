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
using E.ExploreDeezer.UWP.ViewModels;
using E.Deezer.Api;

namespace E.ExploreDeezer.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserOverviewView : Page
    {
        private ContentUserControlViewModel<ITrackViewModel> userFlowViewModel;
        private ContentUserControlViewModel<IPlaylistViewModel> userPlaylistsViewModel;

        public UserOverviewView()
        {
            this.InitializeComponent();
        }


        public IUserOverviewViewModel ViewModel { get; private set; }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Assert.ObjectOfType<UserOverviewViewModelParams>(e.Parameter);
            this.ViewModel = ServiceRegistry.ViewModelFactory.CreateUserOverviewViewModel((UserOverviewViewModelParams)e.Parameter);
            this.DataContext = this.ViewModel;

            this.userFlowViewModel = new ContentUserControlViewModel<ITrackViewModel>(this.ViewModel,
                                                                                      nameof(IUserOverviewViewModel.Flow),
                                                                                      nameof(IUserOverviewViewModel.FlowFetchState),
                                                                                      () => this.ViewModel.Flow,
                                                                                      () => this.ViewModel.FlowFetchState,
                                                                                      _ => { },
                                                                                      ServiceRegistry.PlatformServices);

            this.userPlaylistsViewModel = new ContentUserControlViewModel<IPlaylistViewModel>(this.ViewModel,
                                                                                              nameof(IUserOverviewViewModel.Playlists),
                                                                                              nameof(IUserOverviewViewModel.PlaylistFetchState),
                                                                                              () => this.ViewModel.Playlists,
                                                                                              () => this.ViewModel.PlaylistFetchState,
                                                                                              OnPlaylistSelected,
                                                                                              ServiceRegistry.PlatformServices);

            this.FlowList.DataContext = this.userFlowViewModel;
            this.PlaylistGrid.DataContext = this.userPlaylistsViewModel;

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            this.FlowList.DataContext = null;
            this.PlaylistGrid.DataContext = null;

            this.userFlowViewModel.Dispose();
            this.userPlaylistsViewModel.Dispose();

            this.ViewModel.Dispose();
            this.DataContext = null;
        }


        private void OnPlaylistSelected(int index)
        {
            if (index == -1)
                return;

            Navigation.ShowPlaylistTracklist(this.ViewModel.Playlists.GetItem(index));
        }
    }
}
