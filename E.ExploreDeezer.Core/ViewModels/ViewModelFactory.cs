﻿using System;
using System.Collections.Generic;
using System.Text;

using E.ExploreDeezer.Core.Mvvm;
using E.ExploreDeezer.Core.ViewModels;
using E.ExploreDeezer.Core.ViewModels.Home;

namespace E.ExploreDeezer.Core.ViewModels
{
    public interface IViewModelFactory
    {
        IWhatsNewViewModel CreateWhatsNewViewModel();
        IChartsViewModel CreateChartsViewModel();
        IGenreListViewModel CreateGenreListViewModel();


        ITracklistViewModel CreateTracklistViewModel(ITracklistViewModelParams p);
    }

    internal class ViewModelFactory : IViewModelFactory
    {
        public IWhatsNewViewModel CreateWhatsNewViewModel()
            => new WhatsNewViewModel(ServiceRegistry.DeezerSession,
                                     ServiceRegistry.PlatformServices);

        public IChartsViewModel CreateChartsViewModel()
            => new ChartsViewModel(ServiceRegistry.DeezerSession,
                                   ServiceRegistry.PlatformServices);

        public IGenreListViewModel CreateGenreListViewModel()
            => new GenreListViewModel(ServiceRegistry.DeezerSession,
                                      ServiceRegistry.PlatformServices);



        public ITracklistViewModel CreateTracklistViewModel(ITracklistViewModelParams p)
            => new TracklistViewModel(ServiceRegistry.DeezerSession,
                                      ServiceRegistry.PlatformServices,
                                      p);

    }
}
