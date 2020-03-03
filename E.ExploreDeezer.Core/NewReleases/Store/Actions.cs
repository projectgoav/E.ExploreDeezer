using System;
using System.Collections.Generic;
using System.Text;

using E.Deezer.Api;

namespace E.ExploreDeezer.Core.NewReleases.Store
{

    internal struct FetchNewReleases : IAction
    { }

    internal struct SetNewReleaseFetchSuccess : IAction
    {
        public SetNewReleaseFetchSuccess(IEnumerable<IAlbum> releases)
        {
            this.Releases = releases;
        }

        public IEnumerable<IAlbum> Releases { get; }
    }

    internal struct SetNewReleaseFetchFailure : IAction
    {
        public SetNewReleaseFetchFailure(string message)
        {
            this.Message = message;
        }

        public string Message { get; }
    }


    internal struct FetchDeezerPicks : IAction
    { }

    internal struct SetDeezerPicksFetchSuccess : IAction
    {
        public SetDeezerPicksFetchSuccess(IEnumerable<IAlbum> deezerPicks)
        {
            this.DeezerPicks = deezerPicks;
        }


        public IEnumerable<IAlbum> DeezerPicks { get; }
    }

    internal struct SetDeezerPicksFetchFailure : IAction
    {
        public SetDeezerPicksFetchFailure(string message)
        {
            this.Message = message;
        }

        public string Message { get; }
    }
}
