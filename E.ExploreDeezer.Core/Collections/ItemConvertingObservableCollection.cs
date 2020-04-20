using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

using E.ExploreDeezer.Core.Mvvm;
using E.ExploreDeezer.Core.Util;
using System.Collections;

namespace E.ExploreDeezer.Core.Collections
{
    public class ItemConvertingObservableCollection<TA, TB> : ObservableCollectionBase<TB>
    {
        private class ItemConvertingEnumerator : IEnumerator<TB>
        {
            private readonly IEnumerator<TA> enumerator;
            private readonly Func<TA, TB> aToBConverter;

            
            public ItemConvertingEnumerator(IEnumerator<TA> enumerator,
                                            Func<TA, TB> aToBConverter)
            {
                this.enumerator = enumerator;
                this.aToBConverter = aToBConverter;
            }


            public TB Current => aToBConverter(this.enumerator.Current);

            object IEnumerator.Current => this.enumerator.Current;

            public bool MoveNext()
                => this.enumerator.MoveNext();

            public void Reset()
                => this.enumerator.Reset();

            public void Dispose()
                => this.enumerator.Dispose();
        }



        private readonly Func<TA, TB> aToBConverter;
        private readonly Func<TB, TA> bToAConverter;
        private readonly IObservableCollection<TA> collection;
        private readonly IMainThreadDispatcher mainThreadDispatcher;
        private readonly ResetableCancellationTokenSource tokenSource;


        public ItemConvertingObservableCollection(IObservableCollection<TA> theCollection,
                                                  IMainThreadDispatcher mainThreadDispatcher,
                                                  Func<TA, TB> aToBConverter,
                                                  Func<TB, TA> bToAConverter)
        {
            this.collection = theCollection;
            this.aToBConverter = aToBConverter;
            this.bToAConverter = bToAConverter;
            this.mainThreadDispatcher = mainThreadDispatcher;

            this.tokenSource = new ResetableCancellationTokenSource();

            this.collection.CollectionChanged += Collection_CollectionChanged;
        }


        // ObservableCollectionBase
        public override TB GetItem(int index)
            => this.aToBConverter(this.collection.GetItem(index));

        public override int Count => this.collection.Count;


        public override int IndexOfItem(TB item)
            => this.collection.IndexOfItem(this.bToAConverter(item));

        public override bool ContainsItem(TB item)
            => this.collection.ContainsItem(this.bToAConverter(item));


        public override void Clear()
            => this.collection.Clear();


        public override IEnumerator<TB> GetEnumerator()
            => new ItemConvertingEnumerator(this.collection.GetEnumerator(), aToBConverter);



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
