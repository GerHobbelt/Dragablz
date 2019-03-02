using System;
using System.Windows;

namespace Dragablz
{
    public class NewTabHost<TElement> : INewTabHost<TElement> where TElement : UIElement
    {
        private readonly TElement m_container;
        private readonly TabablzControl m_tabablzControl;

        public NewTabHost(TElement container, TabablzControl tabablzControl)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (tabablzControl == null) throw new ArgumentNullException(nameof(tabablzControl));

            m_container = container;
            m_tabablzControl = tabablzControl;
        }

        public TElement Container
        {
            get { return m_container; }
        }

        public TabablzControl TabablzControl
        {
            get { return m_tabablzControl; }
        }
    }
}