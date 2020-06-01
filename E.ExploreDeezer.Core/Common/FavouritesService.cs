using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using E.Deezer;

using E.ExploreDeezer.Core.Util;
using E.ExploreDeezer.Core.MyDeezer;
using E.ExploreDeezer.Core.Extensions;

namespace E.ExploreDeezer.Core.Common
{
    public enum EFavouriteType : byte
    {
        Album,
        Track,
        Artist,
        Playlist,
        Unknown = byte.MaxValue,
    };

    public delegate void OnFavouritesChangedHandler(object sender);

    public interface IFavouritesService
    {
        bool IsLoggedIn { get; }

        bool CanFavourite(ulong itemId, EFavouriteType type = EFavouriteType.Unknown);

        bool IsFavourited(ulong itemId, EFavouriteType type = EFavouriteType.Unknown);

        Task<bool> ToggleFavourited(ulong itemId, EFavouriteType type);

        event OnFavouritesChangedHandler OnFavouritesChanged;
    }


    internal class FavouritesService : IFavouritesService,
                                       IDisposable
    {
        private const int DOWNLOAD_CHUNK_SIZE = 100;
        private const int MIN_THROTTLE_MS = 50;
        private const int MAX_THROTTLE_MS = 500;

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
            Assert.That(MIN_THROTTLE_MS < MAX_THROTTLE_MS, "Invalid throttle values set.");

            this.session = session;
            this.authService = authService;

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

        public bool CanFavourite(ulong itemId, EFavouriteType type = EFavouriteType.Unknown)
            => this.IsLoggedIn && !this.userPlaylists.Contains(itemId);

        public bool IsFavourited(ulong itemId, EFavouriteType type = EFavouriteType.Unknown)
        {
            switch(type)
            {
                case EFavouriteType.Album:
                    return this.favouriteAlbums.Contains(itemId);

                case EFavouriteType.Track:
                    return this.favouriteTracks.Contains(itemId);

                case EFavouriteType.Artist:
                    return this.favouriteArtists.Contains(itemId);

                case EFavouriteType.Playlist:
                    return this.favouritePlaylists.Contains(itemId);

                default:
                    return this.favouriteTracks.Contains(itemId)
                            || this.favouriteAlbums.Contains(itemId)
                            || this.favouriteArtists.Contains(itemId)
                            || this.favouritePlaylists.Contains(itemId);
            }
        }


        public Task<bool> ToggleFavourited(ulong itemId, EFavouriteType type)
        {
            switch(type)
            {
                case EFavouriteType.Album:
                    return ToggleAlbumFavourite(itemId);

                case EFavouriteType.Track:
                    return ToggleTrackFavourite(itemId);

                case EFavouriteType.Artist:
                    return ToggleArtistFavourite(itemId);

                case EFavouriteType.Playlist:
                    return TogglePlaylistFavourite(itemId);

                default:
                    throw new ArgumentException("Invalid type.");
            }
        }


        private Task<bool> ToggleAlbumFavourite(ulong itemId)
        { 
            if (this.favouriteAlbums.Contains(itemId))
            {
                return this.session.User.UnfavouriteAlbum(itemId, this.tokenSource.Token)
                                        .ContinueWhenNotCancelled(t =>
                                        {
                                            if (t.IsFaulted)
                                                return false;

                                            if (t.Result)
                                            {
                                                this.favouriteAlbums.Remove(itemId);
                                                this.OnFavouritesChanged?.Invoke(this);
                                            }

                                            return t.Result;

                                        }, this.tokenSource.Token);
            }
            else
            {
                return this.session.User.FavouriteAlbum(itemId, this.tokenSource.Token)
                                        .ContinueWhenNotCancelled(t =>
                                        {
                                            if (t.IsFaulted)
                                                return false;

                                            if (t.Result)
                                            {
                                                this.favouriteAlbums.Add(itemId);
                                                this.OnFavouritesChanged?.Invoke(this);
                                            }

                                            return t.Result;

                                        }, this.tokenSource.Token);
            }
        }

        private Task<bool> ToggleTrackFavourite(ulong itemId)
        {
            if (this.favouriteTracks.Contains(itemId))
            {
                return this.session.User.UnfavouriteTrack(itemId, this.tokenSource.Token)
                                        .ContinueWhenNotCancelled(t =>
                                        {
                                            if (t.IsFaulted)
                                                return false;

                                            if (t.Result)
                                            {
                                                this.favouriteTracks.Remove(itemId);
                                                this.OnFavouritesChanged?.Invoke(this);
                                            }

                                            return t.Result;

                                        }, this.tokenSource.Token);
            }
            else
            {
                return this.session.User.FavouriteTrack(itemId, this.tokenSource.Token)
                                        .ContinueWhenNotCancelled(t =>
                                        {
                                            if (t.IsFaulted)
                                                return false;

                                            if (t.Result)
                                            {
                                                this.favouriteTracks.Add(itemId);
                                                this.OnFavouritesChanged?.Invoke(this);
                                            }

                                            return t.Result;

                                        }, this.tokenSource.Token);
            }
        }

        private Task<bool> ToggleArtistFavourite(ulong itemId)
        {
            if (this.favouriteArtists.Contains(itemId))
            {
                return this.session.User.UnfavouriteArtist(itemId, this.tokenSource.Token)
                                        .ContinueWhenNotCancelled(t =>
                                        {
                                            if (t.IsFaulted)
                                                return false;

                                            if (t.Result)
                                            {
                                                this.favouriteArtists.Remove(itemId);
                                                this.OnFavouritesChanged?.Invoke(this);
                                            }

                                            return t.Result;

                                        }, this.tokenSource.Token);
            }
            else
            {
                return this.session.User.FavouriteArtist(itemId, this.tokenSource.Token)
                                        .ContinueWhenNotCancelled(t =>
                                        {
                                            if (t.IsFaulted)
                                                return false;

                                            if (t.Result)
                                            {
                                                this.favouriteArtists.Add(itemId);
                                                this.OnFavouritesChanged?.Invoke(this);
                                            }

                                            return t.Result;

                                        }, this.tokenSource.Token);
            }
        }

        private Task<bool> TogglePlaylistFavourite(ulong itemId)
        {
            if (this.favouritePlaylists.Contains(itemId))
            {
                return this.session.User.UnfavouritePlaylist(itemId, this.tokenSource.Token)
                                        .ContinueWhenNotCancelled(t =>
                                        {
                                            if (t.IsFaulted)
                                                return false;

                                            if (t.Result)
                                            {
                                                this.favouritePlaylists.Remove(itemId);
                                                this.OnFavouritesChanged?.Invoke(this);
                                            }

                                            return t.Result;

                                        }, this.tokenSource.Token);
            }
            else
            {
                return this.session.User.FavouritePlaylist(itemId, this.tokenSource.Token)
                                        .ContinueWhenNotCancelled(t =>
                                        {
                                            if (t.IsFaulted)
                                                return false;

                                            if (t.Result)
                                            {
                                                this.favouritePlaylists.Add(itemId);
                                                this.OnFavouritesChanged?.Invoke(this);
                                            }

                                            return t.Result;

                                        }, this.tokenSource.Token);
            }
        }


        public event OnFavouritesChangedHandler OnFavouritesChanged;


        private void OnAuthStateChanged(object sender, OnAuthenticationStatusChangedEventArgs e)
        {
            this.IsLoggedIn = e.IsAuthenticated;

            this.tokenSource.Reset();

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

                this.tokenSource.Cancel();
                this.tokenSource.Dispose();
            }
        }
    }
}
