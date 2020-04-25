using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

using E.ExploreDeezer.Core;
using E.ExploreDeezer.Core.Common;
using E.ExploreDeezer.Core.ViewModels;

using E.ExploreDeezer.UI.Views;


namespace E.ExploreDeezer.UI
{
    public enum EAnimateSearchRootClosure
    {
        Yes,
        No
    }

    internal interface INavigationService
    {
        void ShowAlbumTracklist(ulong albumId);
        void ShowAlbumTracklist(IAlbumViewModel album);

        void ShowPlaylistTracklist(ulong playlistId);
        void ShowPlaylistTracklist(IPlaylistViewModel playlist);

        void ShowArtistOverview(ulong artistId);
        void ShowArtistOverview(IArtistViewModel artist);

        void ShowUserOverview(ulong userId);
        void ShowUserOverview(IUserProfileViewModel userProfile);
    }


    internal static class Navigation
    {
        public const string NEW_MENU_TAG = "new";
        public const string CHART_MENU_TAG = "charts";

        //public static readonly Type WHATS_NEW_VIEWTYPE = typeof(WhatsNewView);
        //public static readonly Type CHARTS_VIEWTYPE = typeof(ChartsView);


        public static readonly HashSet<Type> TAB_ROOTS = new HashSet<Type>()
        {
            //WHATS_NEW_VIEWTYPE,
            //CHARTS_VIEWTYPE,
        };

        internal static Type GetViewTypeForMenu(string menuItemTag)
        {
            switch (menuItemTag)
            {
                //case NEW_MENU_TAG:
                //    return WHATS_NEW_VIEWTYPE;

                //case CHART_MENU_TAG:
                //    return CHARTS_VIEWTYPE;

                default:
                    throw new InvalidOperationException("Invalid menu tag specified.");
            }
        }

        internal static string GetMenuTagFromView(Type view)
        {
            //if (view == WHATS_NEW_VIEWTYPE)
            //    return NEW_MENU_TAG;

            //else if (view == CHARTS_VIEWTYPE)
            //    return CHART_MENU_TAG;

            //else
            return string.Empty;
        }



        internal static void ShowAlbumTracklist(IAlbumViewModel album)
        {
            var p = ViewModelParamFactory.CreateAlbumTracklistViewModelParams(album);
            ShowTracklist(p);
        }

        internal static void ShowAlbumTracklist(ulong albumId)
        {
            var p = ViewModelParamFactory.CreateAlbumTracklistViewModelParams(albumId);
            ShowTracklist(p);
        }


        internal static void ShowPlaylistTracklist(IPlaylistViewModel playlist)
        {
            var p = ViewModelParamFactory.CreatePlaylistTracklistViewModelParams(playlist);
            ShowTracklist(p);
        }

        internal static void ShowPlaylistTracklist(ulong playlistId)
        {
            var p = ViewModelParamFactory.CreatePlaylistTracklistViewModelParams(playlistId);
            ShowTracklist(p);
        }


        internal static void ShowTracklist(TracklistViewModelParams p)
        //    => ServiceRegistry.GetService<Frame>()
        //                      .ShowNewPage(typeof(TracklistView), p);
        { }


        internal static void ShowArtistOverview(IArtistViewModel artist)
        {
            var p = ViewModelParamFactory.CreateArtistOverviewViewModelParams(artist);
            DoShowArtistOverview(p);
        }

        internal static void ShowArtistOverview(ulong artistId)
        {
            var p = ViewModelParamFactory.CreateArtistOverviewViewModelParams(artistId);
            DoShowArtistOverview(p);
        }

        private static void DoShowArtistOverview(ArtistOverviewViewModelParams p)
        //    => ServiceRegistry.GetService<Frame>()
        //                      .ShowNewPage(typeof(ArtistOverviewView), p);
        { }


        internal static void ShowUserOverview(IUserProfileViewModel userProfile)
        {
            var p = ViewModelParamFactory.CreateUserOverviewViewModelParams(userProfile);
            DoShowUserOverview(p);
        }

        internal static void ShowUserOverview(ulong userId)
        {
            var p = ViewModelParamFactory.CreateUserOverviewViewModelParams(userId);
            DoShowUserOverview(p);
        }

        private static void DoShowUserOverview(UserOverviewViewModelParams p)
        //    => ServiceRegistry.GetService<Frame>()
        //                      .ShowNewPage(typeof(UserOverviewView), p);
        { }
    }

    internal static class FrameExtensions
    {
        private static readonly Type SEARCH_VIEW = typeof(SearchView);

        private static readonly DrillInNavigationTransitionInfo PAGE_TRANSITION_INFO = new DrillInNavigationTransitionInfo();
        private static readonly NavigationTransitionInfo NO_AMINATION_TRANSITION_INFO = new SuppressNavigationTransitionInfo();

        internal static bool IsSearchViewInBackstack(this Frame host)
        {
            if (host.BackStack.Count == 0)
                return false;

            return host.BackStack.Select(x => x.SourcePageType)
                                 .Contains(SEARCH_VIEW);
        }


        // Closes off all views so that SEARCH_VIEW is at the top of the backstack
        internal static void CloseSearchViewsIfPresent(this Frame host)
        {
            if (host.BackStack.Count == 0)
                return;

            bool keepPopping = true;
            bool hasBackstack = host.CanGoBack;

            while (hasBackstack && keepPopping)
            {
                var topMost = host.Content;
                bool isTopMostSearch = topMost is SearchView;

                if (isTopMostSearch)
                {
                    keepPopping = false;
                }
                else
                {
                    host.GoBack(NO_AMINATION_TRANSITION_INFO);
                }

                hasBackstack = host.CanGoBack;
            }
        }


        // Closes off SEARCH_VIEW if it's top of the stack
        internal static void CloseSearchRootViewIfPresent(this Frame host, EAnimateSearchRootClosure shouldAnimate)
        {
            if (host.Content is SearchView)
            {
                bool animate = shouldAnimate == EAnimateSearchRootClosure.Yes;

                if (animate)
                {
                    host.GoBack();
                }
                else
                {
                    host.GoBack(NO_AMINATION_TRANSITION_INFO);
                }
            }
        }



        /* Finds the view type that the root of search was presented over.
         * 
         * Stack:
         *  - E
         *  - D
         *  - SearchRoot
         *  - C
         *  - B
         *  - A
         *  - MainWindow
         *
         * This method would return typeof(C) */
        internal static Type TypeOfViewBefore(this Frame host, Type viewType)
        {
            Type viewBefore = null;

            foreach (var entry in host.BackStack)
            {
                if (entry.SourcePageType == viewType)
                {
                    break;
                }

                viewBefore = entry.SourcePageType;
            }

            return viewBefore;
        }


        internal static void ShowNewPage(this Frame host, Type pageType)
            => ShowNewPage(host, pageType, null);

        internal static void ShowNewPage(this Frame host, Type pageType, object parameter)
            => host.Navigate(pageType, parameter, PAGE_TRANSITION_INFO);

    }
}
