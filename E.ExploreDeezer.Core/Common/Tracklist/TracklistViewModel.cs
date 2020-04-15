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
        int NumberOfTracks { get; }
        
        EFetchState FetchState { get; }
        IObservableCollection<ITrackViewModel> Tracklist { get; }
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
        private string artworkUri;
        private int numberOfTracks;
        private EFetchState fetchState;


        public TracklistViewModel(IPlatformServices platformServices,
                                  TracklistViewModelParams p)
            : base(platformServices)
        {
            this.dataController = ServiceRegistry.GetService<ITracklistDataController>();

            this.tracklist = new MainThreadObservableCollectionAdapter<ITrackViewModel>(this.dataController.Tracklist,
                                                                                        PlatformServices.MainThreadDispatcher);

            this.dataController.OnTracklistFetchStateChanged += OnFetchStateChanged;


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

        public int NumberOfTracks
        {
            get => this.numberOfTracks;
            private set => SetProperty(ref this.numberOfTracks, value);
        }

        public EFetchState FetchState
        {
            get => this.fetchState;
            private set => SetProperty(ref this.fetchState, value);
        }

        public IObservableCollection<ITrackViewModel> Tracklist => this.tracklist;


        private void OnFetchStateChanged(object sender, FetchStateChangedEventArgs e)
            => this.FetchState = e.NewValue;



        private void ConfigureViewModelAsAlbum(IAlbumViewModel album)
        {
            this.Title = album.Title;
            this.Subtitle = album.ArtistName;
            this.ArtworkUri = album.ArtworkUri;
            this.NumberOfTracks = (int)album.NumberOfTracks;

            this.dataController.FetchTracklistAsync(ETracklistType.Album, album.ItemId);
        }

        private void ConfigureViewModelAsPlaylist(IPlaylistViewModel playlist)
        {
            this.Title = playlist.Title;
            this.Subtitle = playlist.CreatorName;
            this.ArtworkUri = playlist.ArtworkUri;
            this.NumberOfTracks = (int)playlist.NumberOfTracks;

            this.dataController.FetchTracklistAsync(ETracklistType.Playlist, playlist.ItemId);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dataController.OnTracklistFetchStateChanged -= OnFetchStateChanged;

                this.tracklist.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
