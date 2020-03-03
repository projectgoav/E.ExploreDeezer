using System;
using System.Collections.Generic;
using System.Text;

using E.ExploreDeezer.Core.NewReleases.Store;

namespace E.ExploreDeezer.Core
{
    public class AppState
    {
        public AppState()
            : this(new NewReleaseState())
        { }

        public AppState(NewReleaseState newReleaseState)
        {
            this.NewReleases = newReleaseState;
        }

        public NewReleaseState NewReleases { get; }



        public static AppState Reduce(AppState state, IAction action)
        {
            return new AppState(NewReleasesReducer.Reduce(state.NewReleases, action));
        }
    }



}
