using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;

using E.Deezer;

using E.ExploreDeezer.Core.Mvvm;
using E.ExploreDeezer.Core.ViewModels;

namespace E.ExploreDeezer.Core
{
    public static class ServiceRegistry
    {
        private static readonly Dictionary<Type, object> additionalServices = new Dictionary<Type, object>();

        public static void Initialise(IPlatformServices platformServices)
        {
            PlatformServices = platformServices;

            ViewModelFactory = new ViewModelFactory();
            DeezerSession = new DeezerSession(new HttpClientHandler());
        }


        public static IDeezerSession DeezerSession { get; private set; }
        public static IPlatformServices PlatformServices { get; private set; }
        public static IViewModelFactory ViewModelFactory { get; private set; }



        /* TODO: This *could* disappear and rely on the ViewModel layer
         * to plub the showing / closing of ViewModels? 
         * 
         * Instead, we can just rely on the View layer implement all it requires. */
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
