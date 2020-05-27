using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using E.Deezer;

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
        private readonly IDeezerSession session;

        private event OnAuthenticationStatusChangedEventHandler OnAuthenticationStatusChangedInternal;


        public AuthenticationService(IDeezerSession session)
        {
            this.session = session;

        }

        // IAuthenticationService
        public Task<bool> Login(OAuthResponse response)
        {
            var loginTask = this.session.Login(response.AccessToken, CancellationToken.None); //TODO: Need to store this token and handle expiry

            loginTask.ContinueWith(t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                    this.OnAuthenticationStatusChangedInternal?.Invoke(this, new OnAuthenticationStatusChangedEventArgs(false));
                else
                    this.OnAuthenticationStatusChangedInternal?.Invoke(this, new OnAuthenticationStatusChangedEventArgs(t.Result));
            });

            return loginTask;
        }

        public Task<bool> Logout()
        {
            var logoutTask = this.session.Logout(CancellationToken.None);

            logoutTask.ContinueWith(t =>
            {
                this.OnAuthenticationStatusChangedInternal?.Invoke(this, new OnAuthenticationStatusChangedEventArgs(false));
            });

            return logoutTask;
        }


        public event OnAuthenticationStatusChangedEventHandler OnAuthenticationStatusChanged
        {
            add
            {
                this.OnAuthenticationStatusChangedInternal += value;
                value(this, new OnAuthenticationStatusChangedEventArgs(false));
            }
            remove => this.OnAuthenticationStatusChangedInternal -= value;
        }


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
