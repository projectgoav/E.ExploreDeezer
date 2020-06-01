using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using E.Deezer;

using E.ExploreDeezer.Core.Util;
using E.ExploreDeezer.Core.MyDeezer;
using E.ExploreDeezer.Core.ViewModels;
using System.Threading;
using System.Diagnostics;

namespace E.ExploreDeezer.Core.Common
{
    public delegate void OnFavouritesChangedHandler(object sender);

    public interface IFavouritesService
    {
        bool IsLoggedIn { get; }

        bool CanFavourite(ulong itemId);
        bool CanFavourite(ITrackViewModel track);
        bool CanFavourite(IAlbumViewModel album);
        bool CanFavourite(IArtistViewModel artist);
        bool CanFavourite(IPlaylistViewModel playlist);


        bool IsFavourited(ulong itemId);
        bool IsFavourited(ITrackViewModel track);
        bool IsFavourited(IAlbumViewModel album);
        bool IsFavourited(IArtistViewModel artist);
        bool IsFavourited(IPlaylistViewModel playlist);

        event OnFavouritesChangedHandler OnFavouritesChanged;
    }


    internal class FavouritesService : IFavouritesService,
                                              IDisposable
    {
        private const int DOWNLOAD_CHUNK_SIZE = 100;
        private const int MIN_THROTTLE_MS = 50;
        private const int MAX_THROTTLE_MS = 500;

        private readonly Random rand;
        private readonly IDeezerSession session;
        private readonly HashSet<ulong> userPlaylists;
        private readonly HashSet<ulong> favouriteTracks;
        private readonly HashSet<ulong> favouriteAlbums;
        private readonly HashSet<ulong> favouriteArtists;
        private readonly HashSet<ulong> favouritePlaylists;
        private readonly IAuthenticationService authService;
        private readonly ResetableCancellationTokenSource tokenSource;


        public FavouritesService(IDeezerSession session,
                                        IAuthenticationService authService)
        {
            Debug.Assert(MIN_THROTTLE_MS < MAX_THROTTLE_MS, "Invalid throttle values set.");

            this.session = session;
            this.authService = authService;

            this.rand = new Random();
            this.userPlaylists = new HashSet<ulong>();
            this.favouriteTracks = new HashSet<ulong>();
            this.favouriteAlbums = new HashSet<ulong>();
            this.favouriteArtists = new HashSet<ulong>();
            this.favouritePlaylists = new HashSet<ulong>();
            this.tokenSource = new ResetableCancellationTokenSource();

            this.IsLoggedIn = false;

            this.authService.OnAuthenticationStatusChanged += OnAuthStateChanged;
        }


        // IFavouritesDataController
        public bool IsLoggedIn { get; private set; }

        public bool CanFavourite(ulong itemId)
            => this.IsLoggedIn && !this.userPlaylists.Contains(itemId);

        public bool CanFavourite(ITrackViewModel track)
            => track != null && this.IsLoggedIn;

        public bool CanFavourite(IAlbumViewModel album)
            => album != null && this.IsLoggedIn;

        public bool CanFavourite(IArtistViewModel artist)
            => artist != null && this.IsLoggedIn;

        public bool CanFavourite(IPlaylistViewModel playlist)
            => playlist != null && this.IsLoggedIn && !this.userPlaylists.Contains(playlist.ItemId);


        public bool IsFavourited(ulong itemId)
        {
            return this.favouriteTracks.Contains(itemId)
                    || this.favouriteAlbums.Contains(itemId)
                    || this.favouriteArtists.Contains(itemId)
                    || this.favouritePlaylists.Contains(itemId);
        }

        public bool IsFavourited(ITrackViewModel track)
            => track != null && this.favouriteTracks.Contains(track.ItemId);

        public bool IsFavourited(IAlbumViewModel album)
            => album != null && this.favouriteAlbums.Contains(album.ItemId);

        public bool IsFavourited(IArtistViewModel artist)
            => artist != null && this.favouriteArtists.Contains(artist.ItemId);

        public bool IsFavourited(IPlaylistViewModel playlist)
            => playlist != null && this.favouritePlaylists.Contains(playlist.ItemId);


        public event OnFavouritesChangedHandler OnFavouritesChanged;


        private void OnAuthStateChanged(object sender, OnAuthenticationStatusChangedEventArgs e)
        {
            this.IsLoggedIn = e.IsAuthenticated;

            if (this.IsLoggedIn)
            {
                UpdateFavouriteListsAsync();
            }
            else
            {
                ClearFavouriteLists();
                this.OnFavouritesChanged?.Invoke(this);
            }
        }


        private void UpdateFavouriteListsAsync()
        {
            Assert.That(this.IsLoggedIn, "Not logged in");

            ClearFavouriteLists();

            var t = new Task[]
            {
                FetchTrackFavouritesAsync(),
                FetchAlbumFavouritesAsync(),
                FetchArtistFavouritesAsync(),
                FetchFavouritePlaylistsAsync(),
            };

            Task.WhenAll(t)
                .ContinueWith(_ =>
                {
                    this.OnFavouritesChanged?.Invoke(this);
                }, this.tokenSource.Token);
        }


        private void ClearFavouriteLists()
        {
            this.userPlaylists.Clear();
            
            this.favouriteTracks.Clear();
            this.favouriteAlbums.Clear();
            this.favouriteArtists.Clear();
            this.favouritePlaylists.Clear();
        }

        private Task FetchTrackFavouritesAsync()
            => Task.Run(async () =>
            {
                uint index = 0;
                var token = this.tokenSource.Token;

                while(!token.IsCancellationRequested)
                {
                    var result = await this.session.User.GetFavouriteTracks(this.session.CurrentUserId, token, index, DOWNLOAD_CHUNK_SIZE);

                    uint resultCount = (uint)result.Count();

                    if (resultCount == 0)
                        break;

                    foreach(var id in result.Select(x => x.Id))
                    {
                        this.favouriteTracks.Add(id);
                    }

                    // Reached the end...
                    if (resultCount < DOWNLOAD_CHUNK_SIZE)
                        break;

                    index += resultCount;              
                }
            }, this.tokenSource.Token);

        private Task FetchAlbumFavouritesAsync()
            => Task.Run(async () =>
            {
                uint index = 0;
                var token = this.tokenSource.Token;

                while (!token.IsCancellationRequested)
                {
                    var result = await this.session.User.GetFavouriteAlbums(this.session.CurrentUserId, token, index, DOWNLOAD_CHUNK_SIZE);

                    uint resultCount = (uint)result.Count();

                    if (resultCount == 0)
                        break;

                    foreach (var id in result.Select(x => x.Id))
                    {
                        this.favouriteAlbums.Add(id);
                    }

                    // Reached the end...
                    if (resultCount < DOWNLOAD_CHUNK_SIZE)
                        break;

                    index += resultCount;
                }
            }, this.tokenSource.Token);


        private Task FetchArtistFavouritesAsync()
            => Task.Run(async () =>
            {
                uint index = 0;
                var token = this.tokenSource.Token;

                while (!token.IsCancellationRequested)
                {
                    var result = await this.session.User.GetFavouriteArtists(this.session.CurrentUserId, token, index, DOWNLOAD_CHUNK_SIZE);

                    uint resultCount = (uint)result.Count();

                    if (resultCount == 0)
                        break;

                    foreach (var id in result.Select(x => x.Id))
                    {
                        this.favouriteArtists.Add(id);
                    }

                    // Reached the end...
                    if (resultCount < DOWNLOAD_CHUNK_SIZE)
                        break;

                    index += resultCount;
                }
            }, this.tokenSource.Token);


        private Task FetchFavouritePlaylistsAsync()
            => Task.Run(async () =>
            {
                uint index = 0;
                var token = this.tokenSource.Token;

                while (!token.IsCancellationRequested)
                {
                    var result = await this.session.User.GetFavouritePlaylists(this.session.CurrentUserId, token, index, DOWNLOAD_CHUNK_SIZE);

                    uint resultCount = (uint)result.Count();

                    if (resultCount == 0)
                        break;

                    // Deezer API returns a flat list of user favourites & their own playlists in a single list
                    // so we require some additional processing
                    foreach(var playlist in result)
                    {
                        if (playlist.IsLovedTrack)
                            this.userPlaylists.Add(playlist.Id);

                        else if (playlist.Creator != null && playlist.Creator.Id == this.session.CurrentUserId)
                            this.userPlaylists.Add(playlist.Id);

                        else
                            this.favouritePlaylists.Add(playlist.Id);
                    }

                    // Reached the end...
                    if (resultCount < DOWNLOAD_CHUNK_SIZE)
                        break;

                    index += resultCount;
                }
            }, this.tokenSource.Token);


        // Querying favourites rapidly can cause throttling 
        // on the Deezer API end. This very crude method
        // simply sleeps the calling thread for a random period
        // of time from 10 -> MAX_THROTTLE_MS..
        private void PreventThrottling()
            => Thread.Sleep(this.rand.Next(10, MAX_THROTTLE_MS));


        // IDisposable
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.authService.OnAuthenticationStatusChanged -= OnAuthStateChanged;

                this.tokenSource.Dispose();
            }
        }
    }
}
