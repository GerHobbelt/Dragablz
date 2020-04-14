using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Dragablz.Core;
using Dragablz.Dockablz;
using Dragablz.Referenceless;

namespace Dragablz
{
    /// <summary>
    /// It is not necessary to use a <see cref="DragablzWindow"/> to gain tab dragging features.
    /// What this Window does is allow a quick way to remove the Window border, and support transparency whilst
    /// dragging.
    /// </summary>
    [TemplatePart(Name = WINDOW_SURFACE_GRID_PART_NAME, Type = typeof(Grid))]
    [TemplatePart(Name = WINDOW_RESTORE_THUMB_PART_NAME, Type = typeof(Thumb))]
    [TemplatePart(Name = WINDOW_RESIZE_THUMB_PART_NAME, Type = typeof(Thumb))]
    public class DragablzWindow : Window
    {
        public const string WINDOW_SURFACE_GRID_PART_NAME = "PART_WindowSurface";
        public const string WINDOW_RESTORE_THUMB_PART_NAME = "PART_WindowRestoreThumb";
        public const string WINDOW_RESIZE_THUMB_PART_NAME = "PART_WindowResizeThumb";
        private readonly SerialDisposable m_templateSubscription = new SerialDisposable();

        public static RoutedCommand CloseWindowCommand = new RoutedCommand();
        public static RoutedCommand RestoreWindowCommand = new RoutedCommand();
        public static RoutedCommand MaximizeWindowCommand = new RoutedCommand();
        public static RoutedCommand MinimizeWindowCommand = new RoutedCommand();

        private const int RESIZE_MARGIN = 4;
        private Size m_sizeWhenResizeBegan;
        private Point m_screenMousePointWhenResizeBegan;
        private Point m_windowLocationPointWhenResizeBegan;
        private SizeGrip m_resizeType;

        private static SizeGrip[] m_leftMode = { SizeGrip.TopLeft, SizeGrip.Left, SizeGrip.BottomLeft };
        private static SizeGrip[] m_rightMode = { SizeGrip.TopRight, SizeGrip.Right, SizeGrip.BottomRight };
        private static SizeGrip[] m_topMode = { SizeGrip.TopLeft, SizeGrip.Top, SizeGrip.TopRight };
        private static SizeGrip[] m_bottomMode = { SizeGrip.BottomLeft, SizeGrip.Bottom, SizeGrip.BottomRight };

        private static double m_xScale = 1;
        private static double m_yScale = 1;
        private static bool m_dpiInitialized = false;

        static DragablzWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragablzWindow), new FrameworkPropertyMetadata(typeof(DragablzWindow)));
        }

        public DragablzWindow()
        {
            AddHandler(DragablzItem.DragStarted, new DragablzDragStartedEventHandler(ItemDragStarted), true);
            AddHandler(DragablzItem.DragCompleted, new DragablzDragCompletedEventHandler(ItemDragCompleted), true);
            CommandBindings.Add(new CommandBinding(CloseWindowCommand, CloseWindowExecuted));
            CommandBindings.Add(new CommandBinding(MaximizeWindowCommand, MaximizeWindowExecuted));
            CommandBindings.Add(new CommandBinding(MinimizeWindowCommand, MinimizeWindowExecuted));
            CommandBindings.Add(new CommandBinding(RestoreWindowCommand, RestoreWindowExecuted));
        }

        private static readonly DependencyPropertyKey IS_WINDOW_BEING_DRAGGED_BY_TAB_PROPERTY_KEY =
            DependencyProperty.RegisterReadOnly(
                "IsBeingDraggedByTab", typeof (bool), typeof (DragablzWindow),
                new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsBeingDraggedByTabProperty =
            IS_WINDOW_BEING_DRAGGED_BY_TAB_PROPERTY_KEY.DependencyProperty;

        public bool IsBeingDraggedByTab
        {
            get => (bool) GetValue(IsBeingDraggedByTabProperty);
          private set => SetValue(IS_WINDOW_BEING_DRAGGED_BY_TAB_PROPERTY_KEY, value);
        }

        private void ItemDragCompleted(object sender, DragablzDragCompletedEventArgs e)
        {
            IsBeingDraggedByTab = false;
        }

        private void ItemDragStarted(object sender, DragablzDragStartedEventArgs e)
        {
            var sourceOfDragItemsControl = ItemsControl.ItemsControlFromItemContainer(e.DragablzItem) as DragablzItemsControl;
            if (sourceOfDragItemsControl == null) return;

            var sourceTab = TabablzControl.GetOwnerOfHeaderItems(sourceOfDragItemsControl);
            if (sourceTab == null) return;

            if (sourceOfDragItemsControl.Items.Count != 1
                || (sourceTab.InterTabController != null && !sourceTab.InterTabController.MoveWindowWithSolitaryTabs)
                || Layout.IsContainedWithinBranch(sourceOfDragItemsControl))
                return;

            IsBeingDraggedByTab = true;
        }

        public override void OnApplyTemplate()
        {
            var windowSurfaceGrid = GetTemplateChild(WINDOW_SURFACE_GRID_PART_NAME) as Grid;
            var windowRestoreThumb = GetTemplateChild(WINDOW_RESTORE_THUMB_PART_NAME) as Thumb;
            var windowResizeThumb = GetTemplateChild(WINDOW_RESIZE_THUMB_PART_NAME) as Thumb;

            m_templateSubscription.Disposable = Disposable.Create(() =>
            {
                if (windowSurfaceGrid != null)
                {
                    windowSurfaceGrid.MouseLeftButtonDown -= WindowSurfaceGridOnMouseLeftButtonDown;
                }

                if (windowRestoreThumb != null)
                {
                    windowRestoreThumb.DragDelta -= WindowMoveThumbOnDragDelta;
                    windowRestoreThumb.MouseDoubleClick -= WindowRestoreThumbOnMouseDoubleClick;
                }

                if (windowResizeThumb == null) return;

                windowResizeThumb.MouseMove -= WindowResizeThumbOnMouseMove;
                windowResizeThumb.DragStarted -= WindowResizeThumbOnDragStarted;
                windowResizeThumb.DragDelta -= WindowResizeThumbOnDragDelta;
                windowResizeThumb.DragCompleted -= WindowResizeThumbOnDragCompleted;
            });

            base.OnApplyTemplate();

            if (windowSurfaceGrid != null)
            {
                windowSurfaceGrid.MouseLeftButtonDown += WindowSurfaceGridOnMouseLeftButtonDown;
            }

            if (windowRestoreThumb != null)
            {
                windowRestoreThumb.DragDelta += WindowMoveThumbOnDragDelta;
                windowRestoreThumb.MouseDoubleClick += WindowRestoreThumbOnMouseDoubleClick;
            }

            if (windowResizeThumb == null) return;

            windowResizeThumb.MouseMove += WindowResizeThumbOnMouseMove;
            windowResizeThumb.DragStarted += WindowResizeThumbOnDragStarted;
            windowResizeThumb.DragDelta += WindowResizeThumbOnDragDelta;
            windowResizeThumb.DragCompleted += WindowResizeThumbOnDragCompleted;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            var resizeThumb = GetTemplateChild(WINDOW_RESIZE_THUMB_PART_NAME) as Thumb;
            if (resizeThumb != null)
            {
                var outerRectangleGeometry = new RectangleGeometry(new Rect(sizeInfo.NewSize));
                var innerRectangleGeometry =
                    new RectangleGeometry(new Rect(RESIZE_MARGIN, RESIZE_MARGIN, sizeInfo.NewSize.Width - RESIZE_MARGIN * 2, sizeInfo.NewSize.Height - RESIZE_MARGIN*2));
                resizeThumb.Clip = new CombinedGeometry(GeometryCombineMode.Exclude, outerRectangleGeometry,
                    innerRectangleGeometry);
            }

            base.OnRenderSizeChanged(sizeInfo);
        }

        protected IntPtr CriticalHandle
        {
            get
            {
                var value = typeof (Window).GetProperty("CriticalHandle", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(this, new object[0]);
                return (IntPtr) value;
            }
        }

        private void WindowSurfaceGridOnMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (mouseButtonEventArgs.ChangedButton != MouseButton.Left) return;
            if (mouseButtonEventArgs.ClickCount == 1)
                DragMove();
            if (mouseButtonEventArgs.ClickCount == 2)
                WindowState = WindowState.Maximized;
        }

        private static void WindowResizeThumbOnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            var thumb = (Thumb)sender;
            var mousePositionInThumb = Mouse.GetPosition(thumb);
            thumb.Cursor = SelectCursor(SelectSizingMode(mousePositionInThumb, thumb.RenderSize));
        }

        private void WindowRestoreThumbOnMouseDoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            WindowState = WindowState.Normal;
        }

        private void WindowResizeThumbOnDragCompleted(object sender, DragCompletedEventArgs dragCompletedEventArgs)
        {
            Cursor = Cursors.Arrow;
        }

        private void WindowResizeThumbOnDragDelta(object sender, DragDeltaEventArgs dragDeltaEventArgs)
        {
            var mousePositionInWindow = Mouse.GetPosition(this);
            var currentScreenMousePoint = PointToScreen(mousePositionInWindow);

            var width = m_sizeWhenResizeBegan.Width;
            var height = m_sizeWhenResizeBegan.Height;
            var left = m_windowLocationPointWhenResizeBegan.X;
            var top = m_windowLocationPointWhenResizeBegan.Y;

            if (m_leftMode.Contains(m_resizeType))
            {
                var diff = currentScreenMousePoint.X - m_screenMousePointWhenResizeBegan.X;
                diff /= m_xScale;
                var suggestedWidth = width + -diff;
                left += diff;
                width = suggestedWidth;
            }
            if (m_rightMode.Contains(m_resizeType))
            {
                var diff = currentScreenMousePoint.X - m_screenMousePointWhenResizeBegan.X;
                diff /= m_xScale;
                width += diff;
            }
            if (m_topMode.Contains(m_resizeType))
            {
                var diff = currentScreenMousePoint.Y - m_screenMousePointWhenResizeBegan.Y;
                diff /= m_yScale;
                height += -diff;
                top += diff;
            }
            if (m_bottomMode.Contains(m_resizeType))
            {
                var diff = currentScreenMousePoint.Y - m_screenMousePointWhenResizeBegan.Y;
                diff /= m_yScale;
                height += diff;
            }

            width = Math.Max(MinWidth, width);
            height = Math.Max(MinHeight, height);
            //TODO must try harder.
            left = Math.Min(left, m_windowLocationPointWhenResizeBegan.X + m_sizeWhenResizeBegan.Width - RESIZE_MARGIN*4);
            //TODO must try harder.
            top = Math.Min(top, m_windowLocationPointWhenResizeBegan.Y + m_sizeWhenResizeBegan.Height - RESIZE_MARGIN * 4);
            SetCurrentValue(WidthProperty, width);
            SetCurrentValue(HeightProperty, height);
            SetCurrentValue(LeftProperty, left);
            SetCurrentValue(TopProperty, top);
        }

        private void GetDpi()
        {
            if (m_dpiInitialized)
            {
                return;
            }

            Matrix m = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
            m_xScale = m.M11;
            m_yScale = m.M22;
            m_dpiInitialized = true;
        }

        private void WindowResizeThumbOnDragStarted(object sender, DragStartedEventArgs dragStartedEventArgs)
        {
            m_sizeWhenResizeBegan = new Size(ActualWidth, ActualHeight);
            m_windowLocationPointWhenResizeBegan = new Point(Left, Top);
            var mousePositionInWindow = Mouse.GetPosition(this);
            m_screenMousePointWhenResizeBegan = PointToScreen(mousePositionInWindow);

            var thumb = (Thumb)sender;
            var mousePositionInThumb = Mouse.GetPosition(thumb);
            m_resizeType = SelectSizingMode(mousePositionInThumb, thumb.RenderSize);

            GetDpi();
        }

        private static SizeGrip SelectSizingMode(Point mousePositionInThumb, Size thumbSize)
        {
            if (mousePositionInThumb.X <= RESIZE_MARGIN)
            {
                if (mousePositionInThumb.Y <= RESIZE_MARGIN)
                    return SizeGrip.TopLeft;
                if (mousePositionInThumb.Y >= thumbSize.Height - RESIZE_MARGIN)
                    return SizeGrip.BottomLeft;
                return SizeGrip.Left;
            }

            if (mousePositionInThumb.X >= thumbSize.Width - RESIZE_MARGIN)
            {
                if (mousePositionInThumb.Y <= RESIZE_MARGIN)
                    return SizeGrip.TopRight;
                if (mousePositionInThumb.Y >= thumbSize.Height - RESIZE_MARGIN)
                    return SizeGrip.BottomRight;
                return SizeGrip.Right;
            }

            if (mousePositionInThumb.Y <= RESIZE_MARGIN)
                return SizeGrip.Top;

            return SizeGrip.Bottom;
        }

        private static Cursor SelectCursor(SizeGrip sizeGrip)
        {
            switch (sizeGrip)
            {
                case SizeGrip.Left:
                    return Cursors.SizeWE;
                case SizeGrip.TopLeft:
                    return Cursors.SizeNWSE;
                case SizeGrip.Top:
                    return Cursors.SizeNS;
                case SizeGrip.TopRight:
                    return Cursors.SizeNESW;
                case SizeGrip.Right:
                    return Cursors.SizeWE;
                case SizeGrip.BottomRight:
                    return Cursors.SizeNWSE;
                case SizeGrip.Bottom:
                    return Cursors.SizeNS;
                case SizeGrip.BottomLeft:
                    return Cursors.SizeNESW;
                default:
                    return Cursors.Arrow;
            }
        }

        private void WindowMoveThumbOnDragDelta(object sender, DragDeltaEventArgs dragDeltaEventArgs)
        {
            if (WindowState != WindowState.Maximized ||
                (!(Math.Abs(dragDeltaEventArgs.HorizontalChange) > 2) &&
                 !(Math.Abs(dragDeltaEventArgs.VerticalChange) > 2))) return;

            var cursorPos = Native.GetRawCursorPos();
            WindowState = WindowState.Normal;

            GetDpi();

            Top = cursorPos.Y / m_yScale - 2;
            Left = cursorPos.X / m_xScale - RestoreBounds.Width / 2;

            var lParam = (int)(uint)cursorPos.X | (cursorPos.Y << 16);
            Native.SendMessage(CriticalHandle, WindowMessage.WmLbuttonup, (IntPtr)HitTest.HtCaption,
                (IntPtr)lParam);
            Native.SendMessage(CriticalHandle, WindowMessage.WmSyscommand, (IntPtr)SystemCommand.ScMousemove,
                IntPtr.Zero);
        }

        private void RestoreWindowExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Native.PostMessage(new WindowInteropHelper(this).Handle, WindowMessage.WmSyscommand, (IntPtr)SystemCommand.ScRestore, IntPtr.Zero);
        }

        private void MinimizeWindowExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Native.PostMessage(new WindowInteropHelper(this).Handle, WindowMessage.WmSyscommand, (IntPtr)SystemCommand.ScMinimize, IntPtr.Zero);
        }

        private void MaximizeWindowExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Native.PostMessage(new WindowInteropHelper(this).Handle, WindowMessage.WmSyscommand, (IntPtr)SystemCommand.ScMaximize, IntPtr.Zero);
        }

        private void CloseWindowExecuted(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            Native.PostMessage(new WindowInteropHelper(this).Handle, WindowMessage.WmClose, IntPtr.Zero, IntPtr.Zero);
        }
    }
}
