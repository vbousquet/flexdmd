/* license

IClassFactory.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;
using System.Runtime.InteropServices;

namespace MediaFoundation.Interop
{
    [ComImport, Guid("00000001-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IClassFactory
    {
        [PreserveSig]
        HResult CreateInstance([In, MarshalAs(UnmanagedType.Interface)] object pUnkOuter, [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out object ppvObject);

        [PreserveSig]
        HResult LockServer([In, MarshalAs(UnmanagedType.Bool)] bool fLock);
    }
}
