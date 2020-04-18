using System;
using System.Threading;

namespace Dragablz.Referenceless
{
    internal sealed class AnonymousDisposable : ICancelable, IDisposable
    {
        private volatile Action m_dispose;

        public bool IsDisposed => this.m_dispose == null;

        public AnonymousDisposable(Action dispose)
        {
            this.m_dispose = dispose;
        }

        public void Dispose()
        {
            var action = Interlocked.Exchange<Action>(ref m_dispose, (Action)null);
            if (action == null)
                return;
            action();
        }
    }
}
