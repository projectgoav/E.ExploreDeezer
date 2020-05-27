using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using E.Deezer;
using E.ExploreDeezer.Core.Mvvm;
using E.ExploreDeezer.Core.OAuth;

namespace E.ExploreDeezer.Core.MyDeezer
{
    public delegate void OnAuthenticationStatusChangedEventHandler(object sender, OnAuthenticationStatusChangedEventArgs e);

    public struct OnAuthenticationStatusChangedEventArgs
    {
        public OnAuthenticationStatusChangedEventArgs(bool isAuthenticated)
        {
            this.IsAuthenticated = isAuthenticated;
        }

        public bool IsAuthenticated { get; }
    }

    public interface IAuthenticationService
    {
        Task<bool> Login(OAuthResponse response);
        Task<bool> Logout();

        event OnAuthenticationStatusChangedEventHandler OnAuthenticationStatusChanged;
    }

    internal class AuthenticationService : IAuthenticationService,
                                           IDisposable
    {
        private const string OAUTH_TOKEN_KEY = "oauth_token";

        private readonly IDeezerSession session;
        private readonly ISecureStorage secureStorage;

        private event OnAuthenticationStatusChangedEventHandler OnAuthenticationStatusChangedInternal;


        public AuthenticationService(IDeezerSession session,
                                     ISecureStorage secureStorage)
        {
            this.session = session;
            this.secureStorage = secureStorage;

            this.secureStorage.ReadValue(OAUTH_TOKEN_KEY)
                              .ContinueWith(t =>
                              {
                                  if (t.Result != null && t.Result.Length > 0)
                                  {
                                      DoLogin(t.Result)
                                        .Wait();
                                  }
                              });
        }

        // IAuthenticationService
        public Task<bool> Login(OAuthResponse response)
            => DoLogin(response.AccessToken);

        public Task<bool> Logout()
        {
            var logoutTask = this.session.Logout(CancellationToken.None);

            logoutTask.ContinueWith(t =>
            {
                this.OnAuthenticationStatusChangedInternal?.Invoke(this, new OnAuthenticationStatusChangedEventArgs(false));

                this.IsLoggedIn = false;

                this.secureStorage.WriteValue(OAUTH_TOKEN_KEY, string.Empty);
            });

            return logoutTask;
        }


        public event OnAuthenticationStatusChangedEventHandler OnAuthenticationStatusChanged
        {
            add
            {
                this.OnAuthenticationStatusChangedInternal += value;
                value(this, new OnAuthenticationStatusChangedEventArgs(this.IsLoggedIn));
            }
            remove => this.OnAuthenticationStatusChangedInternal -= value;
        }


        private Task<bool> DoLogin(string accessToken)
        {
            var loginTask = this.session.Login(accessToken, CancellationToken.None); //TODO: Need to store this token and handle expiry

            loginTask.ContinueWith(t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                {
                    this.IsLoggedIn = false;

                    this.OnAuthenticationStatusChangedInternal?.Invoke(this, new OnAuthenticationStatusChangedEventArgs(false));
                }
                else
                {
                    this.IsLoggedIn = true;

                    this.OnAuthenticationStatusChangedInternal?.Invoke(this, new OnAuthenticationStatusChangedEventArgs(t.Result));
                    this.secureStorage.WriteValue(OAUTH_TOKEN_KEY, accessToken);

                }
            });

            return loginTask;
        }


        private bool IsLoggedIn { get; set; }

        // IDisposable
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Assert.That(this.OnAuthenticationStatusChangedInternal == null, "Dangling auth event handlers.");
            }
        }

    }
}
