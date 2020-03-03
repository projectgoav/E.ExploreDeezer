using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

using System.Reactive.Linq;

using E.Deezer;

using E.ExploreDeezer.Core.Mvvm;
using E.ExploreDeezer.Core.ViewModels;
using E.ExploreDeezer.Core.NewReleases.Store;

namespace E.ExploreDeezer.Core.NewReleases.ViewModels
{
    public interface IWhatsNewViewModel : INotifyPropertyChanged,
                                          IDisposable
    {
        EContentFetchStatus NewAlbumsFetchStatus { get; }
        EContentFetchStatus DeezerPicksFetchStatus { get; }

        IEnumerable<IAlbumViewModel> NewAlbums { get; }
        IEnumerable<IAlbumViewModel> DeezerPicks { get; }

        ITracklistViewModelParams GetTracklistViewModelParams(IAlbumViewModel viewModel);
    }

    internal class WhatsNewViewModel : ViewModelBase,
                                       IWhatsNewViewModel,
                                       IDisposable
    {
        private const uint DEFAULT_GENRE_ID = 0;
        private const uint MAX_ITEM_COUNT = 50;

        private readonly IDeezerSession session;
        private readonly IDisposable storeSubscription;

        private IEnumerable<IAlbumViewModel> newAlbums;
        private IEnumerable<IAlbumViewModel> deezerPicks;
        private EContentFetchStatus newAlbumsFetchStatus;
        private EContentFetchStatus deezerPicksFetchStatus;


        public WhatsNewViewModel(IDeezerSession session,
                                 IPlatformServices platformServices)
            : base(platformServices)
        {
            this.session = session;

            this.storeSubscription = ServiceRegistry.Store
                                                    .Select(state => state.NewReleases)
                                                    .DistinctUntilChanged()
                                                    .Subscribe(UpdateViewModel);

            ServiceRegistry.NewReleasesManager.FetchNewReleases();
            ServiceRegistry.NewReleasesManager.FetchDeezerPicks();
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


        public ITracklistViewModelParams GetTracklistViewModelParams(IAlbumViewModel viewModel)
        {
            if (!viewModel.IsPresent)
                throw new ArgumentException();

            return new TracklistViewModelParams(ETracklistViewModelType.Album, viewModel);
        }


        private void UpdateViewModel(NewReleaseState state)
        {
            try
            {
                this.NewAlbums = state.NewReleases;
                this.NewAlbumsFetchStatus = state.NewReleaseFetchStatus;

                this.DeezerPicks = state.DeezerPicks;
                this.DeezerPicksFetchStatus = state.DeezerPicksFetchStatus;
            }
            catch (Exception e)
            {
                //TODO: Log something please...
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.storeSubscription.Dispose();

                this.NewAlbums = Array.Empty<IAlbumViewModel>();
                this.DeezerPicks = Array.Empty<IAlbumViewModel>();
            }

            base.Dispose(disposing);
        }
    }
}
