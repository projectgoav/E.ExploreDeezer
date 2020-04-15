using System;
using System.Collections.Generic;
using System.Text;

using E.Deezer.Api;

namespace E.ExploreDeezer.Core.ViewModels
{
    public interface IAlbumViewModel
    {
        ulong ItemId { get; }
        string Title { get; }

        ulong ArtistId { get; }
        string ArtistName { get; }

        string ArtworkUri { get; }
    }

    // Properties requiring a fetch of the complete album
    public interface IExtendedAlbumViewModel : IAlbumViewModel
    {
        uint NumberOfFans { get; }
        uint NumberOfTracks { get; }

        string ShareLink { get; }
        string WebsiteLink { get; }
    }


    internal class AlbumViewModel : IAlbumViewModel,
                                    IExtendedAlbumViewModel
    {
        public AlbumViewModel(IAlbum album)
        {
            this.ItemId = album?.Id ?? 0;

            this.Title = album?.Title;
            this.ArtistName = album?.ArtistName;
            this.ArtistId = album?.Artist?.Id ?? 0;

            this.ArtworkUri = album?.CoverArtwork?.Medium ?? "ms-appx:///Assets/StoreLogo.png"; //TODO: Fallback artwork

            this.NumberOfFans = album?.Fans ?? 0u;
            this.NumberOfTracks = album?.TrackCount ?? 0u;

            this.ShareLink = album?.ShareLink ?? string.Empty;
            this.WebsiteLink = album?.Link ?? string.Empty;         
        }


        //IAlbumViewModel
        public ulong ItemId { get; }
        public string Title { get; }
        public ulong ArtistId { get; }
        public string ArtistName { get; }
        public string ArtworkUri { get; }

        public bool IsPresent { get; }

        // IExtendedAlbumViewModel
        public uint NumberOfFans { get; }
        public uint NumberOfTracks { get; }

        public string ShareLink { get; }
        public string WebsiteLink { get; }
    }
}
