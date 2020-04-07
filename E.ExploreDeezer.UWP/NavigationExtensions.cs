using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

using E.ExploreDeezer.Core;
using E.ExploreDeezer.UWP.Views;

namespace E.ExploreDeezer.UWP
{
    public enum EAnimateSearchRootClosure
    {
        Yes,
        No
    }


    internal static class Navigation
    {
        public const string NEW_MENU_TAG = "new";
        public const string CHART_MENU_TAG = "charts";
        public const string GENRE_MENU_TAG = "genre";

        public static readonly Type WHATS_NEW_VIEWTYPE = typeof(WhatsNewView);
        public static readonly Type CHARTS_VIEWTYPE = typeof(ChartsView);
        public static readonly Type GENRE_VIEWTYPE = typeof(GenreView);


        public static readonly HashSet<Type> TAB_ROOTS = new HashSet<Type>()
        {
            WHATS_NEW_VIEWTYPE,
            CHARTS_VIEWTYPE,
            GENRE_VIEWTYPE
        };

        internal static Type GetViewTypeForMenu(string menuItemTag)
        {
            switch(menuItemTag)
            {
                case NEW_MENU_TAG:
                    return WHATS_NEW_VIEWTYPE;

                case CHART_MENU_TAG:
                    return CHARTS_VIEWTYPE;

                case GENRE_MENU_TAG:
                    return GENRE_VIEWTYPE;

                default:
                    throw new InvalidOperationException("Invalid menu tag specified.");
            }
        }

        internal static string GetMenuTagFromView(Type view)
        {
            if (view == WHATS_NEW_VIEWTYPE)
                return NEW_MENU_TAG;

            else if (view == CHARTS_VIEWTYPE)
                return CHART_MENU_TAG;

            else if (view == GENRE_VIEWTYPE)
                return GENRE_MENU_TAG;

            else
                return string.Empty;
        }
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

            while(hasBackstack && keepPopping)
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
