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
        bool IsPresent { get; }
    }

    internal class ArtistViewModel : IArtistViewModel
    {
        public ArtistViewModel(IArtist artist)
        {
            IsPresent = artist != null;

            Id = artist?.Id ?? 0u;
            Name = artist?.Name ?? string.Empty;
            ArtworkUri = artist?.Pictures.Medium ?? string.Empty;
        }


        // IArtistViewModel
        public ulong Id { get; }
        public string Name { get; }
        public bool IsPresent { get; }

        public string ArtworkUri { get; }
    }
}
