using System;
using System.Collections.Generic;
using System.Text;

using E.ExploreDeezer.Core.ViewModels;

namespace E.ExploreDeezer.Core.Common
{
    public static class ViewModelParamFactory
    {
        public static TracklistViewModelParams CreateAlbumTracklistViewModelParams(IAlbumViewModel albumViewModel)
        {
            Assert.That(albumViewModel != null);
            return CreateAlbumTracklistViewModelParams(albumViewModel.ItemId);
        }

        public static TracklistViewModelParams CreateAlbumTracklistViewModelParams(ulong albumId)
            => new TracklistViewModelParams(ETracklistViewModelType.Album, albumId);



        public static TracklistViewModelParams CreatePlaylistTracklistViewModelParams(IPlaylistViewModel playlistViewModel)
        {
            Assert.That(playlistViewModel != null);

            return CreatePlaylistTracklistViewModelParams(playlistViewModel.ItemId);
        }

        public static TracklistViewModelParams CreatePlaylistTracklistViewModelParams(ulong playlistId)
            => new TracklistViewModelParams(ETracklistViewModelType.Playlist, playlistId);



        public static ArtistOverviewViewModelParams CreateArtistOverviewViewModelParams(IArtistViewModel artistViewModel)
        {
            Assert.That(artistViewModel != null);
            return CreateArtistOverviewViewModelParams(artistViewModel.ItemId);
        }

        public static ArtistOverviewViewModelParams CreateArtistOverviewViewModelParams(ulong artistId)
            => new ArtistOverviewViewModelParams(artistId);



        public static UserOverviewViewModelParams CreateUserOverviewViewModelParams(IUserProfileViewModel userProfile)
        {
            Assert.That(userProfile != null);
            return CreateUserOverviewViewModelParams(userProfile.Id);
        }

        public static UserOverviewViewModelParams CreateUserOverviewViewModelParams(ulong userId)
            => new UserOverviewViewModelParams(userId);
    }
}
