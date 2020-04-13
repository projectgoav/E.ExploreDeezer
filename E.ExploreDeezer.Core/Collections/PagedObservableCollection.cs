using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;

using E.ExploreDeezer.Core.Util;

namespace E.ExploreDeezer.Core.Collections
{
    internal delegate Task<IEnumerable<T>> ItemFetcher<T>(int startingIndex,
                                                          int numberOfItems,
                                                          CancellationToken cancellationToken);


    internal class PagedObservableCollection<T> : ObservableCollectionBase<T>
    {
        private class PagedObservableCollectionEnumerator : IEnumerator,
                                                            IEnumerator<T>
        {
            private readonly int pageSize;
            private readonly Dictionary<int, IReadOnlyList<T>> pages;

            private int currentPage;
            private int currentPageIndex;

            public PagedObservableCollectionEnumerator(int pageSize, IReadOnlyDictionary<int, IReadOnlyList<T>> pages)
            {
                this.pageSize = pageSize;
                this.pages = pages.ToDictionary(kvp => kvp.Key, kvp => kvp.Value); //No constructor of Dictionary accepts IReadOnly interface

                this.currentPage = 0;
                this.currentPageIndex = -1;
            }


            public object Current => this.pages[currentPage][currentPageIndex];

            T IEnumerator<T>.Current => this.pages[currentPage][currentPageIndex];

            public bool MoveNext()
            {
                if (this.pages.Count == 0)
                    return false;


                // Step through page
                ++this.currentPageIndex;

                // If we've finished a page, move onto the next one
                if (currentPageIndex >= this.pageSize)
                {
                    ++this.currentPage;

                    // No more pages...
                    if (!this.pages.ContainsKey(this.currentPage))
                        return false;


                    // Pages ready
                    this.currentPageIndex = 0;
                    return true;
                }
                else
                {
                    // Check we've still got contents in the current page to use.
                    return this.currentPageIndex < this.pages[currentPage].Count;
                }
            }

            public void Reset()
            {
                this.currentPage = 0;
                this.currentPageIndex = 0;
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
                    this.pages.Clear();
                }
            }
        }

        private const int DEFAULT_PAGE_SIZE = 50;
        private const int DEFAULT_THREASHOLD = 50;

        private readonly int nextPageThreashold;
        private readonly Dictionary<int, IReadOnlyList<T>> pages;
        private readonly HashSet<int> pageFetchInProgress;
        private readonly ResetableCancellationTokenSource cancellationTokenSource;

        private ItemFetcher<T> itemFetcher;


        public PagedObservableCollection(int pageSize = DEFAULT_PAGE_SIZE,
                                         int nextPageThreashold = DEFAULT_THREASHOLD)
        {
            this.PageSize = pageSize;
            this.nextPageThreashold = nextPageThreashold;

            this.pages = new Dictionary<int, IReadOnlyList<T>>();
            this.pageFetchInProgress = new HashSet<int>();
            this.cancellationTokenSource = new ResetableCancellationTokenSource();
        }


        public int PageSize { get; }

        // ObservableCollectionBase
        public override T GetItem(int index)
        {
            int page = 0;
            int indexInPage = index;

            // Try to be fancy and do:
            // => page = index / this.PageSize
            // => indexInPath = index % this.PageSize
            // in the one set of operations   
            while (indexInPage >= this.PageSize)
            {
                indexInPage -= this.PageSize;
                ++page;
            }

            Assert.That(this.pages.ContainsKey(page), "Attempting to read an item before it's been fetched.");

            // Start populating the next set of items
            // once we get close to the bottom
            if ((indexInPage + 1) >= this.nextPageThreashold)
                SchedulePageFetch(page + 1);

            return this.pages[page][indexInPage];
        }


        public override int Count => this.pages.Sum(kvp => kvp.Value.Count);


        public override int IndexOfItem(T value)
            => this.pages.SelectMany(kvp => kvp.Value)
                         .ToList()
                         .IndexOf(value);

        public override bool ContainsItem(T item)
            => this.pages.SelectMany(kvp => kvp.Value)
                         .Contains(item);

        public override void Clear()
        {
            this.cancellationTokenSource.Reset();

            this.pages.Clear();
            this.pageFetchInProgress.Clear();

            NotifyReset();
        }


        public override IEnumerator<T> GetEnumerator()
            => new PagedObservableCollectionEnumerator(this.PageSize, this.pages);



        public void SetFetcher(ItemFetcher<T> fetcher)
        {
            this.itemFetcher = fetcher;

            ResetContents();

            FetchFirstPage();
        }

        private void ResetContents()
        {
            this.cancellationTokenSource.Reset();

            this.pages.Clear();
            this.pageFetchInProgress.Clear();

            NotifyReset();
        }

        private void FetchFirstPage()
            => FetchPage(0)
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                            return; //TODO: Throw something or report an error?

                        if (t.Result.Any())
                        {
                            this.pages.Add(0, new List<T>(t.Result));
                            NotifyPageAdded(0);
                        }
                    }, this.cancellationTokenSource.Token, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);


        private void NotifyReset()
            => this.NotifyChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

        private void NotifyPageAdded(int pageNumber)
            => this.NotifyChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, this.pages[pageNumber] as IList, pageNumber * this.PageSize));


        private void SchedulePageFetch(int pageNumber)
        {
            if (this.pageFetchInProgress.Contains(pageNumber))
                return;

            this.pageFetchInProgress.Add(pageNumber);

            FetchPage(pageNumber)
                .ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        //TODO
                        return;
                    }

                    if (t.Result.Any())
                    {
                        this.pages.Add(pageNumber, new List<T>(t.Result));
                        NotifyPageAdded(pageNumber);
                    }
                }, this.cancellationTokenSource.Token, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

        }


        private Task<IEnumerable<T>> FetchPage(int number)
        {
            int startingIndex = number * this.PageSize;
            int numberOfItems = this.PageSize;

            return this.itemFetcher(startingIndex, numberOfItems, this.cancellationTokenSource.Token);
        }
    }
}
