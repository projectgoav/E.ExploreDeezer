using System;
using System.Collections.Generic;
using System.Text;

using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections;

namespace E.ExploreDeezer.Core.Collections
{
    internal class FixedSizeObservableCollection<T> : IObservableCollection<T>
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



        public void SetContents(IEnumerable<T> incomingContents)
        {
            this.contents = new List<T>(incomingContents);

            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(Constants.COUNT_PROPERTY_NAME));
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(Constants.INDEXER_PROPERTY_NAME));
            }

            this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, this.contents, 0));
        }


        private T GetItem(int index)
        {
            Assert.That(index > 0, "Index out of range");
            Assert.That(index <= this.Count, "Index out of range");

            return this.contents[index];
        }
    }
}
