using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using E.ExploreDeezer.Core;
using E.ExploreDeezer.UWP.Views;
using E.ExploreDeezer.Core.ViewModels;
using Windows.UI.Xaml.Media.Animation;

namespace E.ExploreDeezer.UWP
{
    public sealed partial class MainPage : Page
    {
        private readonly ISearchViewModel searchViewModel;


        public MainPage()
        {
            this.InitializeComponent();

            ServiceRegistry.Initialise(new UWPPlatformServices(this.Dispatcher));
            ServiceRegistry.Register<Frame>(this.ContentView);

            this.searchViewModel = ServiceRegistry.ViewModelFactory.CreateSearchViewModel();

            this.ContentView.Navigated += OnNavigationOccurs;
            this.MainNav.BackRequested += OnBackRequested;

            this.SearchBox.TextChanged += OnSearchTextChanged;
            this.SearchBox.QuerySubmitted += OnSearchQuerySubmitted;

            this.MainNav.SelectedItem = this.MainNav.MenuItems[0];
        }


        private void OnSearchTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                this.ContentView.CloseSearchViewsIfPresent();
                ShowSearchViewIfRequired();
            }

            this.searchViewModel.SetQuery(this.SearchBox.Text);
        }

        private void OnSearchQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            this.ContentView.CloseSearchViewsIfPresent();
            ShowSearchViewIfRequired();

            this.searchViewModel.SetQuery(args.QueryText);
        }

        private void MainNavSelectionChanged(object sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer != null)
            {
                string tagValue = args.SelectedItemContainer.Tag
                                                            .ToString();

                var newPageType = Navigation.GetViewTypeForMenu(tagValue);

                this.ContentView.CloseSearchViewsIfPresent();

                Type viewBeforeSearch = this.ContentView.TypeOfViewBefore(typeof(SearchView));

                bool shouldAnimate = viewBeforeSearch != null && viewBeforeSearch == newPageType;

                this.ContentView.CloseSearchRootViewIfPresent(shouldAnimate ? EAnimateSearchRootClosure.Yes
                                                                            : EAnimateSearchRootClosure.No);

                bool hasNewPage = newPageType != null;
                bool hasViewBeforeSearch = viewBeforeSearch != null;
                bool isViewBeforeSearch = hasViewBeforeSearch && viewBeforeSearch == newPageType;

                bool shouldShowNewPage = hasNewPage || (hasViewBeforeSearch || isViewBeforeSearch);

                if (shouldShowNewPage)
                {
                    this.ContentView.BackStack.Clear();

                    this.ContentView.Navigate(newPageType);
                }
            }
        }


        private void OnNavigationOccurs(object sender, NavigationEventArgs e)
        {
            this.MainNav.IsBackEnabled = this.ContentView.CanGoBack;

            UpdateSelectedTab();
        }


        private void ShowSearchViewIfRequired()
        {
            if (!(this.ContentView.Content is SearchView))
            {
                this.ContentView.Navigate(typeof(SearchView), this.searchViewModel);
                this.MainNav.SelectedItem = null;
            }
        }


        private void OnBackRequested(object sender, NavigationViewBackRequestedEventArgs e)
        {
            if (this.ContentView.CanGoBack)
            {
                this.ContentView.GoBack();
            }
        }


        private void UpdateSelectedTab()
        {
            var currentViewType = this.ContentView.Content.GetType();
            if (Navigation.TAB_ROOTS.Contains(currentViewType))
            {
                var menuItemTag = Navigation.GetMenuTagFromView(currentViewType);

                switch (menuItemTag)
                {
                    case Navigation.NEW_MENU_TAG:
                        this.MainNav.SelectedItem = this.MainNav.MenuItems[0];
                        break;

                    case Navigation.CHART_MENU_TAG:
                        this.MainNav.SelectedItem = this.MainNav.MenuItems[1];
                        break;

                    case Navigation.GENRE_MENU_TAG:
                        this.MainNav.SelectedItem = this.MainNav.MenuItems[2];
                        break;
                }
            }
        }
    }
}
