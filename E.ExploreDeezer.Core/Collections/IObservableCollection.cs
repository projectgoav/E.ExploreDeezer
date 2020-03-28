using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel;
using System.Collections.Specialized;

namespace E.ExploreDeezer.Core.Collections
{
    public interface IObservableCollection<T> : IReadOnlyList<T>,
                                                INotifyPropertyChanged,
                                                INotifyCollectionChanged
    { }

}
