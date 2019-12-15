/* license

MFPlay.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Drawing;

using MediaFoundation.Misc;
using MediaFoundation.EVR;

namespace MediaFoundation.MFPlayer
{
    #region Declarations

    [Flags, UnmanagedName("MFP_CREATION_OPTIONS")]
    public enum MFP_CREATION_OPTIONS
    {
        None = 0x00000000,
        FreeThreadedCallback = 0x00000001,
        NoMMCSS = 0x00000002,
        NoRemoteDesktopOptimization = 0x00000004
    }

    [UnmanagedName("MFP_MEDIAPLAYER_STATE")]
    public enum MFP_MEDIAPLAYER_STATE
    {
        Empty = 0x00000000,
        Stopped = 0x00000001,
        Playing = 0x00000002,
        Paused = 0x00000003,
        Shutdown = 0x00000004
    }

    [Flags, UnmanagedName("MFP_MEDIAITEM_CHARACTERISTICS")]
    public enum MFP_MEDIAITEM_CHARACTERISTICS
    {
        None = 0x00000000,
        IsLive = 0x00000001,
        CanSeek = 0x00000002,
        CanPause = 0x00000004,
        HasSlowSeek = 0x00000008
    }

    [Flags, UnmanagedName("MFP_CREDENTIAL_FLAGS")]
    public enum MFP_CREDENTIAL_FLAGS
    {
        None = 0x00000000,
        Prompt = 0x00000001,
        Save = 0x00000002,
        DoNotCache = 0x00000004,
        ClearText = 0x00000008,
        Proxy = 0x00000010,
        LoggedOnUser = 0x00000020
    }

    [UnmanagedName("MFP_EVENT_TYPE")]
    public enum MFP_EVENT_TYPE
    {
        Play = 0,
        Pause = 1,
        Stop = 2,
        PositionSet = 3,
        RateSet = 4,
        MediaItemCreated = 5,
        MediaItemSet = 6,
        FrameStep = 7,
        MediaItemCleared = 8,
        MF = 9,
        Error = 10,
        PlaybackEnded = 11,
        AcquireUserCredential = 12
    }

    [StructLayout(LayoutKind.Sequential), UnmanagedName("MFP_EVENT_HEADER")]
    public class MFP_EVENT_HEADER
    {
        public MFP_EVENT_TYPE eEventType;
        public HResult hrEvent;
        public IMFPMediaPlayer pMediaPlayer;
        public MFP_MEDIAPLAYER_STATE eState;
        public IPropertyStore pPropertyStore;

        public IntPtr GetPtr()
        {
            IntPtr ip;

            int iSize = Marshal.SizeOf(this);

            ip = Marshal.AllocCoTaskMem(iSize);
            Marshal.StructureToPtr(this, ip, false);

            return ip;
        }

        public static MFP_EVENT_HEADER PtrToEH(IntPtr pNativeData)
        {
            MFP_EVENT_TYPE met = (MFP_EVENT_TYPE)Marshal.ReadInt32(pNativeData);
            object mce;

            switch (met)
            {
                case MFP_EVENT_TYPE.Play:
                    mce = Marshal.PtrToStructure(pNativeData, typeof(MFP_PLAY_EVENT));
                    break;
                case MFP_EVENT_TYPE.Pause:
                    mce = Marshal.PtrToStructure(pNativeData, typeof(MFP_PAUSE_EVENT));
                    break;
                case MFP_EVENT_TYPE.Stop:
                    mce = Marshal.PtrToStructure(pNativeData, typeof(MFP_STOP_EVENT));
                    break;
                case MFP_EVENT_TYPE.PositionSet:
                    mce = Marshal.PtrToStructure(pNativeData, typeof(MFP_POSITION_SET_EVENT));
                    break;
                case MFP_EVENT_TYPE.RateSet:
                    mce = Marshal.PtrToStructure(pNativeData, typeof(MFP_RATE_SET_EVENT));
                    break;
                case MFP_EVENT_TYPE.MediaItemCreated:
                    mce = Marshal.PtrToStructure(pNativeData, typeof(MFP_MEDIAITEM_CREATED_EVENT));
                    break;
                case MFP_EVENT_TYPE.MediaItemSet:
                    mce = Marshal.PtrToStructure(pNativeData, typeof(MFP_RATE_SET_EVENT));
                    break;
                case MFP_EVENT_TYPE.FrameStep:
                    mce = Marshal.PtrToStructure(pNativeData, typeof(MFP_FRAME_STEP_EVENT));
                    break;
                case MFP_EVENT_TYPE.MediaItemCleared:
                    mce = Marshal.PtrToStructure(pNativeData, typeof(MFP_MEDIAITEM_CLEARED_EVENT));
                    break;
                case MFP_EVENT_TYPE.MF:
                    mce = Marshal.PtrToStructure(pNativeData, typeof(MFP_MF_EVENT));
                    break;
                case MFP_EVENT_TYPE.Error:
                    mce = Marshal.PtrToStructure(pNativeData, typeof(MFP_ERROR_EVENT));
                    break;
                case MFP_EVENT_TYPE.PlaybackEnded:
                    mce = Marshal.PtrToStructure(pNativeData, typeof(MFP_PLAYBACK_ENDED_EVENT));
                    break;

                case MFP_EVENT_TYPE.AcquireUserCredential:
                    mce = Marshal.PtrToStructure(pNativeData, typeof(MFP_ACQUIRE_USER_CREDENTIAL_EVENT));
                    break;

                default:
                    // Don't know what it is.  Send back the header.
                    mce = Marshal.PtrToStructure(pNativeData, typeof(MFP_EVENT_HEADER));
                    break;
            }

            return mce as MFP_EVENT_HEADER;
        }
    }

    [StructLayout(LayoutKind.Sequential), UnmanagedName("MFP_PLAY_EVENT")]
    public class MFP_PLAY_EVENT : MFP_EVENT_HEADER
    {
        public IMFPMediaItem pMediaItem;
    }

    [StructLayout(LayoutKind.Sequential), UnmanagedName("MFP_PAUSE_EVENT")]
    public class MFP_PAUSE_EVENT : MFP_EVENT_HEADER
    {
        public IMFPMediaItem pMediaItem;
    }

    [StructLayout(LayoutKind.Sequential), UnmanagedName("MFP_STOP_EVENT")]
    public class MFP_STOP_EVENT : MFP_EVENT_HEADER
    {
        public IMFPMediaItem pMediaItem;
    }

    [StructLayout(LayoutKind.Sequential), UnmanagedName("MFP_POSITION_SET_EVENT")]
    public class MFP_POSITION_SET_EVENT : MFP_EVENT_HEADER
    {
        public IMFPMediaItem pMediaItem;
    }

    [StructLayout(LayoutKind.Sequential), UnmanagedName("MFP_RATE_SET_EVENT")]
    public class MFP_RATE_SET_EVENT : MFP_EVENT_HEADER
    {
        public IMFPMediaItem pMediaItem;
        public float flRate;
    }

    [StructLayout(LayoutKind.Sequential), UnmanagedName("MFP_MEDIAITEM_CREATED_EVENT")]
    public class MFP_MEDIAITEM_CREATED_EVENT : MFP_EVENT_HEADER
    {
        public IMFPMediaItem pMediaItem;
        public IntPtr dwUserData;
    }

    [StructLayout(LayoutKind.Sequential), UnmanagedName("MFP_MEDIAITEM_SET_EVENT")]
    public class MFP_MEDIAITEM_SET_EVENT : MFP_EVENT_HEADER
    {
        public IMFPMediaItem pMediaItem;
    }

    [StructLayout(LayoutKind.Sequential), UnmanagedName("MFP_FRAME_STEP_EVENT")]
    public class MFP_FRAME_STEP_EVENT : MFP_EVENT_HEADER
    {
        public IMFPMediaItem pMediaItem;
    }

    [StructLayout(LayoutKind.Sequential), UnmanagedName("MFP_MEDIAITEM_CLEARED_EVENT")]
    public class MFP_MEDIAITEM_CLEARED_EVENT : MFP_EVENT_HEADER
    {
        public IMFPMediaItem pMediaItem;
    }

    [StructLayout(LayoutKind.Sequential), UnmanagedName("MFP_MF_EVENT")]
    public class MFP_MF_EVENT : MFP_EVENT_HEADER
    {
        public MediaEventType MFEventType;
        public IMFMediaEvent pMFMediaEvent;
        public IMFPMediaItem pMediaItem;
    }

    [StructLayout(LayoutKind.Sequential), UnmanagedName("MFP_ERROR_EVENT")]
    public class MFP_ERROR_EVENT : MFP_EVENT_HEADER
    {
    }

    [StructLayout(LayoutKind.Sequential), UnmanagedName("MFP_PLAYBACK_ENDED_EVENT")]
    public class MFP_PLAYBACK_ENDED_EVENT : MFP_EVENT_HEADER
    {
        public IMFPMediaItem pMediaItem;
    }

    [StructLayout(LayoutKind.Sequential), UnmanagedName("MFP_ACQUIRE_USER_CREDENTIAL_EVENT")]
    public class MFP_ACQUIRE_USER_CREDENTIAL_EVENT : MFP_EVENT_HEADER
    {
        public IntPtr dwUserData;
        [MarshalAs(UnmanagedType.Bool)]
        public bool fProceedWithAuthentication;
        public HResult hrAuthenticationStatus;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pwszURL;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pwszSite;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pwszRealm;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pwszPackage;
        public int nRetries;
        public MFP_CREDENTIAL_FLAGS flags;
        public IMFNetCredential pCredential;
    }

    #endregion

    #region Interfaces

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("A714590A-58AF-430a-85BF-44F5EC838D85")]
    public interface IMFPMediaPlayer
    {
        [PreserveSig]
        HResult Play();

        [PreserveSig]
        HResult Pause();

        [PreserveSig]
        HResult Stop();

        [PreserveSig]
        HResult FrameStep();

        [PreserveSig]
        HResult SetPosition(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidPositionType,
            [In, MarshalAs(UnmanagedType.LPStruct)] ConstPropVariant pvPositionValue
        );

        [PreserveSig]
        HResult GetPosition(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidPositionType,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalCookie = "IMFPMediaPlayer.GetPosition", MarshalTypeRef = typeof(PVMarshaler))] PropVariant pvPositionValue
        );

        [PreserveSig]
        HResult GetDuration(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidPositionType,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalCookie = "IMFPMediaPlayer.GetDuration", MarshalTypeRef = typeof(PVMarshaler))] PropVariant pvPositionValue
        );

        [PreserveSig]
        HResult SetRate(
            float flRate
        );

        [PreserveSig]
        HResult GetRate(
            out float pflRate
        );

        [PreserveSig]
        HResult GetSupportedRates(
            [MarshalAs(UnmanagedType.Bool)] bool fForwardDirection,
            out float pflSlowestRate,
            out float pflFastestRate
        );

        [PreserveSig]
        HResult GetState(
            out MFP_MEDIAPLAYER_STATE peState
        );

        [PreserveSig]
        HResult CreateMediaItemFromURL(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pwszURL,
            [MarshalAs(UnmanagedType.Bool)] bool fSync,
            IntPtr dwUserData,
            out IMFPMediaItem ppMediaItem
        );

        [PreserveSig]
        HResult CreateMediaItemFromObject(
            [MarshalAs(UnmanagedType.IUnknown)] object pIUnknownObj,
            [MarshalAs(UnmanagedType.Bool)] bool fSync,
            IntPtr dwUserData,
            out IMFPMediaItem ppMediaItem
        );

        [PreserveSig]
        HResult SetMediaItem(
            IMFPMediaItem pIMFPMediaItem
        );

        [PreserveSig]
        HResult ClearMediaItem();

        [PreserveSig]
        HResult GetMediaItem(
            out IMFPMediaItem ppIMFPMediaItem
        );

        [PreserveSig]
        HResult GetVolume(
            out float pflVolume
        );

        [PreserveSig]
        HResult SetVolume(
            float flVolume
        );

        [PreserveSig]
        HResult GetBalance(
            out float pflBalance
        );

        [PreserveSig]
        HResult SetBalance(
            float flBalance
        );

        [PreserveSig]
        HResult GetMute(
            [MarshalAs(UnmanagedType.Bool)] out bool pfMute
        );

        [PreserveSig]
        HResult SetMute(
            [MarshalAs(UnmanagedType.Bool)] bool fMute
        );

        [PreserveSig]
        HResult GetNativeVideoSize(
            [Out, MarshalAs(UnmanagedType.LPStruct)] MFSize pszVideo,
            [Out, MarshalAs(UnmanagedType.LPStruct)] MFSize pszARVideo
        );

        [PreserveSig]
        HResult GetIdealVideoSize(
            [Out, MarshalAs(UnmanagedType.LPStruct)] MFSize pszMin,
            [Out, MarshalAs(UnmanagedType.LPStruct)] MFSize pszMax
        );

        [PreserveSig]
        HResult SetVideoSourceRect(
            [In, MarshalAs(UnmanagedType.LPStruct)] MFVideoNormalizedRect pnrcSource
        );

        [PreserveSig]
        HResult GetVideoSourceRect(
            [Out, MarshalAs(UnmanagedType.LPStruct)] MFVideoNormalizedRect pnrcSource
        );

        [PreserveSig]
        HResult SetAspectRatioMode(
            MFVideoAspectRatioMode dwAspectRatioMode
        );

        [PreserveSig]
        HResult GetAspectRatioMode(
            out MFVideoAspectRatioMode pdwAspectRatioMode
        );

        [PreserveSig]
        HResult GetVideoWindow(
            out IntPtr phwndVideo
        );

        [PreserveSig]
        HResult UpdateVideo();

        [PreserveSig]
        HResult SetBorderColor(
            Color Clr
        );

        [PreserveSig]
        HResult GetBorderColor(
            out Color pClr
        );

        [PreserveSig]
        HResult InsertEffect(
            [MarshalAs(UnmanagedType.IUnknown)] object pEffect,
            [MarshalAs(UnmanagedType.Bool)] bool fOptional
        );

        [PreserveSig]
        HResult RemoveEffect(
            [MarshalAs(UnmanagedType.IUnknown)] object pEffect
        );

        [PreserveSig]
        HResult RemoveAllEffects();

        [PreserveSig]
        HResult Shutdown();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("90EB3E6B-ECBF-45cc-B1DA-C6FE3EA70D57")]
    public interface IMFPMediaItem
    {
        [PreserveSig]
        HResult GetMediaPlayer(
            out IMFPMediaPlayer ppMediaPlayer
        );

        [PreserveSig]
        HResult GetURL(
            [MarshalAs(UnmanagedType.LPWStr)] out string ppwszURL
        );

        [PreserveSig]
        HResult GetObject(
            [MarshalAs(UnmanagedType.IUnknown)] out object ppIUnknown
        );

        [PreserveSig]
        HResult GetUserData(
            out IntPtr pdwUserData
        );

        [PreserveSig]
        HResult SetUserData(
            IntPtr dwUserData
        );

        [PreserveSig]
        HResult GetStartStopPosition(
            [Out, MarshalAs(UnmanagedType.LPStruct)] MFGuid pguidStartPositionType,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalCookie = "IMFPMediaItem.GetStartStopPosition.0", MarshalTypeRef = typeof(PVMarshaler))] PropVariant pvStartValue,
            [Out, MarshalAs(UnmanagedType.LPStruct)] MFGuid pguidStopPositionType,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalCookie = "IMFPMediaItem.GetStartStopPosition.1", MarshalTypeRef = typeof(PVMarshaler))] PropVariant pvStopValue
        );

        [PreserveSig]
        HResult SetStartStopPosition(
            [In, MarshalAs(UnmanagedType.LPStruct)] MFGuid pguidStartPositionType,
            [In, MarshalAs(UnmanagedType.LPStruct)] ConstPropVariant pvStartValue,
            [In, MarshalAs(UnmanagedType.LPStruct)] MFGuid pguidStopPositionType,
            [In, MarshalAs(UnmanagedType.LPStruct)] ConstPropVariant pvStopValue
        );

        [PreserveSig]
        HResult HasVideo(
            [MarshalAs(UnmanagedType.Bool)] out bool pfHasVideo,
            [MarshalAs(UnmanagedType.Bool)] out bool pfSelected
        );

        [PreserveSig]
        HResult HasAudio(
            [MarshalAs(UnmanagedType.Bool)] out bool pfHasAudio,
            [MarshalAs(UnmanagedType.Bool)] out bool pfSelected
        );

        [PreserveSig]
        HResult IsProtected(
            [MarshalAs(UnmanagedType.Bool)] out bool pfProtected
        );

        [PreserveSig]
        HResult GetDuration(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidPositionType,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalCookie = "IMFPMediaItem.GetDuration", MarshalTypeRef = typeof(PVMarshaler))] PropVariant pvDurationValue
        );

        [PreserveSig]
        HResult GetNumberOfStreams(
            out int pdwStreamCount
        );

        [PreserveSig]
        HResult GetStreamSelection(
            int dwStreamIndex,
            [MarshalAs(UnmanagedType.Bool)] out bool pfEnabled
        );

        [PreserveSig]
        HResult SetStreamSelection(
            int dwStreamIndex,
            [MarshalAs(UnmanagedType.Bool)] bool fEnabled
        );

        [PreserveSig]
        HResult GetStreamAttribute(
            int dwStreamIndex,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidMFAttribute,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalCookie = "IMFPMediaItem.GetStreamAttribute", MarshalTypeRef = typeof(PVMarshaler))] PropVariant pvValue
        );

        [PreserveSig]
        HResult GetPresentationAttribute(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidMFAttribute,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalCookie = "IMFPMediaItem.GetPresentationAttribute", MarshalTypeRef = typeof(PVMarshaler))] PropVariant pvValue
        );

        [PreserveSig]
        HResult GetCharacteristics(
            out MFP_MEDIAITEM_CHARACTERISTICS pCharacteristics
        );

        [PreserveSig]
        HResult SetStreamSink(
            int dwStreamIndex,
            [MarshalAs(UnmanagedType.IUnknown)] object pMediaSink
        );

        [PreserveSig]
        HResult GetMetadata(
            out IPropertyStore ppMetadataStore
        );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("766C8FFB-5FDB-4fea-A28D-B912996F51BD")]
    public interface IMFPMediaPlayerCallback
    {
        [PreserveSig]
        HResult OnMediaPlayerEvent(
            [In, MarshalAs(UnmanagedType.CustomMarshaler, MarshalCookie = "IMFPMediaPlayerCallback.OnMediaPlayerEvent", MarshalTypeRef = typeof(EHMarshaler))] MFP_EVENT_HEADER pEventHeader
            );
    }

    #endregion
}
