/* license

MFTransform.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

using System;
using System.Runtime.InteropServices;

using MediaFoundation.Misc;

namespace MediaFoundation.Transform
{
    #region Declarations

    [Flags, UnmanagedName("MF3DVideoOutputType")]
    public enum MF3DVideoOutputType
    {
        BaseView = 0,
        Stereo = 1
    }

    [UnmanagedName("_MFT_DRAIN_TYPE")]
    public enum MFTDrainType
    {
        ProduceTails = 0x00000000,
        NoTails = 0x00000001
    }

    [Flags, UnmanagedName("_MFT_PROCESS_OUTPUT_FLAGS")]
    public enum MFTProcessOutputFlags
    {
        None = 0,
        DiscardWhenNoBuffer = 0x00000001,
        RegenerateLastOutput = 0x00000002
    }

    [Flags, UnmanagedName("_MFT_OUTPUT_STATUS_FLAGS")]
    public enum MFTOutputStatusFlags
    {
        None = 0,
        SampleReady = 0x00000001
    }

    [Flags, UnmanagedName("_MFT_INPUT_STATUS_FLAGS")]
    public enum MFTInputStatusFlags
    {
        None = 0,
        AcceptData = 0x00000001
    }

    [Flags, UnmanagedName("_MFT_SET_TYPE_FLAGS")]
    public enum MFTSetTypeFlags
    {
        None = 0,
        TestOnly = 0x00000001
    }

    [Flags, UnmanagedName("_MFT_OUTPUT_STREAM_INFO_FLAGS")]
    public enum MFTOutputStreamInfoFlags
    {
        None = 0,
        WholeSamples = 0x00000001,
        SingleSamplePerBuffer = 0x00000002,
        FixedSampleSize = 0x00000004,
        Discardable = 0x00000008,
        Optional = 0x00000010,
        ProvidesSamples = 0x00000100,
        CanProvideSamples = 0x00000200,
        LazyRead = 0x00000400,
        Removable = 0x00000800
    }

    [Flags, UnmanagedName("_MFT_INPUT_STREAM_INFO_FLAGS")]
    public enum MFTInputStreamInfoFlags
    {
        None = 0,
        WholeSamples = 0x1,
        SingleSamplePerBuffer = 0x2,
        FixedSampleSize = 0x4,
        HoldsBuffers = 0x8,
        DoesNotAddRef = 0x100,
        Removable = 0x200,
        Optional = 0x400,
        ProcessesInPlace = 0x800
    }

    [UnmanagedName("MFT_MESSAGE_TYPE")]
    public enum MFTMessageType
    {
        CommandDrain = 1,
        CommandFlush = 0,
        NotifyBeginStreaming = 0x10000000,
        NotifyEndOfStream = 0x10000002,
        NotifyEndStreaming = 0x10000001,
        NotifyStartOfStream = 0x10000003,
        SetD3DManager = 2,
        DropSamples = 0x00000003,
        CommandTick = 0x00000004,
        CommandMarker = 0x20000000,
        NotifyReleaseResources = 0x10000004,
        NotifyReacquireResources = 0x10000005,
    }

    [UnmanagedName("_MFT_OUTPUT_DATA_BUFFER_FLAGS")]
    public enum MFTOutputDataBufferFlags
    {
        None = 0,
        Incomplete = 0x01000000,
        FormatChange = 0x00000100,
        StreamEnd = 0x00000200,
        NoSample = 0x00000300
    }

    [Flags, UnmanagedName("_MFT_PROCESS_OUTPUT_STATUS")]
    public enum ProcessOutputStatus
    {
        None = 0,
        NewStreams = 0x00000100
    }

    [Flags, UnmanagedName("MFT_ENUM_FLAG")]
    public enum MFT_EnumFlag
    {
        None = 0x00000000,
        SyncMFT = 0x00000001,   // Enumerates V1 MFTs. This is default.
        AsyncMFT = 0x00000002,   // Enumerates only software async MFTs also known as V2 MFTs
        Hardware = 0x00000004,   // Enumerates V2 hardware async MFTs
        FieldOfUse = 0x00000008,   // Enumerates MFTs that require unlocking
        LocalMFT = 0x00000010,   // Enumerates Locally (in-process) registered MFTs
        TranscodeOnly = 0x00000020,   // Enumerates decoder MFTs used by transcode only
        SortAndFilter = 0x00000040,   // Apply system local, do not use and preferred sorting and filtering
        SortAndFilterApprovedOnly = 0x000000C0,   // Similar to MFT_ENUM_FLAG_SORTANDFILTER, but apply a local policy of: MF_PLUGIN_CONTROL_POLICY_USE_APPROVED_PLUGINS
        SortAndFilterWebOnly = 0x00000140,   // Similar to MFT_ENUM_FLAG_SORTANDFILTER, but apply a local policy of: MF_PLUGIN_CONTROL_POLICY_USE_WEB_PLUGINS
        SortAndFilterWebOnlyEdgemode = 0x00000240,
        All = 0x0000003F    // Enumerates all MFTs including SW and HW MFTs and applies filtering
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8), UnmanagedName("MFT_INPUT_STREAM_INFO")]
    public struct MFTInputStreamInfo
    {
        public long hnsMaxLatency;
        public MFTInputStreamInfoFlags dwFlags;
        public int cbSize;
        public int cbMaxLookahead;
        public int cbAlignment;
    }

    [StructLayout(LayoutKind.Sequential), UnmanagedName("MFT_OUTPUT_DATA_BUFFER")]
    public struct MFTOutputDataBuffer
    {
        public int dwStreamID;
        public IntPtr pSample; // Doesn't release correctly when marshaled as IMFSample
        public MFTOutputDataBufferFlags dwStatus;
        [MarshalAs(UnmanagedType.Interface)]
        public IMFCollection pEvents;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4), UnmanagedName("MFT_OUTPUT_STREAM_INFO")]
    public struct MFTOutputStreamInfo
    {
        public MFTOutputStreamInfoFlags dwFlags;
        public int cbSize;
        public int cbAlignment;
    }

    [StructLayout(LayoutKind.Sequential), UnmanagedName("STREAM_MEDIUM")]
    public struct STREAM_MEDIUM
    {
        Guid gidMedium;
        int unMediumInstance;
    }

    [StructLayout(LayoutKind.Sequential), UnmanagedName("MFT_REGISTER_TYPE_INFO")]
    public class MFTRegisterTypeInfo
    {
        public Guid guidMajorType;
        public Guid guidSubtype;
    }

    [StructLayout(LayoutKind.Sequential), UnmanagedName("MFT_REGISTRATION_INFO")]
    public struct MFT_REGISTRATION_INFO
    {
        public Guid clsid;
        public Guid guidCategory;
        public MFT_EnumFlag uiFlags;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszName;
        public int cInTypes;
        public MFTRegisterTypeInfo[] pInTypes;
        public int cOutTypes;
        public MFTRegisterTypeInfo[] pOutTypes;
    }

    #endregion

    #region Interfaces

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("149c4d73-b4be-4f8d-8b87-079e926b6add")]
    public interface IMFLocalMFTRegistration
    {
        [PreserveSig]
        HResult RegisterMFTs(
            [In, MarshalAs(UnmanagedType.LPArray)] MFT_REGISTRATION_INFO[] pMFTs,
            int cMFTs
        );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("BF94C121-5B05-4E6F-8000-BA598961414D")]
    public interface IMFTransform
    {
        [PreserveSig]
        HResult GetStreamLimits(
            [Out] MFInt pdwInputMinimum,
            [Out] MFInt pdwInputMaximum,
            [Out] MFInt pdwOutputMinimum,
            [Out] MFInt pdwOutputMaximum
        );

        [PreserveSig]
        HResult GetStreamCount(
            [Out] MFInt pcInputStreams,
            [Out] MFInt pcOutputStreams
        );

        [PreserveSig]
        HResult GetStreamIDs(
            int dwInputIDArraySize,
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] int[] pdwInputIDs,
            int dwOutputIDArraySize,
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] int[] pdwOutputIDs
        );

        [PreserveSig]
        HResult GetInputStreamInfo(
            int dwInputStreamID,
            out MFTInputStreamInfo pStreamInfo
        );

        [PreserveSig]
        HResult GetOutputStreamInfo(
            int dwOutputStreamID,
            out MFTOutputStreamInfo pStreamInfo
        );

        [PreserveSig]
        HResult GetAttributes(
            [MarshalAs(UnmanagedType.Interface)] out IMFAttributes pAttributes
        );

        [PreserveSig]
        HResult GetInputStreamAttributes(
            int dwInputStreamID,
            [MarshalAs(UnmanagedType.Interface)] out IMFAttributes pAttributes
        );

        [PreserveSig]
        HResult GetOutputStreamAttributes(
            int dwOutputStreamID,
            [MarshalAs(UnmanagedType.Interface)] out IMFAttributes pAttributes
        );

        [PreserveSig]
        HResult DeleteInputStream(
            int dwStreamID
        );

        [PreserveSig]
        HResult AddInputStreams(
            int cStreams,
            [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] int[] adwStreamIDs
        );

        [PreserveSig]
        HResult GetInputAvailableType(
            int dwInputStreamID,
            int dwTypeIndex,
            [MarshalAs(UnmanagedType.Interface)] out IMFMediaType ppType
        );

        [PreserveSig]
        HResult GetOutputAvailableType(
            int dwOutputStreamID,
            int dwTypeIndex,
            [MarshalAs(UnmanagedType.Interface)] out IMFMediaType ppType
        );

        [PreserveSig]
        HResult SetInputType(
            int dwInputStreamID,
            [In, MarshalAs(UnmanagedType.Interface)] IMFMediaType pType,
            MFTSetTypeFlags dwFlags
        );

        [PreserveSig]
        HResult SetOutputType(
            int dwOutputStreamID,
            [In, MarshalAs(UnmanagedType.Interface)] IMFMediaType pType,
            MFTSetTypeFlags dwFlags
        );

        [PreserveSig]
        HResult GetInputCurrentType(
            int dwInputStreamID,
            [MarshalAs(UnmanagedType.Interface)] out IMFMediaType ppType
        );

        [PreserveSig]
        HResult GetOutputCurrentType(
            int dwOutputStreamID,
            [MarshalAs(UnmanagedType.Interface)] out IMFMediaType ppType
        );

        [PreserveSig]
        HResult GetInputStatus(
            int dwInputStreamID,
            out MFTInputStatusFlags pdwFlags
        );

        [PreserveSig]
        HResult GetOutputStatus(
            out MFTOutputStatusFlags pdwFlags
        );

        [PreserveSig]
        HResult SetOutputBounds(
            long hnsLowerBound,
            long hnsUpperBound
        );

        [PreserveSig]
        HResult ProcessEvent(
            int dwInputStreamID,
            [In, MarshalAs(UnmanagedType.Interface)] IMFMediaEvent pEvent
        );

        [PreserveSig]
        HResult ProcessMessage(
            MFTMessageType eMessage,
            IntPtr ulParam
        );

        [PreserveSig]
        HResult ProcessInput(
            int dwInputStreamID,
            [MarshalAs(UnmanagedType.Interface)] IMFSample pSample,
            int dwFlags // Must be zero
        );

        [PreserveSig]
        HResult ProcessOutput(
            MFTProcessOutputFlags dwFlags,
            int cOutputBufferCount,
            [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] MFTOutputDataBuffer[] pOutputSamples,
            out ProcessOutputStatus pdwStatus
        );
    }

    #endregion
}
