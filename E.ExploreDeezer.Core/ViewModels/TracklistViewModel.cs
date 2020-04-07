using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using E.Deezer;
using E.Deezer.Api;

using E.ExploreDeezer.Core.Mvvm;
using E.ExploreDeezer.Core.Collections;

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

        IObservableCollection<ITrackViewModel> Tracks { get; }

        IInformationViewModel InformationViewModel { get; }
    }


    public struct TracklistViewModelParams
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

        private IAlbumViewModel albumViewModel;
        private IPlaylistViewModel playlistViewModel;
        private IInformationViewModel informationViewModel;

        private MainThreadObservableCollectionAdapter<ITrackViewModel> dataController;


        public TracklistViewModel(IDeezerSession session,
                                  IPlatformServices platformServices,
                                  TracklistViewModelParams p)
            : base(platformServices)
        {
            this.session = session;

            this.Type = p.Type;

            this.AlbumViewModel = p.Album;
            this.PlaylistViewModel = p.Playlist;

            var tracklistController = ServiceRegistry.GetService<TracklistDataController>();

            this.dataController = new MainThreadObservableCollectionAdapter<ITrackViewModel>(tracklistController.Tracklist, platformServices.MainThreadDispatcher);

            switch(this.Type)
            {
                case ETracklistViewModelType.Album:
                    tracklistController.FetchTracklist(ETracklistType.Album, this.AlbumViewModel.ItemId);
                    break;

                case ETracklistViewModelType.Playlist:
                    tracklistController.FetchTracklist(ETracklistType.Playlist, this.PlaylistViewModel.ItemId);
                    break;
            }

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


        public IObservableCollection<ITrackViewModel> Tracks => this.dataController;


        public IInformationViewModel InformationViewModel
        {
            get => this.informationViewModel;
            private set => SetProperty(ref this.informationViewModel, value);
        }



        private void FetchContent()
        {
            switch(this.Type)
            {
                case ETracklistViewModelType.Album:
                    this.session.Albums.GetById(this.AlbumViewModel.ItemId, this.CancellationToken)
                                       .ContinueWith(t =>
                                       {
                                           //TODO: Actually handle failed states
                                           if (t.IsFaulted || t.IsCanceled)
                                               return;

                                           this.AlbumViewModel = new AlbumViewModel(t.Result);

                                           this.InformationViewModel = new InformationViewModel(this.AlbumViewModel, this.session, this.PlatformServices);

                                       }, this.CancellationToken, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

                    break;

                case ETracklistViewModelType.Playlist:
                    this.session.Playlists.GetById(this.PlaylistViewModel.ItemId, this.CancellationToken)
                                          .ContinueWith(t =>
                                          {

                                              if (t.IsFaulted || t.IsCanceled)
                                                  return;

                                              this.PlaylistViewModel = new PlaylistViewModel(t.Result);

                                              this.InformationViewModel = new InformationViewModel(this.PlaylistViewModel, this.session, this.PlatformServices);

                                          }, this.CancellationToken, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
                    break;

                default: //Exit case
                    return;
            } 
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

            }

            base.Dispose(disposing);
        }

    }

}
