using System;
using System.Collections.Generic;
using System.Text;

namespace E.ExploreDeezer.Core
{
    internal interface IDataController
    {
        string Id { get; }
        EFetchState CurrentFetchState { get; }

        event FetchStateChangedEventHandler OnFetchStateChanged;
    }
}
