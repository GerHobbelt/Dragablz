using System;

namespace Dragablz.Referenceless
{
    internal sealed class SerialDisposable : ICancelable, IDisposable
    {
        private readonly object m_gate = new object();
        private IDisposable m_current;
        private bool m_disposed;

        /// <summary>
        /// Gets a value that indicates whether the object is disposed.
        /// 
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                lock (this.m_gate)
                    return this.m_disposed;
            }
        }

        /// <summary>
        /// Gets or sets the underlying disposable.
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// If the SerialDisposable has already been disposed, assignment to this property causes immediate disposal of the given disposable object. Assigning this property disposes the previous disposable object.
        /// </remarks>
        public IDisposable Disposable
        {
            get => this.m_current;
            set
            {
                bool flag = false;
                IDisposable disposable = (IDisposable)null;
                lock (this.m_gate)
                {
                    flag = this.m_disposed;
                    if (!flag)
                    {
                        disposable = this.m_current;
                        this.m_current = value;
                    }
                }
                if (disposable != null)
                    disposable.Dispose();
                if (!flag || value == null)
                    return;
                value.Dispose();
            }
        }

        /// <summary>
        /// Disposes the underlying disposable as well as all future replacements.
        /// 
        /// </summary>
        public void Dispose()
        {
            IDisposable disposable = (IDisposable)null;
            lock (this.m_gate)
            {
                if (!this.m_disposed)
                {
                    this.m_disposed = true;
                    disposable = this.m_current;
                    this.m_current = (IDisposable)null;
                }
            }
            if (disposable == null)
                return;
            disposable.Dispose();
        }
    }
}
