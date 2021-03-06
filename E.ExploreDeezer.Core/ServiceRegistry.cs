﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

using E.Deezer;

using E.ExploreDeezer.Core.Mvvm;
using E.ExploreDeezer.Core.OAuth;
using E.ExploreDeezer.Core.Charts;
using E.ExploreDeezer.Core.Search;
using E.ExploreDeezer.Core.Common;
using E.ExploreDeezer.Core.WhatsNew;
using E.ExploreDeezer.Core.MyDeezer;

namespace E.ExploreDeezer.Core
{
    public interface ISecrets
    {
        string AppId { get; }
        string AppSecret { get; }
        string OAuthRedirectUri { get; }
        DeezerPermissions DesiredPermissions { get; }
    }

    public static class ServiceRegistry
    {
        private static readonly Dictionary<Type, object> additionalServices = new Dictionary<Type, object>();

        public static void Initialise(IPlatformServices platformServices,
                                      ISecrets secrets)
        {
            PlatformServices = platformServices;

            ViewModelFactory = new ViewModelFactory();
            DeezerSession = new DeezerSession(new HttpClientHandler());

            AuthenticationService = new AuthenticationService(DeezerSession,
                                                              platformServices.SecureStorage);

            FavouritesService = new FavouritesService(DeezerSession, AuthenticationService);

            // TODO: Need to work out how to dispose of services when they are dying off
            Register<IGenreListDataController>(new GenreListDataController(DeezerSession));
            Register<IWhatsNewDataController>(new WhatsNewDataController(DeezerSession));
            Register<IChartsDataController>(new ChartsDataController(DeezerSession));
            Register<ISearchDataController>(new SearchDataController(DeezerSession));
            Register<ITracklistDataController>(new TracklistDataController(DeezerSession));
            Register<IArtistOverviewDataController>(new ArtistOverviewDataController(DeezerSession));
            Register<IUserOverviewDataController>(new UserOverviewDataController(DeezerSession));
            Register<IMyDeezerDataController>(new MyDeezerDataController(DeezerSession, AuthenticationService, FavouritesService));



            Register<IOAuthClient>(new OAuthClient(secrets.AppId,
                                                   secrets.AppSecret,
                                                   secrets.OAuthRedirectUri,
                                                   secrets.DesiredPermissions));
        }


        public static IDeezerSession DeezerSession { get; private set; }
        public static IPlatformServices PlatformServices { get; private set; }
        public static IViewModelFactory ViewModelFactory { get; private set; }
        public static IFavouritesService FavouritesService { get; private set; }
        public static IAuthenticationService AuthenticationService { get; private set; }


        public static void Register<TService>(TService service)
            => additionalServices.Add(typeof(TService), service);

        public static TService GetService<TService>()
        {
            if (!additionalServices.TryGetValue(typeof(TService), out object svcImpl))
                throw new Exception("Service not registered.");

            return (TService)svcImpl;
        }
    }
}
