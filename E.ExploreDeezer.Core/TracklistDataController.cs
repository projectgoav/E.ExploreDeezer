using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using E.Deezer;
using E.Deezer.Api;

using E.ExploreDeezer.Core.ViewModels;
using E.ExploreDeezer.Core.Collections;

namespace E.ExploreDeezer.Core
{
    public enum ETracklistType
    {
        Unknown,
        Album,
        Playlist,
    }

    // TODO: We should have some form to state to indicate loading / error etc...

    internal class TracklistDataController : IDisposable
    {
        private const uint MAX_PLAYLIST_TRACKS = 1024;

        private readonly IDeezerSession session;
        private readonly FixedSizeObservableCollection<ITrackViewModel> tracklistInternal;

        private bool disposed;
        private CancellationTokenSource cancellationTokenSource;

        public TracklistDataController(IDeezerSession session)
        {
            this.session = session;

            this.ItemId = 0;
            this.Type = ETracklistType.Unknown;


            this.disposed = false;
            this.tracklistInternal = new FixedSizeObservableCollection<ITrackViewModel>();
        }


        public ulong ItemId { get; private set; }

        public ETracklistType Type { get; private set; }

        public IObservableCollection<ITrackViewModel> Tracklist => this.tracklistInternal;


        public void FetchTracklist(ETracklistType type, ulong itemId)
        {
            if (this.ItemId == itemId)
                return;

            this.ItemId = itemId;
            this.Type = type;

            FetchTracklistAsync();
        }



        private void FetchTracklistAsync()
        {
            Assert.That(this.disposed == false);

            ClearCancellationTokenSource();

            this.cancellationTokenSource = new CancellationTokenSource();

            Task<IEnumerable<ITrack>> tracksTask = null;

            switch(this.Type)
            {
                case ETracklistType.Album:
                    tracksTask = this.session.Albums.GetAlbumTracks(this.ItemId, this.cancellationTokenSource.Token);
                    break;

                case ETracklistType.Playlist:
                    tracksTask = this.session.Playlists.GetTracks(this.ItemId, this.cancellationTokenSource.Token, count: MAX_PLAYLIST_TRACKS);
                    break;

                default:
                    throw new InvalidOperationException("Unknown tracklist type specified.");
            }


            tracksTask.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    return; //TODO
                }


                var tracks = t.Result.Select(x => new TrackViewModel(x, this.Type == ETracklistType.Album ? ETrackLHSMode.Number
                                                                                                          : ETrackLHSMode.Artwork));

                this.tracklistInternal.SetContents(tracks);

            }, this.cancellationTokenSource.Token, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }


        private void ClearCancellationTokenSource()
        {
            if (this.cancellationTokenSource != null)
            {
                this.cancellationTokenSource.Cancel();
                this.cancellationTokenSource.Dispose();
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
                this.disposed = true;
                ClearCancellationTokenSource();
            }
        }

    }
}
