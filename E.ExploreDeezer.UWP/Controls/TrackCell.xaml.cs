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

namespace E.ExploreDeezer.UWP.Controls
{
    public sealed partial class TrackCell : UserControl
    {
        public TrackCell()
        {
            this.InitializeComponent();

            this.DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            this.ViewModel = args.NewValue as ITrackViewModel;

            UpdateControls();

            Bindings.Update();
        }

        public ITrackViewModel ViewModel { get; private set; }


        private void UpdateControls()
        {
            if (this.ViewModel == null)
                return;

            switch (this.ViewModel.LHSMode)
            {
                case ETrackLHSMode.Number:
                    this.TrackNumberLabel.Visibility = Visibility.Visible;
                    this.TrackImage.Visibility = Visibility.Collapsed;
                    break;

                case ETrackLHSMode.Artwork:
                    this.TrackImage.Visibility = Visibility.Visible;
                    this.TrackNumberLabel.Visibility = Visibility.Collapsed;
                    break;
            }
        }
    }
}
