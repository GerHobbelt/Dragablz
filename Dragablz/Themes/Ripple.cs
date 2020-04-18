using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Dragablz.Themes
{
    [TemplateVisualState(GroupName = "CommonStates", Name = TEMPLATE_STATE_NORMAL)]
    [TemplateVisualState(GroupName = "CommonStates", Name = TEMPLATE_STATE_MOUSE_PRESSED)]
    [TemplateVisualState(GroupName = "CommonStates", Name = TEMPLATE_STATE_MOUSE_OUT)]
    public class Ripple : ContentControl
    {
        public const string TEMPLATE_STATE_NORMAL = "Normal";
        public const string TEMPLATE_STATE_MOUSE_PRESSED = "MousePressed";
        public const string TEMPLATE_STATE_MOUSE_OUT = "MouseOut";

        private static readonly HashSet<Ripple> PRESSED_INSTANCES = new HashSet<Ripple>();

        static Ripple()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Ripple), new FrameworkPropertyMetadata(typeof(Ripple)));

            EventManager.RegisterClassHandler(typeof(Window), Mouse.PreviewMouseUpEvent, new MouseButtonEventHandler(MouseButtonEventHandler), true);
            EventManager.RegisterClassHandler(typeof(Window), Mouse.MouseMoveEvent, new MouseEventHandler(MouseMouveEventHandler), true);
            EventManager.RegisterClassHandler(typeof(UserControl), Mouse.PreviewMouseUpEvent, new MouseButtonEventHandler(MouseButtonEventHandler), true);
            EventManager.RegisterClassHandler(typeof(UserControl), Mouse.MouseMoveEvent, new MouseEventHandler(MouseMouveEventHandler), true);
        }

        public Ripple()
        {
            SizeChanged += OnSizeChanged;
        }

        private static void MouseButtonEventHandler(object sender, MouseButtonEventArgs e)
        {
            foreach (var ripple in PRESSED_INSTANCES)
            {
                // adjust the transition scale time according to the current animated scale
                var scaleTrans = ripple.Template.FindName("ScaleTransform", ripple) as ScaleTransform;
                if (scaleTrans != null)
                {
                    double currentScale = scaleTrans.ScaleX;
                    var newTime = TimeSpan.FromMilliseconds(300 * (1.0 - currentScale));

                    // change the scale animation according to the current scale
                    var scaleXKeyFrame = ripple.Template.FindName("MousePressedToNormalScaleXKeyFrame", ripple) as EasingDoubleKeyFrame;
                    if (scaleXKeyFrame != null)
                    {
                        scaleXKeyFrame.KeyTime = KeyTime.FromTimeSpan(newTime);
                    }
                    var scaleYKeyFrame = ripple.Template.FindName("MousePressedToNormalScaleYKeyFrame", ripple) as EasingDoubleKeyFrame;
                    if (scaleYKeyFrame != null)
                    {
                        scaleYKeyFrame.KeyTime = KeyTime.FromTimeSpan(newTime);
                    }
                }

                VisualStateManager.GoToState(ripple, TEMPLATE_STATE_NORMAL, true);
            }
            PRESSED_INSTANCES.Clear();
        }

        private static void MouseMouveEventHandler(object sender, MouseEventArgs e)
        {
            foreach (var ripple in PRESSED_INSTANCES.ToList())
            {
                var relativePosition = Mouse.GetPosition(ripple);
                if (relativePosition.X < 0
                    || relativePosition.Y < 0
                    || relativePosition.X >= ripple.ActualWidth
                    || relativePosition.Y >= ripple.ActualHeight)

                {
                    VisualStateManager.GoToState(ripple, TEMPLATE_STATE_MOUSE_OUT, true);
                    PRESSED_INSTANCES.Remove(ripple);
                }
            }
        }

        public static readonly DependencyProperty FeedbackProperty = DependencyProperty.Register(
            "Feedback", typeof(Brush), typeof(Ripple), new PropertyMetadata(default(Brush)));

        public Brush Feedback
        {
            get => (Brush)GetValue(FeedbackProperty);
            set => SetValue(FeedbackProperty, value);
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            var point = e.GetPosition(this);

            if (RippleAssist.GetIsCentered(this))
            {
                var innerContent = (Content as FrameworkElement);

                if (innerContent != null)
                {
                    var position = innerContent.TransformToAncestor(this)
                        .Transform(new Point(0, 0));

                    RippleX = position.X + innerContent.ActualWidth / 2 - RippleSize / 2;
                    RippleY = position.Y + innerContent.ActualHeight / 2 - RippleSize / 2;
                }
                else
                {
                    RippleX = ActualWidth / 2 - RippleSize / 2;
                    RippleY = ActualHeight / 2 - RippleSize / 2;
                }
            }
            else
            {
                RippleX = point.X - RippleSize / 2;
                RippleY = point.Y - RippleSize / 2;
            }

            VisualStateManager.GoToState(this, TEMPLATE_STATE_NORMAL, false);
            VisualStateManager.GoToState(this, TEMPLATE_STATE_MOUSE_PRESSED, true);
            PRESSED_INSTANCES.Add(this);

            base.OnPreviewMouseLeftButtonDown(e);
        }

        private static readonly DependencyPropertyKey RIPPLE_SIZE_PROPERTY_KEY =
            DependencyProperty.RegisterReadOnly(
                "RippleSize", typeof(double), typeof(Ripple),
                new PropertyMetadata(default(double)));

        public static readonly DependencyProperty RippleSizeProperty =
            RIPPLE_SIZE_PROPERTY_KEY.DependencyProperty;

        public double RippleSize
        {
            get => (double)GetValue(RippleSizeProperty);
            private set => SetValue(RIPPLE_SIZE_PROPERTY_KEY, value);
        }

        private static readonly DependencyPropertyKey RIPPLE_X_PROPERTY_KEY =
            DependencyProperty.RegisterReadOnly(
                "RippleX", typeof(double), typeof(Ripple),
                new PropertyMetadata(default(double)));

        public static readonly DependencyProperty RippleXProperty =
            RIPPLE_X_PROPERTY_KEY.DependencyProperty;

        public double RippleX
        {
            get => (double)GetValue(RippleXProperty);
            private set => SetValue(RIPPLE_X_PROPERTY_KEY, value);
        }

        private static readonly DependencyPropertyKey RIPPLE_Y_PROPERTY_KEY =
            DependencyProperty.RegisterReadOnly(
                "RippleY", typeof(double), typeof(Ripple),
                new PropertyMetadata(default(double)));

        public static readonly DependencyProperty RippleYProperty =
            RIPPLE_Y_PROPERTY_KEY.DependencyProperty;

        public double RippleY
        {
            get => (double)GetValue(RippleYProperty);
            private set => SetValue(RIPPLE_Y_PROPERTY_KEY, value);
        }

        /// <summary>
        ///   The DependencyProperty for the RecognizesAccessKey property.
        ///   Default Value: false
        /// </summary>
        public static readonly DependencyProperty RecognizesAccessKeyProperty =
            DependencyProperty.Register(
                "RecognizesAccessKey", typeof(bool), typeof(Ripple),
                new PropertyMetadata(default(bool)));

        /// <summary>
        ///   Determine if Ripple should use AccessText in its style
        /// </summary>
        public bool RecognizesAccessKey
        {
            get => (bool)GetValue(RecognizesAccessKeyProperty);
            set => SetValue(RecognizesAccessKeyProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            VisualStateManager.GoToState(this, TEMPLATE_STATE_NORMAL, false);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            var innerContent = (Content as FrameworkElement);

            double width, height;

            if (RippleAssist.GetIsCentered(this) && innerContent != null)
            {
                width = innerContent.ActualWidth;
                height = innerContent.ActualHeight;
            }
            else
            {
                width = sizeChangedEventArgs.NewSize.Width;
                height = sizeChangedEventArgs.NewSize.Height;
            }

            var radius = Math.Sqrt(Math.Pow(width, 2) + Math.Pow(height, 2));

            RippleSize = 2 * radius * RippleAssist.GetRippleSizeMultiplier(this);
        }
    }
}
