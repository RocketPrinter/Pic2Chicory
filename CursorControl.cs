using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Text;
using System.Threading;
using System.Windows.Forms;//<UseWindowsForms>true</UseWindowsForms> in project file
// also add <DisableWinExeOutputInference>true</DisableWinExeOutputInference> to fix the most stupid issue ever

namespace Pic2Chicory
{
    //pinvoke.net is great
    public static class CursorControl
    {
        #region Position
        [DllImport("User32.Dll")]
        public static extern long SetCursorPos(int x, int y);

        public static void SetCursorPos01(double x, double y)
        {
            var screenSize = Screen.PrimaryScreen.Bounds;
            SetCursorPos((int)(x * screenSize.Width), (int)(y * screenSize.Height));
        }

        //aka converts window/handle coordinates to screen cordinates
        //if it doesn't work make the method public
        [DllImport("User32.Dll")]
        static extern bool ClientToScreen(IntPtr hWnd, ref POINT point);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;

            public POINT(int X, int Y)
            {
                x = X;
                y = Y;
            }
        }
        #endregion Position

        #region Click
        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [Flags]
        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }
        public static void sendMouseLeftclick()
        {
            mouse_event((int)(MouseEventFlags.LEFTDOWN | MouseEventFlags.LEFTUP), 0, 0, 0, 0);
        }
        public static void sendMouseRightclick()
        {
            mouse_event((int)(MouseEventFlags.RIGHTDOWN | MouseEventFlags.RIGHTUP), 0, 0, 0, 0);
        }

        public static void sendMouseDoubleClick()
        {
            mouse_event((int)(MouseEventFlags.LEFTDOWN | MouseEventFlags.LEFTUP), 0, 0, 0, 0);

            Thread.Sleep(150);

            mouse_event((int)(MouseEventFlags.LEFTDOWN | MouseEventFlags.LEFTUP), 0, 0, 0, 0);
        }

        public static void sendMouseRightDoubleClick()
        {
            mouse_event((int)(MouseEventFlags.RIGHTDOWN | MouseEventFlags.RIGHTUP), 0, 0, 0, 0);

            Thread.Sleep(150);

            mouse_event((int)(MouseEventFlags.RIGHTDOWN | MouseEventFlags.RIGHTUP), 0, 0, 0, 0);
        }

        public static void sendMouseDown()
        {
            //mouse_event((int)MouseEventFlags.LEFTDOWN, 50, 50, 0, 0);
            mouse_event((int)MouseEventFlags.LEFTDOWN, 0, 0, 0, 0);
        }

        public static void sendMouseUp()
        {
            //mouse_event((int)MouseEventFlags.LEFTUP, 50, 50, 0, 0);
            mouse_event((int)MouseEventFlags.LEFTUP, 0, 0, 0, 0);
        }
        #endregion Click
    }
}
