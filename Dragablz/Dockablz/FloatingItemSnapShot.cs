using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Dragablz.Dockablz
{
    /// <summary>
    /// experimentational.  might have to puish this back to mvvm only
    /// </summary>
    internal class FloatingItemSnapShot
    {
        private readonly object m_content;
        private readonly Rect m_location;
        private readonly int m_zIndex;
        private readonly WindowState m_state;

        public FloatingItemSnapShot(object content, Rect location, int zIndex, WindowState state)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

            m_content = content;
            m_location = location;
            m_zIndex = zIndex;
            m_state = state;
        }

        public static FloatingItemSnapShot Take(DragablzItem dragablzItem)
        {
            if (dragablzItem == null) throw new ArgumentNullException(nameof(dragablzItem));

            return new FloatingItemSnapShot(
                dragablzItem.Content,
                new Rect(dragablzItem.X, dragablzItem.Y, dragablzItem.ActualWidth, dragablzItem.ActualHeight),
                Panel.GetZIndex(dragablzItem),
                Layout.GetFloatingItemState(dragablzItem));
        }

        public void Apply(DragablzItem dragablzItem)
        {
            if (dragablzItem == null) throw new ArgumentNullException(nameof(dragablzItem));

            dragablzItem.SetCurrentValue(DragablzItem.XProperty, Location.Left);
            dragablzItem.SetCurrentValue(DragablzItem.YProperty, Location.Top);
            dragablzItem.SetCurrentValue(FrameworkElement.WidthProperty, Location.Width);
            dragablzItem.SetCurrentValue(FrameworkElement.HeightProperty, Location.Height);
            Layout.SetFloatingItemState(dragablzItem, State);
            Panel.SetZIndex(dragablzItem, ZIndex);
        }

        public object Content => m_content;

      public Rect Location => m_location;

      public int ZIndex => m_zIndex;

      public WindowState State => m_state;
    }
}
