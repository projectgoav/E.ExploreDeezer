using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using E.Deezer.Api;

namespace E.ExploreDeezer.ViewModels
{
    public interface IGenreViewModel
    {
        bool IsPresent { get; }

        string Name { get; }

        string ArtworkUri { get; }
    }


    internal class GenreViewModel : IGenreViewModel
    {

        public GenreViewModel(IGenre genre)
        {
            this.IsPresent = genre != null;

            this.Name = genre?.Name ?? string.Empty;

            this.ArtworkUri = genre?.Images?.Medium ?? string.Empty;
        }


        //IGenreViewModel
        public bool IsPresent { get; }

        public string Name { get; }

        public string ArtworkUri { get; }

    }
}
