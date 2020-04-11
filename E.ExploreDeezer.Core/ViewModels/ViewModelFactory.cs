using System;
using System.Collections.Generic;
using System.Text;

using E.ExploreDeezer.Core.Charts;
using E.ExploreDeezer.Core.WhatsNew;
using E.ExploreDeezer.Core.ViewModels;

namespace E.ExploreDeezer.Core.ViewModels
{
    public interface IViewModelFactory
    {
        IWhatsNewViewModel CreateWhatsNewViewModel();
        IChartsViewModel CreateChartsViewModel();
        IGenreListViewModel CreateGenreListViewModel();
        ISearchViewModel CreateSearchViewModel();

        ITracklistViewModel CreateTracklistViewModel(TracklistViewModelParams p);
        IArtistOverviewViewModel CreateArtistOverviewViewModel(ArtistOverviewViewModelParams p);
        IGenreOverviewViewModel CreateGenreOverviewViewModel(GenreOverviewViewModelParams p);
    }

    internal class ViewModelFactory : IViewModelFactory
    {
        public ISearchViewModel CreateSearchViewModel()
            => new SearchViewModel(ServiceRegistry.DeezerSession,
                                   ServiceRegistry.PlatformServices);

        public IWhatsNewViewModel CreateWhatsNewViewModel()
            => new WhatsNewViewModel(ServiceRegistry.PlatformServices);

        public IChartsViewModel CreateChartsViewModel()
            => new ChartsViewModel(ServiceRegistry.PlatformServices);

        public IGenreListViewModel CreateGenreListViewModel()
            => new GenreListViewModel(ServiceRegistry.PlatformServices);



        public ITracklistViewModel CreateTracklistViewModel(TracklistViewModelParams p)
            => new TracklistViewModel(ServiceRegistry.DeezerSession,
                                      ServiceRegistry.PlatformServices,
                                      p);

        public IArtistOverviewViewModel CreateArtistOverviewViewModel(ArtistOverviewViewModelParams p)
            => new ArtistOverviewViewModel(ServiceRegistry.DeezerSession,
                                           ServiceRegistry.PlatformServices,
                                           p);

        public IGenreOverviewViewModel CreateGenreOverviewViewModel(GenreOverviewViewModelParams p)
            => new GenreOverviewViewModel(ServiceRegistry.DeezerSession,
                                          ServiceRegistry.PlatformServices,
                                          p);

    }
}
