using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Dragablz
{
    public delegate void DragablzItemEventHandler(object sender, DragablzItemEventArgs e);

    public class DragablzItemEventArgs : RoutedEventArgs
    {
        private readonly DragablzItem m_dragablzItem;

        public DragablzItemEventArgs(DragablzItem dragablzItem)
        {
            if (dragablzItem == null) throw new ArgumentNullException(nameof(dragablzItem));            

            m_dragablzItem = dragablzItem;
        }

        public DragablzItemEventArgs(RoutedEvent routedEvent, DragablzItem dragablzItem)
            : base(routedEvent)
        {
            m_dragablzItem = dragablzItem;
        }

        public DragablzItemEventArgs(RoutedEvent routedEvent, object source, DragablzItem dragablzItem)
            : base(routedEvent, source)
        {
            m_dragablzItem = dragablzItem;
        }

        public DragablzItem DragablzItem => m_dragablzItem;
    }
}