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
    internal class DeezerPicksDataController
    {
        private readonly IDeezerSession session;
        private readonly PagedObservableCollection<IAlbumViewModel> albumCollection;


        private ulong genreId;

        public DeezerPicksDataController(IDeezerSession session)
        {
            this.session = session;

            this.genreId = ulong.MaxValue;
            this.albumCollection = new PagedObservableCollection<IAlbumViewModel>();
        }


        public void SetGenreId(ulong genreId)
        {
            if (this.genreId == genreId)
                return;

            this.genreId = genreId;


            ItemFetcher<IAlbumViewModel> fetcher = (int startingIndex, int numberOfItems, CancellationToken token)
                => this.session.Genre.GetDeezerSelectionForGenre(this.genreId, token, (uint)startingIndex, (uint)numberOfItems)
                                     .ContinueWith<IEnumerable<IAlbumViewModel>>(t =>
                                     {
                                         if (t.IsFaulted)
                                             throw t.Exception.GetBaseException();

                                         return t.Result.Select(x => new AlbumViewModel(x));

                                     }, token, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

            this.albumCollection.SetFetcher(fetcher);
        }

        public IObservableCollection<IAlbumViewModel> DeezerPicks => this.albumCollection;
    }
}
