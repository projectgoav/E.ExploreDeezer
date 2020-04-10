using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

using E.Deezer;

using E.ExploreDeezer.Core;
using E.ExploreDeezer.Core.Mvvm;
using E.ExploreDeezer.Core.Services;
using E.ExploreDeezer.Core.Collections;

namespace E.ExploreDeezer.Core.ViewModels
{
    public interface IWhatsNewViewModel : IDisposable
    {
        IObservableCollection<IGenreViewModel> GenreList { get; }
        IObservableCollection<IAlbumViewModel> NewReleases { get; }
        IObservableCollection<IAlbumViewModel> DeezerPicks { get; }


        void SetSelectedGenre(IGenreViewModel genre);

        TracklistViewModelParams CreateTracklistViewModelParams(IAlbumViewModel albumViewModel);
    }

    internal class WhatsNewViewModel : ViewModelBase,
                                       IWhatsNewViewModel
    {
        private const ulong DEFAULT_GENRE_ID = 0;

        private readonly IGenreListService genreService;

        private readonly NewReleaseDataController newReleaseDataController;
        private readonly DeezerPicksDataController deezerPicksDataController;

        private readonly MainThreadObservableCollectionAdapter<IGenreViewModel> genreList;
        private readonly MainThreadObservableCollectionAdapter<IAlbumViewModel> newReleases;
        private readonly MainThreadObservableCollectionAdapter<IAlbumViewModel> deezerPicks;


        public WhatsNewViewModel(IPlatformServices platformServices)
            : base(platformServices)
        {
            this.genreService = ServiceRegistry.GetService<IGenreListService>();
            this.genreService.RefreshGenreListAsync();

            this.genreList = new MainThreadObservableCollectionAdapter<IGenreViewModel>(this.genreService.GenreList,
                                                                                        PlatformServices.MainThreadDispatcher);

            this.newReleaseDataController = ServiceRegistry.GetService<NewReleaseDataController>();
            this.deezerPicksDataController = ServiceRegistry.GetService<DeezerPicksDataController>();

            this.newReleases = new MainThreadObservableCollectionAdapter<IAlbumViewModel>(newReleaseDataController.NewReleases,
                                                                                          PlatformServices.MainThreadDispatcher);

            this.deezerPicks = new MainThreadObservableCollectionAdapter<IAlbumViewModel>(deezerPicksDataController.DeezerPicks,
                                                                                          PlatformServices.MainThreadDispatcher);

            newReleaseDataController.SetGenreId(DEFAULT_GENRE_ID);
            deezerPicksDataController.SetGenreId(DEFAULT_GENRE_ID);
        }


        // IWhatsNewViewModel
        public IObservableCollection<IGenreViewModel> GenreList => this.genreList;

        public IObservableCollection<IAlbumViewModel> NewReleases => this.newReleases;

        public IObservableCollection<IAlbumViewModel> DeezerPicks => this.deezerPicks;


        public void SetSelectedGenre(IGenreViewModel genre)
        {
            Assert.That(genre != null);

            this.newReleaseDataController.SetGenreId(genre.Id);
            this.deezerPicksDataController.SetGenreId(genre.Id);
        }


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
