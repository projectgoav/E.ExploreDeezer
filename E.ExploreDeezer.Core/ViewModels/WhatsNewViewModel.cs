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
        IObservableCollection<IAlbumViewModel> NewReleases { get; }
        IObservableCollection<IAlbumViewModel> DeezerPicks { get; }

        TracklistViewModelParams CreateTracklistViewModelParams(IAlbumViewModel albumViewModel);
    }

    internal class WhatsNewViewModel : ViewModelBase,
                                       IWhatsNewViewModel
    {
        private const ulong DEFAULT_GENRE_ID = 0;


        private readonly MainThreadObservableCollectionAdapter<IAlbumViewModel> newReleases;
        private readonly MainThreadObservableCollectionAdapter<IAlbumViewModel> deezerPicks;


        public WhatsNewViewModel(IPlatformServices platformServices)
            : base(platformServices)
        {
            var newReleaseDataController = ServiceRegistry.GetService<NewReleaseDataController>();
            var deezerPicksDataController = ServiceRegistry.GetService<DeezerPicksDataController>();

            this.newReleases = new MainThreadObservableCollectionAdapter<IAlbumViewModel>(newReleaseDataController.NewReleases,
                                                                                          PlatformServices.MainThreadDispatcher);

            this.deezerPicks = new MainThreadObservableCollectionAdapter<IAlbumViewModel>(deezerPicksDataController.DeezerPicks,
                                                                                          PlatformServices.MainThreadDispatcher);

            newReleaseDataController.SetGenreId(DEFAULT_GENRE_ID);
            deezerPicksDataController.SetGenreId(DEFAULT_GENRE_ID);
        }


        // IWhatsNewViewModel
        public IObservableCollection<IAlbumViewModel> NewReleases => this.newReleases;

        public IObservableCollection<IAlbumViewModel> DeezerPicks => this.deezerPicks;


        public TracklistViewModelParams CreateTracklistViewModelParams(IAlbumViewModel albumViewModel)
            => ViewModelParamFactory.CreateTracklistViewModelParams(albumViewModel);


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.newReleases.Dispose();
                this.deezerPicks.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
