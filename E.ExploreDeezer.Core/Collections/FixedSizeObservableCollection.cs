using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;


namespace E.ExploreDeezer.Core.Collections
{
    /* Custom Collections: FixedSizeObservableCollection
     * 
     * This observable collection requires all it's contents to
     * be set or cleared in one go and therefore will only emit
     * NotifyCollectionChanged->Reset actions. 
     * 
     * This is ideal for a small number of items. Larger collections
     * should use the 'Paged' varient which supports incremental
     * loading. */
    internal class FixedSizeObservableCollection<T> : ObservableCollectionBase<T>
    {
        private static readonly List<T> EMPTY = new List<T>(0);

        private List<T> contents;
        
        public FixedSizeObservableCollection()
        {
            this.contents = EMPTY;
        }


        // ObservableCollectionBase
        public override T GetItem(int index)
            => this.contents[index];

        public override int Count => this.contents.Count;


        public override int IndexOfItem(T item)
            => this.contents.IndexOf(item);

        public override bool ContainsItem(T item)
            => this.contents.Contains(item);

        public override void Clear()
            => this.contents.Clear();


        public override IEnumerator<T> GetEnumerator()
            => this.contents.GetEnumerator();



        public void ClearContents()
        {
            this.contents = EMPTY;
            NotifyChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void SetContents(IEnumerable<T> incomingContents)
        {
            this.contents = new List<T>(incomingContents);
            NotifyChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
