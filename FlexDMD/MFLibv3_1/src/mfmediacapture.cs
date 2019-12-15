/* license

mfmediacapture.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

using System;
using System.Runtime.InteropServices;

using MediaFoundation.Misc;

namespace MediaFoundation
{
    #region Interfaces

#if ALLOW_UNTESTED_INTERFACES

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("3DE21209-8BA6-4f2a-A577-2819B56FF14D"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAdvancedMediaCaptureInitializationSettings
    {
        [PreserveSig]
        HResult SetDirectxDeviceManager(
            IMFDXGIDeviceManager value
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("24E0485F-A33E-4aa1-B564-6019B1D14F65"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAdvancedMediaCaptureSettings
    {
        [PreserveSig]
        HResult GetDirectxDeviceManager(
            out IMFDXGIDeviceManager value
            );

        [PreserveSig]
        HResult SetDirectCompositionVisual(
            [MarshalAs(UnmanagedType.IUnknown)] object value
            );

        [PreserveSig]
        HResult UpdateVideo(
             [In] MfFloat pSrcNormalizedTop,
             [In] MfFloat pSrcNormalizedBottom,
             [In] MfFloat pSrcNormalizedRight,
             [In] MfFloat pSrcNormalizedLeft,
             [In] MFRect pDstSize,
             [In] MFInt pBorderClr
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("D0751585-D216-4344-B5BF-463B68F977BB"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAdvancedMediaCapture
    {
        [PreserveSig]
        HResult GetAdvancedMediaCaptureSettings(
            out IAdvancedMediaCaptureSettings value
            );
    }

#endif

    #endregion
}
