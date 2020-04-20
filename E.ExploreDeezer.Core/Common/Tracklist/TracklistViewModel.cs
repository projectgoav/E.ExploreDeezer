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

        EFetchState HeaderFetchState { get; }

        uint NumberOfFans { get; }
        uint NumberOfTracks { get; }
        Uri WebsiteLink { get; }
        
        EFetchState FetchState { get; }
        IObservableCollection<ITrackViewModel> Tracklist { get; }

        UserOverviewViewModelParams CreateUserOverviewViewModelParams();
        ArtistOverviewViewModelParams CreateArtistOverviewViewModelParams();
    }

    public enum ETracklistViewModelType : byte
    {
        Album,
        Playlist
    };


    public struct TracklistViewModelParams
    {
        public TracklistViewModelParams(ETracklistViewModelType type,
                                        object item)
        {
            this.Type = type;
            this.Item = item;
        }


        public object Item { get; }
        public ETracklistViewModelType Type { get; }
    }


    internal static class TracklistViewModelParamsExtensions
    {
        public static IAlbumViewModel ItemAsAlbum(this TracklistViewModelParams p)
        {
            Assert.That(p.Type == ETracklistViewModelType.Album);
            return p.Item as IAlbumViewModel;
        }

        public static IPlaylistViewModel ItemAsPlaylist(this TracklistViewModelParams p)
        {
            Assert.That(p.Type == ETracklistViewModelType.Playlist);
            return p.Item as IPlaylistViewModel;
        }
    }



    internal class TracklistViewModel : ViewModelBase,
                                        ITracklistViewModel
    {
        private readonly ITracklistDataController dataController;
        private readonly MainThreadObservableCollectionAdapter<ITrackViewModel> tracklist;


        private string title;
        private string subtitle;
        private Uri websiteLink;
        private string artworkUri;
        private uint numberOfFans;
        private uint numberOfTracks;
        private EFetchState fetchState;
        private EFetchState headerFetchState;


        public TracklistViewModel(IPlatformServices platformServices,
                                  TracklistViewModelParams p)
            : base(platformServices)
        {
            this.dataController = ServiceRegistry.GetService<ITracklistDataController>();

            this.tracklist = new MainThreadObservableCollectionAdapter<ITrackViewModel>(this.dataController.Tracklist,
                                                                                        PlatformServices.MainThreadDispatcher);

            this.Type = p.Type;
            switch (this.Type)
            {
                case ETracklistViewModelType.Album:
                    ConfigureViewModelAsAlbum(p.ItemAsAlbum());
                    break;

                case ETracklistViewModelType.Playlist:
                    ConfigureViewModelAsPlaylist(p.ItemAsPlaylist());
                    break;
            }

            this.dataController.OnTracklistFetchStateChanged += OnFetchStateChanged;
            this.dataController.OnCompleteItemFetchStateChanged += OnHeaderFetchStateChanged;
        }

        // ITracklistViewModel
        public ETracklistViewModelType Type { get; }

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



        private void OnFetchStateChanged(object sender, FetchStateChangedEventArgs e)
            => this.FetchState = e.NewValue;

        private void OnHeaderFetchStateChanged(object sender, FetchStateChangedEventArgs e)
            => this.HeaderFetchState = e.NewValue;


        private void ConfigureViewModelAsAlbum(IAlbumViewModel album)
        {
            this.Title = album.Title;
            this.Subtitle = album.ArtistName;
            this.ArtworkUri = album.ArtworkUri;

            this.dataController.FetchTracklistAsync(ETracklistType.Album, album.ItemId);
        }

        private void ConfigureViewModelAsPlaylist(IPlaylistViewModel playlist)
        {
            this.Title = playlist.Title;
            this.Subtitle = playlist.CreatorName;
            this.ArtworkUri = playlist.ArtworkUri;

            this.dataController.FetchTracklistAsync(ETracklistType.Playlist, playlist.ItemId);
        }

        private void UpdateHeaderProperties()
        {
            switch(this.Type)
            {
                case ETracklistViewModelType.Album:
                    if (this.dataController.CompleteAlbum != null)
                    {
                        this.Title = this.dataController.CompleteAlbum.Title;
                        this.Subtitle = this.dataController.CompleteAlbum.ArtistName;

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


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dataController.OnTracklistFetchStateChanged -= OnFetchStateChanged;
                this.dataController.OnCompleteItemFetchStateChanged -= OnHeaderFetchStateChanged;

                this.tracklist.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
