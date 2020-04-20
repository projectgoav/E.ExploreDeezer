using System;
using System.Collections.Generic;
using System.Text;

using E.ExploreDeezer.Core.Charts;
using E.ExploreDeezer.Core.Search;
using E.ExploreDeezer.Core.WhatsNew;

namespace E.ExploreDeezer.Core.Common
{
    public interface IViewModelFactory
    {
        IWhatsNewViewModel CreateWhatsNewViewModel();
        IChartsViewModel CreateChartsViewModel();
        ISearchViewModel CreateSearchViewModel();

        ITracklistViewModel CreateTracklistViewModel(TracklistViewModelParams p);
        IUserOverviewViewModel CreateUserOverviewViewModel(UserOverviewViewModelParams p);
        IArtistOverviewViewModel CreateArtistOverviewViewModel(ArtistOverviewViewModelParams p);
    }

    internal class ViewModelFactory : IViewModelFactory
    {
        public ISearchViewModel CreateSearchViewModel()
            => new SearchViewModel(ServiceRegistry.PlatformServices);

        public IWhatsNewViewModel CreateWhatsNewViewModel()
            => new WhatsNewViewModel(ServiceRegistry.PlatformServices);

        public IChartsViewModel CreateChartsViewModel()
            => new ChartsViewModel(ServiceRegistry.PlatformServices);



        public ITracklistViewModel CreateTracklistViewModel(TracklistViewModelParams p)
            => new TracklistViewModel(ServiceRegistry.PlatformServices,
                                      p);

        public IUserOverviewViewModel CreateUserOverviewViewModel(UserOverviewViewModelParams p)
            => new UserOverviewViewModel(ServiceRegistry.PlatformServices,
                                         p);

        public IArtistOverviewViewModel CreateArtistOverviewViewModel(ArtistOverviewViewModelParams p)
            => new ArtistOverviewViewModel(ServiceRegistry.PlatformServices,
                                           p);
    }
}
