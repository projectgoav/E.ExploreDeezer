using System;
using System.Collections.Generic;
using System.Text;

using System.Collections;
using System.ComponentModel;
using System.Collections.Specialized;

namespace E.ExploreDeezer.Core.Collections
{
    /* Custom Collection: IObservableCollection<T>
     * 
     * Implements both INotifyPropertyChanged and INotifyCollectionChanged
     * as certain controls (not necessarily UWP) require both interfaces to 
     * operate as efficently as possible.
     * 
     * By default, most controls use the IList and IEnumerable interface when
     * requesting items. This seems wrong as IReadOnlyList or IList<T> would
     * be preferable but there's nothing we can do about it. 
     * 
     * This interface basically mimics IReadOnlyList<T> by adding typed helper
     * methods to make searching for and retrieving items easier and without
     * having to cast */
    public interface IObservableCollection<T> : IList,
                                                IEnumerable,
                                                IEnumerable<T>,
                                                INotifyPropertyChanged,
                                                INotifyCollectionChanged
    {
        T GetItem(int index);
        bool ContainsItem(T item);
        int IndexOfItem(T item);
    }


    /* CustomCollection: ObservableCollectionBase
     * 
     * Re: the comments above, this class stubs out all required IList
     * methods that don't make much sense in this application. 
     * 
     * NOTE: This makes an assumption that all ObservableCollections
     * are READ-ONLY and modifications will be provided to the collection
     * at a later stage, instead of modifying the collection directly.
     * 
     * NOTE: Any events fired by this collection are done so on the calling
     * thread. For UI updates you must marshal these updates onto the UI
     * thread manually or use the adapter class from this namespace
     * 
     * Stub implmentations are also given to the get and search methods
     * of the IList interface which forward onto our typed versions
     * meaning that any class inheriting from this needs to provide
     * minimal implementation */
    internal abstract class ObservableCollectionBase<T> : IObservableCollection<T>,
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
