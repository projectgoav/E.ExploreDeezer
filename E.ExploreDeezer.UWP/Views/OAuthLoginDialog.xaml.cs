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
using E.ExploreDeezer.Core.MyDeezer;
// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace E.ExploreDeezer.UWP.Views
{
    public sealed partial class OAuthLoginDialog : ContentDialog
    {
        public OAuthLoginDialog()
        {
            this.InitializeComponent();

            this.Opened += OnDialogOpened;
            this.Closed += OnDialogClosed;


        }


        private void OnDialogOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            this.ViewModel = ServiceRegistry.ViewModelFactory.CreateLoginViewModel();
            this.DataContext = this.ViewModel;

            this.LoginWebView.Visibility = Visibility.Collapsed;
            this.LoadingContainer.Visibility = Visibility.Visible;

            this.LoginWebView.LoadCompleted += OnWebViewLoadCompleted;
            this.LoginWebView.NavigationStarting += OnWebViewNavigationStarting;

            this.LoginWebView.Navigate(new Uri(this.ViewModel.LoginUri));      
        }

        private void OnDialogClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            this.LoginWebView.LoadCompleted -= OnWebViewLoadCompleted;
            this.LoginWebView.NavigationStarting -= OnWebViewNavigationStarting;

            this.ViewModel.Dispose();
            this.DataContext = null;
        }


        private void OnWebViewLoadCompleted(object sender, NavigationEventArgs e)
        {
            this.LoginWebView.Visibility = Visibility.Visible;
            this.LoadingContainer.Visibility = Visibility.Collapsed;
        }

        private void OnWebViewNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            this.LoadingContainer.Visibility = Visibility.Visible;
            this.LoginWebView.Visibility = Visibility.Collapsed;

            if (this.ViewModel.IsTokenCallback(args.Uri))
            {
                this.ViewModel.ParseTokenCallback(args.Uri);
                args.Cancel = true;

                this.Hide();
            }
        }




        public ILoginViewModel ViewModel { get; private set; }


        private void OnCancelClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Hide();
        }

    }
}
