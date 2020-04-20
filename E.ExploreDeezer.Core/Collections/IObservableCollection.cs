using System;
using System.Collections.Generic;
using System.Text;

using System.Collections;
using System.ComponentModel;
using System.Collections.Specialized;

namespace E.ExploreDeezer.Core.Collections
{
    public interface IObservableCollection<T> : IList,
                                                IEnumerable,
                                                IEnumerable<T>,
                                                INotifyPropertyChanged,
                                                INotifyCollectionChanged
    {
        // Custom IReadonlyList<T> helpers since we can't implement both IList & IList<T>,
        // but collections only bind to IList instances
        T GetItem(int index);
        bool ContainsItem(T item);
        int IndexOfItem(T item);
    }


    public abstract class ObservableCollectionBase<T> : IObservableCollection<T>,
                                                        IDisposable
    {
        private const string READONLY_MESSAGE = "Collection is read-only.";

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        // INotifyCollectionChanged
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        
        protected void NotifyChanged(NotifyCollectionChangedEventArgs args)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(Constants.COUNT_PROPERTY_NAME));
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(Constants.INDEXER_PROPERTY_NAME));
            }

            this.CollectionChanged?.Invoke(this, args);
        }



        // IObservableCollection
        public abstract T GetItem(int index);
        public abstract int IndexOfItem(T item);
        public abstract bool ContainsItem(T item);

        public abstract int Count { get; }

        public abstract void Clear();


        public object this[int index]
        { 
            get => GetItem(index);
            set => throw new NotSupportedException(READONLY_MESSAGE);
        }

        public bool IsReadOnly => true;
        public bool IsFixedSize => false;

        public bool IsSynchronized => false;
        public object SyncRoot => throw new NotSupportedException("Collection not synchronized");


        public bool Contains(object value)
            => (value is T castValue) ? ContainsItem(castValue)
                                      : false;

        public int IndexOf(object value)
            => (value is T castValue) ? IndexOfItem(castValue)
                                      : Constants.NOT_FOUND_INDEX;


        public int Add(object value)
            => throw new NotSupportedException(READONLY_MESSAGE);

        public void Insert(int index, object value)
            => throw new NotSupportedException(READONLY_MESSAGE);

        public void Remove(object value)
            => throw new NotSupportedException(READONLY_MESSAGE);

        public void RemoveAt(int index)
            => throw new NotSupportedException(READONLY_MESSAGE);


        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }


        // IEnumerator (IEnumerator<T>)
        public abstract IEnumerator<T> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();


        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) { }
    }
}
