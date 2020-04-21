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
using Windows.ApplicationModel.Chat;

namespace E.ExploreDeezer.UWP.Controls
{
    public sealed partial class TrackListCell : UserControl
    {
        public TrackListCell()
        {
            this.InitializeComponent();

            this.DataContextChanged += OnDataContextChanged;

            this.ArtistNameLink.Click += OnArtistNameClick;
        }


        public ITrackViewModel ViewModel { get; private set; }


        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            this.ViewModel = args.NewValue as ITrackViewModel;

            UpdateControls();

            Bindings.Update();
        }


        private void OnArtistNameClick(object sender, RoutedEventArgs e)
            => Navigation.ShowArtistOverview(this.ViewModel.ArtistId);


        private void UpdateControls()
        {
            if (this.ViewModel == null)
                return;

            switch(this.ViewModel.ArtistMode)
            {
                case ETrackArtistMode.Name:
                    this.ArtistNameLabel.Visibility = Visibility.Visible;
                    this.ArtistNameLink.Visibility = Visibility.Collapsed;

                    this.DetailsStack.Spacing = 5;
                    break;

                case ETrackArtistMode.NameWithLink:
                    this.ArtistNameLabel.Visibility = Visibility.Collapsed;
                    this.ArtistNameLink.Visibility = Visibility.Visible;

                    this.DetailsStack.Spacing = 0;
                    break;
            }
        }
    }
}
