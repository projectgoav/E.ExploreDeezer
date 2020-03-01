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
using E.ExploreDeezer.Core.ViewModels.Home;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace E.ExploreDeezer.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WhatsNewView : Page
    {
        public WhatsNewView()
        {
            this.InitializeComponent();
        }

        public IWhatsNewViewModel ViewModel => this.DataContext as IWhatsNewViewModel;


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.DataContext = new WhatsNewViewModel(ServiceRegistry.DeezerSession,
                                                     ServiceRegistry.PlatformServices);

            this.NewAlbumGrid.SelectionChanged += GridSelectionChanged;
            this.DeezerPicksGrid.SelectionChanged += GridSelectionChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            this.NewAlbumGrid.SelectionChanged -= GridSelectionChanged;
            this.DeezerPicksGrid.SelectionChanged -= GridSelectionChanged;

            this.ViewModel.Dispose();

            this.DataContext = null;
        }



        private void GridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IAlbumViewModel selectedItem = null;

            if (sender == this.NewAlbumGrid)
                selectedItem = this.ViewModel.NewAlbums.ElementAt(this.NewAlbumGrid.SelectedIndex);

            else if (sender == this.DeezerPicksGrid)
                selectedItem = this.ViewModel.DeezerPicks.ElementAt(this.DeezerPicksGrid.SelectedIndex);

            else
                return; // TODO

            // TODO : Centralise the navigation somewhere?
            var p = this.ViewModel.GetTracklistViewModelParams(selectedItem);

            ServiceRegistry.ApplicationFrame.Navigate(typeof(TracklistView), p);
        }
    }
}
