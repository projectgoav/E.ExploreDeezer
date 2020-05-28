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

using E.ExploreDeezer.UWP.ViewModels;
using E.ExploreDeezer.Core.ViewModels;

namespace E.ExploreDeezer.UWP.Controls
{
    public sealed partial class ArtistGrid : UserControl
    {
        public ArtistGrid()
        {
            this.InitializeComponent();

            this.DataContextChanged += OnDataContextChanged;

            this.TheGrid.SelectionChanged += OnGridSelectionChanged;
        }

        public IContentUserControlViewModel<IArtistViewModel> ViewModel { get; private set; }


        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
            => this.ViewModel = args.NewValue as IContentUserControlViewModel<IArtistViewModel>;

        private void OnGridSelectionChanged(object sender, SelectionChangedEventArgs e)
            => this.ViewModel?.OnItemSelected(this.TheGrid.SelectedIndex);

    }
}
