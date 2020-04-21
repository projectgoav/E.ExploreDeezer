using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using E.Deezer;

using E.ExploreDeezer.Core.Util;
using E.ExploreDeezer.Core.ViewModels;
using E.ExploreDeezer.Core.Collections;

namespace E.ExploreDeezer.Core.Common
{

    internal interface ITracklistDataController
    {
        IExtendedAlbumViewModel CompleteAlbum { get; }
        IExtendedPlaylistViewModel CompletePlaylist { get; }
        event FetchStateChangedEventHandler OnCompleteItemFetchStateChanged;

        EFetchState TracklistFetchState { get; }
        IObservableCollection<ITrackViewModel> Tracklist { get; }
        event FetchStateChangedEventHandler OnTracklistFetchStateChanged;

        void FetchTracklistAsync(ETracklistType type, ulong itemId);
    }

    public enum ETracklistType
    {
        Unknown,
        Album,
        Playlist,
    }


    internal class TracklistDataController : ITracklistDataController,
                                             IDisposable
    {
        private readonly IDeezerSession session;
        private readonly UpdatableFetchState fetchState;
        private readonly UpdatableFetchState completeItemFetchState;
        private readonly ResetableCancellationTokenSource tokenSource;
        private readonly PagedObservableCollection<ITrackViewModel> tracklist;


        public TracklistDataController(IDeezerSession session)
        {
            this.session = session;

            this.fetchState = new UpdatableFetchState();
            this.completeItemFetchState = new UpdatableFetchState();

            this.tokenSource = new ResetableCancellationTokenSource();
            this.tracklist = new PagedObservableCollection<ITrackViewModel>();

            this.ItemId = 0;
            this.Type = ETracklistType.Unknown;
        }


        public ulong ItemId { get; private set; }
        public ETracklistType Type { get; private set; }


        // ITracklistDataController
        public IExtendedAlbumViewModel CompleteAlbum { get; private set; }
        public IExtendedPlaylistViewModel CompletePlaylist { get; private set; }

        public event FetchStateChangedEventHandler OnCompleteItemFetchStateChanged
        {
            add => this.completeItemFetchState.OnFetchStateChanged += value;
            remove => this.completeItemFetchState.OnFetchStateChanged -= value;
        }


        public EFetchState TracklistFetchState => this.fetchState.CurrentState;

        public IObservableCollection<ITrackViewModel> Tracklist => this.tracklist;

        public event FetchStateChangedEventHandler OnTracklistFetchStateChanged
        {
            add => this.fetchState.OnFetchStateChanged += value;
            remove => this.fetchState.OnFetchStateChanged -= value;
        }


        public void FetchTracklistAsync(ETracklistType type, ulong itemId)
        {
            if (this.ItemId == itemId)
                return;

            this.Type = type;
            this.ItemId = itemId;

            DoFetchTracklist();
        }


        private void DoFetchTracklist()
        {
            this.tokenSource.Reset();

            this.fetchState.SetLoading();
            this.completeItemFetchState.SetLoading();

            this.CompleteAlbum = null;
            this.CompletePlaylist = null;

            switch(this.Type)
            {
                case ETracklistType.Album:

                    this.session.Albums.GetById(this.ItemId, this.tokenSource.Token)
                                       .ContinueWith(t =>
                                       {
                                           if (t.IsFaulted)
                                           {
                                               this.completeItemFetchState.SetError();

                                               var ex = t.Exception.GetBaseException();
                                               System.Diagnostics.Debug.WriteLine($"Failed to fetch complete album {ex}");

                                               throw ex;
                                           }

                                           this.CompleteAlbum = new AlbumViewModel(t.Result);
                                           this.completeItemFetchState.SetAvailable();

                                       }, this.tokenSource.Token, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

                    this.tracklist.SetFetcher((start, count, ct) => this.session.Albums.GetAlbumTracks(this.ItemId, ct, (uint)start, (uint)count)
                                                                                       .ContinueWith<IEnumerable<ITrackViewModel>>(t =>
                                                                                       {
                                                                                           if (t.IsFaulted)
                                                                                           {
                                                                                               this.fetchState.SetError();

                                                                                               var ex = t.Exception.GetBaseException();
                                                                                               System.Diagnostics.Debug.WriteLine($"Failed to get album tracklist. {ex}");

                                                                                               throw ex;
                                                                                           }

                                                                                           var items = t.Result.Select(x => new TrackViewModel(x, 
                                                                                                                                               ETrackLHSMode.Number, 
                                                                                                                                               ETrackArtistMode.Name));

                                                                                           bool hasContents = this.tracklist.Count > 0 || items.Any();

                                                                                           if (hasContents)
                                                                                           {
                                                                                               this.fetchState.SetAvailable();
                                                                                           }
                                                                                           else
                                                                                           {
                                                                                               this.fetchState.SetEmpty();
                                                                                           }

                                                                                           return items;

                                                                                       }, ct, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default));

                    break;

                case ETracklistType.Playlist:

                    this.session.Playlists.GetById(this.ItemId, this.tokenSource.Token)
                                          .ContinueWith(t =>
                                          {
                                              if (t.IsFaulted)
                                              {
                                                  this.completeItemFetchState.SetError();

                                                  var ex = t.Exception.GetBaseException();
                                                  System.Diagnostics.Debug.WriteLine($"Failed to fetch complete playlist. {ex}");

                                                  throw ex;
                                              }

                                              this.CompletePlaylist = new PlaylistViewModel(t.Result);
                                              this.completeItemFetchState.SetAvailable();

                                          }, this.tokenSource.Token, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);


                    this.tracklist.SetFetcher((start, count, ct) => this.session.Playlists.GetTracks(this.ItemId, ct, (uint)start, (uint)count)
                                                                   .ContinueWith<IEnumerable<ITrackViewModel>>(t =>
                                                                   {
                                                                       if (t.IsFaulted)
                                                                       {
                                                                           this.fetchState.SetError();

                                                                           var ex = t.Exception.GetBaseException();
                                                                           System.Diagnostics.Debug.WriteLine($"Failed to get album tracklist. {ex}");

                                                                           throw ex;
                                                                       }

                                                                       var items = t.Result.Select(x => new TrackViewModel(x, 
                                                                                                                           ETrackLHSMode.Artwork, 
                                                                                                                           ETrackArtistMode.NameWithLink));

                                                                       bool hasContents = this.tracklist.Count > 0 || items.Any();

                                                                       if (hasContents)
                                                                       {
                                                                           this.fetchState.SetAvailable();
                                                                       }
                                                                       else
                                                                       {
                                                                           this.fetchState.SetEmpty();
                                                                       }

                                                                       return items;

                                                                   }, ct, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default));

                    break;

                default:
                    throw new InvalidOperationException("Unknown tracklist type specified.");
            }
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

                this.fetchState.Dispose();
            }
        }

    }
}
