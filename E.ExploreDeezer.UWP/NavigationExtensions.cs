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

    internal static class FrameExtensions
    {
        private static readonly Type SEARCH_VIEW = typeof(SearchView);
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


    }


}
