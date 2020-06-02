using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Threading;

using E.ExploreDeezer.Core.Mvvm;
using E.ExploreDeezer.Core.Util;

namespace E.ExploreDeezer.Core.Collections
{
    /* CustomCollections: MainThreadObservableCollectionAdapter
     * 
     * Since most UI components backed by a collection of items require
     * updates to be carried out on the main thread, this adapter class
     * can be used to marshall events onto the UI thread.
     * 
     * Once this class has been disposed, any pending and future raises
     * of events will not be executed. 
     * 
     * NOTE: Class could be improved slightly by keep a queue of all
     * events that occur while waiting for the main thread to execute
     * them. If there are any duplicated events in that time period we'd
     * only have to execute one of them. However, as the collections in this
     * application don't update that frequently the complexity in managing
     * that is a little OTT */
    internal class MainThreadObservableCollectionAdapter<T> : ObservableCollectionBase<T>
    {
        private readonly IObservableCollection<T> collection;
        private readonly IMainThreadDispatcher mainThreadDispatcher;
        private readonly ResetableCancellationTokenSource tokenSource;


        public MainThreadObservableCollectionAdapter(IObservableCollection<T> theCollection,
                                                     IMainThreadDispatcher mainThreadDispatcher)
        {
            this.collection = theCollection;
            this.mainThreadDispatcher = mainThreadDispatcher;

            this.tokenSource = new ResetableCancellationTokenSource();

            this.collection.CollectionChanged += Collection_CollectionChanged;
        }


        // ObservableCollectionBase
        public override T GetItem(int index)
            => this.collection.GetItem(index);

        public override int Count => this.collection.Count;


        public override int IndexOfItem(T item)
            => this.collection.IndexOfItem(item);

        public override bool ContainsItem(T item)
            => this.collection.ContainsItem(item);


        public override void Clear()
            => this.collection.Clear();


        public override IEnumerator<T> GetEnumerator()
            => this.collection.GetEnumerator();



        private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var token = this.tokenSource.Token;

            this.mainThreadDispatcher.ExecuteOnMainThreadAsync(() =>
            {
                if (!token.IsCancellationRequested)
                {
                    this.NotifyChanged(e);
                }
            });
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.tokenSource.Cancel();
                this.tokenSource.Dispose();

                this.collection.CollectionChanged -= Collection_CollectionChanged;
            }

            base.Dispose(disposing);
        }
    }
}
