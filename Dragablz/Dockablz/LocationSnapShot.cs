﻿using System;
using System.Windows;

namespace Dragablz.Dockablz
{
    /// <summary>
    /// Initially needed to restore MDI dragablz items styles after a max then restore,
    /// as the trigger which binds the item width to the canvas width sets the  Width back to the default
    /// (e.g double.NaN) when the trigger is unset.  so we need to re-apply sizes manually
    /// </summary>
    internal class LocationSnapShot
    {
        private readonly double m_width;
        private readonly double m_height;

        public static LocationSnapShot Take(FrameworkElement frameworkElement)
        {
            if (frameworkElement == null) throw new ArgumentNullException(nameof(frameworkElement));

            return new LocationSnapShot(frameworkElement.Width, frameworkElement.Height);
        }

        private LocationSnapShot(double width, double height)
        {
            m_width = width;
            m_height = height;
        }

        public void Apply(FrameworkElement frameworkElement)
        {
            if (frameworkElement == null) throw new ArgumentNullException(nameof(frameworkElement));

            frameworkElement.SetCurrentValue(FrameworkElement.WidthProperty, m_width);
            frameworkElement.SetCurrentValue(FrameworkElement.HeightProperty, m_height);
        }
    }
}