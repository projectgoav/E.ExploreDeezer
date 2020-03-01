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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace E.ExploreDeezer.UWP.Controls
{
    public sealed partial class PlaylistTracklistHeader : UserControl
    {
        public PlaylistTracklistHeader()
        {
            this.InitializeComponent();

            this.DataContextChanged += (s, e) =>
            {
                this.Bindings.Update();
            };
        }


        public IPlaylistViewModel ViewModel => this.DataContext as IPlaylistViewModel;
    }
}
