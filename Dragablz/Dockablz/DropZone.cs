using System.Windows;
using System.Windows.Controls;

namespace Dragablz.Dockablz
{
    public class DropZone : Control
    {
        static DropZone()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropZone), new FrameworkPropertyMetadata(typeof(DropZone)));
        }

        public static readonly DependencyProperty LocationProperty = DependencyProperty.Register(
          nameof(Location), typeof(DropZoneLocation), typeof(DropZone), new PropertyMetadata(default(DropZoneLocation)));

        public DropZoneLocation Location
        {
            get => (DropZoneLocation)GetValue(LocationProperty);
            set => SetValue(LocationProperty, value);
        }

        private static readonly DependencyPropertyKey IS_OFFERED_PROPERTY_KEY =
            DependencyProperty.RegisterReadOnly(
                nameof(IsOffered), typeof(bool), typeof(DropZone),
                new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsOfferedProperty =
          IS_OFFERED_PROPERTY_KEY.DependencyProperty;

        public bool IsOffered
        {
            get => (bool)GetValue(IsOfferedProperty);
            internal set => SetValue(IS_OFFERED_PROPERTY_KEY, value);
        }
    }
}
