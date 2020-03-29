using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

using E.Deezer;

using E.ExploreDeezer.Core;
using E.ExploreDeezer.Core.Mvvm;
using E.ExploreDeezer.Core.Collections;

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
        private const uint MAX_DEEZER_PICKS = 25;
        private const ulong DEFAULT_GENRE_ID = 0;

        private readonly IDeezerSession session;
        private readonly NewReleaseDataController newReleaseDataController;

        private readonly MainThreadObservableCollectionAdapter<IAlbumViewModel> newReleases;

        private IEnumerable<IAlbumViewModel> deezerPicks;

        public WhatsNewViewModel(IDeezerSession session,
                                 IPlatformServices platformServices)
            : base(platformServices)
        {
            this.session = session;

            this.deezerPicks = Array.Empty<IAlbumViewModel>();

            this.newReleaseDataController = ServiceRegistry.GetService<NewReleaseDataController>();

            this.newReleases = new MainThreadObservableCollectionAdapter<IAlbumViewModel>(this.newReleaseDataController.NewReleases,
                                                                                          PlatformServices.MainThreadDispatcher);

            this.newReleaseDataController.SetGenreId(DEFAULT_GENRE_ID);

            FetchContent();
        }


        // IWhatsNewViewModel
        public IEnumerable<IAlbumViewModel> NewReleases => this.newReleases;

        public IEnumerable<IAlbumViewModel> DeezerPicks
        {
            get => this.deezerPicks;
            private set => SetProperty(ref this.deezerPicks, value);
        }


        public TracklistViewModelParams CreateTracklistViewModelParams(IAlbumViewModel albumViewModel)
            => ViewModelParamFactory.CreateTracklistViewModelParams(albumViewModel);


        private void FetchContent()
        {
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
                this.DeezerPicks = Array.Empty<IAlbumViewModel>();

                this.newReleases.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
