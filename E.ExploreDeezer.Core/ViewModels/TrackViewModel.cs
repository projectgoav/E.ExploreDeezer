using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using E.Deezer.Api;

namespace E.ExploreDeezer.Core.ViewModels
{
    public enum ETrackLHSMode : byte
    {
        Number,
        Artwork
    }

    public enum ETrackArtistMode : byte
    {
        Name,
        NameWithLink
    }

    public interface ITrackViewModel
    {
        ETrackLHSMode LHSMode { get; }
        ETrackArtistMode ArtistMode { get; }

        ulong ItemId { get; }
        string Title { get; }
        string Artist { get; }
        ulong ArtistId { get; }

        string TrackNumber { get; }

        string ArtworkUri { get; }
    }

    
    internal class TrackViewModel : ITrackViewModel
    {

        public TrackViewModel(ITrack track, 
                              ETrackLHSMode lhsMode,
                              ETrackArtistMode artistMode)
        {
            this.LHSMode = lhsMode;
            this.ArtistMode = artistMode;

            this.ItemId = track?.Id ?? 0u;
            this.Title = track?.Title ?? string.Empty;
            this.Artist = track?.ArtistName ?? string.Empty;
            this.ArtistId = track?.Artist?.Id ?? 0u;

            // Too many ? ?? Are you sure??
            this.ArtworkUri = track?.Artwork?.HasPictureOfSize(PictureSize.Medium) ?? false ? track.Artwork.Medium
                                                                                            : "ms-appx:///Assets/StoreLogo.png";
                
            this.TrackNumber = track?.TrackNumber.ToString() ?? string.Empty;
        }


        // ITrackViewModel  
        public ETrackLHSMode LHSMode { get; }
        public ETrackArtistMode ArtistMode { get; }

        public ulong ItemId { get; }
        public string Title { get; }
        public string Artist { get; }
        public ulong ArtistId { get; }

        public string ArtworkUri { get; }

        public string TrackNumber { get; }
    }
}
