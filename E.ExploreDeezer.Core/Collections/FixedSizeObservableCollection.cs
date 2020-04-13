using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;


namespace E.ExploreDeezer.Core.Collections
{
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
