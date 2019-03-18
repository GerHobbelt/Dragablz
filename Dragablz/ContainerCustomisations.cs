using System;
using System.Windows;

namespace Dragablz
{
    internal class ContainerCustomisations
    {
        private readonly Func<DragablzItem> m_getContainerForItemOverride;
        private readonly Action<DependencyObject, object> m_prepareContainerForItemOverride;
        private readonly Action<DependencyObject, object> m_clearingContainerForItemOverride;

        public ContainerCustomisations(Func<DragablzItem> getContainerForItemOverride = null, Action<DependencyObject, object> prepareContainerForItemOverride = null, Action<DependencyObject, object> clearingContainerForItemOverride = null)
        {
            m_getContainerForItemOverride = getContainerForItemOverride;
            m_prepareContainerForItemOverride = prepareContainerForItemOverride;
            m_clearingContainerForItemOverride = clearingContainerForItemOverride;
        }

        public Func<DragablzItem> GetContainerForItemOverride => m_getContainerForItemOverride;

      public Action<DependencyObject, object> PrepareContainerForItemOverride => m_prepareContainerForItemOverride;

      public Action<DependencyObject, object> ClearingContainerForItemOverride => m_clearingContainerForItemOverride;
    }
}