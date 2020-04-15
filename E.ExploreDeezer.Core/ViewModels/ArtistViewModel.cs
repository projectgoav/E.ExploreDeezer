using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using E.Deezer.Api;

namespace E.ExploreDeezer.Core.ViewModels
{
    public interface IArtistViewModel
    {
        ulong Id { get; }
        string Name { get; }
        string ArtworkUri { get; }
    }

    // Simply a marker interface to indicate where additional work needs to be done
    // in order to fetch these properties, as by default they are not returned by
    // the Deezer API
    public interface IExtendedArtistViewModel
    {
        uint NumberOfFans { get; }
        uint NumberOfAlbums { get; }

        string ShareLink { get; }
        string WebsiteLink { get; }
    }

    internal class ArtistViewModel : IArtistViewModel,
                                     IExtendedArtistViewModel
    {
        public ArtistViewModel(IArtist artist)
        {
            Id = artist?.Id ?? 0u;
            Name = artist?.Name ?? string.Empty;
            ArtworkUri = artist?.Pictures.Medium ?? "ms-appx:///Assets/StoreLogo.png"; //TODO: Fallback artwork

            NumberOfFans = artist?.NumberOfFans ?? 0u;
            NumberOfAlbums = artist?.NumberOfAlbums ?? 0u;

            WebsiteLink = artist?.Link ?? string.Empty;
            ShareLink = artist?.ShareLink ?? string.Empty;
        }


        // IArtistViewModel
        public ulong Id { get; }
        public string Name { get; }
        public bool IsPresent { get; }

        public string ArtworkUri { get; }


        // IExtendedArtistViewModel
        public uint NumberOfFans { get; }
        public uint NumberOfAlbums { get; }

        public string ShareLink { get; }
        public string WebsiteLink { get; }
    }
}
