using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using System.Windows.Controls;

using E.ExploreDeezer.Core.ViewModels;

namespace E.ExploreDeezer.WPF.Controls
{
    public partial class AlbumGridCell : UserControl
    { 
        public AlbumGridCell()
        {
            InitializeComponent();
        }

        //private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        //{
        //    this.ViewModel = args.NewValue as IAlbumViewModel;
        //    Bindings.Update();
        //}

        //public IAlbumViewModel ViewModel { get; private set; }
    }
}
