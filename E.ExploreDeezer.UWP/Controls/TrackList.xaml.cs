﻿using System;
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
    public sealed partial class TrackList : UserControl
    {
        public TrackList()
        {
            this.InitializeComponent();

            this.DataContextChanged += OnDataContextChanged;
        }

        public IContentUserControlViewModel<ITrackViewModel> ViewModel { get; private set; }


        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
            => this.ViewModel = args.NewValue as IContentUserControlViewModel<ITrackViewModel>;
    }
}
