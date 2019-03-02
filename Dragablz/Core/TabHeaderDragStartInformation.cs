using System;

namespace Dragablz.Core
{
    internal class TabHeaderDragStartInformation
    {
        private readonly DragablzItem m_dragItem;
        private readonly double m_dragablzItemsControlHorizontalOffset;
        private readonly double m_dragablzItemControlVerticalOffset; 
        private readonly double m_dragablzItemHorizontalOffset;
        private readonly double m_dragablzItemVerticalOffset;

        public TabHeaderDragStartInformation(
            DragablzItem dragItem,
            double dragablzItemsControlHorizontalOffset, double dragablzItemControlVerticalOffset, double dragablzItemHorizontalOffset, double dragablzItemVerticalOffset)
        {
            if (dragItem == null) throw new ArgumentNullException(nameof(dragItem));

            m_dragItem = dragItem;
            m_dragablzItemsControlHorizontalOffset = dragablzItemsControlHorizontalOffset;
            m_dragablzItemControlVerticalOffset = dragablzItemControlVerticalOffset;
            m_dragablzItemHorizontalOffset = dragablzItemHorizontalOffset;
            m_dragablzItemVerticalOffset = dragablzItemVerticalOffset;
        }

        public double DragablzItemsControlHorizontalOffset
        {
            get { return m_dragablzItemsControlHorizontalOffset; }
        }

        public double DragablzItemControlVerticalOffset
        {
            get { return m_dragablzItemControlVerticalOffset; }
        }

        public double DragablzItemHorizontalOffset
        {
            get { return m_dragablzItemHorizontalOffset; }
        }

        public double DragablzItemVerticalOffset
        {
            get { return m_dragablzItemVerticalOffset; }
        }

        public DragablzItem DragItem
        {
            get { return m_dragItem; }
        }
    }
}