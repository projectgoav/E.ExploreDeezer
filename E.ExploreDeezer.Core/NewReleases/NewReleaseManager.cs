using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using E.Deezer;

using E.ExploreDeezer.Core.NewReleases.Store;

namespace E.ExploreDeezer.Core.NewReleases
{
    public interface INewReleaseManager
    {
        void FetchNewReleases();

        void FetchDeezerPicks();
    }


    internal class NewReleaseManager : INewReleaseManager, 
                                       IDisposable
    {
        private const ulong DEFAULT_GENRE_ID = 0;

        private readonly object lockObject;
        private readonly IDeezerSession session;
        private readonly IStore<AppState> store;
        private readonly CancellationTokenSource cancellationTokenSource;

        private bool fetchedNewReleases;
        private bool fetchedDeezerPicks;

        public NewReleaseManager(IStore<AppState> store,
                                 IDeezerSession session)
        {
            this.store = store;
            this.session = session;

            this.lockObject = new object();
            this.cancellationTokenSource = new CancellationTokenSource();
        }


        public void FetchNewReleases()
        {
            lock(this.lockObject)
            {
                if (fetchedNewReleases)
                    return;

                this.fetchedNewReleases = true;
            }

            this.store.Dispatch(new FetchNewReleases());

            this.session.Genre.GetNewReleasesForGenre(DEFAULT_GENRE_ID, this.cancellationTokenSource.Token, 0, 50)
                              .ContinueWith(t =>
                              {
                                  if (t.IsFaulted)
                                  {
                                      this.store.Dispatch(new SetNewReleaseFetchFailure(t.Exception
                                                                                         .GetBaseException()
                                                                                         .Message));
                                  }
                                  else
                                  {
                                      this.store.Dispatch(new SetNewReleaseFetchSuccess(t.Result));
                                  }
                              }, 
                              this.cancellationTokenSource.Token,
                              TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously,
                              TaskScheduler.Default);
        }

        public void FetchDeezerPicks()
        {
            lock(this.lockObject)
            {
                if (fetchedDeezerPicks)
                    return;

                this.fetchedDeezerPicks = true;
            }

            this.session.Genre.GetDeezerSelectionForGenre(DEFAULT_GENRE_ID, this.cancellationTokenSource.Token, 0, 50)
                              .ContinueWith(t =>
                              {
                                  if (t.IsFaulted)
                                  {
                                      this.store.Dispatch(new SetDeezerPicksFetchFailure(t.Exception
                                                                                          .GetBaseException()
                                                                                          .Message));
                                  }
                                  else
                                  {
                                      this.store.Dispatch(new SetDeezerPicksFetchSuccess(t.Result));
                                  }
                              },
                              this.cancellationTokenSource.Token,
                              TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously,
                              TaskScheduler.Default);
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
                this.cancellationTokenSource.Cancel();
                this.cancellationTokenSource.Dispose();
            }
        }


    }
}
