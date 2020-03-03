using System;
using System.Collections.Generic;
using System.Text;

using E.ExploreDeezer.Core.ViewModels;

namespace E.ExploreDeezer.Core.NewReleases.Store
{
    public class NewReleaseState
    {
        public NewReleaseState()
            : this(EContentFetchStatus.Empty,
                   Array.Empty<IAlbumViewModel>(),
                   EContentFetchStatus.Empty,
                   Array.Empty<IAlbumViewModel>())
        { }


        public NewReleaseState(EContentFetchStatus newReleaseFetchStatus,
                               IEnumerable<IAlbumViewModel> newReleases,
                               EContentFetchStatus deezerPicksFetchStatus,
                               IEnumerable<IAlbumViewModel> deezerPicks)
        {
            this.NewReleaseFetchStatus = newReleaseFetchStatus;
            this.NewReleases = newReleases;

            this.DeezerPicksFetchStatus = deezerPicksFetchStatus;
            this.DeezerPicks = deezerPicks;
        }


        public IEnumerable<IAlbumViewModel> NewReleases { get; }
        public EContentFetchStatus NewReleaseFetchStatus { get; }

        public IEnumerable<IAlbumViewModel> DeezerPicks { get; }
        public EContentFetchStatus DeezerPicksFetchStatus { get; }
    }
}
