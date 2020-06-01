using System;
using System.Collections.Generic;
using System.Text;

using E.ExploreDeezer.Core.Mvvm;
using E.ExploreDeezer.Core.ViewModels;
using E.ExploreDeezer.Core.Collections;

namespace E.ExploreDeezer.Core.Common
{
    public interface ITracklistViewModel
    {
        ETracklistViewModelType Type { get; }

        string Title { get; }
        string Subtitle { get; }
        string ArtworkUri { get; }

        bool CanFavourite { get; }
        bool IsFavourited { get; }

        EFetchState HeaderFetchState { get; }

        uint NumberOfFans { get; }
        uint NumberOfTracks { get; }
        Uri WebsiteLink { get; }
        
        EFetchState FetchState { get; }
        IObservableCollection<ITrackViewModel> Tracklist { get; }

        UserOverviewViewModelParams CreateUserOverviewViewModelParams();
        ArtistOverviewViewModelParams CreateArtistOverviewViewModelParams();
        ArtistOverviewViewModelParams CreateArtistOverviewViewModelParams(ulong artistId);
    }

    public enum ETracklistViewModelType : byte
    {
        Album,
        Playlist
    };


    public struct TracklistViewModelParams
    {
        public TracklistViewModelParams(ETracklistViewModelType type,
                                        ulong itemId)
        {
            this.Type = type;
            this.ItemId = itemId;
        }


        public ulong ItemId { get; }
        public ETracklistViewModelType Type { get; }
    }

    internal class TracklistViewModel : ViewModelBase,
                                        ITracklistViewModel
    {
        private readonly IFavouritesService favouritesService;
        private readonly ITracklistDataController dataController;
        private readonly MainThreadObservableCollectionAdapter<ITrackViewModel> tracklist;


        private string title;
        private string subtitle;
        private Uri websiteLink;
        private bool canFavourite;
        private bool isFavourited;
        private string artworkUri;
        private uint numberOfFans;
        private uint numberOfTracks;
        private EFetchState fetchState;
        private EFetchState headerFetchState;


        public TracklistViewModel(IPlatformServices platformServices,
                                  ITracklistDataController dataController,
                                  IFavouritesService favouritesService,
                                  TracklistViewModelParams p)
            : base(platformServices)
        {
            this.dataController = dataController;
            this.favouritesService = favouritesService;

            this.tracklist = new MainThreadObservableCollectionAdapter<ITrackViewModel>(this.dataController.Tracklist,
                                                                                        PlatformServices.MainThreadDispatcher);

            this.ItemId = p.ItemId;
            this.Type = p.Type;

            this.Title = string.Empty;
            this.Subtitle = string.Empty;
            this.ArtworkUri = "ms-appx://Assets/StoreLogo.png";
            this.WebsiteLink = null;

            switch (this.Type)
            {
                case ETracklistViewModelType.Album:
                    this.dataController.FetchTracklistAsync(ETracklistType.Album, this.ItemId);
                    break;

                case ETracklistViewModelType.Playlist:
                    this.dataController.FetchTracklistAsync(ETracklistType.Playlist, this.ItemId);
                    break;
            }

            this.dataController.OnTracklistFetchStateChanged += OnFetchStateChanged;
            this.dataController.OnCompleteItemFetchStateChanged += OnHeaderFetchStateChanged;

            this.favouritesService.OnFavouritesChanged += OnFavouritesChanged;

            UpdateFavouriteState();
        }


        // ITracklistViewModel
        public ulong ItemId { get; }
        public ETracklistViewModelType Type { get; }

        public bool CanFavourite
        {
            get => this.canFavourite;
            private set => SetProperty(ref this.canFavourite, value);
        }

        public bool IsFavourited
        {
            get => this.isFavourited;
            private set => SetProperty(ref this.isFavourited, value);
        }

        public string Title
        {
            get => this.title;
            private set => SetProperty(ref this.title, value);
        }

        public string Subtitle
        {
            get => this.subtitle;
            private set => SetProperty(ref this.subtitle, value);
        }

        public string ArtworkUri
        {
            get => this.artworkUri;
            private set => SetProperty(ref this.artworkUri, value);
        }

        public uint NumberOfTracks
        {
            get => this.numberOfTracks;
            private set => SetProperty(ref this.numberOfTracks, value);
        }

        public uint NumberOfFans
        {
            get => this.numberOfFans;
            private set => SetProperty(ref this.numberOfFans, value);
        }

        public Uri WebsiteLink
        {
            get => this.websiteLink;
            private set => SetProperty(ref this.websiteLink, value);
        }

        public EFetchState FetchState
        {
            get => this.fetchState;
            private set => SetProperty(ref this.fetchState, value);
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

        public IObservableCollection<ITrackViewModel> Tracklist => this.tracklist;


        public UserOverviewViewModelParams CreateUserOverviewViewModelParams()
        {
            Assert.That(this.Type == ETracklistViewModelType.Playlist);
            return new UserOverviewViewModelParams(this.dataController.CompletePlaylist.CreatorId);
        }

        public ArtistOverviewViewModelParams CreateArtistOverviewViewModelParams()
        {
            Assert.That(this.Type == ETracklistViewModelType.Album);
            return new ArtistOverviewViewModelParams(this.dataController.CompleteAlbum.ArtistId);
        }

        public ArtistOverviewViewModelParams CreateArtistOverviewViewModelParams(ulong artistId)
            => new ArtistOverviewViewModelParams(artistId);



        private void OnFetchStateChanged(object sender, FetchStateChangedEventArgs e)
            => this.FetchState = e.NewValue;

        private void OnHeaderFetchStateChanged(object sender, FetchStateChangedEventArgs e)
            => this.HeaderFetchState = e.NewValue;

        private void UpdateHeaderProperties()
        {
            switch(this.Type)
            {
                case ETracklistViewModelType.Album:
                    if (this.dataController.CompleteAlbum != null)
                    {
                        this.Title = this.dataController.CompleteAlbum.Title;
                        this.Subtitle = this.dataController.CompleteAlbum.ArtistName;
                        this.ArtworkUri = this.dataController.CompleteAlbum.ArtworkUri;

                        this.NumberOfFans = this.dataController.CompleteAlbum.NumberOfFans;
                        this.NumberOfTracks = this.dataController.CompleteAlbum.NumberOfTracks;
                        this.WebsiteLink = new Uri(this.dataController.CompleteAlbum.WebsiteLink);
                        return;
                    }
                    break;

                case ETracklistViewModelType.Playlist:
                    if (this.dataController.CompletePlaylist != null)
                    {
                        this.Title = this.dataController.CompletePlaylist.Title;
                        this.Subtitle = this.dataController.CompletePlaylist.CreatorName;
                        this.ArtworkUri = this.dataController.CompletePlaylist.ArtworkUri;

                        this.NumberOfFans = this.dataController.CompletePlaylist.NumberOfFans;
                        this.NumberOfTracks = this.dataController.CompletePlaylist.NumberOfTracks;
                        this.WebsiteLink = new Uri(this.dataController.CompletePlaylist.WebsiteLink);
                        return;
                    }
                    break;
            }

            // No item
            this.NumberOfFans = 0;
            this.NumberOfTracks = 0;
            this.WebsiteLink = null;
        }


        private void OnFavouritesChanged(object sender)
            => UpdateFavouriteState();

        private void UpdateFavouriteState()
        {
            CanFavourite = this.favouritesService.CanFavourite(this.ItemId);
            IsFavourited = this.favouritesService.IsFavourited(this.ItemId);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dataController.OnTracklistFetchStateChanged -= OnFetchStateChanged;
                this.dataController.OnCompleteItemFetchStateChanged -= OnHeaderFetchStateChanged;

                this.favouritesService.OnFavouritesChanged -= OnFavouritesChanged;

                this.tracklist.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
