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

using E.ExploreDeezer.ViewModels.Home;

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
        }



        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            this.ViewModel.Dispose();

            this.DataContext = null;
        }
    }
}
