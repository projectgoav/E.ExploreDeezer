using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;

using E.Deezer;

using E.ExploreDeezer.Mvvm;

namespace E.ExploreDeezer.UWP
{
    internal static class ServiceRegistry
    {
        public static void Initialise(IPlatformServices platformServices)
        {
            PlatformServices = platformServices;

            DeezerSession = new DeezerSession(new HttpClientHandler());
        }


        public static IPlatformServices PlatformServices { get; private set; }

        public static IDeezerSession DeezerSession { get; private set; }

    }
}
