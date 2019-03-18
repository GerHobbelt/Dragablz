using System;
using System.Collections.Generic;

namespace Dragablz.Core
{
    internal class FuncComparer<TObject> : IComparer<TObject>
    {
        private readonly Func<TObject, TObject, int> m_comparer;

        public FuncComparer(Func<TObject, TObject, int> comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            
            m_comparer = comparer;
        }

        public int Compare(TObject x, TObject y)
        {
            return m_comparer(x, y);
        }
    }
}