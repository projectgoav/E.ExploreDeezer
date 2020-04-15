using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using E.Deezer.Api;

namespace E.ExploreDeezer.Core.ViewModels
{
    public interface IPlaylistViewModel
    {
        bool IsPresent { get; }

        ulong ItemId { get; }

        string Title { get; }
        string CreatorName { get; }

        string ArtworkUri { get; }

        uint NumberOfTracks { get; }
    }

    internal class PlaylistViewModel : IPlaylistViewModel
    {
        public PlaylistViewModel(IPlaylist playlist)
        {
            this.IsPresent = playlist != null;

            this.ItemId = playlist?.Id ?? 0u;

            this.Title = playlist?.Title ?? string.Empty;
            this.CreatorName = playlist?.Creator?.Username ?? string.Empty;

            this.ArtworkUri = playlist?.Images?.HasPictureOfSize(PictureSize.Medium) ?? false ? playlist.Images.Medium
                                                                                              : "ms-appx:///Assets/StoreLogo.png";

            this.NumberOfTracks = playlist?.NumberOfTracks ?? 0;
        }

        // IPlaylistViewModel
        public bool IsPresent { get; }

        public ulong ItemId { get; }

        public string Title { get; }

        public string CreatorName { get; }

        public string ArtworkUri { get; }

        public uint NumberOfTracks { get; }
    }
}
