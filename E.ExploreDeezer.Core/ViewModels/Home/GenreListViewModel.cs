using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using E.Deezer;

using E.ExploreDeezer.Core.Mvvm;

namespace E.ExploreDeezer.Core.ViewModels.Home
{
    public interface IGenreListViewModel : IDisposable
    {
        EContentFetchStatus FetchStatus { get; }

        IEnumerable<IGenreViewModel> GenreList { get; }

        GenreOverviewViewModelParams CreateGenreOverviewViewModelParams(IGenreViewModel genre);
    }

    internal class GenreListViewModel : ViewModelBase,
                                        IGenreListViewModel,
                                        IDisposable
    {
        private readonly IDeezerSession session;


        private EContentFetchStatus fetchStatus;
        private IEnumerable<IGenreViewModel> genreList;

        public GenreListViewModel(IDeezerSession session,
                                  IPlatformServices platformServices)
            : base(platformServices)
        {
            this.session = session;

            FetchContent();
        }


        // IGenreViewModel
        public EContentFetchStatus FetchStatus
        {
            get => this.fetchStatus;
            private set => SetProperty(ref this.fetchStatus, value);
        }

        public IEnumerable<IGenreViewModel> GenreList
        {
            get => this.genreList;
            private set => SetProperty(ref this.genreList, value);
        }


        public GenreOverviewViewModelParams CreateGenreOverviewViewModelParams(IGenreViewModel genreViewModel)
        {
            if (genreViewModel == null || !genreViewModel.IsPresent)
                throw new ArgumentException();

            return new GenreOverviewViewModelParams(genreViewModel);
        }



        private void FetchContent()
        {
            this.FetchStatus = EContentFetchStatus.Loading;

            this.session.Genre.GetCommonGenre(this.CancellationToken)
                              .ContinueWith(t =>
                              {
                                  if (t.IsFaulted || t.IsCanceled)
                                  {
                                      this.FetchStatus = EContentFetchStatus.Error;
                                      return;
                                  }

                                  var genreList = t.Result.Select(x => new GenreViewModel(x))
                                                          .ToList();

                                  this.GenreList = genreList;

                                  this.FetchStatus = genreList.Count == 0 ? EContentFetchStatus.Empty
                                                                          : EContentFetchStatus.Available;
                              }, this.CancellationToken, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }




        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.GenreList = Array.Empty<IGenreViewModel>();
            }

            base.Dispose(disposing);
        }

    }
}
