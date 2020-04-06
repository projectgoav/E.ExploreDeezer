using System;
using System.Collections.Generic;
using System.Text;

using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections;

namespace E.ExploreDeezer.Core.Collections
{
    internal class FixedSizeObservableCollection<T> : IObservableCollection<T>,
                                                      IList,
                                                      IList<T>
    {
        private static readonly List<T> EMPTY = new List<T>(0);


        private List<T> contents;
        
        public FixedSizeObservableCollection()
        {
            this.contents = EMPTY;
        }



        // IObservableCollection<T>
        public T this[int index] => GetItem(index);

        public int Count => this.contents.Count;

      
        public IEnumerator<T> GetEnumerator()
            => this.contents.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => this.contents.GetEnumerator();


        public event PropertyChangedEventHandler PropertyChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged;



        public void ClearContents()
        {
            this.contents = EMPTY;

            RaisePropertiesChanged();
        }

        public void SetContents(IEnumerable<T> incomingContents)
        {
            this.contents = new List<T>(incomingContents);

            RaisePropertiesChanged();
        }


        private T GetItem(int index)
        {
            Assert.That(index >= 0, "Index out of range");
            Assert.That(index <= this.Count, "Index out of range");

            return this.contents[index];
        }


        private void RaisePropertiesChanged()
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(Constants.COUNT_PROPERTY_NAME));
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(Constants.INDEXER_PROPERTY_NAME));
            }

            this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }


        public bool IsFixedSize => true;

        public bool IsReadOnly => true;

        public bool IsSynchronized => false;

        public object SyncRoot => throw new NotImplementedException();

        T IList<T>.this[int index]
        {
            get => GetItem(index);
            set => throw new NotImplementedException();
        }

        object IList.this[int index]
        {
            get => GetItem(index);
            set => throw new NotImplementedException();
        }


        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();  
        }

        public int IndexOf(object value)
            => this.contents.IndexOf((T)value);


        public void Insert(int index, object value)
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

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(T item)
            => this.contents.IndexOf(item);

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }
    }
}
