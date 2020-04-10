using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using E.Deezer;

using E.ExploreDeezer.Core.Mvvm;
using E.ExploreDeezer.Core.Services;
using E.ExploreDeezer.Core.Collections;

namespace E.ExploreDeezer.Core.ViewModels
{
    public interface IGenreListViewModel : IDisposable
    {
        EFetchState FetchState { get; }
        IObservableCollection<IGenreViewModel> GenreList { get; }

        GenreOverviewViewModelParams CreateGenreOverviewViewModelParams(IGenreViewModel genre);
    }

    internal class GenreListViewModel : ViewModelBase,
                                        IGenreListViewModel,
                                        IDisposable
    {
        private readonly IGenreListService service;
        private readonly MainThreadObservableCollectionAdapter<IGenreViewModel> genreList;

        private EFetchState fetchState;

        public GenreListViewModel(IPlatformServices platformServices)
            : base(platformServices)
        {
            this.service = ServiceRegistry.GetService<IGenreListService>();

            this.genreList = new MainThreadObservableCollectionAdapter<IGenreViewModel>(this.service.GenreList,
                                                                                        this.PlatformServices.MainThreadDispatcher);

            this.service.OnFetchStateChanged += OnFetchStateChanged;

            this.service.RefreshGenreListAsync();
        }

        // IGenreViewModel
        public EFetchState FetchState
        {
            get => this.fetchState;
            private set => SetProperty(ref this.fetchState, value);
        }

        public IObservableCollection<IGenreViewModel> GenreList => this.genreList;


        public GenreOverviewViewModelParams CreateGenreOverviewViewModelParams(IGenreViewModel genreViewModel)
        {
            if (genreViewModel == null || !genreViewModel.IsPresent)
                throw new ArgumentException();

            return new GenreOverviewViewModelParams(genreViewModel);
        }


        private void OnFetchStateChanged(object sender, FetchStateChangedEventArgs e)
            => this.FetchState = e.NewValue;



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.service.OnFetchStateChanged -= OnFetchStateChanged;

                this.genreList.Dispose();
            }

            base.Dispose(disposing);
        }

    }
}
