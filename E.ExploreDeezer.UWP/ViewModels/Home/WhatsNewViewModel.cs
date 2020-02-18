using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

using E.Deezer;

using E.ExploreDeezer.Mvvm;

namespace E.ExploreDeezer.ViewModels.Home
{
    public interface IWhatsNewViewModel : INotifyPropertyChanged,
                                          IDisposable
    {
        EContentFetchStatus NewAlbumsFetchStatus { get; }
        EContentFetchStatus DeezerPicksFetchStatus { get; }

        IEnumerable<IAlbumViewModel> NewAlbums { get; }
        IEnumerable<IAlbumViewModel> DeezerPicks { get; }
    }

    internal class WhatsNewViewModel : ViewModelBase,
                                       IWhatsNewViewModel,
                                       IDisposable
    {
        private const uint DEFAULT_GENRE_ID = 0;
        private const uint MAX_ITEM_COUNT = 50;

        private readonly IDeezerSession session;

        private IEnumerable<IAlbumViewModel> newAlbums;
        private IEnumerable<IAlbumViewModel> deezerPicks;
        private EContentFetchStatus newAlbumsFetchStatus;
        private EContentFetchStatus deezerPicksFetchStatus;


        public WhatsNewViewModel(IDeezerSession session,
                                 IPlatformServices platformServices)
            : base(platformServices)
        {
            this.session = session;

            FetchContents();
        }


        // IWhatsNewViewModel
        public EContentFetchStatus NewAlbumsFetchStatus
        {
            get => this.newAlbumsFetchStatus;
            private set => SetProperty(ref this.newAlbumsFetchStatus, value);
        }

        public EContentFetchStatus DeezerPicksFetchStatus
        {
            get => this.deezerPicksFetchStatus;
            private set => SetProperty(ref this.deezerPicksFetchStatus, value);
        }

        public IEnumerable<IAlbumViewModel> NewAlbums
        {
            get => this.newAlbums;
            private set => SetProperty(ref this.newAlbums, value);
        }

        public IEnumerable<IAlbumViewModel> DeezerPicks
        {
            get => this.deezerPicks;
            private set => SetProperty(ref this.deezerPicks, value);
        }



        private void FetchContents()
        {
            this.NewAlbumsFetchStatus = EContentFetchStatus.Loading;
            this.DeezerPicksFetchStatus = EContentFetchStatus.Loading;

            this.session.Genre.GetNewReleasesForGenre(DEFAULT_GENRE_ID, this.CancellationToken, 0, MAX_ITEM_COUNT)
                              .ContinueWith(t =>
                              {
                                  if (t.IsFaulted || t.IsCanceled)
                                  {
                                      this.NewAlbums = Array.Empty<IAlbumViewModel>();
                                      this.NewAlbumsFetchStatus = EContentFetchStatus.Error;
                                  }
                                  else
                                  {
                                      var contents = t.Result
                                                      .Select(x => new AlbumViewModel(x))
                                                      .ToList();

                                      this.NewAlbums = contents;

                                      this.NewAlbumsFetchStatus = contents.Count == 0 ? EContentFetchStatus.Empty
                                                                                      : EContentFetchStatus.Available;
                                  }
                              },
                              this.CancellationToken,
                              TaskContinuationOptions.ExecuteSynchronously,
                              TaskScheduler.Default);

            this.session.Genre.GetDeezerSelectionForGenre(DEFAULT_GENRE_ID, this.CancellationToken, 0, MAX_ITEM_COUNT)
                              .ContinueWith(t =>
                              {
                                  if (t.IsFaulted || t.IsCanceled)
                                  {
                                      this.DeezerPicks = Array.Empty<IAlbumViewModel>();
                                      this.DeezerPicksFetchStatus = EContentFetchStatus.Error;
                                  }
                                  else
                                  {
                                      var contents = t.Result
                                                      .Select(x => new AlbumViewModel(x))
                                                      .ToList();

                                      this.DeezerPicks = contents;

                                      this.DeezerPicksFetchStatus = contents.Count == 0 ? EContentFetchStatus.Empty
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
                this.NewAlbums = null;
                this.DeezerPicks = null;
            }

            base.Dispose(disposing);
        }
    }
}
