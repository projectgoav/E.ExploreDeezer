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

        bool IsTokenCallback(Uri uri);
        void ParseTokenCallback(Uri uri);
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

        public bool IsTokenCallback(Uri uri)
        {
            return this.oauthClient.CanParseRedirect(uri);
        }

        public void ParseTokenCallback(Uri uri)
        {
            this.oauthClient.TryParseRedirect(uri)
                            .ContinueWith(t =>
                            {
                                if (t.Result.State == OAuthLoginState.Success)
                                {
                                    this.authService.Login(t.Result.TokenResponse);
                                }
                                else if (t.Result.State == OAuthLoginState.UnknownError)
                                {
                                    //TODO: Failed to get token... Need to show a message here...
                                }

                                // If the user cancels it we just hide the login view, so don't worry
                                // about doing anything.
                            });
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

            }

            base.Dispose(disposing);
        }
    }
}
