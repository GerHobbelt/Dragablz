using System;

namespace Dragablz.Dockablz
{
    internal class FloatTransfer
    {
        private readonly double m_width;
        private readonly double m_height;
        private readonly object m_content;

        public FloatTransfer(double width, double height, object content)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            
            m_width = width;
            m_height = height;
            m_content = content;
        }

        public static FloatTransfer TakeSnapshot(DragablzItem dragablzItem, TabablzControl sourceTabControl)
        {
            if (dragablzItem == null) throw new ArgumentNullException(nameof(dragablzItem));

            return new FloatTransfer(sourceTabControl.ActualWidth, sourceTabControl.ActualHeight, dragablzItem.UnderlyingContent ?? dragablzItem.Content ?? dragablzItem);
        }

        [Obsolete]
        //TODO width and height transfer obsolete
        public double Width
        {
            get { return m_width; }
        }

        [Obsolete]
        //TODO width and height transfer obsolete
        public double Height
        {
            get { return m_height; }
        }

        public object Content
        {
            get { return m_content; }
        }
    }
}