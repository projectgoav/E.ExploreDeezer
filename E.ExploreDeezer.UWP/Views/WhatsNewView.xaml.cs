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
using E.ExploreDeezer.Core.WhatsNew;
using E.ExploreDeezer.Core.ViewModels;
using E.ExploreDeezer.UWP.ViewModels;
using Windows.Networking.Vpn;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace E.ExploreDeezer.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WhatsNewView : Page
    {
        private ContentUserControlViewModel<IAlbumViewModel> newAlbumsViewModel;
        private ContentUserControlViewModel<IAlbumViewModel> deezerPicksViewModel;

        public WhatsNewView()
        {
            this.InitializeComponent();
        }

        public IWhatsNewViewModel ViewModel { get; private set; }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.ViewModel = ServiceRegistry.ViewModelFactory.CreateWhatsNewViewModel();
            this.DataContext = this.ViewModel;

            this.newAlbumsViewModel = new ContentUserControlViewModel<IAlbumViewModel>(this.ViewModel,
                                                                                       nameof(IWhatsNewViewModel.NewReleases),
                                                                                       nameof(IWhatsNewViewModel.NewReleaseFetchState),
                                                                                       () => this.ViewModel.NewReleases,
                                                                                       () => this.ViewModel.NewReleaseFetchState,
                                                                                       OnNewReleaseSelected,
                                                                                       ServiceRegistry.PlatformServices);

            this.deezerPicksViewModel = new ContentUserControlViewModel<IAlbumViewModel>(this.ViewModel,
                                                                                         nameof(IWhatsNewViewModel.DeezerPicks),
                                                                                         nameof(IWhatsNewViewModel.DeezerPicksFetchState),
                                                                                         () => this.ViewModel.DeezerPicks,
                                                                                         () => this.ViewModel.DeezerPicksFetchState,
                                                                                         OnDeezerPickSelected,
                                                                                         ServiceRegistry.PlatformServices);

            this.NewAlbumGrid.DataContext = this.newAlbumsViewModel;
            this.DeezerPicksGrid.DataContext = this.deezerPicksViewModel;
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            this.NewAlbumGrid.DataContext = null;
            this.DeezerPicksGrid.DataContext = null;

            this.newAlbumsViewModel.Dispose();
            this.deezerPicksViewModel.Dispose();

            this.ViewModel.Dispose();
            this.DataContext = null;
        }


        private void OnNewReleaseSelected(int index)
        {
            if (index == -1)
                return;

            Navigation.ShowAlbumTracklist(this.ViewModel.NewReleases.GetItem(index));
        }

        private void OnDeezerPickSelected(int index)
        {
            if (index == -1)
                return;

            Navigation.ShowAlbumTracklist(this.ViewModel.DeezerPicks.GetItem(index));
        }
    }
}
