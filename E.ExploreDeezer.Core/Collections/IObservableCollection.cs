using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections;

namespace E.ExploreDeezer.Core.Collections
{
    public interface IObservableCollection<T> : //IList<T>,
                                                IList,
                                                IEnumerable,
                                                IEnumerable<T>,
                                                INotifyPropertyChanged,
                                                INotifyCollectionChanged
    { }

}
