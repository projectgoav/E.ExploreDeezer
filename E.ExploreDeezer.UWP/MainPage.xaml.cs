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

namespace E.ExploreDeezer.UWP
{
    public sealed partial class MainPage : Page
    {
        private const string NEW_MENU_TAG = "new";
        private const string CHART_MENU_TAG = "charts";
        private const string GENRE_MENU_TAG = "genre";

        private static readonly Dictionary<string, Type> navLookup = new Dictionary<string, Type>()
        {
            { CHART_MENU_TAG, typeof(Views.ChartsView) },
            { GENRE_MENU_TAG, typeof(Views.GenreView) },
        };


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


        //TODO: Search - Once the query has been removed, should we close the search views off from the stack??
        private void OnSearchTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                ShowSearchViewIfRequired();
            }

            this.searchViewModel.SetQuery(this.SearchBox.Text);
        }

        private void OnSearchQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            this.searchViewModel.SetQuery(args.QueryText);
        }

        private void MainNavSelectionChanged(object sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer != null)
            {
                Type newPageType;
                string tagValue = args.SelectedItemContainer.Tag
                                                            .ToString();

                object viewModelParams = null;

                switch(tagValue)
                {
                    case NEW_MENU_TAG:
                        {
                            newPageType = typeof(Views.WhatsNewView);
                            viewModelParams = null;
                            break;
                        }
                    case CHART_MENU_TAG:
                    case GENRE_MENU_TAG:
                        newPageType = navLookup[tagValue];
                        break;

                    default:
                        return;
                }


                if (newPageType != null)
                {
                    this.ContentView.Navigate(newPageType, viewModelParams, args.RecommendedNavigationTransitionInfo);
                }
            }
        }


        private void OnNavigationOccurs(object sender, NavigationEventArgs e)
        {
            this.MainNav.IsBackEnabled = this.ContentView.CanGoBack;
        }


        private void ShowSearchViewIfRequired()
        {
            if (!(this.ContentView.Content is SearchView))
            {
                this.ContentView.Navigate(typeof(SearchView), this.searchViewModel);
            }
        }

        private void OnBackRequested(object sender, NavigationViewBackRequestedEventArgs e)
        {
            if (this.ContentView.CanGoBack)
            {
                this.ContentView.GoBack();
            }
        }

    }
}
