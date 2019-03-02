using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Dragablz.Dockablz;

namespace Dragablz.Core
{
    internal enum InterTabTransferReason
    {
        Breach,
        Reentry
    }

    internal class InterTabTransfer
    {
        private readonly object m_item;
        private readonly DragablzItem m_originatorContainer;
        private readonly Orientation m_breachOrientation;
        private readonly Point m_dragStartWindowOffset;
        private readonly Point m_dragStartItemOffset;
        private readonly Point m_itemPositionWithinHeader;
        private readonly Size m_itemSize;
        private readonly IList<FloatingItemSnapShot> m_floatingItemSnapShots;
        private readonly bool m_isTransposing;
        private readonly InterTabTransferReason m_transferReason; 

        public InterTabTransfer(object item, DragablzItem originatorContainer, Orientation breachOrientation, Point dragStartWindowOffset, Point dragStartItemOffset, Point itemPositionWithinHeader, Size itemSize, IList<FloatingItemSnapShot> floatingItemSnapShots, bool isTransposing)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (originatorContainer == null) throw new ArgumentNullException(nameof(originatorContainer));
            if (floatingItemSnapShots == null) throw new ArgumentNullException(nameof(floatingItemSnapShots));

            m_transferReason = InterTabTransferReason.Breach;

            m_item = item;
            m_originatorContainer = originatorContainer;
            m_breachOrientation = breachOrientation;
            m_dragStartWindowOffset = dragStartWindowOffset;
            m_dragStartItemOffset = dragStartItemOffset;
            m_itemPositionWithinHeader = itemPositionWithinHeader;
            m_itemSize = itemSize;
            m_floatingItemSnapShots = floatingItemSnapShots;
            m_isTransposing = isTransposing;
        }

        public InterTabTransfer(object item, DragablzItem originatorContainer, Point dragStartItemOffset,
            IList<FloatingItemSnapShot> floatingItemSnapShots)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (originatorContainer == null) throw new ArgumentNullException(nameof(originatorContainer));
            if (floatingItemSnapShots == null) throw new ArgumentNullException(nameof(floatingItemSnapShots));

            m_transferReason = InterTabTransferReason.Reentry;

            m_item = item;
            m_originatorContainer = originatorContainer;
            m_dragStartItemOffset = dragStartItemOffset;
            m_floatingItemSnapShots = floatingItemSnapShots;
        }

        public Orientation BreachOrientation => m_breachOrientation;

      public Point DragStartWindowOffset => m_dragStartWindowOffset;

      public object Item => m_item;

      public DragablzItem OriginatorContainer => m_originatorContainer;

      public InterTabTransferReason TransferReason => m_transferReason;

      public Point DragStartItemOffset => m_dragStartItemOffset;

      public Point ItemPositionWithinHeader => m_itemPositionWithinHeader;

      public Size ItemSize => m_itemSize;

      public IList<FloatingItemSnapShot> FloatingItemSnapShots => m_floatingItemSnapShots;

      public bool IsTransposing => m_isTransposing;
    }
}