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
    public sealed partial class GenreOverviewView : Page
    {
        public GenreOverviewView()
        {
            this.InitializeComponent();
        }


        private IGenreOverviewViewModel ViewModel => this.DataContext as IGenreOverviewViewModel;


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Assert.ObjectOfType<IGenreOverviewViewModelParams>(e.Parameter);

            this.DataContext = ServiceRegistry.ViewModelFactory.CreateGenreOverviewViewModel(e.Parameter as IGenreOverviewViewModelParams);

            this.NewReleaseGrid.SelectionChanged += OnGridSelectionChanged;
            this.DeezerPicksGrid.SelectionChanged += OnGridSelectionChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            (this.ViewModel as IDisposable)?.Dispose();

            this.NewReleaseGrid.SelectionChanged -= OnGridSelectionChanged;
            this.DeezerPicksGrid.SelectionChanged -= OnGridSelectionChanged;
        }


        private void OnGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == this.NewReleaseGrid)
            {
                var album = this.ViewModel.NewReleases.ElementAt(this.NewReleaseGrid.SelectedIndex);
                var p = this.ViewModel.CreateTracklistViewModelParams(album);

                ServiceRegistry.GetService<Frame>()
                               .Navigate(typeof(TracklistView), p);
            }
            else if (sender == this.DeezerPicksGrid)
            {
                var album = this.ViewModel.DeezerPicks.ElementAt(this.DeezerPicksGrid.SelectedIndex);
                var p = this.ViewModel.CreateTracklistViewModelParams(album);

                ServiceRegistry.GetService<Frame>()
                               .Navigate(typeof(TracklistView), p);

            }
        }
    }
}
