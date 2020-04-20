﻿using System;
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

            // Too many ? ?? Are you sure??
            this.ArtworkUri = track?.Artwork?.HasPictureOfSize(PictureSize.Medium) ?? false ? track.Artwork.Medium
                                                                                            : "ms-appx:///Assets/StoreLogo.png";
                
            this.TrackNumber = track?.TrackNumber.ToString() ?? string.Empty;
        }


        // ITrackViewModel  
        public bool IsPresent { get; }

        public ETrackLHSMode LHSMode { get; }

        public string Title { get; }
        public string Artist { get; }

        public string ArtworkUri { get; }

        public string TrackNumber { get; }
    }
}
