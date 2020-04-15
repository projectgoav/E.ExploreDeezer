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
        ulong ItemId { get; }

        string Title { get; }
        string CreatorName { get; }

        string ArtworkUri { get; }
    }

    public interface IExtendedPlaylistViewModel
    {
        uint NumberOfFans { get; }
        uint NumberOfTracks { get; }

        string ShareLink { get; }
        string WebsiteLink { get; }
    }


    internal class PlaylistViewModel : IPlaylistViewModel,
                                       IExtendedPlaylistViewModel
    {
        public PlaylistViewModel(IPlaylist playlist)
        {;
            this.ItemId = playlist?.Id ?? 0u;

            this.Title = playlist?.Title ?? string.Empty;
            this.CreatorName = playlist?.Creator?.Username ?? string.Empty;

            this.ArtworkUri = playlist?.Images?.HasPictureOfSize(PictureSize.Medium) ?? false ? playlist.Images.Medium
                                                                                              : "ms-appx:///Assets/StoreLogo.png";
            this.NumberOfFans = playlist?.NumberOfFans ?? 0u;
            this.NumberOfTracks = playlist?.NumberOfTracks ?? 0u;

            this.WebsiteLink = playlist?.Link ?? string.Empty;
            this.ShareLink = playlist?.ShareLink ?? string.Empty;
        }

        // IPlaylistViewModel
        public ulong ItemId { get; }

        public string Title { get; }
        public string CreatorName { get; }
        public string ArtworkUri { get; }

        // IExtndedPlaylistViewModel
        public uint NumberOfFans { get; }
        public uint NumberOfTracks { get; }

        public string ShareLink { get; }
        public string WebsiteLink { get; }
    }
}
