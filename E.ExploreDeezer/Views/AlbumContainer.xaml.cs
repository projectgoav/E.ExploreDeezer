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

using System.Net.Http;
using System.Threading;

using E.ExploreDeezer.ViewModels;


namespace E.ExploreDeezer.Views
{
    /// <summary>
    /// Interaction logic for AlbumContainer.xaml
    /// </summary>
    public partial class AlbumContainer : UserControl
    {
        //TODO: This should be done as part of a service most likely...
        private static readonly HttpClient ARTWORK_CLIENT = new HttpClient(new HttpClientHandler());


        private CancellationTokenSource cancellationTokenSource;


        public AlbumContainer()
        {
            InitializeComponent();

            DataContextChanged += OnDataContextChanged;
        }


        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.cancellationTokenSource != null)
            {
                this.cancellationTokenSource.Cancel();
                this.cancellationTokenSource.Dispose();
                this.cancellationTokenSource = null;
            }

            if (this.DataContext == null)
                return;

            var album = this.DataContext as IAlbumViewModel;

            string artworkUri = album.ArtworkUri;
            if (string.IsNullOrEmpty(artworkUri))
                return; //TODO: Fill with a solid colour or something??

            this.cancellationTokenSource = new CancellationTokenSource();
            var token = this.cancellationTokenSource.Token;

            ARTWORK_CLIENT.GetAsync(artworkUri)
                          .ContinueWith(async t =>
                          {
                              if (t.IsFaulted)
                                  return; //TODO: Fill with a solid colour or something??


                              var artworkStream = await t.Result.Content.ReadAsStreamAsync();
                                                            
                              var artworkBitmap = BitmapFrame.Create(artworkStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);

                              this.Dispatcher.Invoke(() =>
                              {
                                  if (!token.IsCancellationRequested)
                                  {
                                      this.Artwork.Source = artworkBitmap;
                                  }
                              });
                          }, token);
        }
    }
}
