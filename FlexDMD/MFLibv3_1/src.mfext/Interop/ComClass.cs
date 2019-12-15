/* license

ComClass.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Win32;

namespace MediaFoundation.Interop
{
    public static class ComClass
    {

        /// <summary>
        /// Verifiy by checking the Registery that a CLSID is registered. 
        /// </summary>
        /// <param name="classId">The COM CLSID to check.</param>
        /// <returns>True if the CLSID is likely registered; False otherwise.</returns>
        public static bool IsClassIdRegistered(Guid classId)
        {
            // Assume that if HKEY_CLASSES_ROOT\CLSID\{classId}\InprocServer32\(Default) exists and the dll path is valid, then the class have a good chance to be registered.
            using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(string.Format(CultureInfo.InvariantCulture, @"CLSID\{0}\InprocServer32", classId.ToString("B")), false))
            {
                if (key != null)
                {
                    string dllPath = (string)key.GetValue("", null);
                    return File.Exists(dllPath);
                }

                return false;
            }
        }

        /// <summary>
        /// Realease a COM instance and set the variable to null.
        /// </summary>
        /// <typeparam name="T">A COM Interface.</typeparam>
        /// <param name="obj">A COM instance to release and then set to null.</param>
        public static void SafeRelease<T>(ref T obj) where T : class
        {
            if (obj == null) return;

            if (!Marshal.IsComObject(obj))
                throw new ArgumentException("obj is not a COM object.", "obj");

            Marshal.ReleaseComObject(obj);
            obj = null;
        }

        /// <summary>
        /// Create an instance of a COM object designated by its CLSID.
        /// </summary>
        /// <param name="classId">The CLSID of the COM object to create.</param>
        /// <returns>An instance of the COM object</returns>
        /// <remarks>The COM object is created in the current thread apartment context. If the thread apartment is STA, the created object can only be used in the current thread.</remarks>
        public static object CreateInstance(Guid classId)
        {
            return Activator.CreateInstance(Type.GetTypeFromCLSID(classId));
        }

        /// <summary>
        /// Create an instance of a COM object designated by its CLSID.
        /// </summary>
        /// <typeparam name="T">A COM Interface implemented by the created COM object.</typeparam>
        /// <param name="classId">The CLSID of the COM object to create.</param>
        /// <returns>An instance of the requested COM interface</returns>
        public static T CreateInstance<T>(Guid classId) where T : class
        {
            if (!typeof(T).IsComInterface())
                throw new ArgumentException("T must designate a COM interface.", "T");

            return (T)Activator.CreateInstance(Type.GetTypeFromCLSID(classId));
        }

        /// <summary>
        /// Create an instance of a COM object from from a dll filename and its CLSID
        /// </summary>
        /// <param name="filePath">The dll file hosting the COM object.</param>
        /// <param name="classId">The CLSID of the COM object to create.</param>
        /// <returns>An instance of the COM object</returns>
        public static object CreateInstanceFromFile(string filePath, Guid classId)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");

            if (!File.Exists(filePath))
                throw new FileNotFoundException();

            return CreateInstanceFromFileInternal(filePath, classId, new Guid(0x00000000, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46)); // IID_IUnknown
        }

        /// <summary>
        /// Create an instance of a COM object from from a dll filename and its CLSID
        /// </summary>
        /// <typeparam name="T">A COM Interface implemented by the created COM object.</typeparam>
        /// <param name="filePath">The dll file hosting the COM object.</param>
        /// <param name="classId">The CLSID of the COM object to create.</param>
        /// <returns>An instance of the COM object</returns>
        public static T CreateInstanceFromFile<T>(string filePath, Guid classId) where T : class
        {
            if (!typeof(T).IsComInterface())
                throw new ArgumentException("T must designate a COM interface.", "T");

            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");

            if (!File.Exists(filePath))
                throw new FileNotFoundException();

            return (T)CreateInstanceFromFileInternal(filePath, classId, typeof(T).GUID);
        }

        private static object CreateInstanceFromFileInternal(string filePath, Guid classId, Guid iid)
        {
            IntPtr hModule, getClassObjectPtr, classFactoryPtr = IntPtr.Zero;
            IClassFactory classFactory = null;

            try
            {
                hModule = NativeMethods.LoadLibrary(filePath);
                if (hModule == IntPtr.Zero)
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

                getClassObjectPtr = NativeMethods.GetProcAddress(hModule, "DllGetClassObject");
                if (getClassObjectPtr == IntPtr.Zero)
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

                DllGetClassObjectSig DllGetClassObject = (DllGetClassObjectSig)Marshal.GetDelegateForFunctionPointer(getClassObjectPtr, typeof(DllGetClassObjectSig));

                HResult hr = DllGetClassObject(classId, typeof(IClassFactory).GUID, out classFactoryPtr);
                hr.ThrowExceptionOnError();

                classFactory = (IClassFactory)Marshal.GetObjectForIUnknown(classFactoryPtr);

                object result;

                hr = classFactory.CreateInstance(null, iid, out result);
                hr.ThrowExceptionOnError();

                return result;
            }
            finally
            {
                if (classFactory != null)
                {
                    int i = Marshal.ReleaseComObject(classFactory);
                    classFactory = null;
                }

                if (classFactoryPtr != IntPtr.Zero)
                {
                    int i = Marshal.Release(classFactoryPtr);
                    classFactoryPtr = IntPtr.Zero;
                }
            }
        }
    }

    internal static partial class NativeMethods
    {
        //[DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "LoadLibraryExW", SetLastError = true, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
        //private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, int dwFlags);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true), SuppressUnmanagedCodeSecurity]
        internal static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true), SuppressUnmanagedCodeSecurity]
        [return : MarshalAs(UnmanagedType.Bool)]
        internal static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true), SuppressUnmanagedCodeSecurity]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

    }

    internal delegate HResult DllGetClassObjectSig([In, MarshalAs(UnmanagedType.LPStruct)] Guid clsid, [In, MarshalAs(UnmanagedType.LPStruct)] Guid iid, out IntPtr obj);
}
