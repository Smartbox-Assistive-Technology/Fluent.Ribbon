namespace Fluent.Helpers
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interop;
    using ControlzEx.Native;
    using ControlzEx.Standard;

    /// <summary>
    /// Class which offers helper methods for steering the window
    /// </summary>
    public static class WindowSteeringHelper
    {
        /// <summary>
        /// Shows the system menu at the current mouse position.
        /// </summary>
        /// <param name="e">The mouse event args.</param>
        /// <param name="handleDragMove">Defines if window dragging should be handled.</param>
        /// <param name="handleStateChange">Defines if window state changes should be handled.</param>
        public static void HandleMouseLeftButtonDown(MouseButtonEventArgs e, bool handleDragMove, bool handleStateChange)
        {
            var dependencyObject = e.Source as DependencyObject;

            if (dependencyObject is null)
            {
                return;
            }

            HandleMouseLeftButtonDown(dependencyObject, e, handleDragMove, handleStateChange);
        }

#pragma warning disable 618
        /// <summary>
        /// Shows the system menu at the current mouse position.
        /// </summary>
        /// <param name="dependencyObject">The object which was the source of the mouse event.</param>
        /// <param name="e">The mouse event args.</param>
        /// <param name="handleDragMove">Defines if window dragging should be handled.</param>
        /// <param name="handleStateChange">Defines if window state changes should be handled.</param>
        public static void HandleMouseLeftButtonDown(DependencyObject dependencyObject, MouseButtonEventArgs e, bool handleDragMove, bool handleStateChange)
        {
            var window = Window.GetWindow(dependencyObject);

            if (window is null)
            {
                return;
            }

            if (handleDragMove
                && e.ClickCount == 1)
            {
                e.Handled = true;

                // taken from DragMove internal code
                window.VerifyAccess();

                // for the touch usage
                UnsafeNativeMethods.ReleaseCapture();

                // Avoid private-reflection access to Window.CriticalHandle, which throws NullReferenceException
                // on .NET 10 because the underlying field is no longer set when the property is read indirectly.
                // WindowInteropHelper.Handle returns the same HWND for an opened window.
                var handle = new WindowInteropHelper(window).Handle;
#pragma warning disable 618
                if (handle != IntPtr.Zero)
                {
                    // these lines are from DragMove
                    // NativeMethods.SendMessage(handle, WM.SYSCOMMAND, (IntPtr)SC.MOUSEMOVE, IntPtr.Zero);
                    // NativeMethods.SendMessage(handle, WM.LBUTTONUP, IntPtr.Zero, IntPtr.Zero);

                    var wpfPoint = window.PointToScreen(Mouse.GetPosition(window));
                    var x = (int)wpfPoint.X;
                    var y = (int)wpfPoint.Y;
                    NativeMethods.SendMessage(handle, WM.NCLBUTTONDOWN, (IntPtr)HT.CAPTION, new IntPtr(x | (y << 16)));
                }
            }
            else if (handleStateChange
                && e.ClickCount == 2
                && (window.ResizeMode == ResizeMode.CanResize || window.ResizeMode == ResizeMode.CanResizeWithGrip))
            {
                e.Handled = true;

                if (window.WindowState == WindowState.Normal)
                {
                    ControlzEx.Windows.Shell.SystemCommands.MaximizeWindow(window);
                }
                else
                {
                    ControlzEx.Windows.Shell.SystemCommands.RestoreWindow(window);
                }
            }
        }

#pragma warning restore 618

        /// <summary>
        /// Shows the system menu at the current mouse position.
        /// </summary>
        /// <param name="dependencyObject">The object which was the source of the mouse event.</param>
        /// <param name="e">The mouse event args.</param>
        public static void ShowSystemMenu(DependencyObject dependencyObject, MouseButtonEventArgs e)
        {
            var window = Window.GetWindow(dependencyObject);

            if (window is null)
            {
                return;
            }

            ShowSystemMenu(window, e);
        }

        /// <summary>
        /// Shows the system menu at the current mouse position.
        /// </summary>
        /// <param name="window">The window for which the system menu should be shown.</param>
        /// <param name="e">The mouse event args.</param>
        public static void ShowSystemMenu(Window window, MouseButtonEventArgs e)
        {
            e.Handled = true;

#pragma warning disable 618
            ControlzEx.Windows.Shell.SystemCommands.ShowSystemMenu(window, e);
#pragma warning restore 618
        }

        /// <summary>
        /// Shows the system menu at <paramref name="screenLocation"/>.
        /// </summary>
        /// <param name="window">The window for which the system menu should be shown.</param>
        /// <param name="screenLocation">The location at which the system menu should be shown.</param>
        public static void ShowSystemMenu(Window window, Point screenLocation)
        {
#pragma warning disable 618
            ControlzEx.Windows.Shell.SystemCommands.ShowSystemMenuPhysicalCoordinates(window, screenLocation);
#pragma warning restore 618
        }
    }
}