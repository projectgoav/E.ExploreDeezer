﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using E.Deezer;
using E.Deezer.Api;

using E.ExploreDeezer.Core.Util;
using E.ExploreDeezer.Core.ViewModels;
using E.ExploreDeezer.Core.Collections;

namespace E.ExploreDeezer.Core.Common
{
    internal interface IArtistOverviewDataController
    {
        IExtendedArtistViewModel CompleteArtist { get; }
        event FetchStateChangedEventHandler OnCompleteArtistFetchStateChanged;

        IObservableCollection<IAlbumViewModel> Albums { get; }
        event FetchStateChangedEventHandler OnAlbumFetchStateChanged;

        IObservableCollection<ITrackViewModel> TopTracks { get; }
        event FetchStateChangedEventHandler OnTopTrackFetchStateChanged;

        IObservableCollection<IPlaylistViewModel> Playlists { get; }
        event FetchStateChangedEventHandler OnPlaylistFetchStateChanged;

        IObservableCollection<IArtistViewModel> RelatedArtists { get; }
        event FetchStateChangedEventHandler OnRelatedArtistFetchStateChanged;


        void FetchOverviewAsync(ulong artistId);
    }


    internal class ArtistOverviewDataController : IArtistOverviewDataController,
                                                  IDisposable                          
    {
        private readonly IDeezerSession session;

        private readonly UpdatableFetchState albumFetchState;
        private readonly UpdatableFetchState topTrackFetchState;
        private readonly UpdatableFetchState playlistFetchState;
        private readonly UpdatableFetchState completeArtistFetchState;
        private readonly UpdatableFetchState relatedArtistsFetchState;
        private readonly ResetableCancellationTokenSource tokenSource;
        private readonly PagedObservableCollection<IAlbumViewModel> albums;
        private readonly PagedObservableCollection<ITrackViewModel> topTracks;
        private readonly PagedObservableCollection<IPlaylistViewModel> playlists;
        private readonly PagedObservableCollection<IArtistViewModel> relatedArtists;

        public ArtistOverviewDataController(IDeezerSession session)
        {
            this.session = session;

            this.ArtistId = ulong.MaxValue;
            this.tokenSource = new ResetableCancellationTokenSource();

            this.albumFetchState = new UpdatableFetchState();
            this.topTrackFetchState = new UpdatableFetchState();
            this.playlistFetchState = new UpdatableFetchState();
            this.completeArtistFetchState = new UpdatableFetchState();
            this.relatedArtistsFetchState = new UpdatableFetchState();

            this.albums = new PagedObservableCollection<IAlbumViewModel>();
            this.topTracks = new PagedObservableCollection<ITrackViewModel>();
            this.playlists = new PagedObservableCollection<IPlaylistViewModel>();
            this.relatedArtists = new PagedObservableCollection<IArtistViewModel>();
        }


        private ulong ArtistId { get; set; }


        // IArtistOverviewDataController
        public IExtendedArtistViewModel CompleteArtist { get; private set; }

        public event FetchStateChangedEventHandler OnCompleteArtistFetchStateChanged
        {
            add => this.completeArtistFetchState.OnFetchStateChanged += value;
            remove => this.completeArtistFetchState.OnFetchStateChanged -= value;
        }


        public IObservableCollection<IAlbumViewModel> Albums => this.albums;

        public event FetchStateChangedEventHandler OnAlbumFetchStateChanged
        {
            add => this.albumFetchState.OnFetchStateChanged += value;
            remove => this.albumFetchState.OnFetchStateChanged -= value;
        }


        public IObservableCollection<ITrackViewModel> TopTracks => this.topTracks;

        public event FetchStateChangedEventHandler OnTopTrackFetchStateChanged
        {
            add => this.topTrackFetchState.OnFetchStateChanged += value;
            remove => this.topTrackFetchState.OnFetchStateChanged -= value;
        }


        public IObservableCollection<IPlaylistViewModel> Playlists => this.playlists;

        public event FetchStateChangedEventHandler OnPlaylistFetchStateChanged
        {
            add => this.playlistFetchState.OnFetchStateChanged += value;
            remove => this.playlistFetchState.OnFetchStateChanged -= value;
        }


        public IObservableCollection<IArtistViewModel> RelatedArtists => this.relatedArtists;   

        public event FetchStateChangedEventHandler OnRelatedArtistFetchStateChanged
        {
            add => this.relatedArtistsFetchState.OnFetchStateChanged += value;
            remove => this.relatedArtistsFetchState.OnFetchStateChanged -= value;
        }


        public void FetchOverviewAsync(ulong artistId)
        {
            if (this.ArtistId == artistId)
                return;

            this.tokenSource.Reset();

            this.ArtistId = artistId;
            this.CompleteArtist = null;

            this.albumFetchState.SetLoading();
            this.topTrackFetchState.SetLoading();
            this.playlistFetchState.SetLoading();
            this.completeArtistFetchState.SetLoading();
            this.relatedArtistsFetchState.SetLoading();

            this.session.Artists.GetById(this.ArtistId, this.tokenSource.Token)
                                .ContinueWith(t =>
                                {
                                    if (t.IsFaulted)
                                    {
                                        this.completeArtistFetchState.SetError();

                                        var ex = t.Exception.GetBaseException();
                                        System.Diagnostics.Debug.WriteLine($"Failed to fetch complete artist model. {ex}");

                                        throw ex;
                                    }

                                    this.CompleteArtist = new ArtistViewModel(t.Result);
                                    this.completeArtistFetchState.SetAvailable();

                                }, this.tokenSource.Token, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);



            this.albums.SetFetcher((start, count, ct) => this.session.Artists.GetArtistsAlbums(this.ArtistId, ct, (uint)start, (uint)count)
                                                                             .ContinueWith<IEnumerable<IAlbumViewModel>>(t =>
                                                                             {
                                                                                 if (t.IsFaulted)
                                                                                 {
                                                                                     this.albumFetchState.SetError();

                                                                                     var ex = t.Exception.GetBaseException();
                                                                                     System.Diagnostics.Debug.WriteLine($"Failed to fetch artists albums. {ex}");

                                                                                     throw ex;
                                                                                 }

                                                                                 var items = t.Result.Select(x => new AlbumViewModel(x));

                                                                                 bool hasContents = this.albums.Count > 0 || items.Any();
                                                                                 if (hasContents)
                                                                                 {
                                                                                     this.albumFetchState.SetAvailable();
                                                                                 }
                                                                                 else
                                                                                 {
                                                                                     this.albumFetchState.SetEmpty();
                                                                                 }

                                                                                 return items;

                                                                             }, ct, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default));


            this.topTracks.SetFetcher((start, count, ct) => this.session.Artists.GetArtistsTopTracks(this.ArtistId, ct, (uint)start, (uint)count)
                                                                                .ContinueWith<IEnumerable<ITrackViewModel>>(t =>
                                                                                {
                                                                                    if (t.IsFaulted)
                                                                                    {
                                                                                        this.topTrackFetchState.SetError();

                                                                                        var ex = t.Exception.GetBaseException();
                                                                                        System.Diagnostics.Debug.WriteLine($"Failed to fetch artists op tracks. {ex}");

                                                                                        throw ex;
                                                                                    }

                                                                                    var items = t.Result.Select(x => new TrackViewModel(x));

                                                                                    bool hasContents = this.topTracks.Count > 0 || items.Any();
                                                                                    if (hasContents)
                                                                                    {
                                                                                        this.topTrackFetchState.SetAvailable();
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        this.topTrackFetchState.SetEmpty();
                                                                                    }

                                                                                    return items;

                                                                                 }, ct, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default));


            this.playlists.SetFetcher((start, count, ct) => this.session.Artists.GetPlaylistsFeaturingArtist(this.ArtistId, ct, (uint)start, (uint)count)
                                                                                .ContinueWith<IEnumerable<IPlaylistViewModel>>(t =>
                                                                                {
                                                                                    if (t.IsFaulted)
                                                                                    {
                                                                                        this.playlistFetchState.SetError();

                                                                                        var ex = t.Exception.GetBaseException();
                                                                                        System.Diagnostics.Debug.WriteLine($"Failed to fetch artists playlists. {ex}");

                                                                                        throw ex;
                                                                                    }

                                                                                    var items = t.Result.Select(x => new PlaylistViewModel(x));

                                                                                    bool hasContents = this.playlists.Count > 0 || items.Any();
                                                                                    if (hasContents)
                                                                                    {
                                                                                        this.playlistFetchState.SetAvailable();
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        this.playlistFetchState.SetEmpty();
                                                                                    }

                                                                                    return items;

                                                                                 }, ct, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default));


            this.relatedArtists.SetFetcher((start, count, ct) => this.session.Artists.GetRelatedArtists(this.ArtistId, ct, (uint)start, (uint)count)
                                                                                     .ContinueWith<IEnumerable<IArtistViewModel>>(t =>
                                                                                     {
                                                                                         if (t.IsFaulted)
                                                                                         {
                                                                                             this.relatedArtistsFetchState.SetError();

                                                                                             var ex = t.Exception.GetBaseException();
                                                                                             System.Diagnostics.Debug.WriteLine($"Failed to fetch related artists. {ex}");

                                                                                             throw ex;
                                                                                         }

                                                                                         var items = t.Result.Select(x => new ArtistViewModel(x));

                                                                                         bool hasContents = this.relatedArtists.Count > 0 || items.Any();
                                                                                         if (hasContents)
                                                                                         {
                                                                                             this.relatedArtistsFetchState.SetAvailable();
                                                                                         }
                                                                                         else
                                                                                         {
                                                                                             this.relatedArtistsFetchState.SetEmpty();
                                                                                         }

                                                                                         return items;

                                                                                     }, ct, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default));
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
                this.tokenSource.Dispose();

                this.albums.Dispose();
                this.topTracks.Dispose();
                this.playlists.Dispose();
                this.relatedArtists.Dispose();

                this.albumFetchState.Dispose();
                this.topTrackFetchState.Dispose();
                this.playlistFetchState.Dispose();
                this.relatedArtistsFetchState.Dispose();
            }
        }
    }
}