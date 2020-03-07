using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using E.Deezer;
using E.Deezer.Api;

using E.ExploreDeezer.Core.Mvvm;

namespace E.ExploreDeezer.Core.ViewModels
{
    public enum ETracklistViewModelType : byte
    {
        Album,
        Playlist
    };

    public interface ITracklistViewModel : INotifyPropertyChanged, 
                                           IDisposable
    {
        // Need something to display details about the actual item (album or playlist)
        ETracklistViewModelType Type { get; }

        string PageTitle { get; }
        IAlbumViewModel AlbumViewModel { get; }
        IPlaylistViewModel PlaylistViewModel { get; }

        EContentFetchStatus FetchStatus { get; }

        IEnumerable<ITrackViewModel> Tracks { get; }


        IInformationViewModel InformationViewModel { get; }
    }


    public interface ITracklistViewModelParams
    {
        object Item { get; }
        ETracklistViewModelType Type { get; }    
        
        // Helpers
        IAlbumViewModel Album { get; }
        IPlaylistViewModel Playlist { get; }
    }


    internal struct TracklistViewModelParams : ITracklistViewModelParams
    {
        public TracklistViewModelParams(ETracklistViewModelType type,
                                        object item)
        {
            this.Type = type;
            this.Item = item;
        }


        public object Item { get; }
        public ETracklistViewModelType Type { get; }

        // Helpers
        public IAlbumViewModel Album
        {
            get
            {
                //Assert type
                return this.Item as IAlbumViewModel;
            }
        }

        public IPlaylistViewModel Playlist
        {
            get
            {
                //Assert type
                return this.Item as IPlaylistViewModel;
            }
        }
    }



    internal class TracklistViewModel : ViewModelBase,
                                        ITracklistViewModel,
                                        IDisposable
    {
        private readonly IDeezerSession session;

        private EContentFetchStatus fetchStatus;
        private IEnumerable<ITrackViewModel> tracks;

        private IAlbumViewModel albumViewModel;
        private IPlaylistViewModel playlistViewModel;
        private IInformationViewModel informationViewModel;


        public TracklistViewModel(IDeezerSession session,
                                  IPlatformServices platformServices,
                                  ITracklistViewModelParams p)
            : base(platformServices)
        {
            this.session = session;

            this.Type = p.Type;

            this.AlbumViewModel = p.Album;
            this.PlaylistViewModel = p.Playlist;

            FetchContent();
        }



        // ITracklistViewModel
        public string PageTitle => this.AlbumViewModel?.Title ?? this.PlaylistViewModel?.Title ?? string.Empty;


        public ETracklistViewModelType Type { get; }

        public IAlbumViewModel AlbumViewModel 
        {
            get => this.albumViewModel;
            private set => SetProperty(ref this.albumViewModel, value);
        }

        public IPlaylistViewModel PlaylistViewModel 
        {
            get => this.playlistViewModel;
            private set => SetProperty(ref this.playlistViewModel, value);
        }


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


        public IInformationViewModel InformationViewModel
        {
            get => this.informationViewModel;
            private set => SetProperty(ref this.informationViewModel, value);
        }



        private void FetchContent()
        {
            Task<IEnumerable<ITrack>> fetchTask = null;
            
            this.FetchStatus = EContentFetchStatus.Loading;

            switch(this.Type)
            {
                case ETracklistViewModelType.Album:
                    fetchTask = this.session.Albums.GetAlbumTracks(this.AlbumViewModel.ItemId, this.CancellationToken);
                    this.session.Albums.GetById(this.AlbumViewModel.ItemId, this.CancellationToken)
                                       .ContinueWith(t =>
                                       {
                                           //TODO: Actually handle failed states
                                           if (t.IsFaulted || t.IsCanceled)
                                               return;

                                           this.AlbumViewModel = new AlbumViewModel(t.Result);

                                           this.InformationViewModel = new InformationViewModel(this.AlbumViewModel, this.PlatformServices);

                                       }, this.CancellationToken, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

                    break;

                case ETracklistViewModelType.Playlist:
                    fetchTask = this.session.Playlists.GetTracks(this.PlaylistViewModel.ItemId, this.CancellationToken);

                    this.session.Playlists.GetById(this.PlaylistViewModel.ItemId, this.CancellationToken)
                                          .ContinueWith(t =>
                                          {

                                              if (t.IsFaulted || t.IsCanceled)
                                                  return;

                                              this.PlaylistViewModel = new PlaylistViewModel(t.Result);

                                              this.InformationViewModel = new InformationViewModel(this.PlaylistViewModel, this.PlatformServices);

                                          }, this.CancellationToken, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
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

                var tracks = t.Result.Select(x => new TrackViewModel(x, this.Type == ETracklistViewModelType.Playlist ? ETrackLHSMode.Artwork 
                                                                                                                      : ETrackLHSMode.Number))
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
