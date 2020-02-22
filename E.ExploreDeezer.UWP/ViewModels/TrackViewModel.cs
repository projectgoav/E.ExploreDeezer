using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using E.Deezer.Api;

namespace E.ExploreDeezer.ViewModels
{
    public interface ITrackViewModel
    {
        bool IsPresent { get; }

        string Title { get; }
        string Artist { get; }

        string ArtworkUri { get; }
    }

    
    internal class TrackViewModel : ITrackViewModel
    {

        public TrackViewModel(ITrack track)
        {
            this.IsPresent = track != null;

            this.Title = track?.Title ?? string.Empty;
            this.Artist = track?.ArtistName ?? string.Empty;

            this.ArtworkUri = track?.Artwork?.Medium ?? string.Empty;
        }


        // ITrackViewModel  
        public bool IsPresent { get; }

        public string Title { get; }
        public string Artist { get; }

        public string ArtworkUri { get; }

    }
}
