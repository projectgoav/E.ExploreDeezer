using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using E.Deezer;
using E.ExploreDeezer.Mvvm;

namespace E.ExploreDeezer.ViewModels.Home
{
    public interface IChartsViewModel : INotifyPropertyChanged,
                                        IDisposable
    {
        EContentFetchStatus AlbumsFetchStatus { get; }
        EContentFetchStatus ArtistFetchStatus { get; }

        IEnumerable<IAlbumViewModel> Albums { get; }
        IEnumerable<IArtistViewModel> Artists { get; }
    }

    internal class ChartsViewModel : ViewModelBase,
                                     IChartsViewModel,
                                     IDisposable
    {
        private const uint MAX_ITEM_COUNT = 50;


        private readonly IDeezerSession session;

        private EContentFetchStatus albumsStatus;
        private EContentFetchStatus artistStatus;

        private IEnumerable<IAlbumViewModel> albums;
        private IEnumerable<IArtistViewModel> artists;

        public ChartsViewModel(IDeezerSession session,
                               IPlatformServices platformServices)
            : base(platformServices)
        {
            this.session = session;

            FetchContents();
        }



        // IChartViewModel
        public EContentFetchStatus AlbumsFetchStatus
        {
            get => this.albumsStatus;
            private set => SetProperty(ref this.albumsStatus, value);
        }

        public EContentFetchStatus ArtistFetchStatus
        {
            get => this.artistStatus;
            private set => SetProperty(ref this.artistStatus, value);
        }

        public IEnumerable<IAlbumViewModel> Albums
        {
            get => this.albums;
            private set => SetProperty(ref this.albums, value);
        }

        public IEnumerable<IArtistViewModel> Artists
        {
            get => this.artists;
            private set => SetProperty(ref this.artists, value);
        }


        private void FetchContents()
        {
            this.AlbumsFetchStatus = EContentFetchStatus.Loading;
            this.ArtistFetchStatus = EContentFetchStatus.Loading;

            this.session.Charts.GetCharts(this.CancellationToken, 0, MAX_ITEM_COUNT)
                              .ContinueWith(t =>
                              {
                                  if (t.IsFaulted || t.IsCanceled)
                                  {
                                      this.Albums = Array.Empty<IAlbumViewModel>();
                                      this.AlbumsFetchStatus = EContentFetchStatus.Error;
                                  }
                                  else
                                  {
                                      var chart = t.Result;

                                      var albums = chart.Albums.Select(x => new AlbumViewModel(x))
                                                               .ToList();

                                      var artists = chart.Artists.Select(x => new ArtistViewModel(x))
                                                                 .ToList();

                                      this.Albums = albums;
                                      this.Artists = artists;

                                      this.AlbumsFetchStatus = albums.Count == 0 ? EContentFetchStatus.Empty
                                                                                 : EContentFetchStatus.Available;

                                      this.ArtistFetchStatus = artists.Count == 0 ? EContentFetchStatus.Empty
                                                                                  : EContentFetchStatus.Available;
                                  }
                              },
                              this.CancellationToken,
                              TaskContinuationOptions.ExecuteSynchronously,
                              TaskScheduler.Default);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Albums = Array.Empty<IAlbumViewModel>();
            }

            base.Dispose(disposing);
        }

    }
}
