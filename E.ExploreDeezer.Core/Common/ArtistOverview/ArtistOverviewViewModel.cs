using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

using E.ExploreDeezer.Core.Mvvm;
using E.ExploreDeezer.Core.ViewModels;
using E.ExploreDeezer.Core.Collections;

namespace E.ExploreDeezer.Core.Common
{
    public interface IArtistOverviewViewModel
    {
        string ArtistName { get; }
        string ArtistImage { get; }

        EFetchState AlbumFetchState { get; }
        IObservableCollection<IAlbumViewModel> Albums { get; }

        EFetchState TopTrackFetchState { get; }
        IObservableCollection<ITrackViewModel> TopTracks { get; }

        EFetchState RelatedArtistFetchState { get; }
        IObservableCollection<IArtistViewModel> RelatedArtists { get; }

        EFetchState FeaturedPlaylistFetchState { get; }
        IObservableCollection<IPlaylistViewModel> FeaturedPlaylists { get; }

        TracklistViewModelParams CreateTracklistViewModelParams(IAlbumViewModel album);
        TracklistViewModelParams CreateTracklistViewModelParams(IPlaylistViewModel playlist);
        ArtistOverviewViewModelParams CreateArtistOverviewViewModelParams(IArtistViewModel artist);
    }

    public struct ArtistOverviewViewModelParams
    {
        public ArtistOverviewViewModelParams(IArtistViewModel artistViewModel)
        {
            this.Artist = artistViewModel;
        }

        public IArtistViewModel Artist { get; }
    }


    internal class ArtistOverviewViewModel : ViewModelBase,
                                             IArtistOverviewViewModel
                        
    {
        private readonly IArtistViewModel artist;
        private readonly IArtistOverviewDataController dataController;
        private readonly MainThreadObservableCollectionAdapter<IAlbumViewModel> albums;
        private readonly MainThreadObservableCollectionAdapter<ITrackViewModel> topTracks;
        private readonly MainThreadObservableCollectionAdapter<IArtistViewModel> relatedArtists;
        private readonly MainThreadObservableCollectionAdapter<IPlaylistViewModel> featuredPlaylists;

        private EFetchState albumFetchState;
        private EFetchState topTrackFetchState;
        private EFetchState playlistFetchState;
        private EFetchState relatedArtistsFetchState;


        public ArtistOverviewViewModel(IPlatformServices platformServices,
                                       ArtistOverviewViewModelParams p)
            : base(platformServices)
        {
            this.dataController = ServiceRegistry.GetService<IArtistOverviewDataController>();

            this.albums = new MainThreadObservableCollectionAdapter<IAlbumViewModel>(this.dataController.Albums,
                                                                                     PlatformServices.MainThreadDispatcher);

            this.topTracks = new MainThreadObservableCollectionAdapter<ITrackViewModel>(this.dataController.TopTracks,
                                                                                        PlatformServices.MainThreadDispatcher);

            this.relatedArtists = new MainThreadObservableCollectionAdapter<IArtistViewModel>(this.dataController.RelatedArtists,
                                                                                              PlatformServices.MainThreadDispatcher);

            this.featuredPlaylists = new MainThreadObservableCollectionAdapter<IPlaylistViewModel>(this.dataController.Playlists,
                                                                                                   PlatformServices.MainThreadDispatcher);


            this.dataController.OnAlbumFetchStateChanged += OnAlbumFetchStateChanged;
            this.dataController.OnTopTrackFetchStateChanged += OnTopTrackFetchStateChanged;
            this.dataController.OnPlaylistFetchStateChanged += OnPlaylistFetchStateChanged;
            this.dataController.OnRelatedArtistFetchStateChanged += OnRelatedArtistFetchStateChanged;


            this.artist = p.Artist;

            this.ArtistName = this.artist.Name;
            this.ArtistImage = this.artist.ArtworkUri;

            this.dataController.FetchOverviewAsync(p.Artist.Id);
        }


        // IArtistOverviewViewModel
        public string ArtistName { get; }
        public string ArtistImage { get; }

        public EFetchState AlbumFetchState
        {
            get => this.albumFetchState;
            private set => SetProperty(ref this.albumFetchState, value);
        }

        public IObservableCollection<IAlbumViewModel> Albums => this.albums;


        public EFetchState TopTrackFetchState
        {
            get => this.topTrackFetchState;
            private set => SetProperty(ref this.topTrackFetchState, value);
        }

        public IObservableCollection<ITrackViewModel> TopTracks => this.topTracks;


        public EFetchState FeaturedPlaylistFetchState
        {
            get => this.playlistFetchState;
            private set => SetProperty(ref this.playlistFetchState, value);
        }

        public IObservableCollection<IPlaylistViewModel> FeaturedPlaylists => this.featuredPlaylists;


        public EFetchState RelatedArtistFetchState
        {
            get => this.relatedArtistsFetchState;
            private set => SetProperty(ref this.relatedArtistsFetchState, value);
        }

        public IObservableCollection<IArtistViewModel> RelatedArtists => this.relatedArtists;



        private void OnPlaylistFetchStateChanged(object sender, FetchStateChangedEventArgs e)
            => this.FeaturedPlaylistFetchState = e.NewValue;

        private void OnRelatedArtistFetchStateChanged(object sender, FetchStateChangedEventArgs e)
            => this.RelatedArtistFetchState = e.NewValue;


        private void OnTopTrackFetchStateChanged(object sender, FetchStateChangedEventArgs e)
            => this.TopTrackFetchState = e.NewValue;

        private void OnAlbumFetchStateChanged(object sender, FetchStateChangedEventArgs e)
            => this.AlbumFetchState = e.NewValue;



        public TracklistViewModelParams CreateTracklistViewModelParams(IAlbumViewModel album)
            => ViewModelParamFactory.CreateTracklistViewModelParams(album);

        public TracklistViewModelParams CreateTracklistViewModelParams(IPlaylistViewModel playlist)
            => ViewModelParamFactory.CreateTracklistViewModelParams(playlist);

        public ArtistOverviewViewModelParams CreateArtistOverviewViewModelParams(IArtistViewModel artist)
            => ViewModelParamFactory.CreateArtistOverviewViewModelParams(artist);


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dataController.OnAlbumFetchStateChanged -= OnAlbumFetchStateChanged;
                this.dataController.OnTopTrackFetchStateChanged -= OnTopTrackFetchStateChanged;
                this.dataController.OnPlaylistFetchStateChanged -= OnPlaylistFetchStateChanged;
                this.dataController.OnRelatedArtistFetchStateChanged -= OnRelatedArtistFetchStateChanged;

                this.albums.Dispose();
                this.topTracks.Dispose();
                this.relatedArtists.Dispose();
                this.featuredPlaylists.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
