using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FlexDMD
{
    class WindowHandle
    {

        public WindowHandle(IntPtr rawPtr)
        {
            RawPtr = rawPtr;
        }

        public IntPtr RawPtr { get; }

        public bool IsVisible() => IsWindowVisible(RawPtr);

        public bool IsWindow() => IsWindow(RawPtr);

        public bool Destroy() => DestroyWindow(RawPtr);

        public int SendMessage(uint msg, int wParam, int lParam) => SendMessage(RawPtr, msg, wParam, lParam);

        public string GetClassName()
        {
            int size = 255;
            int actualSize = 0;
            StringBuilder builder;
            do
            {
                builder = new StringBuilder(size);
                actualSize = GetClassName(RawPtr, builder, builder.Capacity);
                size *= 2;
            } while (actualSize == size - 1);
            return builder.ToString();
        }

        public string GetWindowText()
        {
            int size = GetWindowTextLength(RawPtr);
            if (size > 0)
            {
                var builder = new StringBuilder(size + 1);
                GetWindowText(RawPtr, builder, builder.Capacity);
                return builder.ToString();
            }
            return String.Empty;
        }

        public static WindowHandle FindWindow(Predicate<WindowHandle> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            WindowHandle found = null;
            EnumWindows(delegate (IntPtr wnd, IntPtr param)
            {
                var window = new WindowHandle(wnd);
                if (callback.Invoke(window))
                {
                    found = window;
                    return EnumWindows_StopEnumerating;
                }

                return EnumWindows_ContinueEnumerating;
            },
                                      IntPtr.Zero);

            return found;
        }

        internal delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        public const bool EnumWindows_ContinueEnumerating = true;
        public const bool EnumWindows_StopEnumerating = false;

        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll")]
        internal static extern IntPtr FindWindow(string sClassName, string sAppName);

        [DllImport("user32.dll")]
        internal static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        internal static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);
    }
}
