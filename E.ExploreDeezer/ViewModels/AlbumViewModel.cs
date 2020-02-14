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

        bool IsPresent { get; }
    }


    internal class AlbumViewModel : IAlbumViewModel
    {
        public AlbumViewModel(IAlbum album)
        {
            this.IsPresent = album != null;

            this.Title = album?.Title;
            this.ArtistName = album?.ArtistName;
        }


        //IAlbumViewModel
        public string Title { get; }
        public string ArtistName { get; }

        public bool IsPresent { get; }


        public static IAlbumViewModel Empty => new AlbumViewModel(null);
    }
}
