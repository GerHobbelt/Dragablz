using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Dragablz
{
    public delegate void DragablzDragStartedEventHandler(object sender, DragablzDragStartedEventArgs e);

    public class DragablzDragStartedEventArgs : DragablzItemEventArgs
    {
        private readonly DragStartedEventArgs m_dragStartedEventArgs;

        public DragablzDragStartedEventArgs(DragablzItem dragablzItem, DragStartedEventArgs dragStartedEventArgs)
            : base(dragablzItem)
        {
            if (dragStartedEventArgs == null) throw new ArgumentNullException(nameof(dragStartedEventArgs));

            m_dragStartedEventArgs = dragStartedEventArgs;
        }

        public DragablzDragStartedEventArgs(RoutedEvent routedEvent, DragablzItem dragablzItem, DragStartedEventArgs dragStartedEventArgs)
            : base(routedEvent, dragablzItem)
        {
            m_dragStartedEventArgs = dragStartedEventArgs;
        }

        public DragablzDragStartedEventArgs(RoutedEvent routedEvent, object source, DragablzItem dragablzItem, DragStartedEventArgs dragStartedEventArgs)
            : base(routedEvent, source, dragablzItem)
        {
            m_dragStartedEventArgs = dragStartedEventArgs;
        }

        public DragStartedEventArgs DragStartedEventArgs => m_dragStartedEventArgs;
    }
}