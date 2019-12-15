/* Copyright 2019 Vincent Bousquet

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
   */
using NLog;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace FlexDMD
{
    public static class NativeLibrary
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);
    }

    public class DMDDevice
    {
        // see https://github.com/Studiofreya/code-samples/blob/master/samples/dynamic-loading-of-native-dlls/NativeConsumer/Program.cs
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int OpenCloseDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void RenderDelegate(ushort width, ushort height, IntPtr currbuffer);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void GameSettingsDelegate(string gameName, ulong hardwareGeneration, IntPtr options);

        IntPtr _dllhandle = IntPtr.Zero;
        OpenCloseDelegate _open = null;
        OpenCloseDelegate _close = null;
        RenderDelegate _renderRgb24 = null;
        RenderDelegate _renderGray4 = null;
        RenderDelegate _renderGray2 = null;
        GameSettingsDelegate _gameSettings = null;

        public DMDDevice()
        {
            string libraryName = "dmddevice.dll";
            if (System.Environment.Is64BitProcess) libraryName = "dmddevice64.dll";
            var fullPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), libraryName);
            _dllhandle = NativeLibrary.LoadLibrary(fullPath);
            if (_dllhandle != IntPtr.Zero)
            {
                var openHandle = NativeLibrary.GetProcAddress(_dllhandle, "Open");
                if (openHandle != IntPtr.Zero) _open = (OpenCloseDelegate)Marshal.GetDelegateForFunctionPointer(openHandle, typeof(OpenCloseDelegate));
                var closeHandle = NativeLibrary.GetProcAddress(_dllhandle, "Close");
                if (closeHandle != IntPtr.Zero) _close = (OpenCloseDelegate)Marshal.GetDelegateForFunctionPointer(closeHandle, typeof(OpenCloseDelegate));
                var renderGray2Handle = NativeLibrary.GetProcAddress(_dllhandle, "Render_4_Shades");
                if (renderGray2Handle != IntPtr.Zero) _renderGray2 = (RenderDelegate)Marshal.GetDelegateForFunctionPointer(renderGray2Handle, typeof(RenderDelegate));
                var renderGray4Handle = NativeLibrary.GetProcAddress(_dllhandle, "Render_16_Shades");
                if (renderGray4Handle != IntPtr.Zero) _renderGray4 = (RenderDelegate)Marshal.GetDelegateForFunctionPointer(renderGray4Handle, typeof(RenderDelegate));
                var renderRgb24Handle = NativeLibrary.GetProcAddress(_dllhandle, "Render_RGB24");
                if (renderRgb24Handle != IntPtr.Zero) _renderRgb24 = (RenderDelegate)Marshal.GetDelegateForFunctionPointer(renderRgb24Handle, typeof(RenderDelegate));
                var gameSettingsHandle = NativeLibrary.GetProcAddress(_dllhandle, "PM_GameSettings");
                if (gameSettingsHandle != IntPtr.Zero) _gameSettings = (GameSettingsDelegate)Marshal.GetDelegateForFunctionPointer(gameSettingsHandle, typeof(GameSettingsDelegate));
            }
            else
            {
                LogManager.GetCurrentClassLogger().Error("Failed to load {0} in {1}", libraryName, fullPath);
            }
        }

        ~DMDDevice()
        {
            if (_dllhandle != IntPtr.Zero) NativeLibrary.FreeLibrary(_dllhandle);
        }

        public int Open()
        {
            if (_open != null) return _open();
            return 0;
        }

        public int Close()
        {
            if (_close != null) return _close();
            return 0;
        }

        public void RenderRgb24(ushort width, ushort height, IntPtr currbuffer)
        {
            if (_renderRgb24 != null) _renderRgb24(width, height, currbuffer);
        }

        public void RenderGray4(ushort width, ushort height, IntPtr currbuffer)
        {
            if (_renderGray4 != null) _renderGray4(width, height, currbuffer);
        }

        public void RenderGray2(ushort width, ushort height, IntPtr currbuffer)
        {
            if (_renderGray2 != null) _renderGray2(width, height, currbuffer);
        }

        public void GameSettings(string gameName, ulong hardwareGeneration, PMoptions options)
        {
            if (_gameSettings != null)
            {
                IntPtr ptr = Marshal.AllocHGlobal(19 * sizeof(int));
                Marshal.StructureToPtr(options, ptr, true);
                _gameSettings(gameName, hardwareGeneration, ptr);
                Marshal.FreeHGlobal(ptr);
            }
        }

        /*
        [DllImport("dmddevice", EntryPoint = "Open", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Open();

        [DllImport("dmddevice", EntryPoint = "Close", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Close();

        [DllImport("dmddevice", EntryPoint = "PM_GameSettings", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GameSettings(string gameName, ulong hardwareGeneration, IntPtr options);

        [DllImport("dmddevice", EntryPoint = "Console_Data", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ConsoleData(byte data);

        [DllImport("dmddevice", EntryPoint = "Render_RGB24", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RenderRgb24(ushort width, ushort height, IntPtr currbuffer);

        [DllImport("dmddevice", EntryPoint = "Render_16_Shades", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RenderGray4(ushort width, ushort height, IntPtr currbuffer);

        [DllImport("dmddevice", EntryPoint = "Render_4_Shades", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RenderGray2(ushort width, ushort height, IntPtr currbuffer);

        [DllImport("dmddevice", EntryPoint = "Render_PM_Alphanumeric_Frame", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RenderAlphaNum(NumericalLayout numericalLayout, IntPtr seg_data, IntPtr seg_data2);

        [DllImport("dmddevice", EntryPoint = "Set_4_Colors_Palette", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetGray2Palette(Rgb24 color0, Rgb24 color33, Rgb24 color66, Rgb24 color100);

        [DllImport("dmddevice", EntryPoint = "Set_16_Colors_Palette", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetGray4Palette(IntPtr palette);*/

        public enum NumericalLayout
        {
            None,
            __2x16Alpha,
            __2x20Alpha,
            __2x7Alpha_2x7Num,
            __2x7Alpha_2x7Num_4x1Num,
            __2x7Num_2x7Num_4x1Num,
            __2x7Num_2x7Num_10x1Num,
            __2x7Num_2x7Num_4x1Num_gen7,
            __2x7Num10_2x7Num10_4x1Num,
            __2x6Num_2x6Num_4x1Num,
            __2x6Num10_2x6Num10_4x1Num,
            __4x7Num10,
            __6x4Num_4x1Num,
            __2x7Num_4x1Num_1x16Alpha,
            __1x16Alpha_1x16Num_1x7Num
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PMoptions
        {
            public int Red, Green, Blue;
            public int Perc66, Perc33, Perc0;
            public int DmdOnly, Compact, Antialias;
            public int Colorize;
            public int Red66, Green66, Blue66;
            public int Red33, Green33, Blue33;
            public int Red0, Green0, Blue0;
        }

        [StructLayout(LayoutKind.Sequential), Serializable]
        public struct Rgb24
        {
            public char Red;
            public char Green;
            public char Blue;
        }
    }
}
