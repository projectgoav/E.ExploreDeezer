using System;
using System.Collections.Generic;
using System.Text;

using E.Deezer.Api;

namespace E.ExploreDeezer.ViewModels
{
    public interface IAlbumViewModel
    {
        ulong ItemId { get; }
        string Title { get; }
        string ArtistName { get; }

        uint NumberOfTracks { get; }

        //TODO: Artwork :)
        string ArtworkUri { get; }

        bool IsPresent { get; }
    }


    internal class AlbumViewModel : IAlbumViewModel
    {
        public AlbumViewModel(IAlbum album)
        {
            this.IsPresent = album != null;

            this.ItemId = album?.Id ?? 0;
            this.Title = album?.Title;
            this.ArtistName = album?.ArtistName;
            this.NumberOfTracks = album?.TrackCount ?? 0u;
            this.ArtworkUri = album?.CoverArtwork.Medium;
        }


        //IAlbumViewModel
        public ulong ItemId { get; }
        public string Title { get; }
        public uint NumberOfTracks { get; }
        public string ArtistName { get; }
        public string ArtworkUri { get; }

        public bool IsPresent { get; }


        public static IAlbumViewModel Empty => new AlbumViewModel(null);
    }
}
