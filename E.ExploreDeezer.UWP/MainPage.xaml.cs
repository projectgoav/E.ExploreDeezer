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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace E.ExploreDeezer.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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


        public MainPage()
        {
            this.InitializeComponent();

            ServiceRegistry.Initialise(new UWPPlatformServices(this.Dispatcher));
            ServiceRegistry.Register<Frame>(this.ContentView);


            this.ContentView.Navigated += OnNavigationOccurs;
            this.MainNav.BackRequested += OnBackRequested;

            this.MainNav.SelectedItem = this.MainNav.MenuItems[0];
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


        private void OnBackRequested(object sender, NavigationViewBackRequestedEventArgs e)
        {
            if (this.ContentView.CanGoBack)
            {
                this.ContentView.GoBack();
            }
        }


    }
}
