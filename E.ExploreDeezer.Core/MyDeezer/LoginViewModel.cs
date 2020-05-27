using System;
using System.Collections.Generic;
using System.Text;

using E.ExploreDeezer.Core.Mvvm;
using E.ExploreDeezer.Core.OAuth;

namespace E.ExploreDeezer.Core.MyDeezer
{
    public interface ILoginViewModel : IViewModel,
                                       IDisposable
    {
        string LoginUri { get; }
    }

    internal class LoginViewModel : ViewModelBase,
                                    ILoginViewModel
    {
        private readonly IOAuthClient oauthClient;
        private readonly IAuthenticationService authService;

        public LoginViewModel(IOAuthClient oAuthClient,
                              IAuthenticationService authService,
                              IPlatformServices platformServices)
            : base(platformServices)
        {
            this.oauthClient = oAuthClient;
            this.authService = authService;

            this.LoginUri = this.oauthClient.LoginUri;
        }


        // ILoginViewModel
        public string LoginUri { get; }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

            }

            base.Dispose(disposing);
        }
    }
}
