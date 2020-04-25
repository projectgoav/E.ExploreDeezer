using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Wpf.UI.XamlHost;

using E.ExploreDeezer.Core;
using E.ExploreDeezer.WPF.Views;
using Windows.Devices.PointOfService;

namespace E.ExploreDeezer.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NavigationView navView;

        public MainWindow()
        {
            InitializeComponent();

            ServiceRegistry.Initialise(new WPFPlatformServices(this.Dispatcher));
            //ServiceRegistry.Register<Frame>(this.MainFrame);

            this.navViewHost2.ChildChanged += OnNavViewHostChildChanged;
        }

        private void OnNavViewHostChildChanged(object sender, EventArgs e)
        {
            // Hook up x:Bind source.
            global::Microsoft.Toolkit.Wpf.UI.XamlHost.WindowsXamlHost windowsXamlHost = sender as global::Microsoft.Toolkit.Wpf.UI.XamlHost.WindowsXamlHost;
            global::E.ExploreDeezer.UI.HostMe userControl = windowsXamlHost.GetUwpInternalObject() as global::E.ExploreDeezer.UI.HostMe;


            /*
            var castSender = sender as WindowsXamlHost;

            this.navView = castSender.Child as NavigationView;
            Assert.That(this.navView != null);

            ConfigureNavView();
            */
        }




        private void ConfigureNavView()
        {
            this.navView.MenuItemsSource = new List<NavigationViewItem>()
            {
                new NavigationViewItem() { Content = "Whats New", Tag = "new" },
                new NavigationViewItem() { Content = "Charts", Tag = "charts" },
            };

            this.navView.PaneTitle = "Explore Deezer";

            this.navView.IsSettingsVisible = false;
            this.navView.IsBackEnabled = true;
        }
    }
}
