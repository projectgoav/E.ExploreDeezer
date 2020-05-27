using System;
using System.Collections.Generic;
using System.Text;

using E.ExploreDeezer.Core.Charts;
using E.ExploreDeezer.Core.Search;
using E.ExploreDeezer.Core.MyDeezer;
using E.ExploreDeezer.Core.WhatsNew;
using E.ExploreDeezer.Core.OAuth;

namespace E.ExploreDeezer.Core.Common
{
    public interface IViewModelFactory
    {
        IWhatsNewViewModel CreateWhatsNewViewModel();
        IChartsViewModel CreateChartsViewModel();
        ISearchViewModel CreateSearchViewModel();

        ILoginViewModel CreateLoginViewModel();
        IMyDeezerViewModel CreateMyDeezerViewModel();

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



        public ILoginViewModel CreateLoginViewModel()
            => new LoginViewModel(ServiceRegistry.GetService<IOAuthClient>(),
                                  ServiceRegistry.AuthenticationService,
                                  ServiceRegistry.PlatformServices);

        public IMyDeezerViewModel CreateMyDeezerViewModel()
            => new MyDeezerViewModel(ServiceRegistry.AuthenticationService,
                                     ServiceRegistry.PlatformServices);



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
