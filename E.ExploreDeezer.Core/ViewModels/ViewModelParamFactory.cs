using System;
using System.Collections.Generic;
using System.Text;

namespace E.ExploreDeezer.Core.ViewModels
{
    public static class ViewModelParamFactory
    {

        public static TracklistViewModelParams CreateTracklistViewModelParams(IAlbumViewModel albumViewModel)
        {
            if (albumViewModel == null || !albumViewModel.IsPresent)
                throw new ArgumentException();

            return new TracklistViewModelParams(ETracklistViewModelType.Album, albumViewModel);
        }

        public static TracklistViewModelParams CreateTracklistViewModelParams(IPlaylistViewModel playlistViewModel)
        {
            if (playlistViewModel == null || !playlistViewModel.IsPresent)
                throw new ArgumentException();

            return new TracklistViewModelParams(ETracklistViewModelType.Playlist, playlistViewModel);
        }

        public static ArtistOverviewViewModelParams CreateArtistOverviewViewModelParams(IArtistViewModel artistViewModel)
        {
            if (artistViewModel == null || !artistViewModel.IsPresent)
                throw new ArgumentException();

            return new ArtistOverviewViewModelParams(artistViewModel);
        }
    }
}
