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

            this.collection.CollectionChanged += Collection_CollectionChanged;

            this.tokenSource = new ResetableCancellationTokenSource();
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
