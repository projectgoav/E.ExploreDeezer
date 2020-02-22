using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using E.Deezer.Api;

namespace E.ExploreDeezer.ViewModels
{
    public interface IPlaylistViewModel
    {
        bool IsPresent { get; }

        string Title { get; }
        string CreatorName { get; }

        string ArtworkUri { get; }
    }

    internal class PlaylistViewModel : IPlaylistViewModel
    {
        public PlaylistViewModel(IPlaylist playlist)
        {
            this.IsPresent = playlist != null;

            this.Title = playlist?.Title ?? string.Empty;
            this.CreatorName = playlist?.Creator?.Username ?? string.Empty;
            this.ArtworkUri = playlist?.Images?.Medium ?? string.Empty;
        }

        // IPlaylistViewModel
        public bool IsPresent { get; }

        public string Title { get; }

        public string CreatorName { get; }

        public string ArtworkUri { get; }
    }
}
