/* license

EVR.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

using System;
using System.Runtime.InteropServices;

using MediaFoundation.Misc;
using MediaFoundation.Transform;

namespace MediaFoundation.EVR
{
    #region COM Class Objects

    [UnmanagedName("CLSID_EnhancedVideoRenderer"),
    ComImport,
    Guid("FA10746C-9B63-4b6c-BC49-FC300EA5F256")]
    public class EnhancedVideoRenderer
    {
    }

    [UnmanagedName("CLSID_MFVideoMixer9"),
    ComImport,
    Guid("E474E05A-AB65-4f6a-827C-218B1BAAF31F")]
    public class MFVideoMixer9
    {
    }

    [UnmanagedName("CLSID_MFVideoPresenter9"),
    ComImport,
    Guid("98455561-5136-4d28-AB08-4CEE40EA2781")]
    public class MFVideoPresenter9
    {
    }

    [UnmanagedName("CLSID_EVRTearlessWindowPresenter9"),
    ComImport,
    Guid("a0a7a57b-59b2-4919-a694-add0a526c373")]
    public class EVRTearlessWindowPresenter9
    {
    }

    #endregion

    #region Declarations

    [UnmanagedName("MFVP_MESSAGE_TYPE")]
    public enum MFVPMessageType
    {
        Flush = 0,
        InvalidateMediaType,
        ProcessInputNotify,
        BeginStreaming,
        EndStreaming,
        EndOfStream,
        Step,
        CancelStep
    }

    [UnmanagedName("MF_SERVICE_LOOKUP_TYPE")]
    public enum MFServiceLookupType
    {
        Upstream = 0,
        UpstreamDirect,
        Downstream,
        DownstreamDirect,
        All,
        Global
    }

    [Flags, UnmanagedName("MFVideoRenderPrefs")]
    public enum MFVideoRenderPrefs
    {
        None = 0,
        DoNotRenderBorder = 0x00000001,
        DoNotClipToDevice = 0x00000002,
        AllowOutputThrottling = 0x00000004,
        ForceOutputThrottling = 0x00000008,
        ForceBatching = 0x00000010,
        AllowBatching = 0x00000020,
        ForceScaling = 0x00000040,
        AllowScaling = 0x00000080,
        DoNotRepaintOnStop = 0x00000100,
        Mask = 0x000001ff,
    }

    [Flags, UnmanagedName("MFVideoAspectRatioMode")]
    public enum MFVideoAspectRatioMode
    {
        None = 0x00000000,
        PreservePicture = 0x00000001,
        PreservePixel = 0x00000002,
        NonLinearStretch = 0x00000004,
        Mask = 0x00000007
    }

    [Flags, UnmanagedName("DXVA2_ProcAmp_* defines")]
    public enum DXVA2ProcAmp
    {
        None = 0,
        Brightness = 0x0001,
        Contrast = 0x0002,
        Hue = 0x0004,
        Saturation = 0x0008
    }

    [UnmanagedName("Unnamed enum")]
    public enum DXVA2Filters
    {
        None = 0,
        NoiseFilterLumaLevel = 1,
        NoiseFilterLumaThreshold = 2,
        NoiseFilterLumaRadius = 3,
        NoiseFilterChromaLevel = 4,
        NoiseFilterChromaThreshold = 5,
        NoiseFilterChromaRadius = 6,
        DetailFilterLumaLevel = 7,
        DetailFilterLumaThreshold = 8,
        DetailFilterLumaRadius = 9,
        DetailFilterChromaLevel = 10,
        DetailFilterChromaThreshold = 11,
        DetailFilterChromaRadius = 12
    }

    [Flags, UnmanagedName("MFVideoAlphaBitmapFlags")]
    public enum MFVideoAlphaBitmapFlags
    {
        None = 0,
        EntireDDS = 0x00000001,
        SrcColorKey = 0x00000002,
        SrcRect = 0x00000004,
        DestRect = 0x00000008,
        FilterMode = 0x00000010,
        Alpha = 0x00000020,
        BitMask = 0x0000003f
    }

    [Flags, UnmanagedName("MFVideoMixPrefs")]
    public enum MFVideoMixPrefs
    {
        None = 0,
        ForceHalfInterlace = 0x00000001,
        AllowDropToHalfInterlace = 0x00000002,
        AllowDropToBob = 0x00000004,
        ForceBob = 0x00000008,
        Mask = 0x0000000f
    }

    [Flags, UnmanagedName("EVRFilterConfigPrefs")]
    public enum EVRFilterConfigPrefs
    {
        None = 0,
        EnableQoS = 0x1,
        Mask = 0x1
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4), UnmanagedName("MFVideoNormalizedRect")]
    public class MFVideoNormalizedRect
    {
        public float left;
        public float top;
        public float right;
        public float bottom;

        public MFVideoNormalizedRect()
        {
        }

        public MFVideoNormalizedRect(float l, float t, float r, float b)
        {
            left = l;
            top = t;
            right = r;
            bottom = b;
        }

        public override string ToString()
        {
            return string.Format("left = {0}, top = {1}, right = {2}, bottom = {3}", left, top, right, bottom);
        }

        public override int GetHashCode()
        {
            return left.GetHashCode() |
                top.GetHashCode() |
                right.GetHashCode() |
                bottom.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is MFVideoNormalizedRect)
            {
                MFVideoNormalizedRect cmp = (MFVideoNormalizedRect)obj;

                return right == cmp.right && bottom == cmp.bottom && left == cmp.left && top == cmp.top;
            }

            return false;
        }

        public bool IsEmpty()
        {
            return (right <= left || bottom <= top);
        }

        public void CopyFrom(MFVideoNormalizedRect from)
        {
            left = from.left;
            top = from.top;
            right = from.right;
            bottom = from.bottom;
        }
    }

    [StructLayout(LayoutKind.Sequential), UnmanagedName("DXVA2_VideoProcessorCaps")]
    public struct DXVA2VideoProcessorCaps
    {
        public int DeviceCaps;
        public int InputPool;
        public int NumForwardRefSamples;
        public int NumBackwardRefSamples;
        public int Reserved;
        public int DeinterlaceTechnology;
        public int ProcAmpControlCaps;
        public int VideoProcessorOperations;
        public int NoiseFilterTechnology;
        public int DetailFilterTechnology;
    }

    [StructLayout(LayoutKind.Sequential), UnmanagedName("DXVA2_ValueRange")]
    public struct DXVA2ValueRange
    {
        public int MinValue;
        public int MaxValue;
        public int DefaultValue;
        public int StepSize;
    }

    [StructLayout(LayoutKind.Sequential), UnmanagedName("DXVA2_ProcAmpValues")]
    public class DXVA2ProcAmpValues
    {
        public int Brightness;
        public int Contrast;
        public int Hue;
        public int Saturation;
    }

    [StructLayout(LayoutKind.Sequential), UnmanagedName("MFVideoAlphaBitmapParams")]
    public class MFVideoAlphaBitmapParams
    {
        public MFVideoAlphaBitmapFlags dwFlags;
        public int clrSrcKey;
        public MFRect rcSrc;
        public MFVideoNormalizedRect nrcDest;
        public float fAlpha;
        public int dwFilterMode;
    }

    [StructLayout(LayoutKind.Sequential), UnmanagedName("MFVideoAlphaBitmap")]
    public class MFVideoAlphaBitmap
    {
        public bool GetBitmapFromDC;
        public IntPtr stru;
        public MFVideoAlphaBitmapParams paras;
    }

    #endregion

    #region Interfaces

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("83A4CE40-7710-494b-A893-A472049AF630")]
    public interface IEVRTrustedVideoPlugin
    {
        [PreserveSig]
        HResult IsInTrustedVideoMode(
            [MarshalAs(UnmanagedType.Bool)] out bool pYes
            );

        [PreserveSig]
        HResult CanConstrict(
            [MarshalAs(UnmanagedType.Bool)] out bool pYes
            );

        [PreserveSig]
        HResult SetConstriction(
            int dwKPix
            );

        [PreserveSig]
        HResult DisableImageExport(
            [MarshalAs(UnmanagedType.Bool)] bool bDisable
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("AEA36028-796D-454F-BEEE-B48071E24304")]
    public interface IEVRFilterConfigEx : IEVRFilterConfig
    {
        #region IEVRFilterConfig methods

        [PreserveSig]
        new HResult SetNumberOfStreams(
            int dwMaxStreams
        );

        [PreserveSig]
        new HResult GetNumberOfStreams(
            out int pdwMaxStreams
        );

        #endregion

        [PreserveSig]
        HResult SetConfigPrefs(
            [In] EVRFilterConfigPrefs dwConfigFlags
        );

        [PreserveSig]
        HResult GetConfigPrefs(
            out EVRFilterConfigPrefs pdwConfigFlags
        );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("8459616D-966E-4930-B658-54FA7E5A16D3")]
    public interface IMFVideoMixerControl2 : IMFVideoMixerControl
    {
        #region IMFVideoMixerControl methods

        [PreserveSig]
        new HResult SetStreamZOrder(
            [In] int dwStreamID,
            [In] int dwZ
            );

        [PreserveSig]
        new HResult GetStreamZOrder(
            [In] int dwStreamID,
            out int pdwZ
            );

        [PreserveSig]
        new HResult SetStreamOutputRect(
            [In] int dwStreamID,
            [In] MFVideoNormalizedRect pnrcOutput
            );

        [PreserveSig]
        new HResult GetStreamOutputRect(
            [In] int dwStreamID,
            [Out, MarshalAs(UnmanagedType.LPStruct)] MFVideoNormalizedRect pnrcOutput
            );

        #endregion

        [PreserveSig]
        HResult SetMixingPrefs(
            [In] MFVideoMixPrefs dwMixFlags
        );

        [PreserveSig]
        HResult GetMixingPrefs(
            out MFVideoMixPrefs pdwMixFlags
        );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("A490B1E4-AB84-4D31-A1B2-181E03B1077A"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFVideoDisplayControl
    {
        [PreserveSig]
        HResult GetNativeVideoSize(
            [Out] MFSize pszVideo,
            [Out] MFSize pszARVideo
            );

        [PreserveSig]
        HResult GetIdealVideoSize(
            [Out] MFSize pszMin,
            [Out] MFSize pszMax
            );

        [PreserveSig]
        HResult SetVideoPosition(
            [In] MFVideoNormalizedRect pnrcSource,
            [In] MFRect prcDest
            );

        [PreserveSig]
        HResult GetVideoPosition(
            [Out] MFVideoNormalizedRect pnrcSource,
            [Out] MFRect prcDest
            );

        [PreserveSig]
        HResult SetAspectRatioMode(
            [In] MFVideoAspectRatioMode dwAspectRatioMode
            );

        [PreserveSig]
        HResult GetAspectRatioMode(
            out MFVideoAspectRatioMode pdwAspectRatioMode
            );

        [PreserveSig]
        HResult SetVideoWindow(
            [In] IntPtr hwndVideo
            );

        [PreserveSig]
        HResult GetVideoWindow(
            out IntPtr phwndVideo
            );

        [PreserveSig]
        HResult RepaintVideo();

        [PreserveSig]
        HResult GetCurrentImage(
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalCookie = "IMFVideoDisplayControl.GetCurrentImage", MarshalTypeRef = typeof(BMMarshaler))] BitmapInfoHeader pBih,
            out IntPtr pDib,
            out int pcbDib,
            out long pTimeStamp
            );

        [PreserveSig]
        HResult SetBorderColor(
            [In] int Clr
            );

        [PreserveSig]
        HResult GetBorderColor(
            out int pClr
            );

        [PreserveSig]
        HResult SetRenderingPrefs(
            [In] MFVideoRenderPrefs dwRenderFlags
            );

        [PreserveSig]
        HResult GetRenderingPrefs(
            out MFVideoRenderPrefs pdwRenderFlags
            );

        [PreserveSig]
        HResult SetFullscreen(
            [In, MarshalAs(UnmanagedType.Bool)] bool fFullscreen
            );

        [PreserveSig]
        HResult GetFullscreen(
            [MarshalAs(UnmanagedType.Bool)] out bool pfFullscreen
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("83E91E85-82C1-4ea7-801D-85DC50B75086")]
    public interface IEVRFilterConfig
    {
        [PreserveSig]
        HResult SetNumberOfStreams(
            int dwMaxStreams
            );

        [PreserveSig]
        HResult GetNumberOfStreams(
            out int pdwMaxStreams
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("1F6A9F17-E70B-4E24-8AE4-0B2C3BA7A4AE"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFVideoPositionMapper
    {
        [PreserveSig]
        HResult MapOutputCoordinateToInputStream(
            [In] float xOut,
            [In] float yOut,
            [In] int dwOutputStreamIndex,
            [In] int dwInputStreamIndex,
            out float pxIn,
            out float pyIn
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("A5C6C53F-C202-4AA5-9695-175BA8C508A5")]
    public interface IMFVideoMixerControl
    {
        [PreserveSig]
        HResult SetStreamZOrder(
            [In] int dwStreamID,
            [In] int dwZ
            );

        [PreserveSig]
        HResult GetStreamZOrder(
            [In] int dwStreamID,
            out int pdwZ
            );

        [PreserveSig]
        HResult SetStreamOutputRect(
            [In] int dwStreamID,
            [In] MFVideoNormalizedRect pnrcOutput
            );

        [PreserveSig]
        HResult GetStreamOutputRect(
            [In] int dwStreamID,
            [Out, MarshalAs(UnmanagedType.LPStruct)] MFVideoNormalizedRect pnrcOutput
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("FA99388A-4383-415A-A930-DD472A8CF6F7"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFTopologyServiceLookupClient
    {
        [PreserveSig]
        HResult InitServicePointers(
            IMFTopologyServiceLookup pLookup
            );

        [PreserveSig]
        HResult ReleaseServicePointers();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("29AFF080-182A-4A5D-AF3B-448F3A6346CB")]
    public interface IMFVideoPresenter : IMFClockStateSink
    {
        #region IMFClockStateSink

        [PreserveSig]
        new HResult OnClockStart(
            [In] long hnsSystemTime,
            [In] long llClockStartOffset
            );

        [PreserveSig]
        new HResult OnClockStop(
            [In] long hnsSystemTime
            );

        [PreserveSig]
        new HResult OnClockPause(
            [In] long hnsSystemTime
            );

        [PreserveSig]
        new HResult OnClockRestart(
            [In] long hnsSystemTime
            );

        [PreserveSig]
        new HResult OnClockSetRate(
            [In] long hnsSystemTime,
            [In] float flRate
            );

        #endregion

        [PreserveSig]
        HResult ProcessMessage(
            MFVPMessageType eMessage,
            IntPtr ulParam
            );

        [PreserveSig]
        HResult GetCurrentMediaType(
            [MarshalAs(UnmanagedType.Interface)] out IMFVideoMediaType ppMediaType
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("FA993889-4383-415A-A930-DD472A8CF6F7"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFTopologyServiceLookup
    {
        [PreserveSig]
        HResult LookupService(
            [In] MFServiceLookupType type,
            [In] int dwIndex,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidService,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Interface), Out] object[] ppvObjects,
            [In, Out] ref int pnObjects
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("A38D9567-5A9C-4F3C-B293-8EB415B279BA")]
    public interface IMFVideoDeviceID
    {
        [PreserveSig]
        HResult GetDeviceID(
            out Guid pDeviceID
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("DFDFD197-A9CA-43D8-B341-6AF3503792CD"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFVideoRenderer
    {
        [PreserveSig]
        HResult InitializeRenderer(
            [In, MarshalAs(UnmanagedType.Interface)] IMFTransform pVideoMixer,
            [In, MarshalAs(UnmanagedType.Interface)] IMFVideoPresenter pVideoPresenter
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("245BF8E9-0755-40F7-88A5-AE0F18D55E17")]
    public interface IMFTrackedSample
    {
        [PreserveSig]
        HResult SetAllocator(
            [In, MarshalAs(UnmanagedType.Interface)] IMFAsyncCallback pSampleAllocator,
            [In, MarshalAs(UnmanagedType.IUnknown)] object pUnkState
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("56C294D0-753E-4260-8D61-A3D8820B1D54"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFDesiredSample
    {
        [PreserveSig]
        HResult GetDesiredSampleTimeAndDuration(
            out long phnsSampleTime,
            out long phnsSampleDuration
            );

        [PreserveSig]
        void SetDesiredSampleTimeAndDuration(
            [In] long hnsSampleTime,
            [In] long hnsSampleDuration
            );

        [PreserveSig]
        void Clear();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("6AB0000C-FECE-4d1f-A2AC-A9573530656E")]
    public interface IMFVideoProcessor
    {
        [PreserveSig]
        HResult GetAvailableVideoProcessorModes(
            out int lpdwNumProcessingModes,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0)] out Guid[] ppVideoProcessingModes);

        [PreserveSig]
        HResult GetVideoProcessorCaps(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid lpVideoProcessorMode,
            out DXVA2VideoProcessorCaps lpVideoProcessorCaps);

        [PreserveSig]
        HResult GetVideoProcessorMode(
            out Guid lpMode);

        [PreserveSig]
        HResult SetVideoProcessorMode(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid lpMode);

        [PreserveSig]
        HResult GetProcAmpRange(
            DXVA2ProcAmp dwProperty,
            out DXVA2ValueRange pPropRange);

        [PreserveSig]
        HResult GetProcAmpValues(
            DXVA2ProcAmp dwFlags,
            [Out, MarshalAs(UnmanagedType.LPStruct)] DXVA2ProcAmpValues Values);

        [PreserveSig]
        HResult SetProcAmpValues(
            DXVA2ProcAmp dwFlags,
            [In] DXVA2ProcAmpValues pValues);

        [PreserveSig]
        HResult GetFilteringRange(
            DXVA2Filters dwProperty,
            out DXVA2ValueRange pPropRange);

        [PreserveSig]
        HResult GetFilteringValue(
            DXVA2Filters dwProperty,
            out int pValue);

        [PreserveSig]
        HResult SetFilteringValue(
            DXVA2Filters dwProperty,
            [In] ref int pValue);

        [PreserveSig]
        HResult GetBackgroundColor(
            out int lpClrBkg);

        [PreserveSig]
        HResult SetBackgroundColor(
            int ClrBkg);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("814C7B20-0FDB-4eec-AF8F-F957C8F69EDC")]
    public interface IMFVideoMixerBitmap
    {
        [PreserveSig]
        HResult SetAlphaBitmap(
            [In, MarshalAs(UnmanagedType.LPStruct)] MFVideoAlphaBitmap pBmpParms);

        [PreserveSig]
        HResult ClearAlphaBitmap();

        [PreserveSig]
        HResult UpdateAlphaBitmapParameters(
            [In] MFVideoAlphaBitmapParams pBmpParms);

        [PreserveSig]
        HResult GetAlphaBitmapParameters(
            [Out] MFVideoAlphaBitmapParams pBmpParms);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("d0cfe38b-93e7-4772-8957-0400c49a4485")]
    public interface IEVRVideoStreamControl
    {
        [PreserveSig]
        HResult SetStreamActiveState(
            [MarshalAs(UnmanagedType.Bool)] bool fActive);

        [PreserveSig]
        HResult GetStreamActiveState(
            [MarshalAs(UnmanagedType.Bool)] out bool lpfActive);
    }

    #endregion
}
