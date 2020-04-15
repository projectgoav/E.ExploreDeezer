using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    internal interface ITracklistDataController
    {
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
        private const uint MAX_PLAYLIST_TRACKS = 1024;

        private readonly IDeezerSession session;
        private readonly UpdatableFetchState fetchState;
        private readonly ResetableCancellationTokenSource tokenSource;
        private readonly FixedSizeObservableCollection<ITrackViewModel> tracklist;


        public TracklistDataController(IDeezerSession session)
        {
            this.session = session;

            this.fetchState = new UpdatableFetchState();
            this.tokenSource = new ResetableCancellationTokenSource();
            this.tracklist = new FixedSizeObservableCollection<ITrackViewModel>();

            this.ItemId = 0;
            this.Type = ETracklistType.Unknown;
        }


        public ulong ItemId { get; private set; }
        public ETracklistType Type { get; private set; }


        // ITracklistDataController
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

            this.tracklist.ClearContents();

            this.fetchState.SetLoading();

            Task<IEnumerable<ITrack>> tracksTask = null;

            switch(this.Type)
            {
                case ETracklistType.Album:
                    tracksTask = this.session.Albums.GetAlbumTracks(this.ItemId, this.tokenSource.Token);
                    break;

                case ETracklistType.Playlist:
                    tracksTask = this.session.Playlists.GetTracks(this.ItemId, this.tokenSource.Token, count: MAX_PLAYLIST_TRACKS);
                    break;

                default:
                    throw new InvalidOperationException("Unknown tracklist type specified.");
            }


            tracksTask.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    this.fetchState.SetEmpty();

                    var ex = t.Exception.GetBaseException();
                    System.Diagnostics.Debug.WriteLine($"Failed to fetch tracklist. {ex}");

                    throw ex;
                }


                var tracks = t.Result.Select(x => new TrackViewModel(x, this.Type == ETracklistType.Album ? ETrackLHSMode.Number
                                                                                                          : ETrackLHSMode.Artwork));

                this.tracklist.SetContents(tracks);

                if (this.tracklist.Count == 0)
                {
                    this.fetchState.SetEmpty();
                }
                else
                {
                    this.fetchState.SetAvailable();
                }


            }, this.tokenSource.Token, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
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
