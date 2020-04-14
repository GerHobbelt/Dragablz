﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Dragablz.Core
{
    internal static class Native
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int X;
            public int Y;

            public static implicit operator System.Windows.Point(Point point)
            {
                return new System.Windows.Point(point.X, point.Y);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }   

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out Point lpPoint);

        public static Point GetRawCursorPos()
        {
            Point lpPoint;
            GetCursorPos(out lpPoint);
            return lpPoint;
        }

        public static System.Windows.Point GetCursorPos()
        {
            Point lpPoint;
            GetCursorPos(out lpPoint);
            return lpPoint;
        }

        [DllImport("User32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("user32.dll")]
        private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);

        public static System.Windows.Point ToWpf(this System.Windows.Point pixelPoint)
        {
            var desktop = GetDC(IntPtr.Zero); 
            var dpi = GetDeviceCaps(desktop, 88);
            ReleaseDC(IntPtr.Zero, desktop);

            var physicalUnitSize = 96d / dpi ;
            var wpfPoint = new System.Windows.Point(physicalUnitSize * pixelPoint.X, physicalUnitSize * pixelPoint.Y);

            return wpfPoint;
        }

        public static IEnumerable<Window> SortWindowsTopToBottom(IEnumerable<Window> windows)
        {
            var windowsByHandle = windows.Select(window =>
            {
                var hwndSource = PresentationSource.FromVisual(window) as HwndSource;
                var handle = hwndSource != null ? hwndSource.Handle : IntPtr.Zero;
                return new {window, handle};
            }).Where(x => x.handle != IntPtr.Zero)
                .ToDictionary(x => x.handle, x => x.window);

            for (var hWnd = GetTopWindow(IntPtr.Zero); hWnd != IntPtr.Zero; hWnd = GetWindow(hWnd, GW_HWNDNEXT))
                if (windowsByHandle.ContainsKey((hWnd)))
                    yield return windowsByHandle[hWnd];
        }

        public const int SW_SHOWNORMAL = 1;
        [DllImport("user32.dll")]
        public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref Windowplacement lpwndpl);

        private const uint GW_HWNDNEXT = 2;
        [DllImport("User32")]
        public static extern IntPtr GetTopWindow(IntPtr hWnd);

        [DllImport("User32")]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint wCmd);

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct Windowplacement
        {
            public int length;
            public int flags;
            public int showCmd;
            public Point minPosition;
            public Point maxPosition;
            public Rect normalPosition;
        }        

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, WindowMessage msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr PostMessage(IntPtr hWnd, WindowMessage msg, IntPtr wParam, IntPtr lParam);


        [DllImport("dwmapi.dll", EntryPoint = "#127")]
        internal static extern void DwmGetColorizationParameters(ref Dwmcolorizationparams dp);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Dwmcolorizationparams
        {
            public UInt32 ColorizationColor;
            public UInt32 ColorizationAfterglow;
            public UInt32 ColorizationColorBalance;
            public UInt32 ColorizationAfterglowBalance;
            public UInt32 ColorizationBlurBalance;
            public UInt32 ColorizationGlassReflectionIntensity;
            public UInt32 ColorizationOpaqueBlend;
        }

    }
}
