using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using E.ExploreDeezer.Core.ViewModels;

namespace E.ExploreDeezer.Core.NewReleases.Store
{
    internal static class NewReleasesReducer
    {
        public static NewReleaseState Reduce(NewReleaseState state, IAction action)
        {
            switch(action)
            {
                case FetchNewReleases a:
                    return OnBeginFetchNewReleases(state, a);

                case SetNewReleaseFetchSuccess a:
                    return OnNewReleaseFetchSuccess(state, a);

                case SetNewReleaseFetchFailure a:
                    return OnNewReleaseFetchFailure(state, a);


                case FetchDeezerPicks a:
                    return OnBeginFetchDeezerPicks(state, a);

                case SetDeezerPicksFetchSuccess a:
                    return OnDeezerPicksFetchSuccess(state, a);

                case SetDeezerPicksFetchFailure a:
                    return OnDeezerPicksFetchFailure(state, a);

                default:
                    return state;
            }
        }



        private static NewReleaseState OnBeginFetchNewReleases(NewReleaseState state, FetchNewReleases action)
        {
            return new NewReleaseState(EContentFetchStatus.Loading,
                                       Array.Empty<IAlbumViewModel>(),
                                       state.DeezerPicksFetchStatus,
                                       state.DeezerPicks);
        }

        private static NewReleaseState OnNewReleaseFetchSuccess(NewReleaseState state, SetNewReleaseFetchSuccess action)
        {
            var viewModels = action.Releases.Select(x => new AlbumViewModel(x))
                                            .ToList();

            return new NewReleaseState(viewModels.Count == 0 ? EContentFetchStatus.Empty
                                                             : EContentFetchStatus.Available,
                                       viewModels,
                                       state.DeezerPicksFetchStatus,
                                       state.DeezerPicks);
        }

        private static NewReleaseState OnNewReleaseFetchFailure(NewReleaseState state, SetNewReleaseFetchFailure action)
        {
            //TODO: Store the error message...
            return new NewReleaseState(EContentFetchStatus.Error,
                                       Array.Empty<IAlbumViewModel>(),
                                       state.DeezerPicksFetchStatus,
                                       state.DeezerPicks);
        }



        private static NewReleaseState OnBeginFetchDeezerPicks(NewReleaseState state, FetchDeezerPicks action)
        {
            return new NewReleaseState(state.NewReleaseFetchStatus,
                                       state.NewReleases,
                                       EContentFetchStatus.Loading,
                                       Array.Empty<IAlbumViewModel>());
        }

        private static NewReleaseState OnDeezerPicksFetchSuccess(NewReleaseState state, SetDeezerPicksFetchSuccess action)
        {
            var viewModels = action.DeezerPicks.Select(x => new AlbumViewModel(x))
                                               .ToList();

            return new NewReleaseState(state.NewReleaseFetchStatus,
                                       state.NewReleases,
                                       viewModels.Count == 0 ? EContentFetchStatus.Empty
                                                                 : EContentFetchStatus.Available,
                                       viewModels);
        }

        private static NewReleaseState OnDeezerPicksFetchFailure(NewReleaseState state, SetDeezerPicksFetchFailure action)
        {
            //TODO: Store the error message...
            return new NewReleaseState(state.NewReleaseFetchStatus,
                                       state.NewReleases,
                                       EContentFetchStatus.Error,
                                       Array.Empty<IAlbumViewModel>());
        }
    }
}
