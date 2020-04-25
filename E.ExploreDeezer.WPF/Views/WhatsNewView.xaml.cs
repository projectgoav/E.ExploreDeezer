using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using E.ExploreDeezer.Core;
using E.ExploreDeezer.Core.ViewModels;

namespace E.ExploreDeezer.WPF.Views
{
    /// <summary>
    /// Interaction logic for WhatsNewView.xaml
    /// </summary>
    public partial class WhatsNewView : Page
    {
        public WhatsNewView()
        {
            InitializeComponent();

            this.DataContext = ServiceRegistry.ViewModelFactory.CreateWhatsNewViewModel();

            //this.NavigationService.Navigated += NavigationService_Navigated;
        }

        private void NavigationService_Navigated(object sender, NavigationEventArgs e)
        {
            var c = e.Content;
            var d = e.Uri;

            System.Diagnostics.Debug.WriteLine("...");
        }
    }
}
