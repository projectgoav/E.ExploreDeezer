using System;
using System.Collections.Generic;
using System.Text;

using E.Deezer;
using E.Deezer.Api;

using E.ExploreDeezer.Core.ViewModels;
using E.ExploreDeezer.Core.Extensions;
using E.ExploreDeezer.Core.Collections;

namespace E.ExploreDeezer.Core.Common
{
    internal interface IUserOverviewDataController
    {
        /*
        IObservableCollection<IAlbumViewModel> FavouriteAlbums { get; }
        event FetchStateChangedEventHandler OnFavouriteAlbumsFetchStateChanged;

        IObservableCollection<IArtistViewModel> FavouriteArtists { get; }
        event FetchStateChangedEventHandler OnFavouriteArtistsFetchStateChanged;

        IObservableCollection<ITrackViewModel> FavouriteTracks { get; }
        event FetchStateChangedEventHandler OnFavouriteTracksFetchStateChanged;

        IObservableCollection<IPlaylistViewModel> UserPlaylists { get; }
        event FetchStateChangedEventHandler OnUserPlaylistFetchStateChanged;

        IObservableCollection<IPlaylistViewModel> FavouritePlaylists { get; }
        event FetchStateChangedEventHandler OnFavouritePlaylistsFetchStateChanged;
        */
    
        // Flow?
        // Followers  / Following...
    }

    internal class UserOverviewDataController : IUserOverviewDataController,
                                                IDisposable
    {
        private readonly IDeezerSession session;
        private readonly UpdatableFetchState userPlaylistFetchState;
        private readonly UpdatableFetchState favouriteAlbumFetchState;
        private readonly UpdatableFetchState favouriteTrackFetchState;
        private readonly UpdatableFetchState favouriteArtistFetchState;
        private readonly UpdatableFetchState favouritePlaylistFetchState;
        private readonly PagedObservableCollection<IAlbumViewModel> favouriteAlbums;
        private readonly PagedObservableCollection<ITrackViewModel> favouriteTracks;
        private readonly PagedObservableCollection<IArtistViewModel> favouriteArtists;
        private readonly FixedSizeObservableCollection<IPlaylistViewModel> userPlaylists;
        private readonly FixedSizeObservableCollection<IPlaylistViewModel> favouritePlaylists;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {

            }
        }
    }
}
