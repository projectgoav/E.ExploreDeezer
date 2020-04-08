using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using E.Deezer;

using E.ExploreDeezer.Core.Mvvm;
using E.ExploreDeezer.Core.Collections;

namespace E.ExploreDeezer.Core.ViewModels
{
    public interface IGenreListViewModel : IDisposable
    {
        EContentFetchStatus FetchStatus { get; }

        IObservableCollection<IGenreViewModel> GenreList { get; }

        GenreOverviewViewModelParams CreateGenreOverviewViewModelParams(IGenreViewModel genre);
    }

    internal class GenreListViewModel : ViewModelBase,
                                        IGenreListViewModel,
                                        IDisposable
    {
        private readonly MainThreadObservableCollectionAdapter<IGenreViewModel> genreList;

        private EContentFetchStatus fetchStatus;

        public GenreListViewModel(IPlatformServices platformServices)
            : base(platformServices)
        {
            var dataController = ServiceRegistry.GetService<GenreListDataController>();

            this.genreList = new MainThreadObservableCollectionAdapter<IGenreViewModel>(dataController.GenreList,
                                                                                        this.PlatformServices.MainThreadDispatcher);
        }


        // IGenreViewModel
        public EContentFetchStatus FetchStatus
        {
            get => this.fetchStatus;
            private set => SetProperty(ref this.fetchStatus, value);
        }

        public IObservableCollection<IGenreViewModel> GenreList => this.genreList;


        public GenreOverviewViewModelParams CreateGenreOverviewViewModelParams(IGenreViewModel genreViewModel)
        {
            if (genreViewModel == null || !genreViewModel.IsPresent)
                throw new ArgumentException();

            return new GenreOverviewViewModelParams(genreViewModel);
        }




        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.genreList.Dispose();
            }

            base.Dispose(disposing);
        }

    }
}
