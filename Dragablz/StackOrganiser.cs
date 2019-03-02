using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Dragablz.Core;

namespace Dragablz
{
    public abstract class StackOrganiser : IItemsOrganiser
    {
        private readonly Orientation m_orientation;
        private readonly double m_itemOffset;
        private readonly Func<DragablzItem, double> m_getDesiredSize;
        private readonly Func<DragablzItem, double> m_getLocation;
        private readonly DependencyProperty m_canvasDependencyProperty;
        private readonly Action<DragablzItem, double> m_setLocation;

        private readonly Dictionary<DragablzItem, double> m_activeStoryboardTargetLocations =
            new Dictionary<DragablzItem, double>();

        protected StackOrganiser(Orientation orientation, double itemOffset = 0)
        {
            m_orientation = orientation;
            m_itemOffset = itemOffset;

            switch (orientation)
            {
                case Orientation.Horizontal:
                    m_getDesiredSize = item => item.DesiredSize.Width;
                    m_getLocation = item => item.X;
                    m_setLocation = (item, coord) => item.SetCurrentValue(DragablzItem.XProperty, coord);
                    m_canvasDependencyProperty = Canvas.LeftProperty;
                    break;
                case Orientation.Vertical:
                    m_getDesiredSize = item => item.DesiredSize.Height;
                    m_getLocation = item => item.Y;
                    m_setLocation = (item, coord) => item.SetCurrentValue(DragablzItem.YProperty, coord);
                    m_canvasDependencyProperty = Canvas.TopProperty;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation));
            }
        }

        #region LocationInfo

        private class LocationInfo
        {
            private readonly DragablzItem m_item;
            private readonly double m_start;
            private readonly double m_mid;
            private readonly double m_end;

            public LocationInfo(DragablzItem item, double start, double mid, double end)
            {
                m_item = item;
                m_start = start;
                m_mid = mid;
                m_end = end;
            }

            public double Start
            {
                get { return m_start; }
            }

            public double Mid
            {
                get { return m_mid; }
            }

            public double End
            {
                get { return m_end; }
            }

            public DragablzItem Item
            {
                get { return m_item; }
            }
        }

        #endregion

        public virtual Orientation Orientation
        {
            get { return m_orientation; }
        }

        public virtual void Organise(DragablzItemsControl requestor, Size measureBounds, IEnumerable<DragablzItem> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            OrganiseInternal(
                requestor, 
                measureBounds,
                items.Select((di, idx) => new Tuple<int, DragablzItem>(idx, di))
                        .OrderBy(tuple => tuple,
                            MultiComparer<Tuple<int, DragablzItem>>.Ascending(tuple => m_getLocation(tuple.Item2))
                                .ThenAscending(tuple => tuple.Item1))
                        .Select(tuple => tuple.Item2));            
        }

        public virtual void Organise(DragablzItemsControl requestor, Size measureBounds, IOrderedEnumerable<DragablzItem> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            OrganiseInternal(
                requestor,
                measureBounds,
                items);
        }

        private void OrganiseInternal(DragablzItemsControl requestor, Size measureBounds,
            IEnumerable<DragablzItem> items)
        {
            var currentCoord = 0.0;
            var z = int.MaxValue;
            var logicalIndex = 0;
            foreach (var newItem in items)
            {
                Panel.SetZIndex(newItem, newItem.IsSelected ? int.MaxValue : --z);
                SetLocation(newItem, currentCoord);
                newItem.LogicalIndex = logicalIndex++;
                newItem.Measure(measureBounds);
                var desiredSize = m_getDesiredSize(newItem);
                if (desiredSize == 0.0) desiredSize = 1.0; //no measure? create something to help sorting
                currentCoord += desiredSize + m_itemOffset;
            }
        }


        public virtual void OrganiseOnMouseDownWithin(DragablzItemsControl requestor, Size measureBounds,
            List<DragablzItem> siblingItems, DragablzItem dragablzItem)
        {

        }

        private IDictionary<DragablzItem, LocationInfo> m_siblingItemLocationOnDragStart;

        public virtual void OrganiseOnDragStarted(DragablzItemsControl requestor, Size measureBounds,
            IEnumerable<DragablzItem> siblingItems, DragablzItem dragItem)
        {
            if (siblingItems == null) throw new ArgumentNullException(nameof(siblingItems));
            if (dragItem == null) throw new ArgumentNullException(nameof(dragItem));

            m_siblingItemLocationOnDragStart = siblingItems.Select(GetLocationInfo).ToDictionary(loc => loc.Item);
        }

        public virtual void OrganiseOnDrag(DragablzItemsControl requestor, Size measureBounds,
            IEnumerable<DragablzItem> siblingItems, DragablzItem dragItem)
        {
            if (siblingItems == null) throw new ArgumentNullException(nameof(siblingItems));
            if (dragItem == null) throw new ArgumentNullException(nameof(dragItem));

            var currentLocations = siblingItems
                .Select(GetLocationInfo)
                .Union(new[] {GetLocationInfo(dragItem)})
                .OrderBy(loc => loc.Item == dragItem ? loc.Start : m_siblingItemLocationOnDragStart[loc.Item].Start);

            var currentCoord = 0.0;
            var zIndex = int.MaxValue;
            foreach (var location in currentLocations)
            {
                if (!Equals(location.Item, dragItem))
                {
                    SendToLocation(location.Item, currentCoord);
                    Panel.SetZIndex(location.Item, --zIndex);
                }
                currentCoord += m_getDesiredSize(location.Item) + m_itemOffset;                
            }
            Panel.SetZIndex(dragItem, int.MaxValue);
        }

        public virtual void OrganiseOnDragCompleted(DragablzItemsControl requestor, Size measureBounds,
            IEnumerable<DragablzItem> siblingItems, DragablzItem dragItem)
        {
            if (siblingItems == null) throw new ArgumentNullException(nameof(siblingItems));
            var currentLocations = siblingItems
                .Select(GetLocationInfo)
                .Union(new[] {GetLocationInfo(dragItem)})
                .OrderBy(loc => loc.Item == dragItem ? loc.Start : m_siblingItemLocationOnDragStart[loc.Item].Start);

            var currentCoord = 0.0;
            var z = int.MaxValue;
            var logicalIndex = 0;
            foreach (var location in currentLocations)
            {
                SetLocation(location.Item, currentCoord);
                currentCoord += m_getDesiredSize(location.Item) + m_itemOffset;
                Panel.SetZIndex(location.Item, --z);
                location.Item.LogicalIndex = logicalIndex++;
            }
            Panel.SetZIndex(dragItem, int.MaxValue);
        }

        public virtual Point ConstrainLocation(DragablzItemsControl requestor, Size measureBounds, Point itemCurrentLocation,
            Size itemCurrentSize, Point itemDesiredLocation, Size itemDesiredSize)
        {
            var fixedItems = requestor.FixedItemCount;
            var lowerBound = fixedItems == 0
                ? -1d
                : GetLocationInfo(requestor.DragablzItems()
                    .Take(fixedItems)
                    .Last()).End + m_itemOffset - 1;

            return new Point(
                m_orientation == Orientation.Vertical
                    ? 0
                    : Math.Min(Math.Max(lowerBound, itemDesiredLocation.X), (measureBounds.Width) + 1),
                m_orientation == Orientation.Horizontal
                    ? 0
                    : Math.Min(Math.Max(lowerBound, itemDesiredLocation.Y), (measureBounds.Height) + 1)
                );
        }

        public virtual Size Measure(DragablzItemsControl requestor, Size availableSize, IEnumerable<DragablzItem> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            var size = new Size(double.PositiveInfinity, double.PositiveInfinity);

            double width = 0, height = 0;
            var isFirst = true;
            foreach (var dragablzItem in items)
            {
                dragablzItem.Measure(size);
                if (m_orientation == Orientation.Horizontal)
                {
                    width += !dragablzItem.IsLoaded ? dragablzItem.DesiredSize.Width : dragablzItem.ActualWidth;
                    if (!isFirst)
                        width += m_itemOffset;
                    height = Math.Max(height,
                        !dragablzItem.IsLoaded ? dragablzItem.DesiredSize.Height : dragablzItem.ActualHeight);
                }
                else
                {
                    width = Math.Max(width,
                        !dragablzItem.IsLoaded ? dragablzItem.DesiredSize.Width : dragablzItem.ActualWidth);
                    height += !dragablzItem.IsLoaded ? dragablzItem.DesiredSize.Height : dragablzItem.ActualHeight;
                    if (!isFirst)
                        height += m_itemOffset;
                }

                isFirst = false;
            }

            return new Size(Math.Max(width, 0), Math.Max(height, 0));
        }

        public virtual IEnumerable<DragablzItem> Sort(IEnumerable<DragablzItem> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            return items.OrderBy(i => GetLocationInfo(i).Start);
        }

        private void SetLocation(DragablzItem dragablzItem, double location)
        {                     
            m_setLocation(dragablzItem, location);
        }
        
        private void SendToLocation(DragablzItem dragablzItem, double location)
        {                        
            double activeTarget;
            if (Math.Abs(m_getLocation(dragablzItem) - location) < 1.0
                ||
                m_activeStoryboardTargetLocations.TryGetValue(dragablzItem, out activeTarget)
                && Math.Abs(activeTarget - location) < 1.0)
            {             
                return;
            }            

            m_activeStoryboardTargetLocations[dragablzItem] = location;

            var storyboard = new Storyboard {FillBehavior = FillBehavior.Stop};
            storyboard.WhenComplete(sb =>
            {
                m_setLocation(dragablzItem, location);
                sb.Remove(dragablzItem);
                m_activeStoryboardTargetLocations.Remove(dragablzItem);
            });

            var timeline = new DoubleAnimationUsingKeyFrames();
            timeline.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath(m_canvasDependencyProperty));
            timeline.KeyFrames.Add(
                new EasingDoubleKeyFrame(location, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(200)))
                {
                    EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
                });
            storyboard.Children.Add(timeline);            
            storyboard.Begin(dragablzItem, true);            
        }

        private LocationInfo GetLocationInfo(DragablzItem item)
        {
            var size = m_getDesiredSize(item);
            double startLocation;
            if (!m_activeStoryboardTargetLocations.TryGetValue(item, out startLocation))
                startLocation = m_getLocation(item);
            var midLocation = startLocation + size / 2;
            var endLocation = startLocation + size;

            return new LocationInfo(item, startLocation, midLocation, endLocation);
        }
    }
}