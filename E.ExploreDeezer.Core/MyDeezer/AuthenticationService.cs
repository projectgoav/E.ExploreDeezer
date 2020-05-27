using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using E.Deezer;
using Microsoft.Win32.SafeHandles;

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
