using System;
using System.Collections.Generic;
using System.Text;

using E.Deezer.Api;

namespace E.ExploreDeezer.ViewModels
{
    public interface IAlbumViewModel
    {
        string Title { get; }
        string ArtistName { get; }

        //TODO: Artwork :)
        string ArtworkUri { get; }

        bool IsPresent { get; }
    }


    internal class AlbumViewModel : IAlbumViewModel
    {
        public AlbumViewModel(IAlbum album)
        {
            this.IsPresent = album != null;

            this.Title = album?.Title;
            this.ArtistName = album?.ArtistName;
            this.ArtworkUri = album?.CoverArtwork.Medium;
        }


        //IAlbumViewModel
        public string Title { get; }
        public string ArtistName { get; }
        public string ArtworkUri { get; }

        public bool IsPresent { get; }


        public static IAlbumViewModel Empty => new AlbumViewModel(null);
    }
}
