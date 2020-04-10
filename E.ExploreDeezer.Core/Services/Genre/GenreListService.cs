using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using E.Deezer;

using E.ExploreDeezer.Core.Util;
using E.ExploreDeezer.Core.ViewModels;
using E.ExploreDeezer.Core.Collections;

namespace E.ExploreDeezer.Core.Services.Genre
{
    internal class GenreListService : IGenreListService
    {
        private readonly IDeezerSession session;
        private readonly UpdatableFetchState fetchState;
        private readonly ResetableCancellationTokenSource tokenSource;
        private readonly FixedSizeObservableCollection<IGenreViewModel> genreList;

        public GenreListService(IDeezerSession session)
        {
            this.session = session;

            this.fetchState = new UpdatableFetchState();
            this.tokenSource = new ResetableCancellationTokenSource();
            this.genreList = new FixedSizeObservableCollection<IGenreViewModel>();
        }


        // IDataFetchingService
        public string Id => "GenreList";

        public EFetchState FetchState => this.fetchState.CurrentState;

        public event FetchStateChangedEventHandler OnFetchStateChanged
        {
            add => this.fetchState.OnFetchStateChanged += value;
            remove => this.fetchState.OnFetchStateChanged -= value;
        }

        //IGenreListService
        public IObservableCollection<IGenreViewModel> GenreList => this.genreList;


        public Task RefreshGenreListAsync()
        {
            if (this.genreList.Count > 0)
                return Task.CompletedTask;

            this.fetchState.SetLoading();


            return this.session.Genre.GetCommonGenre(this.tokenSource.Token)
                                     .ContinueWith(t =>
                                     {
                                         if (t.IsFaulted)
                                         {
                                             //TODO: Proper logging
                                             System.Diagnostics.Debug.WriteLine($"Failed to fetch genre list.\n{t.Exception.GetBaseException()}");
                                             this.fetchState.SetError();
                                             return;
                                         }
 
                                         this.genreList.SetContents(t.Result.Select(x => new GenreViewModel(x)));
 
                                         if (this.genreList.Count == 0)
                                         {
                                             this.fetchState.SetEmpty();
                                         }
                                         else
                                         {
                                             this.fetchState.SetAvailable();
                                         }

                                    }, this.tokenSource.Token, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

        }


        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.genreList.ClearContents();

                this.tokenSource.Dispose();
                this.fetchState.Dispose();
            }
        }
    }

}
