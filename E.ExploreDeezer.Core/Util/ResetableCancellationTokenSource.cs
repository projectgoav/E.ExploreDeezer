using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;

namespace E.ExploreDeezer.Core.Util
{
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
