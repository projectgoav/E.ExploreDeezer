using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using E.Deezer;
using E.Deezer.Api;

using E.ExploreDeezer.Mvvm;

namespace E.ExploreDeezer.ViewModels
{
    public enum ETracklistViewModelType : byte
    {
        Album,
        Playlist
    };

    public interface ITracklistViewModel : IDisposable
    {
        // Need something to display details about the actual item (album or playlist)

        EContentFetchStatus FetchStatus { get; }

        IEnumerable<ITrackViewModel> Tracks { get; }
    }


    public interface ITracklistViewModelParams
    {
        ulong ItemId { get; }
        ETracklistViewModelType Type { get; }     
    }


    internal struct TracklistViewModelParams : ITracklistViewModelParams
    {
        public TracklistViewModelParams(ETracklistViewModelType type,
                                        ulong itemId)
        {
            this.Type = type;
            this.ItemId = itemId;
        }


        public ulong ItemId { get; }
        public ETracklistViewModelType Type { get; }
    }



    internal class TracklistViewModel : ViewModelBase,
                                        ITracklistViewModel,
                                        IDisposable
    {
        private readonly IDeezerSession session;

        private EContentFetchStatus fetchStatus;
        private IEnumerable<ITrackViewModel> tracks;


        public TracklistViewModel(IDeezerSession session,
                                  IPlatformServices platformServices,
                                  ITracklistViewModelParams p)
            : base(platformServices)
        {
            this.session = session;

            FetchContent(p);
        }



        // ITracklistViewModel
        public EContentFetchStatus FetchStatus
        {
            get => this.fetchStatus;
            private set => SetProperty(ref this.fetchStatus, value);
        }

        public IEnumerable<ITrackViewModel> Tracks
        {
            get => this.tracks;
            private set => SetProperty(ref this.tracks, value);
        }



        private void FetchContent(ITracklistViewModelParams p)
        {
            Task<IEnumerable<ITrack>> fetchTask = null;
            
            this.FetchStatus = EContentFetchStatus.Loading;

            switch(p.Type)
            {
                case ETracklistViewModelType.Album:
                    fetchTask = this.session.Albums.GetAlbumTracks(p.ItemId, this.CancellationToken);
                    break;

                case ETracklistViewModelType.Playlist:
                    fetchTask = this.session.Playlists.GetTracks(p.ItemId, this.CancellationToken);
                    break;

                default: //Exit case
                    this.FetchStatus = EContentFetchStatus.Error;
                    return;
            }

            //Assert we don't have null task here

            fetchTask.ContinueWith(t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                {
                    this.FetchStatus = EContentFetchStatus.Error;
                    return;
                }

                var tracks = t.Result.Select(x => new TrackViewModel(x))
                                     .ToList();

                this.Tracks = tracks;

                this.FetchStatus = tracks.Count == 0 ? EContentFetchStatus.Empty
                                                     : EContentFetchStatus.Available;

            }, this.CancellationToken, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Tracks = Array.Empty<ITrackViewModel>();
            }

            base.Dispose(disposing);
        }

    }

}
