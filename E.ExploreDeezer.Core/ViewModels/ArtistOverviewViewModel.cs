using System;
using System.Collections.Generic;
using System.Text;

using E.Deezer;

using E.ExploreDeezer.Core.Mvvm;
using E.ExploreDeezer.Core.ViewModels;

namespace E.ExploreDeezer.Core.ViewModels
{
    public interface IArtistOverviewViewModel
    {
        string PageTitle { get; }
       
        IArtistViewModel Artist { get; }
    }

    public interface IArtistOverviewViewModelParams
    {
        IArtistViewModel Artist { get; }
    }


    internal class ArtistOverviewViewModelParams : IArtistOverviewViewModelParams
    {
        public ArtistOverviewViewModelParams(IArtistViewModel artistViewModel)
        {
            this.Artist = artistViewModel;
        }

        public IArtistViewModel Artist { get; }
    }


    internal class ArtistOverviewViewModel : ViewModelBase,
                                             IArtistOverviewViewModel
                        
    {
        private readonly IDeezerSession session;


        public ArtistOverviewViewModel(IDeezerSession session,
                                       IPlatformServices platformServices,
                                       IArtistOverviewViewModelParams p)
            : base(platformServices)
        {
            this.Artist = p.Artist;
        }



        public string PageTitle => this.Artist.Name;
        
        public IArtistViewModel Artist { get; }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

            }

            base.Dispose(disposing);
        }
    }
}
