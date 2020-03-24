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


        ITracklistViewModelParams CreateTracklistViewModelParams(IAlbumViewModel album);
    }

    public interface IArtistOverviewViewModelParams
    {
        IArtistViewModel Artist { get; }
    }


    internal class ArtistOverviewViewModelParams : IArtistOverviewViewModelParams
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

        private readonly IDeezerSession session;

        private IEnumerable<IAlbumViewModel> albums;


        public ArtistOverviewViewModel(IDeezerSession session,
                                       IPlatformServices platformServices,
                                       IArtistOverviewViewModelParams p)
            : base(platformServices)
        {
            this.session = session;
            this.Artist = p.Artist;

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
        }


        public ITracklistViewModelParams CreateTracklistViewModelParams(IAlbumViewModel album)
        {
            if (album == null || !album.IsPresent)
                throw new ArgumentException();

            return new TracklistViewModelParams(ETracklistViewModelType.Album, album);
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Albums = Array.Empty<IAlbumViewModel>();
            }

            base.Dispose(disposing);
        }
    }
}
