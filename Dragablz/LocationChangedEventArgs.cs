using System;
using System.Windows;

namespace Dragablz
{
    public class LocationChangedEventArgs : EventArgs
    {
        private readonly object m_item;
        private readonly Point m_location;

        public LocationChangedEventArgs(object item, Point location)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            
            m_item = item;
            m_location = location;
        }

        public object Item => m_item;

      public Point Location => m_location;
    }
}