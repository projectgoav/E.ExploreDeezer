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


using System.Net.Http;
using System.Threading;


using E.Deezer;

using E.ExploreDeezer.Services;

namespace E.ExploreDeezer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IDeezerSession session;


        private readonly GenreService genreService;

        public MainWindow()
        {
            InitializeComponent();

            this.session = new DeezerSession(new HttpClientHandler());

            this.genreService = new GenreService(this.session);



            var genre = this.genreService.GetCommonGenreAsync()
                                         .Result;

            System.Diagnostics.Debug.WriteLine("Genre Ids:");
            foreach(var g in genre)
            {
                System.Diagnostics.Debug.WriteLine("> {0}", g.Id);
            }

        }
    }
}
