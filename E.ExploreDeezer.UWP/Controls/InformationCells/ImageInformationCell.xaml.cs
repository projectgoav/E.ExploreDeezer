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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

using E.ExploreDeezer.Core.ViewModels;

namespace E.ExploreDeezer.UWP.Controls.InformationCells
{
    public sealed partial class ImageInformationCell : UserControl
    {
        public ImageInformationCell()
        {
            this.InitializeComponent();

            this.DataContextChanged += OnDataContextChanged;
        }

        public InformationEntry ViewModel { get; private set; }


        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            this.ViewModel = args.NewValue as InformationEntry;
            Bindings.Update();
        }
    }
}
