﻿using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Dragablz
{
    public delegate void DragablzDragCompletedEventHandler(object sender, DragablzDragCompletedEventArgs e);

    public class DragablzDragCompletedEventArgs : RoutedEventArgs
    {
        private readonly DragablzItem m_dragablzItem;
        private readonly bool m_isDropTargetFound;
        private readonly DragCompletedEventArgs m_dragCompletedEventArgs;

        public DragablzDragCompletedEventArgs(DragablzItem dragablzItem, DragCompletedEventArgs dragCompletedEventArgs)
        {
            if (dragablzItem == null) throw new ArgumentNullException(nameof(dragablzItem));
            if (dragCompletedEventArgs == null) throw new ArgumentNullException(nameof(dragCompletedEventArgs));

            m_dragablzItem = dragablzItem;
            m_dragCompletedEventArgs = dragCompletedEventArgs;
        }

        public DragablzDragCompletedEventArgs(RoutedEvent routedEvent, DragablzItem dragablzItem, DragCompletedEventArgs dragCompletedEventArgs)
            : base(routedEvent)
        {
            if (dragablzItem == null) throw new ArgumentNullException(nameof(dragablzItem));
            if (dragCompletedEventArgs == null) throw new ArgumentNullException(nameof(dragCompletedEventArgs));

            m_dragablzItem = dragablzItem;
            m_dragCompletedEventArgs = dragCompletedEventArgs;
        }

        public DragablzDragCompletedEventArgs(RoutedEvent routedEvent, object source, DragablzItem dragablzItem, DragCompletedEventArgs dragCompletedEventArgs)
            : base(routedEvent, source)
        {
            if (dragablzItem == null) throw new ArgumentNullException(nameof(dragablzItem));
            if (dragCompletedEventArgs == null) throw new ArgumentNullException(nameof(dragCompletedEventArgs));

            m_dragablzItem = dragablzItem;
            m_dragCompletedEventArgs = dragCompletedEventArgs;
        }

        public DragablzItem DragablzItem => m_dragablzItem;

        public DragCompletedEventArgs DragCompletedEventArgs => m_dragCompletedEventArgs;
    }
}