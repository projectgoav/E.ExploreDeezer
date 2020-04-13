using System;
using System.Collections.Generic;
using System.Text;

using E.ExploreDeezer.Core.Mvvm;
using E.ExploreDeezer.Core.Common;
using E.ExploreDeezer.Core.ViewModels;
using E.ExploreDeezer.Core.Collections;

namespace E.ExploreDeezer.Core.Search
{
    public interface ISearchViewModel
    {
        string CurrentQuery { get; set; }

        IObservableCollection<IAlbumViewModel> Albums { get; }

        IObservableCollection<ITrackViewModel> Tracks { get; }

        IObservableCollection<IArtistViewModel> Artists { get; }

        IObservableCollection<IPlaylistViewModel> Playlists { get; }


        TracklistViewModelParams CreateTracklistViewModelParams(IAlbumViewModel albumViewModel);

        TracklistViewModelParams CreateTracklistViewModelParams(IPlaylistViewModel playlistViewModel);

        ArtistOverviewViewModelParams CreateArtistOverviewViewModelParams(IArtistViewModel artistViewModel);
    }

    internal class SearchViewModel : ViewModelBase,
                                     ISearchViewModel
    {
        private readonly ISearchDataController dataController;
        private readonly MainThreadObservableCollectionAdapter<IAlbumViewModel> albums;
        private readonly MainThreadObservableCollectionAdapter<ITrackViewModel> tracks;
        private readonly MainThreadObservableCollectionAdapter<IArtistViewModel> artists;
        private readonly MainThreadObservableCollectionAdapter<IPlaylistViewModel> playlists;

        private string currentQuery;

        public SearchViewModel(IPlatformServices platformServices)
            : base(platformServices)
        {

            this.dataController = ServiceRegistry.GetService<ISearchDataController>();

            this.albums = new MainThreadObservableCollectionAdapter<IAlbumViewModel>(this.dataController.AlbumResults,
                                                                                     PlatformServices.MainThreadDispatcher);

            this.tracks = new MainThreadObservableCollectionAdapter<ITrackViewModel>(this.dataController.TrackResults,
                                                                                     PlatformServices.MainThreadDispatcher);

            this.artists = new MainThreadObservableCollectionAdapter<IArtistViewModel>(this.dataController.ArtistResults,
                                                                                       PlatformServices.MainThreadDispatcher);

            this.playlists = new MainThreadObservableCollectionAdapter<IPlaylistViewModel>(this.dataController.PlaylistResults,
                                                                                           PlatformServices.MainThreadDispatcher);
        }


        // ISearchViewModel
        public string CurrentQuery
        {
            get => this.currentQuery;
            set
            {
                if (SetProperty(ref this.currentQuery, value))
                {
                    this.dataController.SearchAsync(value);
                }
            }
        }


        public IObservableCollection<IAlbumViewModel> Albums => this.albums;

        public IObservableCollection<ITrackViewModel> Tracks => this.tracks;

        public IObservableCollection<IArtistViewModel> Artists => this.artists;

        public IObservableCollection<IPlaylistViewModel> Playlists => this.playlists;



        public TracklistViewModelParams CreateTracklistViewModelParams(IAlbumViewModel albumViewModel)
            => ViewModelParamFactory.CreateTracklistViewModelParams(albumViewModel);

        public TracklistViewModelParams CreateTracklistViewModelParams(IPlaylistViewModel playlistViewModel)
            => ViewModelParamFactory.CreateTracklistViewModelParams(playlistViewModel);

        public ArtistOverviewViewModelParams CreateArtistOverviewViewModelParams(IArtistViewModel artistViewModel)
            => ViewModelParamFactory.CreateArtistOverviewViewModelParams(artistViewModel);



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.albums.Dispose();
                this.tracks.Dispose();
                this.artists.Dispose();
                this.playlists.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
