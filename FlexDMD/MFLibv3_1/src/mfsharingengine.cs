/* license

mfsharingengine.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

using System;
using System.Runtime.InteropServices;

using MediaFoundation.Misc;
using MediaFoundation.EVR;

namespace MediaFoundation
{
#if ALLOW_UNTESTED_INTERFACES

    #region Declarations

    [UnmanagedName("MF_SHARING_ENGINE_EVENT")]
    public enum MF_SHARING_ENGINE_EVENT
    {
        Disconnect = 2000,
        LocalRenderingStarted = 2001,
        LocalRenderingEnded = 2002,
        Stopped = 2003,
        Error = 2501,
    }

    [UnmanagedName("MF_MEDIA_SHARING_ENGINE_EVENT")]
    public enum MF_MEDIA_SHARING_ENGINE_EVENT
    {
        Disconnect = 2000
    }

    [Flags, UnmanagedName("PLAYTO_SOURCE_CREATEFLAGS")]
    public enum PLAYTO_SOURCE_CREATEFLAGS
    {
        None = 0x0,
        Image = 0x1,
        Audio = 0x2,
        Video = 0x4,
        Protected = 0x8,
    }

    [StructLayout(LayoutKind.Sequential), UnmanagedName("DEVICE_INFO")]
    public struct DEVICE_INFO
    {
        [MarshalAs(UnmanagedType.BStr)]
        string pFriendlyDeviceName;

        [MarshalAs(UnmanagedType.BStr)]
        string pUniqueDeviceName;

        [MarshalAs(UnmanagedType.BStr)]
        string pManufacturerName;

        [MarshalAs(UnmanagedType.BStr)]
        string pModelName;

        [MarshalAs(UnmanagedType.BStr)]
        string pIconURL;
    }

    #endregion

    #region Interfaces

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("2BA61F92-8305-413B-9733-FAF15F259384"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFSharingEngineClassFactory
    {
        [PreserveSig]
        HResult CreateInstance(
            MF_MEDIA_ENGINE_CREATEFLAGS dwFlags,
            IMFAttributes pAttr,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppEngine
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("8D3CE1BF-2367-40E0-9EEE-40D377CC1B46"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaSharingEngine : IMFMediaEngine
    {
        #region IMFMediaEngine methods

        [PreserveSig]
        new HResult GetError(
            out IMFMediaError ppError
            );

        [PreserveSig]
        new HResult SetErrorCode(
            MF_MEDIA_ENGINE_ERR error
            );

        [PreserveSig]
        new HResult SetSourceElements(
            IMFMediaEngineSrcElements pSrcElements
            );

        [PreserveSig]
        new HResult SetSource(
            [MarshalAs(UnmanagedType.BStr)] string pUrl
            );

        [PreserveSig]
        new HResult GetCurrentSource(
            [MarshalAs(UnmanagedType.BStr)] out string ppUrl
            );

        [PreserveSig]
        new MF_MEDIA_ENGINE_NETWORK GetNetworkState();

        [PreserveSig]
        new MF_MEDIA_ENGINE_PRELOAD GetPreload();

        [PreserveSig]
        new HResult SetPreload(
            MF_MEDIA_ENGINE_PRELOAD Preload
            );

        [PreserveSig]
        new HResult GetBuffered(
            out IMFMediaTimeRange ppBuffered
            );

        [PreserveSig]
        new HResult Load();

        [PreserveSig]
        new HResult CanPlayType(
            [MarshalAs(UnmanagedType.BStr)] string type,
            out MF_MEDIA_ENGINE_CANPLAY pAnswer
            );

        [PreserveSig]
        new MF_MEDIA_ENGINE_READY GetReadyState();

        [return: MarshalAs(UnmanagedType.Bool)]
        new bool IsSeeking();

        [PreserveSig]
        new double GetCurrentTime();

        [PreserveSig]
        new HResult SetCurrentTime(
            double seekTime
            );

        [PreserveSig]
        new double GetStartTime();

        [PreserveSig]
        new double GetDuration();

        [return: MarshalAs(UnmanagedType.Bool)]
        new bool IsPaused();

        [PreserveSig]
        new double GetDefaultPlaybackRate();

        [PreserveSig]
        new HResult SetDefaultPlaybackRate(
            double Rate
            );

        [PreserveSig]
        new double GetPlaybackRate();

        [PreserveSig]
        new HResult SetPlaybackRate(
            double Rate
            );

        [PreserveSig]
        new HResult GetPlayed(
            out IMFMediaTimeRange ppPlayed
            );

        [PreserveSig]
        new HResult GetSeekable(
            out IMFMediaTimeRange ppSeekable
            );

        [return: MarshalAs(UnmanagedType.Bool)]
        new bool IsEnded();

        [return: MarshalAs(UnmanagedType.Bool)]
        new bool GetAutoPlay();

        [PreserveSig]
        new HResult SetAutoPlay(
            [MarshalAs(UnmanagedType.Bool)] bool AutoPlay
            );

        [return: MarshalAs(UnmanagedType.Bool)]
        new bool GetLoop();

        [PreserveSig]
        new HResult SetLoop(
            [MarshalAs(UnmanagedType.Bool)] bool Loop
            );

        [PreserveSig]
        new HResult Play();

        [PreserveSig]
        new HResult Pause();

        [return: MarshalAs(UnmanagedType.Bool)]
        new bool GetMuted();

        [PreserveSig]
        new HResult SetMuted(
            [MarshalAs(UnmanagedType.Bool)] bool Muted
            );

        [PreserveSig]
        new double GetVolume();

        [PreserveSig]
        new HResult SetVolume(
            double Volume
            );

        [return: MarshalAs(UnmanagedType.Bool)]
        new bool HasVideo();

        [return: MarshalAs(UnmanagedType.Bool)]
        new bool HasAudio();

        [PreserveSig]
        new HResult GetNativeVideoSize(
            out int cx,
            out int cy
            );

        [PreserveSig]
        new HResult GetVideoAspectRatio(
            out int cx,
            out int cy
            );

        [PreserveSig]
        new HResult Shutdown();

        [PreserveSig]
        new HResult TransferVideoFrame(
            [In, MarshalAs(UnmanagedType.IUnknown)] object pDstSurf,
            [In] MFVideoNormalizedRect pSrc,
            [In] MFRect pDst,
            [In] MFARGB pBorderClr
            );

        [PreserveSig]
        new HResult OnVideoStreamTick(
            out long pPts
            );

        #endregion

        [PreserveSig]
        HResult GetDevice(
            out DEVICE_INFO pDevice
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("524D2BC4-B2B1-4FE5-8FAC-FA4E4512B4E0"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaSharingEngineClassFactory
    {
        [PreserveSig]
        HResult CreateInstance(
            int dwFlags,
            IMFAttributes pAttr,
            out IMFMediaSharingEngine ppEngine
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("CFA0AE8E-7E1C-44D2-AE68-FC4C148A6354"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFImageSharingEngine
    {
        [PreserveSig]
        HResult SetSource(
            [MarshalAs(UnmanagedType.IUnknown)] object pStream
            );

        [PreserveSig]
        HResult GetDevice(
            out DEVICE_INFO pDevice
            );

        [PreserveSig]
        HResult Shutdown();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("1FC55727-A7FB-4FC8-83AE-8AF024990AF1"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFImageSharingEngineClassFactory
    {
        [PreserveSig]
        HResult CreateInstanceFromUDN(
            [MarshalAs(UnmanagedType.BStr)]
            string pUniqueDeviceName,
            out IMFImageSharingEngine ppEngine
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("607574EB-F4B6-45C1-B08C-CB715122901D"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPlayToControl
    {
        [PreserveSig]
        HResult Connect(
            IMFSharingEngineClassFactory pFactory
            );

        [PreserveSig]
        HResult Disconnect();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("842B32A3-9B9B-4D1C-B3F3-49193248A554"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPlayToSourceClassFactory
    {
        [PreserveSig]
        HResult CreateInstance(
            PLAYTO_SOURCE_CREATEFLAGS dwFlags,
            IPlayToControl pControl,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppSource
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("AA9DD80F-C50A-4220-91C1-332287F82A34"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPlayToControlWithCapabilities : IPlayToControl
    {
        #region IPlayToControl methods

        [PreserveSig]
        new HResult Connect(
            IMFSharingEngineClassFactory pFactory
            );

        [PreserveSig]
        new HResult Disconnect();

        #endregion

        [PreserveSig]
        HResult GetCapabilities(
            out PLAYTO_SOURCE_CREATEFLAGS pCapabilities
            );
    }
    #endregion

#endif
}
