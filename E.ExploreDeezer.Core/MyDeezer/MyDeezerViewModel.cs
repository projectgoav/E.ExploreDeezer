using System;
using System.Collections.Generic;
using System.Text;

using E.ExploreDeezer.Core.Mvvm;

namespace E.ExploreDeezer.Core.MyDeezer
{
    public interface IMyDeezerViewModel : IViewModel,
                                          IDisposable
    {
        bool IsLoggedIn { get; }
        bool IsLoggedOut { get; }

    }
     
    internal class MyDeezerViewModel : ViewModelBase,
                                       IMyDeezerViewModel
    {
        private readonly IAuthenticationService authService;

        private bool isLoggedIn;


        public MyDeezerViewModel(IAuthenticationService authService,
                                 IPlatformServices platformServices)
            : base(platformServices)
        {
            this.authService = authService;

            this.authService.OnAuthenticationStatusChanged += OnAuthenticationStatusChanged;
        }


        // IMyDeezerViewModel
        private bool LoggedIn
        {
            get => this.isLoggedIn;
            set
            {
                if (SetProperty(ref this.isLoggedIn, value))
                {
                    RaisePropertyChangedSafe(nameof(IsLoggedIn));
                    RaisePropertyChangedSafe(nameof(IsLoggedOut));
                }
            }
        }

        public bool IsLoggedIn => this.LoggedIn;
        public bool IsLoggedOut => !this.LoggedIn;



        private void OnAuthenticationStatusChanged(object sender, OnAuthenticationStatusChangedEventArgs e)
        {
            this.LoggedIn = e.IsAuthenticated;
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.authService.OnAuthenticationStatusChanged -= OnAuthenticationStatusChanged;
            }

            base.Dispose(disposing);
        }
    }
}
