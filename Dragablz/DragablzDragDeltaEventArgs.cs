using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Dragablz
{
    public delegate void DragablzDragDeltaEventHandler(object sender, DragablzDragDeltaEventArgs e);

    public class DragablzDragDeltaEventArgs : DragablzItemEventArgs
    {
        private readonly DragDeltaEventArgs m_dragDeltaEventArgs;

        public DragablzDragDeltaEventArgs(DragablzItem dragablzItem, DragDeltaEventArgs dragDeltaEventArgs)
            : base(dragablzItem)
        {
            if (dragDeltaEventArgs == null) throw new ArgumentNullException(nameof(dragDeltaEventArgs));

            m_dragDeltaEventArgs = dragDeltaEventArgs;
        }

        public DragablzDragDeltaEventArgs(RoutedEvent routedEvent, DragablzItem dragablzItem, DragDeltaEventArgs dragDeltaEventArgs) 
            : base(routedEvent, dragablzItem)
        {
            if (dragDeltaEventArgs == null) throw new ArgumentNullException(nameof(dragDeltaEventArgs));

            m_dragDeltaEventArgs = dragDeltaEventArgs;
        }

        public DragablzDragDeltaEventArgs(RoutedEvent routedEvent, object source, DragablzItem dragablzItem, DragDeltaEventArgs dragDeltaEventArgs) 
            : base(routedEvent, source, dragablzItem)
        {
            if (dragDeltaEventArgs == null) throw new ArgumentNullException(nameof(dragDeltaEventArgs));

            m_dragDeltaEventArgs = dragDeltaEventArgs;
        }

        public DragDeltaEventArgs DragDeltaEventArgs
        {
            get { return m_dragDeltaEventArgs; }
        }

        public bool Cancel { get; set; }        
    }
}