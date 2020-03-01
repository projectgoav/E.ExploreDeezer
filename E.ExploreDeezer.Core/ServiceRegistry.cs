using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;

using E.Deezer;

using E.ExploreDeezer.Core.Mvvm;

namespace E.ExploreDeezer.Core
{
    //TODO: FIX ME
    internal class Frame
    { }


    internal static class ServiceRegistry
    {
        public static void Initialise(IPlatformServices platformServices,
                                      Frame applicationFrame)
        {
            PlatformServices = platformServices;
            ApplicationFrame = applicationFrame;

            DeezerSession = new DeezerSession(new HttpClientHandler());

        }


        public static IPlatformServices PlatformServices { get; private set; }

        public static IDeezerSession DeezerSession { get; private set; }

        public static Frame ApplicationFrame { get; private set; }

    }
}
