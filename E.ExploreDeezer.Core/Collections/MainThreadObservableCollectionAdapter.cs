using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Threading;

using E.ExploreDeezer.Core.Mvvm;

namespace E.ExploreDeezer.Core.Collections
{
    internal class MainThreadObservableCollectionAdapter<T> : IObservableCollection<T>,
                                                              IList<T>,
                                                              IList,
                                                              IDisposable
    {
        private static readonly CancellationToken CANCELLED_TOKEN = new CancellationToken(canceled: true);


        private readonly IObservableCollection<T> collection;
        private readonly IMainThreadDispatcher mainThreadDispatcher;
        private readonly CancellationTokenSource cancellationTokenSource;


        public MainThreadObservableCollectionAdapter(IObservableCollection<T> theCollection,
                                                     IMainThreadDispatcher mainThreadDispatcher)
        {
            this.collection = theCollection;
            this.mainThreadDispatcher = mainThreadDispatcher;

            this.collection.PropertyChanged += Collection_PropertyChanged;
            this.collection.CollectionChanged += Collection_CollectionChanged;

            this.cancellationTokenSource = new CancellationTokenSource();
        }


        private CancellationToken CancellationToken => this.cancellationTokenSource.IsCancellationRequested ? CANCELLED_TOKEN
                                                                                                            : this.cancellationTokenSource.Token;


        private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.mainThreadDispatcher.ExecuteOnMainThreadAsync(() =>
            {
                if (!this.CancellationToken.IsCancellationRequested)
                {
                    this.CollectionChanged?.Invoke(sender, e);
                }
            });
        }

        private void Collection_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.mainThreadDispatcher.ExecuteOnMainThreadAsync(() =>
            {
                if (!this.CancellationToken.IsCancellationRequested)
                {
                    this.PropertyChanged?.Invoke(sender, e);
                }
            });
        }


        // IObservableCollection
        public T this[int index] => this.collection[index];

        T IList<T>.this[int index] 
        {
            get => this.collection[index];
            set => throw new NotImplementedException();
        }

        object IList.this[int index] 
        {
            get => this.collection[index];
            set => throw new NotImplementedException();
        }


        public int Count => this.collection.Count;

        public bool IsReadOnly => true;

        public bool IsFixedSize => true;

        public bool IsSynchronized => false;

        public object SyncRoot => throw new NotImplementedException();


        public event PropertyChangedEventHandler PropertyChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }



        public void Dispose()
        {
            this.cancellationTokenSource.Cancel();
            this.cancellationTokenSource.Dispose();

            this.collection.PropertyChanged -= Collection_PropertyChanged;
            this.collection.CollectionChanged -= Collection_CollectionChanged;
        }
    }
}
