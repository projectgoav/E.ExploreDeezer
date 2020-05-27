using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using E.Deezer;
using E.Deezer.Api;
using E.ExploreDeezer.Core.ViewModels;
using E.ExploreDeezer.Core.Collections;
using E.ExploreDeezer.Core.Extensions;

namespace E.ExploreDeezer.Core.MyDeezer
{
    internal interface IMyDeezerDataController
    {
        bool IsLoggedIn { get; }

        IObservableCollection<IAlbumViewModel> FavouriteAlbums { get; }
        event FetchStateChangedEventHandler OnFavouriteAlbumFetchStateChanged;

        IObservableCollection<IArtistViewModel> FavouriteArtists { get; }
        event FetchStateChangedEventHandler OnFavouriteArtistsFetchStateChanged;

        IObservableCollection<ITrackViewModel> FavouriteTracks { get; }
        event FetchStateChangedEventHandler OnFavouriteTracksFetchStateChanged;
    }


    internal class MyDeezerDataController : IMyDeezerDataController,
                                            IDisposable
    {
        private readonly IDeezerSession session;
        private readonly IAuthenticationService authService;
        private readonly UpdatableFetchState trackFetchState;
        private readonly UpdatableFetchState albumsFetchState;
        private readonly UpdatableFetchState artistFetchState;
        private readonly PagedObservableCollection<ITrackViewModel> favouriteTracks;
        private readonly PagedObservableCollection<IAlbumViewModel> favouriteAlbums;
        private readonly PagedObservableCollection<IArtistViewModel> favouriteArtists;


        public MyDeezerDataController(IDeezerSession session,
                                      IAuthenticationService authService)
        {
            this.session = session;
            this.authService = authService;

            this.trackFetchState = new UpdatableFetchState();
            this.albumsFetchState = new UpdatableFetchState();
            this.artistFetchState = new UpdatableFetchState();

            this.favouriteTracks = new PagedObservableCollection<ITrackViewModel>();
            this.favouriteAlbums = new PagedObservableCollection<IAlbumViewModel>();
            this.favouriteArtists = new PagedObservableCollection<IArtistViewModel>();

            this.authService.OnAuthenticationStatusChanged += OnAuthenticationStateChanged;
        }


        // IMyDeezerDataController
        public bool IsLoggedIn { get; private set; }


        public IObservableCollection<IAlbumViewModel> FavouriteAlbums => this.favouriteAlbums;

        public event FetchStateChangedEventHandler OnFavouriteAlbumFetchStateChanged
        {
            add => this.albumsFetchState.OnFetchStateChanged += value;
            remove => this.albumsFetchState.OnFetchStateChanged -= value;
        }


        public IObservableCollection<ITrackViewModel> FavouriteTracks => this.favouriteTracks;

        public event FetchStateChangedEventHandler OnFavouriteTracksFetchStateChanged
        {
            add => this.trackFetchState.OnFetchStateChanged += value;
            remove => this.trackFetchState.OnFetchStateChanged -= value;
        }


        public IObservableCollection<IArtistViewModel> FavouriteArtists => this.favouriteArtists;

        public event FetchStateChangedEventHandler OnFavouriteArtistsFetchStateChanged
        {
            add => this.artistFetchState.OnFetchStateChanged += value;
            remove => this.artistFetchState.OnFetchStateChanged -= value;
        }





        private void OnAuthenticationStateChanged(object sender, OnAuthenticationStatusChangedEventArgs e)
        {
            this.IsLoggedIn = e.IsAuthenticated;

            if (this.IsLoggedIn)
            {
                UpdateCollectionsWhenAuthenticated();
            }
            else
            {
                UpdateCollectionsWhenNotAuthenticated();
            }
        }

        private void UpdateCollectionsWhenAuthenticated()
        {
            this.trackFetchState.SetLoading();
            this.albumsFetchState.SetLoading();
            this.artistFetchState.SetLoading();

            this.favouriteTracks.SetFetcher((s, c, ct) => this.session.User.GetFavouriteTracks(this.session.CurrentUserId, ct, (uint)s, (uint)c)
                                                                           .ContinueWhenNotCancelled<IEnumerable<ITrack>, IEnumerable<ITrackViewModel>>(t =>
                                                                           {
                                                                               (bool faulted, Exception ex) = t.CheckIfFailed();

                                                                               if (faulted)
                                                                               {
                                                                                   this.trackFetchState.SetError();
                                                                                   System.Diagnostics.Debug.WriteLine($"Failed to fetch favourite tracks. {ex}");
                                                                                   return null;
                                                                               }

                                                                               var items = t.Result.Select(x => new TrackViewModel(x, ETrackLHSMode.Artwork, ETrackArtistMode.NameWithLink));

                                                                               bool hasContents = this.favouriteTracks.Count > 0 || items.Any();
                                                                               if (hasContents)
                                                                               {
                                                                                   this.trackFetchState.SetAvailable();
                                                                               }
                                                                               else
                                                                               {
                                                                                   this.trackFetchState.SetEmpty();
                                                                               }

                                                                               return items;

                                                                           }, ct));

            this.favouriteAlbums.SetFetcher((s, c, ct) => this.session.User.GetFavouriteAlbums(this.session.CurrentUserId, ct, (uint)s, (uint)c)
                                                                           .ContinueWhenNotCancelled<IEnumerable<IAlbum>, IEnumerable<IAlbumViewModel>>(t =>
                                                                           {
                                                                               (bool faulted, Exception ex) = t.CheckIfFailed();

                                                                               if (faulted)
                                                                               {
                                                                                   this.albumsFetchState.SetError();
                                                                                   System.Diagnostics.Debug.WriteLine($"Failed to fetch favourite albums. {ex}");
                                                                                   return null;
                                                                               }

                                                                               var items = t.Result.Select(x => new AlbumViewModel(x));

                                                                               bool hasContents = this.favouriteAlbums.Count > 0 || items.Any();
                                                                               if (hasContents)
                                                                               {
                                                                                   this.albumsFetchState.SetAvailable();
                                                                               }
                                                                               else
                                                                               {
                                                                                   this.albumsFetchState.SetEmpty();
                                                                               }

                                                                               return items;

                                                                           }, ct));

            this.favouriteArtists.SetFetcher((s, c, ct) => this.session.User.GetFavouriteArtists(this.session.CurrentUserId, ct, (uint)s, (uint)c)
                                                                            .ContinueWhenNotCancelled<IEnumerable<IArtist>, IEnumerable<IArtistViewModel>>(t =>
                                                                            {
                                                                                (bool faulted, Exception ex) = t.CheckIfFailed();
 
                                                                                if (faulted)
                                                                                {
                                                                                    this.artistFetchState.SetError();
                                                                                    System.Diagnostics.Debug.WriteLine($"Failed to fetch favourite artists. {ex}");
                                                                                    return null;
                                                                                }
 
                                                                                var items = t.Result.Select(x => new ArtistViewModel(x));
 
                                                                                bool hasContents = this.favouriteArtists.Count > 0 || items.Any();
                                                                                if (hasContents)
                                                                                {
                                                                                    this.artistFetchState.SetAvailable();
                                                                                }
                                                                                else
                                                                                {
                                                                                    this.artistFetchState.SetEmpty();
                                                                                }
 
                                                                                return items;
 
                                                                            }, ct));
        }


        private void UpdateCollectionsWhenNotAuthenticated()
        {
            this.favouriteTracks.SetFetcher((s, c, ct) => Task.FromResult<IEnumerable<ITrackViewModel>>(Array.Empty<ITrackViewModel>()));
            this.favouriteAlbums.SetFetcher((s, c, ct) => Task.FromResult<IEnumerable<IAlbumViewModel>>(Array.Empty<IAlbumViewModel>()));
            this.favouriteArtists.SetFetcher((s, c, ct) => Task.FromResult<IEnumerable<IArtistViewModel>>(Array.Empty<IArtistViewModel>()));

            this.trackFetchState.SetEmpty();
            this.albumsFetchState.SetEmpty();
            this.artistFetchState.SetEmpty();
        }


        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.trackFetchState.Dispose();
                this.albumsFetchState.Dispose();
                this.artistFetchState.Dispose();

                this.favouriteTracks.Dispose();
                this.favouriteAlbums.Dispose();
                this.favouriteArtists.Dispose();
            }
        }
    }
}
