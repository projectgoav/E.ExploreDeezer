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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238


using E.ExploreDeezer.Core.Mvvm;
using E.ExploreDeezer.Core.ViewModels.Home;

namespace E.ExploreDeezer.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GenreView : Page
    {
        public GenreView()
        {
            this.InitializeComponent();
        }


        public IGenreListViewModel ViewModel => this.DataContext as IGenreListViewModel;


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.DataContext = new GenreListViewModel(ServiceRegistry.DeezerSession,
                                                      ServiceRegistry.PlatformServices);
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            this.ViewModel.Dispose();

            this.DataContext = null;
        }
    }
}
