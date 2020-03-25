using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

using E.Deezer;

using E.ExploreDeezer.Core.Mvvm;
using E.ExploreDeezer.Core.ViewModels;

namespace E.ExploreDeezer.Core.ViewModels
{
    public interface IArtistOverviewViewModel
    {
        string PageTitle { get; }
       
        IArtistViewModel Artist { get; }

        IEnumerable<IAlbumViewModel> Albums { get; }
        IEnumerable<ITrackViewModel> TopTracks { get; }
        IEnumerable<IArtistViewModel> RelatedArtists { get; }
        IEnumerable<IPlaylistViewModel> FeaturedPlaylists { get; }


        TracklistViewModelParams CreateTracklistViewModelParams(IAlbumViewModel album);
        TracklistViewModelParams CreateTracklistViewModelParams(IPlaylistViewModel playlist);
        ArtistOverviewViewModelParams CreateArtistOverviewViewModelParams(IArtistViewModel artist);
    }

    public struct ArtistOverviewViewModelParams
    {
        public ArtistOverviewViewModelParams(IArtistViewModel artistViewModel)
        {
            this.Artist = artistViewModel;
        }

        public IArtistViewModel Artist { get; }
    }


    internal class ArtistOverviewViewModel : ViewModelBase,
                                             IArtistOverviewViewModel
                        
    {
        private const uint kMaxAlbumCount = 100;
        private const uint kMaxTopTrackCount = 25;
        private const uint kMaxPlaylistCount = 100;
        private const uint kMaxRelatedArtistCount = 25;

        private readonly IDeezerSession session;

        private IEnumerable<IAlbumViewModel> albums;
        private IEnumerable<ITrackViewModel> topTracks;
        private IEnumerable<IArtistViewModel> relatedArtists;
        private IEnumerable<IPlaylistViewModel> featuredPlaylists;


        public ArtistOverviewViewModel(IDeezerSession session,
                                       IPlatformServices platformServices,
                                       ArtistOverviewViewModelParams p)
            : base(platformServices)
        {
            this.session = session;
            this.Artist = p.Artist;

            this.albums = Array.Empty<IAlbumViewModel>();
            this.topTracks = Array.Empty<ITrackViewModel>();
            this.relatedArtists = Array.Empty<IArtistViewModel>();
            this.featuredPlaylists = Array.Empty<IPlaylistViewModel>();

            FetchContent();
        }



        // IArtistOverviewViewModel
        public string PageTitle => this.Artist.Name;
        
        public IArtistViewModel Artist { get; }

        public IEnumerable<IAlbumViewModel> Albums
        {
            get => this.albums;
            private set => SetProperty(ref this.albums, value);
        }

        public IEnumerable<ITrackViewModel> TopTracks
        {
            get => this.topTracks;
            private set => SetProperty(ref this.topTracks, value);
        }

        public IEnumerable<IArtistViewModel> RelatedArtists
        {
            get => this.relatedArtists;
            private set => SetProperty(ref this.relatedArtists, value);
        }

        public IEnumerable<IPlaylistViewModel> FeaturedPlaylists
        {
            get => this.featuredPlaylists;
            private set => SetProperty(ref this.featuredPlaylists, value);
        }


        private void FetchContent()
        {
            this.session.Artists.GetArtistsAlbums(this.Artist.Id, this.CancellationToken, 0, kMaxAlbumCount)
                                .ContinueWith(t =>
                                {
                                    if (t.IsFaulted)
                                        return; //TODO

                                    this.Albums = t.Result.Select(x => new AlbumViewModel(x))
                                                          .ToList();

                                }, this.CancellationToken, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

            this.session.Artists.GetArtistsTopTracks(this.Artist.Id, this.CancellationToken, 0, kMaxTopTrackCount)
                                .ContinueWith(t =>
                                {
                                    if (t.IsFaulted)
                                        return; //TODO

                                    this.TopTracks = t.Result.Select(x => new TrackViewModel(x))
                                                             .ToList();

                                }, this.CancellationToken, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

            this.session.Artists.GetPlaylistsFeaturingArtist(this.Artist.Id, this.CancellationToken, 0, kMaxPlaylistCount)
                                .ContinueWith(t =>
                                {
                                    if (t.IsFaulted)
                                        return; //TODO

                                    this.FeaturedPlaylists = t.Result.Select(x => new PlaylistViewModel(x))
                                                                     .ToList();

                                }, this.CancellationToken, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

            this.session.Artists.GetRelatedArtists(this.Artist.Id, this.CancellationToken, 0, kMaxRelatedArtistCount)
                                .ContinueWith(t =>
                                {
                                    if (t.IsFaulted)
                                        return; //TODO

                                    this.RelatedArtists = t.Result.Select(x => new ArtistViewModel(x))
                                                                  .ToList();

                                }, this.CancellationToken, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }


        public TracklistViewModelParams CreateTracklistViewModelParams(IAlbumViewModel album)
            => ViewModelParamFactory.CreateTracklistViewModelParams(album);

        public TracklistViewModelParams CreateTracklistViewModelParams(IPlaylistViewModel playlist)
            => ViewModelParamFactory.CreateTracklistViewModelParams(playlist);

        public ArtistOverviewViewModelParams CreateArtistOverviewViewModelParams(IArtistViewModel artist)
            => ViewModelParamFactory.CreateArtistOverviewViewModelParams(artist);


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Albums = Array.Empty<IAlbumViewModel>();
                this.TopTracks = Array.Empty<ITrackViewModel>();
                this.RelatedArtists = Array.Empty<IArtistViewModel>();
                this.FeaturedPlaylists = Array.Empty<IPlaylistViewModel>();
            }

            base.Dispose(disposing);
        }
    }
}
