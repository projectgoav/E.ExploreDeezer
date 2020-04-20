using System;
using System.Collections.Generic;
using System.Text;

using E.ExploreDeezer.Core.Mvvm;
using E.ExploreDeezer.Core.ViewModels;
using E.ExploreDeezer.Core.Collections;

namespace E.ExploreDeezer.Core.Common
{
    public interface IUserOverviewViewModel
    {
        string Username { get; }
        string ProfilePictureUri { get; }

        EFetchState HeaderFetchState { get; }
        
        EFetchState FlowFetchState { get; }
        IObservableCollection<ITrackViewModel> Flow { get; }

        EFetchState PlaylistFetchState { get; }
        IObservableCollection<IPlaylistViewModel> Playlists { get; }

        TracklistViewModelParams CreateTracklistViewModelParams(IPlaylistViewModel playlist);
    }

    public struct UserOverviewViewModelParams
    {
        public UserOverviewViewModelParams(ulong userId)
        {
            this.UserId = userId;
        }

        public ulong UserId { get; }
    }


    internal class UserOverviewViewModel : ViewModelBase,
                                           IUserOverviewViewModel
    {
        private readonly IUserOverviewDataController dataController;
        private readonly MainThreadObservableCollectionAdapter<ITrackViewModel> flow;
        private readonly MainThreadObservableCollectionAdapter<IPlaylistViewModel> playlists;

        private string username;
        private string profilePictureUri;
        private EFetchState flowFetchState;
        private EFetchState headerFetchState;
        private EFetchState playlistFetchState;


        public UserOverviewViewModel(IPlatformServices platformServices,
                                     UserOverviewViewModelParams p)
            : base(platformServices)
        {

            this.dataController = ServiceRegistry.GetService<IUserOverviewDataController>();

            this.flow = new MainThreadObservableCollectionAdapter<ITrackViewModel>(dataController.Flow,
                                                                                   PlatformServices.MainThreadDispatcher);

            this.playlists = new MainThreadObservableCollectionAdapter<IPlaylistViewModel>(dataController.Playlists,
                                                                                           PlatformServices.MainThreadDispatcher);

            this.dataController.OnFlowFetchStateChanged += OnFlowFetchStateChanged;
            this.dataController.OnPlaylistFetchStateChanged += OnPlaylistFetchStateChanged;
            this.dataController.OnCompleteProfileFetchStateChanged += OnHeaderFetchStateChanged;

            this.dataController.FetchUserProfileAsync(p.UserId);
        }


        // IUserOverviewViewModel
        public string Username
        {
            get => this.username;
            private set => SetProperty(ref this.username, value);
        }

        public string ProfilePictureUri
        {
            get => this.profilePictureUri;
            private set => SetProperty(ref this.profilePictureUri, value);
        }

        public EFetchState HeaderFetchState
        {
            get => this.headerFetchState;
            private set
            {
                if (SetProperty(ref this.headerFetchState, value))
                {
                    UpdateHeader();
                }
            }
        }

        public EFetchState FlowFetchState
        {
            get => this.flowFetchState;
            private set => SetProperty(ref this.flowFetchState, value);
        }

        public IObservableCollection<ITrackViewModel> Flow => this.flow;

        public EFetchState PlaylistFetchState
        {
            get => this.playlistFetchState;
            private set => SetProperty(ref this.playlistFetchState, value);
        }

        public IObservableCollection<IPlaylistViewModel> Playlists => this.playlists;


        public TracklistViewModelParams CreateTracklistViewModelParams(IPlaylistViewModel playlistViewModel)
            => ViewModelParamFactory.CreateTracklistViewModelParams(playlistViewModel);


        private void OnFlowFetchStateChanged(object sender, FetchStateChangedEventArgs e)
            => this.FlowFetchState = e.NewValue;

        private void OnPlaylistFetchStateChanged(object sender, FetchStateChangedEventArgs e)
            => this.PlaylistFetchState = e.NewValue;

        private void OnHeaderFetchStateChanged(object sender, FetchStateChangedEventArgs e)
            => this.HeaderFetchState = e.NewValue;


        private void UpdateHeader()
        {
            if (this.dataController.CompleteProfile != null)
            {
                this.Username = this.dataController.CompleteProfile.Username;
                this.ProfilePictureUri = this.dataController.CompleteProfile.ProfilePicture;
            }
            else
            {
                this.Username = string.Empty;
                this.ProfilePictureUri = null;
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dataController.OnFlowFetchStateChanged -= OnFlowFetchStateChanged;
                this.dataController.OnPlaylistFetchStateChanged -= OnPlaylistFetchStateChanged;
                this.dataController.OnCompleteProfileFetchStateChanged -= OnHeaderFetchStateChanged;

                this.flow.Dispose();
                this.playlists.Dispose();
            }

            base.Dispose(disposing);
        }

    }
}
