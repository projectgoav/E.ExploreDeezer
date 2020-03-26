using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

using E.Deezer;

using E.ExploreDeezer.Core;
using E.ExploreDeezer.Core.Mvvm;

namespace E.ExploreDeezer.Core.ViewModels
{
    public interface IWhatsNewViewModel : IDisposable
    {
        IEnumerable<IAlbumViewModel> NewReleases { get; }
        IEnumerable<IAlbumViewModel> DeezerPicks { get; }

        TracklistViewModelParams CreateTracklistViewModelParams(IAlbumViewModel albumViewModel);
    }

    internal class WhatsNewViewModel : ViewModelBase,
                                       IWhatsNewViewModel
    {
        private const uint MAX_NEW_RELEASES = 50;
        private const uint MAX_DEEZER_PICKS = 25;
        private const ulong DEFAULT_GENRE_ID = 0;

        private readonly IDeezerSession session;

        private IEnumerable<IAlbumViewModel> newReleases;
        private IEnumerable<IAlbumViewModel> deezerPicks;

        public WhatsNewViewModel(IDeezerSession session,
                                 IPlatformServices platformServices)
            : base(platformServices)
        {
            this.session = session;

            this.newReleases = Array.Empty<IAlbumViewModel>();
            this.deezerPicks = Array.Empty<IAlbumViewModel>();

            FetchContent();
        }


        // IWhatsNewViewModel
        public IEnumerable<IAlbumViewModel> NewReleases
        {
            get => this.newReleases;
            private set => SetProperty(ref this.newReleases, value);
        }

        public IEnumerable<IAlbumViewModel> DeezerPicks
        {
            get => this.deezerPicks;
            private set => SetProperty(ref this.deezerPicks, value);
        }


        public TracklistViewModelParams CreateTracklistViewModelParams(IAlbumViewModel albumViewModel)
            => ViewModelParamFactory.CreateTracklistViewModelParams(albumViewModel);


        private void FetchContent()
        {
            this.session.Genre.GetNewReleasesForGenre(DEFAULT_GENRE_ID, this.CancellationToken, 0, MAX_NEW_RELEASES)
                              .ContinueWith(t =>
                              {
                                  if (t.IsFaulted)
                                      return; //TODO

                                  this.NewReleases = t.Result.Select(x => new AlbumViewModel(x))
                                                             .ToList();
                              }, this.CancellationToken, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

            this.session.Genre.GetDeezerSelectionForGenre(DEFAULT_GENRE_ID, this.CancellationToken, 0, MAX_DEEZER_PICKS)
                              .ContinueWith(t =>
                              {
                                  if (t.IsFaulted)
                                      return; //TODO

                                  this.DeezerPicks = t.Result.Select(x => new AlbumViewModel(x))
                                                             .ToList();

                              }, this.CancellationToken, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.NewReleases = Array.Empty<IAlbumViewModel>();
                this.DeezerPicks = Array.Empty<IAlbumViewModel>();
            }

            base.Dispose(disposing);
        }
    }
}
