﻿using System;

namespace Dragablz
{
    public class OrderChangedEventArgs : EventArgs
    {
        private readonly object[] m_previousOrder;
        private readonly object[] m_newOrder;

        public OrderChangedEventArgs(object[] previousOrder, object[] newOrder)
        {
            if (newOrder == null) throw new ArgumentNullException(nameof(newOrder));

            m_previousOrder = previousOrder;
            m_newOrder = newOrder;
        }

        public object[] PreviousOrder
        {
            get { return m_previousOrder; }
        }

        public object[] NewOrder
        {
            get { return m_newOrder; }
        }
    }
}