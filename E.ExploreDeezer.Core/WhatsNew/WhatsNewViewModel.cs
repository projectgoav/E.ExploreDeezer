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
        EFetchState GenreListFetchState { get; }
        IObservableCollection<IGenreViewModel> GenreList { get; }

        EFetchState NewReleaseFetchState { get; }
        IObservableCollection<IAlbumViewModel> NewReleases { get; }

        EFetchState DeezerPicksFetchState { get; }
        IObservableCollection<IAlbumViewModel> DeezerPicks { get; }

        int SelectedGenreIndex { get; set; }
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

        private int selectedGenreIndex;
        private EFetchState genreListFetchState;
        private EFetchState newReleaseFetchState;
        private EFetchState deezerPicksFetchState;


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

            this.genreListDataController.OnFetchStateChanged += OnGenreListFetchStateChanged;

            this.whatsNewDataController.OnNewReleaseFetchStateChanged += OnNewReleaseFetchStateChanged;
            this.whatsNewDataController.OnDeezerPicksFetchStateChanged += OnDeezerPicksFetchStateChanged;

            this.selectedGenreIndex = this.genreList.Count == 0 ? 0
                                                                : this.genreList.Select((x, i) => new { Genre = x, Index = i })
                                                                                .Where(x => x.Genre.Id == this.whatsNewDataController.CurrentGenreFilter)
                                                                                .Single()
                                                                                .Index;
                                                    
            this.whatsNewDataController.BeginPopulateAsync();
            this.genreListDataController.RefreshGenreListAsync();
        }


        // IWhatsNewViewModel
        public EFetchState GenreListFetchState
        {
            get => this.genreListFetchState;
            private set => SetProperty(ref this.genreListFetchState, value);
        }

        public IObservableCollection<IGenreViewModel> GenreList => this.genreList;

        public EFetchState NewReleaseFetchState
        {
            get => this.newReleaseFetchState;
            private set => SetProperty(ref this.newReleaseFetchState, value);
        }

        public IObservableCollection<IAlbumViewModel> NewReleases => this.newReleases;

        public EFetchState DeezerPicksFetchState
        {
            get => this.deezerPicksFetchState;
            private set => SetProperty(ref this.deezerPicksFetchState, value);
        }

        public IObservableCollection<IAlbumViewModel> DeezerPicks => this.deezerPicks;


        public int SelectedGenreIndex
        {
            get => this.selectedGenreIndex;
            set 
            {
                if (SetProperty(ref this.selectedGenreIndex, value))
                {
                    this.whatsNewDataController.SetGenreFilter(this.genreList.GetItem(value).Id);
                }
            }
        }



        public TracklistViewModelParams CreateTracklistViewModelParams(IAlbumViewModel albumViewModel)
            => ViewModelParamFactory.CreateTracklistViewModelParams(albumViewModel);


        private void OnGenreListFetchStateChanged(object sender, FetchStateChangedEventArgs args)
            => this.GenreListFetchState = args.NewValue;


        private void OnNewReleaseFetchStateChanged(object sender, FetchStateChangedEventArgs args)
            => this.NewReleaseFetchState = args.NewValue;

        private void OnDeezerPicksFetchStateChanged(object sender, FetchStateChangedEventArgs args)
            => this.DeezerPicksFetchState = args.NewValue;


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.genreListDataController.OnFetchStateChanged -= OnGenreListFetchStateChanged;

                this.whatsNewDataController.OnNewReleaseFetchStateChanged -= OnNewReleaseFetchStateChanged;
                this.whatsNewDataController.OnDeezerPicksFetchStateChanged -= OnDeezerPicksFetchStateChanged;

                this.newReleases.Dispose();
                this.deezerPicks.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
