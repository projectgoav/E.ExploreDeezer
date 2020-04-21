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

        uint NumberOfFans { get; }
        uint NumberOfAlbums { get; }
        Uri DeezerWebsiteLink { get; }

        EFetchState AlbumFetchState { get; }
        IObservableCollection<IAlbumViewModel> Albums { get; }

        EFetchState TopTrackFetchState { get; }
        IObservableCollection<ITrackViewModel> TopTracks { get; }

        EFetchState RelatedArtistFetchState { get; }
        IObservableCollection<IArtistViewModel> RelatedArtists { get; }

        EFetchState FeaturedPlaylistFetchState { get; }
        IObservableCollection<IPlaylistViewModel> FeaturedPlaylists { get; }
    }

    public struct ArtistOverviewViewModelParams
    {
        public ArtistOverviewViewModelParams(ulong artistId)
        {
            this.ArtistId = artistId;
        }

        public ulong ArtistId { get; }
    }


    internal class ArtistOverviewViewModel : ViewModelBase,
                                             IArtistOverviewViewModel
                        
    {
        private readonly IArtistOverviewDataController dataController;
        private readonly MainThreadObservableCollectionAdapter<IAlbumViewModel> albums;
        private readonly MainThreadObservableCollectionAdapter<ITrackViewModel> topTracks;
        private readonly MainThreadObservableCollectionAdapter<IArtistViewModel> relatedArtists;
        private readonly MainThreadObservableCollectionAdapter<IPlaylistViewModel> featuredPlaylists;

        private Uri websiteLink;
        private string artistName;
        private string artistImage;
        private uint numberOfFans;
        private uint numberOfAlbums;
        private EFetchState albumFetchState;
        private EFetchState headerFetchState;
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

            // TODO: Default artwork for artist...
            // NEEDS to be done before the fetch status changed is added, as event will fire before we've set fallback
            this.ArtistImage = "ms-appx:///Assets/StoreLogo.png";


            this.dataController.OnAlbumFetchStateChanged += OnAlbumFetchStateChanged;
            this.dataController.OnTopTrackFetchStateChanged += OnTopTrackFetchStateChanged;
            this.dataController.OnPlaylistFetchStateChanged += OnPlaylistFetchStateChanged;
            this.dataController.OnRelatedArtistFetchStateChanged += OnRelatedArtistFetchStateChanged;
            this.dataController.OnCompleteArtistFetchStateChanged += OnCompleteArtistFetchStateChanged;


            this.dataController.FetchOverviewAsync(p.ArtistId);
        }


        // IArtistOverviewViewModel
        public string ArtistName 
        {
            get => this.artistName;
            private set => SetProperty(ref this.artistName, value);
        }

        public string ArtistImage 
        {
            get => this.artistImage;
            private set => SetProperty(ref this.artistImage, value);
        }

        public uint NumberOfFans
        {
            get => this.numberOfFans;
            private set => SetProperty(ref this.numberOfFans, value);
        }

        public uint NumberOfAlbums
        {
            get => this.numberOfAlbums;
            private set => SetProperty(ref this.numberOfAlbums, value);
        }

        public Uri DeezerWebsiteLink
        {
            get => this.websiteLink;
            private set => SetProperty(ref this.websiteLink, value);
        }

        public EFetchState HeaderFetchState
        {
            get => this.headerFetchState;
            private set
            {
                if (SetProperty(ref this.headerFetchState, value))
                {
                    UpdateHeaderProperties();
                }
            }
        }



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

        private void OnCompleteArtistFetchStateChanged(object sender, FetchStateChangedEventArgs e)
            => this.HeaderFetchState = e.NewValue;


        private void UpdateHeaderProperties()
        {
            var artist = this.dataController.CompleteArtist;

            if (artist != null)
            {
                this.ArtistName = artist.Name;
                this.ArtistImage = artist.ArtworkUri;

                this.NumberOfFans = artist.NumberOfFans;
                this.NumberOfAlbums = artist.NumberOfAlbums;
                this.DeezerWebsiteLink = new Uri(artist.WebsiteLink);
            }
            else
            {
                this.NumberOfFans = 0;
                this.NumberOfAlbums = 0;
                this.DeezerWebsiteLink = null;
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dataController.OnAlbumFetchStateChanged -= OnAlbumFetchStateChanged;
                this.dataController.OnTopTrackFetchStateChanged -= OnTopTrackFetchStateChanged;
                this.dataController.OnPlaylistFetchStateChanged -= OnPlaylistFetchStateChanged;
                this.dataController.OnRelatedArtistFetchStateChanged -= OnRelatedArtistFetchStateChanged;
                this.dataController.OnCompleteArtistFetchStateChanged -= OnCompleteArtistFetchStateChanged;

                this.albums.Dispose();
                this.topTracks.Dispose();
                this.relatedArtists.Dispose();
                this.featuredPlaylists.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
