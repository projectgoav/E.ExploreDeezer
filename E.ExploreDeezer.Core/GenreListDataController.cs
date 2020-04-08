using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using E.Deezer;

using E.ExploreDeezer.Core.ViewModels;
using E.ExploreDeezer.Core.Collections;

namespace E.ExploreDeezer.Core
{
    internal class GenreListDataController
    {
        private readonly IDeezerSession session;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly FixedSizeObservableCollection<IGenreViewModel> genreList;


        public GenreListDataController(IDeezerSession session)
        {
            this.session = session;

            this.cancellationTokenSource = new CancellationTokenSource();
            this.genreList = new FixedSizeObservableCollection<IGenreViewModel>();

            FetchGenreList();
        }


        public IObservableCollection<IGenreViewModel> GenreList => this.genreList;



        private void FetchGenreList()
        {
            this.session.Genre.GetCommonGenre(this.cancellationTokenSource.Token)
                              .ContinueWith(t =>
                              {
                                  if (t.IsFaulted)
                                      return; //TODO

                                  this.genreList.SetContents(t.Result.Select(x => new GenreViewModel(x)));

                              }, this.cancellationTokenSource.Token, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }
    }

}
