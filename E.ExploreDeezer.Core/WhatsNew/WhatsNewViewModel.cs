using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

using E.Deezer;

using E.ExploreDeezer.Core;
using E.ExploreDeezer.Core.Mvvm;
using E.ExploreDeezer.Core.Common;
using E.ExploreDeezer.Core.ViewModels;
using E.ExploreDeezer.Core.Collections;

namespace E.ExploreDeezer.Core.WhatsNew
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
        private readonly IWhatsNewDataController whatsNewDataController;
        private readonly IGenreListDataController genreListDataController;

        private readonly MainThreadObservableCollectionAdapter<IGenreViewModel> genreList;
        private readonly MainThreadObservableCollectionAdapter<IAlbumViewModel> newReleases;
        private readonly MainThreadObservableCollectionAdapter<IAlbumViewModel> deezerPicks;


        public WhatsNewViewModel(IPlatformServices platformServices)
            : base(platformServices)
        {
            this.whatsNewDataController = ServiceRegistry.GetService<IWhatsNewDataController>();
            this.genreListDataController = ServiceRegistry.GetService<IGenreListDataController>();

            this.genreList = new MainThreadObservableCollectionAdapter<IGenreViewModel>(this.genreListDataController.TheList,
                                                                                        PlatformServices.MainThreadDispatcher);

            this.newReleases = new MainThreadObservableCollectionAdapter<IAlbumViewModel>(this.whatsNewDataController.NewReleases,
                                                                                          PlatformServices.MainThreadDispatcher);

            this.deezerPicks = new MainThreadObservableCollectionAdapter<IAlbumViewModel>(this.whatsNewDataController.DeezerPicks,
                                                                                          PlatformServices.MainThreadDispatcher);

            this.whatsNewDataController.BeginPopulateAsync();
            this.genreListDataController.RefreshGenreListAsync();
        }


        // IWhatsNewViewModel
        public IObservableCollection<IGenreViewModel> GenreList => this.genreList;

        public IObservableCollection<IAlbumViewModel> NewReleases => this.newReleases;

        public IObservableCollection<IAlbumViewModel> DeezerPicks => this.deezerPicks;


        public void SetSelectedGenre(IGenreViewModel genre)
        {
            Assert.That(genre != null);

            this.whatsNewDataController.SetGenreFilter(genre.Id);
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
