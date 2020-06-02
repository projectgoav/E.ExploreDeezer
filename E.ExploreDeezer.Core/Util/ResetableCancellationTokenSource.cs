using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;

namespace E.ExploreDeezer.Core.Util
{
    /* Util: ResetableCancellationTokenSource
     * 
     * Normal token sources will throw if you try and access the token
     * once they have been disposed. This class helps manage this by
     * providing a pre-cancelled token in this instance so that we will
     * not throw. 
     * 
     * Most data controllers were also having to cancel a token source then
     * construct a new one in order to cancel previous fetches and start
     * new ones. Logic to cancel, dispose and re-create sources is wrapped
     * in this class as well :) */
    internal class ResetableCancellationTokenSource : IDisposable
    {
        private static readonly CancellationToken CANCELLED_TOKEN = new CancellationToken(canceled: true);

        private CancellationTokenSource tokenSource;


        public ResetableCancellationTokenSource()
        {
            this.tokenSource = new CancellationTokenSource();
        }


        // CancellationToken interface
        public CancellationToken Token => GetTokenSafe();


        public void Cancel()
            => this.tokenSource?.Cancel();

        
        public void Reset()
        {
            DisposeTokenSource();
            this.tokenSource = new CancellationTokenSource();
        }



        private CancellationToken GetTokenSafe()
        {
            if (this.tokenSource == null)
                return CANCELLED_TOKEN;

            // If token source is disposed, accessing it's token can throw
            if (this.tokenSource.IsCancellationRequested)
                return CANCELLED_TOKEN;

            return this.tokenSource.Token;
        }

        private void DisposeTokenSource()
        {
            if (this.tokenSource != null)
            {
                this.tokenSource.Cancel();
                this.tokenSource.Dispose();
                this.tokenSource = null;
            }
        }


        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeTokenSource();
            }
        }
    }
}
