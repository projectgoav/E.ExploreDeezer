using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

using E.Deezer;

using E.ExploreDeezer.Core.Mvvm;


namespace E.ExploreDeezer.Core.ViewModels
{
    public interface IGenreOverviewViewModel 
    {
        string PageTitle { get; }
        IGenreViewModel Genre { get; }

        IEnumerable<IAlbumViewModel> NewReleases { get; }
        IEnumerable<IAlbumViewModel> DeezerPicks { get; }

        TracklistViewModelParams CreateTracklistViewModelParams(IAlbumViewModel albumViewModel);
    }

    public interface IGenreOverviewViewModelParams
    {
        IGenreViewModel Genre { get; }
    }


    public class GenreOverviewViewModelParams : IGenreOverviewViewModelParams
    {
        public GenreOverviewViewModelParams(IGenreViewModel genre)
        {
            this.Genre = genre;
        }

        public IGenreViewModel Genre { get; }
    }
    

    internal class GenreOverviewViewModel : ViewModelBase,
                                            IGenreOverviewViewModel
    {
        private const uint kMaxNewReleases = 25;
        private const uint kMaxDeezerPicks = 25;

        private readonly IDeezerSession session;

        private IEnumerable<IAlbumViewModel> newReleases;
        private IEnumerable<IAlbumViewModel> deezerPicks;

        public GenreOverviewViewModel(IDeezerSession session,
                                      IPlatformServices platformServices,
                                      IGenreOverviewViewModelParams p)
            : base(platformServices)
        {
            this.session = session;

            this.newReleases = Array.Empty<IAlbumViewModel>();
            this.deezerPicks = Array.Empty<IAlbumViewModel>();

            this.Genre = p.Genre;

            FetchContents();
        }

        // IGenreOverviewViewModel
        public string PageTitle => Genre.Name;

        public IGenreViewModel Genre { get; }

        public IEnumerable<IAlbumViewModel> NewReleases
        {
            get => this.newReleases;
            private set => SetProperty(ref this.newReleases, value);
        }

        public IEnumerable<IAlbumViewModel> DeezerPicks
        {
            get => this.deezerPicks;
            private set => SetProperty(ref this.deezerPicks, value);
        }


        public TracklistViewModelParams CreateTracklistViewModelParams(IAlbumViewModel albumViewModel)
            => ViewModelParamFactory.CreateTracklistViewModelParams(albumViewModel);


        private void FetchContents()
        {
            this.session.Genre.GetNewReleasesForGenre(this.Genre.Id, this.CancellationToken, 0, kMaxNewReleases)
                              .ContinueWith(t =>
                              {
                                  if (t.IsFaulted)
                                      return; //TODO

                                  this.NewReleases = t.Result.Select(x => new AlbumViewModel(x))
                                                             .ToList();

                              }, this.CancellationToken, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

            this.session.Genre.GetDeezerSelectionForGenre(this.Genre.Id, this.CancellationToken, 0, kMaxDeezerPicks)
                              .ContinueWith(t =>
                              {
                                  if (t.IsFaulted)
                                      return; //TODO

                                  this.DeezerPicks = t.Result.Select(x => new AlbumViewModel(x))
                                                             .ToList();

                              }, this.CancellationToken, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.NewReleases = Array.Empty<IAlbumViewModel>();
                this.DeezerPicks = Array.Empty<IAlbumViewModel>();
            }
            
            base.Dispose(disposing);
        }
    }
}
