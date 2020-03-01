using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using E.Deezer.Api;

namespace E.ExploreDeezer.Core.ViewModels
{
    public enum ETrackLHSMode
    {
        Number,
        Artwork
    }

    public interface ITrackViewModel
    {
        bool IsPresent { get; }
        ETrackLHSMode LHSMode { get; }

        string Title { get; }
        string Artist { get; }

        string TrackNumber { get; }
        string AllArtistNames { get; }

        string ArtworkUri { get; }
    }

    
    internal class TrackViewModel : ITrackViewModel
    {

        public TrackViewModel(ITrack track, ETrackLHSMode lhsMode = ETrackLHSMode.Number)
        {
            this.IsPresent = track != null;

            this.LHSMode = lhsMode;

            this.Title = track?.Title ?? string.Empty;
            this.Artist = track?.ArtistName ?? string.Empty;

            this.ArtworkUri = track?.Artwork?.Medium ?? string.Empty;

            this.TrackNumber = track?.TrackNumber.ToString() ?? string.Empty;

            this.AllArtistNames = GetAllArtistNames(track);
        }


        // ITrackViewModel  
        public bool IsPresent { get; }

        public ETrackLHSMode LHSMode { get; }

        public string Title { get; }
        public string Artist { get; }

        public string ArtworkUri { get; }

        public string TrackNumber { get; }
        public string AllArtistNames { get; }



        private string GetAllArtistNames(ITrack track)
        {
            if (track == null)
                return string.Empty;

            bool hasMainArtist = !string.IsNullOrEmpty(track.ArtistName);
            bool hasContributors = track.Contributors.Any();

            if (hasMainArtist && !hasContributors)
            {
                return track.ArtistName;
            }

            var sb = new StringBuilder(256);

            sb.Append(track.ArtistName);
            sb.Append(" ft. ");

            sb.Append(track.Contributors.First()
                                        .Name);

            foreach (var artist in track.Contributors.Skip(1))
            {
                sb.Append(", ");
                sb.Append(artist.Name);
            }


            return sb.ToString();
        }

    }
}
