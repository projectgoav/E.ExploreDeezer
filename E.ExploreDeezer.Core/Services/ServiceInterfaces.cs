using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using E.ExploreDeezer.Core.ViewModels;
using E.ExploreDeezer.Core.Collections;

namespace E.ExploreDeezer.Core.Services
{
    internal interface IGenreListService : IDataFetchingService
    {
        IObservableCollection<IGenreViewModel> GenreList { get; }

        Task RefreshGenreListAsync();
    }


    internal interface IGenreNewReleaseService : IDataFetchingService
    {

    }
}
