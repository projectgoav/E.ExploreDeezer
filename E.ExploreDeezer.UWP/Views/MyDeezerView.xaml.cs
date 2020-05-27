using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using E.ExploreDeezer.Core;
using E.ExploreDeezer.Core.MyDeezer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace E.ExploreDeezer.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MyDeezerView : Page
    {
        public MyDeezerView()
        {
            this.InitializeComponent();
        }

        public IMyDeezerViewModel ViewModel { get; private set; }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var vm = ServiceRegistry.ViewModelFactory.CreateMyDeezerViewModel();

            this.ViewModel = vm;
            this.DataContext = vm;

            this.LoginButton.Click += OnLoginButtonClicked;
            this.LogoutButton.Click += OnLogoutButtonClicked;
        }



        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            this.DataContext = null;

            this.ViewModel.Dispose();
        }


        private void OnLoginButtonClicked(object sender, RoutedEventArgs e)
        {
            var loginDialog = new OAuthLoginDialog();

            loginDialog.ShowAsync();
        }

        private void OnLogoutButtonClicked(object sender, RoutedEventArgs e)
            => this.ViewModel.LogoutUser();

    }
}
