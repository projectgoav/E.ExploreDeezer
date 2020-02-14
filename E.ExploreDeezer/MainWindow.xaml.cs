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
using System.Windows.Controls.Primitives;


using System.Net.Http;
using System.Threading;


using E.Deezer;

using E.ExploreDeezer.Mvvm;
using E.ExploreDeezer.ViewModels.Home;

namespace E.ExploreDeezer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IDeezerSession session;
        private readonly IPlatformServices platformServices;

        public MainWindow()
        {
            InitializeComponent();

            this.session = new DeezerSession(new HttpClientHandler());
            this.platformServices = new WPFPlatformServices(this.Dispatcher);

            this.DataContext = new WhatsNewViewModel(this.session, this.platformServices);

        }

        public IWhatsNewViewModel ViewModel => this.DataContext as IWhatsNewViewModel;

    }
}
