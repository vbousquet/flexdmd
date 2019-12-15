/* license

WMContainer.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

using System;
using System.Text;
using System.Runtime.InteropServices;

using MediaFoundation.Misc;

namespace MediaFoundation
{
    #region Declarations

#if ALLOW_UNTESTED_INTERFACES

    [UnmanagedName("MFSINK_WMDRMACTION")]
    public enum MFSinkWMDRMAction
    {
        Undefined = 0,
        Encode = 1,
        Transcode = 2,
        Transcrypt = 3,
        Last = 3
    }

#endif

    [Flags, UnmanagedName("MFASF_SPLITTERFLAGS")]
    public enum MFASFSplitterFlags
    {
        None = 0,
        Reverse = 0x00000001,
        WMDRM = 0x00000002
    }

    [Flags, UnmanagedName("ASF_STATUSFLAGS")]
    public enum ASFStatusFlags
    {
        None = 0,
        Incomplete = 0x1,
        NonfatalError = 0x2
    }

    [Flags, UnmanagedName("MFASF_STREAMSELECTORFLAGS")]
    public enum MFAsfStreamSelectorFlags
    {
        None = 0x00000000,
        DisableThinning = 0x00000001,
        UseAverageBitrate = 0x00000002
    }

    [UnmanagedName("ASF_SELECTION_STATUS")]
    public enum ASFSelectionStatus
    {
        NotSelected = 0,
        CleanPointsOnly = 1,
        AllDataUnits = 2
    }

    [Flags, UnmanagedName("MFASF_INDEXERFLAGS")]
    public enum MFAsfIndexerFlags
    {
        None = 0x0,
        WriteNewIndex = 0x00000001,
        ReadForReversePlayback = 0x00000004,
        WriteForLiveRead = 0x00000008
    }

    [Flags, UnmanagedName("MFASF_MULTIPLEXERFLAGS")]
    public enum MFASFMultiplexerFlags
    {
        None = 0,
        AutoAdjustBitrate = 0x00000001
    }

    [StructLayout(LayoutKind.Sequential), UnmanagedName("ASF_INDEX_IDENTIFIER")]
    public class ASFIndexIdentifier
    {
        public Guid guidIndexType;
        public short wStreamNumber;
    }

    [StructLayout(LayoutKind.Sequential), UnmanagedName("ASF_MUX_STATISTICS")]
    public struct ASFMuxStatistics
    {
        public int cFramesWritten;
        public int cFramesDropped;
    }

    public static class MFPKEY_ASFMEDIASINK
    {
        public static readonly PropertyKey MFPKEY_ASFMEDIASINK_BASE_SENDTIME = new PropertyKey(new Guid(0xcddcbc82, 0x3411, 0x4119, 0x91, 0x35, 0x84, 0x23, 0xc4, 0x1b, 0x39, 0x57), 3);
        public static readonly PropertyKey MFPKEY_ASFMEDIASINK_AUTOADJUST_BITRATE = new PropertyKey(new Guid(0xcddcbc82, 0x3411, 0x4119, 0x91, 0x35, 0x84, 0x23, 0xc4, 0x1b, 0x39, 0x57), 4);
        public static readonly PropertyKey MFPKEY_ASFMEDIASINK_DRMACTION = new PropertyKey(new Guid(0xa1db6f6c, 0x1d0a, 0x4cb6, 0x82, 0x54, 0xcb, 0x36, 0xbe, 0xed, 0xbc, 0x48), 5);
    }

    public static class MFASFSampleExtension
    {
        public static readonly Guid SampleDuration = new Guid(0xc6bd9450, 0x867f, 0x4907, 0x83, 0xa3, 0xc7, 0x79, 0x21, 0xb7, 0x33, 0xad);
        public static readonly Guid OutputCleanPoint = new Guid(0xf72a3c6f, 0x6eb4, 0x4ebc, 0xb1, 0x92, 0x9, 0xad, 0x97, 0x59, 0xe8, 0x28);
        public static readonly Guid SMPTE = new Guid(0x399595ec, 0x8667, 0x4e2d, 0x8f, 0xdb, 0x98, 0x81, 0x4c, 0xe7, 0x6c, 0x1e);
        public static readonly Guid FileName = new Guid(0xe165ec0e, 0x19ed, 0x45d7, 0xb4, 0xa7, 0x25, 0xcb, 0xd1, 0xe2, 0x8e, 0x9b);
        public static readonly Guid ContentType = new Guid(0xd590dc20, 0x07bc, 0x436c, 0x9c, 0xf7, 0xf3, 0xbb, 0xfb, 0xf1, 0xa4, 0xdc);
        public static readonly Guid PixelAspectRatio = new Guid(0x1b1ee554, 0xf9ea, 0x4bc8, 0x82, 0x1a, 0x37, 0x6b, 0x74, 0xe4, 0xc4, 0xb8);
        public static readonly Guid Encryption_SampleID = new Guid(0x6698B84E, 0x0AFA, 0x4330, 0xAE, 0xB2, 0x1C, 0x0A, 0x98, 0xD7, 0xA4, 0x4D);
        public static readonly Guid Encryption_KeyID = new Guid(0x76376591, 0x795f, 0x4da1, 0x86, 0xed, 0x9d, 0x46, 0xec, 0xa1, 0x09, 0xa9);
    }

    #endregion

    #region Interfaces

#if ALLOW_UNTESTED_INTERFACES

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("699bdc27-bbaf-49ff-8e38-9c39c9b5e088"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFASFStreamPrioritization
    {
        [PreserveSig]
        HResult GetStreamCount(
            out int pdwStreamCount);

        [PreserveSig]
        HResult GetStream(
            [In] int dwStreamIndex,
            out short pwStreamNumber,
            out short pwStreamFlags); // bool

        [PreserveSig]
        HResult AddStream(
            [In] short wStreamNumber,
            [In] short wStreamFlags); // bool

        [PreserveSig]
        HResult RemoveStream(
            [In] int dwStreamIndex);

        [PreserveSig]
        HResult Clone(
            out IMFASFStreamPrioritization ppIStreamPrioritization);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("3D1FF0EA-679A-4190-8D46-7FA69E8C7E15")]
    public interface IMFDRMNetHelper
    {
        [PreserveSig]
        HResult ProcessLicenseRequest(
            [In] IntPtr pLicenseRequest,
            [In] int cbLicenseRequest,
            [Out] out IntPtr ppLicenseResponse,
            out int pcbLicenseResponse,
            [MarshalAs(UnmanagedType.BStr)] out string pbstrKID
        );

        [PreserveSig]
        HResult GetChainedLicenseResponse(
            [Out] out IntPtr ppLicenseResponse,
            out int pcbLicenseResponse
        );
    }

#endif

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("D267BF6A-028B-4e0d-903D-43F0EF82D0D4"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFASFProfile : IMFAttributes
    {
        #region IMFAttributes methods

        [PreserveSig]
        new HResult GetItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalCookie = "IMFASFProfile.GetItem", MarshalTypeRef = typeof(PVMarshaler))] PropVariant pValue
            );

        [PreserveSig]
        new HResult GetItemType(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out MFAttributeType pType
            );

        [PreserveSig]
        new HResult CompareItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] ConstPropVariant Value,
            [MarshalAs(UnmanagedType.Bool)] out bool pbResult
            );

        [PreserveSig]
        new HResult Compare(
            [MarshalAs(UnmanagedType.Interface)] IMFAttributes pTheirs,
            MFAttributesMatchType MatchType,
            [MarshalAs(UnmanagedType.Bool)] out bool pbResult
            );

        [PreserveSig]
        new HResult GetUINT32(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int punValue
            );

        [PreserveSig]
        new HResult GetUINT64(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out long punValue
            );

        [PreserveSig]
        new HResult GetDouble(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out double pfValue
            );

        [PreserveSig]
        new HResult GetGUID(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out Guid pguidValue
            );

        [PreserveSig]
        new HResult GetStringLength(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int pcchLength
            );

        [PreserveSig]
        new HResult GetString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszValue,
            int cchBufSize,
            out int pcchLength
            );

        [PreserveSig]
        new HResult GetAllocatedString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [MarshalAs(UnmanagedType.LPWStr)] out string ppwszValue,
            out int pcchLength
            );

        [PreserveSig]
        new HResult GetBlobSize(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int pcbBlobSize
            );

        [PreserveSig]
        new HResult GetBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [Out, MarshalAs(UnmanagedType.LPArray)] byte[] pBuf,
            int cbBufSize,
            out int pcbBlobSize
            );

        // Use GetBlob instead of this
        [PreserveSig]
        new HResult GetAllocatedBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out IntPtr ip,  // Read w/Marshal.Copy, Free w/Marshal.FreeCoTaskMem
            out int pcbSize
            );

        [PreserveSig]
        new HResult GetUnknown(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppv
            );

        [PreserveSig]
        new HResult SetItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] ConstPropVariant Value
            );

        [PreserveSig]
        new HResult DeleteItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey
            );

        [PreserveSig]
        new HResult DeleteAllItems();

        [PreserveSig]
        new HResult SetUINT32(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            int unValue
            );

        [PreserveSig]
        new HResult SetUINT64(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            long unValue
            );

        [PreserveSig]
        new HResult SetDouble(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            double fValue
            );

        [PreserveSig]
        new HResult SetGUID(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidValue
            );

        [PreserveSig]
        new HResult SetString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPWStr)] string wszValue
            );

        [PreserveSig]
        new HResult SetBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pBuf,
            int cbBufSize
            );

        [PreserveSig]
        new HResult SetUnknown(
            [MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.IUnknown)] object pUnknown
            );

        [PreserveSig]
        new HResult LockStore();

        [PreserveSig]
        new HResult UnlockStore();

        [PreserveSig]
        new HResult GetCount(
            out int pcItems
            );

        [PreserveSig]
        new HResult GetItemByIndex(
            int unIndex,
            out Guid pguidKey,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalCookie = "IMFASFProfile.GetItemByIndex", MarshalTypeRef = typeof(PVMarshaler))] PropVariant pValue
            );

        [PreserveSig]
        new HResult CopyAllItems(
            [In, MarshalAs(UnmanagedType.Interface)] IMFAttributes pDest
            );

        #endregion

        [PreserveSig]
        HResult GetStreamCount(
            out int pcStreams);

        [PreserveSig]
        HResult GetStream(
            [In] int dwStreamIndex,
            out short pwStreamNumber,
            out IMFASFStreamConfig ppIStream);

        [PreserveSig]
        HResult GetStreamByNumber(
            [In] short wStreamNumber,
            out IMFASFStreamConfig ppIStream);

        [PreserveSig]
        HResult SetStream(
            [In] IMFASFStreamConfig pIStream);

        [PreserveSig]
        HResult RemoveStream(
            [In] short wStreamNumber);

        [PreserveSig]
        HResult CreateStream(
            [In] IMFMediaType pIMediaType,
            out IMFASFStreamConfig ppIStream);

        [PreserveSig]
        HResult GetMutualExclusionCount(
            out int pcMutexs);

        [PreserveSig]
        HResult GetMutualExclusion(
            [In] int dwMutexIndex,
            out IMFASFMutualExclusion ppIMutex);

        [PreserveSig]
        HResult AddMutualExclusion(
            [In] IMFASFMutualExclusion pIMutex);

        [PreserveSig]
        HResult RemoveMutualExclusion(
            [In] int dwMutexIndex);

        [PreserveSig]
        HResult CreateMutualExclusion(
            out IMFASFMutualExclusion ppIMutex);

        [PreserveSig]
        HResult GetStreamPrioritization(
#if ALLOW_UNTESTED_INTERFACES
            out IMFASFStreamPrioritization ppIStreamPrioritization);
#else
            [MarshalAs(UnmanagedType.IUnknown)] out object ppIStreamPrioritization);
#endif

        [PreserveSig]
        HResult AddStreamPrioritization(
#if ALLOW_UNTESTED_INTERFACES
            [In] IMFASFStreamPrioritization pIStreamPrioritization);
#else
            [MarshalAs(UnmanagedType.IUnknown)] object pIStreamPrioritization);
#endif

        [PreserveSig]
        HResult RemoveStreamPrioritization();

        [PreserveSig]
        HResult CreateStreamPrioritization(
#if ALLOW_UNTESTED_INTERFACES
            out IMFASFStreamPrioritization ppIStreamPrioritization);
#else
            [MarshalAs(UnmanagedType.IUnknown)] out object ppIStreamPrioritization);
#endif

        [PreserveSig]
        HResult Clone(
            out IMFASFProfile ppIProfile);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("9E8AE8D2-DBBD-4200-9ACA-06E6DF484913"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFASFStreamConfig : IMFAttributes
    {
        #region IMFAttributes methods

        [PreserveSig]
        new HResult GetItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalCookie = "IMFASFStreamConfig.GetItem", MarshalTypeRef = typeof(PVMarshaler))] PropVariant pValue
            );

        [PreserveSig]
        new HResult GetItemType(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out MFAttributeType pType
            );

        [PreserveSig]
        new HResult CompareItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] ConstPropVariant Value,
            [MarshalAs(UnmanagedType.Bool)] out bool pbResult
            );

        [PreserveSig]
        new HResult Compare(
            [MarshalAs(UnmanagedType.Interface)] IMFAttributes pTheirs,
            MFAttributesMatchType MatchType,
            [MarshalAs(UnmanagedType.Bool)] out bool pbResult
            );

        [PreserveSig]
        new HResult GetUINT32(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int punValue
            );

        [PreserveSig]
        new HResult GetUINT64(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out long punValue
            );

        [PreserveSig]
        new HResult GetDouble(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out double pfValue
            );

        [PreserveSig]
        new HResult GetGUID(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out Guid pguidValue
            );

        [PreserveSig]
        new HResult GetStringLength(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int pcchLength
            );

        [PreserveSig]
        new HResult GetString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszValue,
            int cchBufSize,
            out int pcchLength
            );

        [PreserveSig]
        new HResult GetAllocatedString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [MarshalAs(UnmanagedType.LPWStr)] out string ppwszValue,
            out int pcchLength
            );

        [PreserveSig]
        new HResult GetBlobSize(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int pcbBlobSize
            );

        [PreserveSig]
        new HResult GetBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [Out, MarshalAs(UnmanagedType.LPArray)] byte[] pBuf,
            int cbBufSize,
            out int pcbBlobSize
            );

        // Use GetBlob instead of this
        [PreserveSig]
        new HResult GetAllocatedBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out IntPtr ip,  // Read w/Marshal.Copy, Free w/Marshal.FreeCoTaskMem
            out int pcbSize
            );

        [PreserveSig]
        new HResult GetUnknown(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppv
            );

        [PreserveSig]
        new HResult SetItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] ConstPropVariant Value
            );

        [PreserveSig]
        new HResult DeleteItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey
            );

        [PreserveSig]
        new HResult DeleteAllItems();

        [PreserveSig]
        new HResult SetUINT32(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            int unValue
            );

        [PreserveSig]
        new HResult SetUINT64(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            long unValue
            );

        [PreserveSig]
        new HResult SetDouble(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            double fValue
            );

        [PreserveSig]
        new HResult SetGUID(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidValue
            );

        [PreserveSig]
        new HResult SetString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPWStr)] string wszValue
            );

        [PreserveSig]
        new HResult SetBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pBuf,
            int cbBufSize
            );

        [PreserveSig]
        new HResult SetUnknown(
            [MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.IUnknown)] object pUnknown
            );

        [PreserveSig]
        new HResult LockStore();

        [PreserveSig]
        new HResult UnlockStore();

        [PreserveSig]
        new HResult GetCount(
            out int pcItems
            );

        [PreserveSig]
        new HResult GetItemByIndex(
            int unIndex,
            out Guid pguidKey,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalCookie = "IMFASFStreamConfig.GetItemByIndex", MarshalTypeRef = typeof(PVMarshaler))] PropVariant pValue
            );

        [PreserveSig]
        new HResult CopyAllItems(
            [In, MarshalAs(UnmanagedType.Interface)] IMFAttributes pDest
            );

        #endregion

        [PreserveSig]
        HResult GetStreamType(
            out Guid pguidStreamType);

        [PreserveSig]
        short GetStreamNumber();

        [PreserveSig]
        HResult SetStreamNumber(
            [In] short wStreamNum);

        [PreserveSig]
        HResult GetMediaType(
            out IMFMediaType ppIMediaType);

        [PreserveSig]
        HResult SetMediaType(
            [In] IMFMediaType pIMediaType);

        [PreserveSig]
        HResult GetPayloadExtensionCount(
            out short pcPayloadExtensions);

        [PreserveSig]
        HResult GetPayloadExtension(
            [In] short wPayloadExtensionNumber,
            out Guid pguidExtensionSystemID,
            out short pcbExtensionDataSize,
            IntPtr pbExtensionSystemInfo,
            ref int pcbExtensionSystemInfo);

        [PreserveSig]
        HResult AddPayloadExtension(
            [In, MarshalAs(UnmanagedType.Struct)] Guid guidExtensionSystemID,
            [In] short cbExtensionDataSize,
            IntPtr pbExtensionSystemInfo,
            [In] int cbExtensionSystemInfo);

        [PreserveSig]
        HResult RemoveAllPayloadExtensions();

        [PreserveSig]
        HResult Clone(
            out IMFASFStreamConfig ppIStreamConfig);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("d01bad4a-4fa0-4a60-9349-c27e62da9d41"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFASFStreamSelector
    {
        [PreserveSig]
        HResult GetStreamCount(
            out int pcStreams);

        [PreserveSig]
        HResult GetOutputCount(
            out int pcOutputs);

        [PreserveSig]
        HResult GetOutputStreamCount(
            [In] int dwOutputNum,
            out int pcStreams);

        [PreserveSig]
        HResult GetOutputStreamNumbers(
            [In] int dwOutputNum,
            [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I2)] short[] rgwStreamNumbers);

        [PreserveSig]
        HResult GetOutputFromStream(
            [In] short wStreamNum,
            out int pdwOutput);

        [PreserveSig]
        HResult GetOutputOverride(
            [In] int dwOutputNum,
            out ASFSelectionStatus pSelection);

        [PreserveSig]
        HResult SetOutputOverride(
            [In] int dwOutputNum,
            [In] ASFSelectionStatus Selection);

        [PreserveSig]
        HResult GetOutputMutexCount(
            [In] int dwOutputNum,
            out int pcMutexes);

        [PreserveSig]
        HResult GetOutputMutex(
            [In] int dwOutputNum,
            [In] int dwMutexNum,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppMutex);

        [PreserveSig]
        HResult SetOutputMutexSelection(
            [In] int dwOutputNum,
            [In] int dwMutexNum,
            [In] short wSelectedRecord);

        [PreserveSig]
        HResult GetBandwidthStepCount(
            out int pcStepCount);

        [PreserveSig]
        HResult GetBandwidthStep(
            [In] int dwStepNum,
            out int pdwBitrate,
            [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I2)] short[] rgwStreamNumbers,
            [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I4)] ASFSelectionStatus[] rgSelections);

        [PreserveSig]
        HResult BitrateToStepNumber(
            [In] int dwBitrate,
            out int pdwStepNum);

        [PreserveSig]
        HResult SetStreamSelectorFlags(
            [In] MFAsfStreamSelectorFlags dwStreamSelectorFlags);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("12558291-E399-11D5-BC2A-00B0D0F3F4AB"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFASFMutualExclusion
    {
        [PreserveSig]
        HResult GetType(
            out Guid pguidType);

        [PreserveSig]
        HResult SetType(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidType);

        [PreserveSig]
        HResult GetRecordCount(
            out int pdwRecordCount);

        [PreserveSig]
        HResult GetStreamsForRecord(
            [In] int dwRecordNumber,
            [In, Out, MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.I4)] short [] pwStreamNumArray,
            ref int pcStreams);

        [PreserveSig]
        HResult AddStreamForRecord(
            [In] int dwRecordNumber,
            [In] short wStreamNumber);

        [PreserveSig]
        HResult RemoveStreamFromRecord(
            [In] int dwRecordNumber,
            [In] short wStreamNumber);

        [PreserveSig]
        HResult RemoveRecord(
            [In] int dwRecordNumber);

        [PreserveSig]
        HResult AddRecord(
            out int pdwRecordNumber);

        [PreserveSig]
        HResult Clone(
            out IMFASFMutualExclusion ppIMutex);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("12558295-E399-11D5-BC2A-00B0D0F3F4AB"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFASFSplitter
    {
        [PreserveSig]
        HResult Initialize(
            [In] IMFASFContentInfo pIContentInfo);

        [PreserveSig]
        HResult SetFlags(
            [In] MFASFSplitterFlags dwFlags);

        [PreserveSig]
        HResult GetFlags(
            out MFASFSplitterFlags pdwFlags);

        [PreserveSig]
        HResult SelectStreams(
            [In, MarshalAs(UnmanagedType.LPArray)] short[] pwStreamNumbers,
            [In] short wNumStreams);

        [PreserveSig]
        HResult GetSelectedStreams(
            [Out, MarshalAs(UnmanagedType.LPArray)] short[] pwStreamNumbers,
            ref short pwNumStreams);

        [PreserveSig]
        HResult ParseData(
            [In] IMFMediaBuffer pIBuffer,
            [In] int cbBufferOffset,
            [In] int cbLength);

        [PreserveSig]
        HResult GetNextSample(
            out ASFStatusFlags pdwStatusFlags,
            out short pwStreamNumber,
            out IMFSample ppISample);

        [PreserveSig]
        HResult Flush();

        [PreserveSig]
        HResult GetLastSendTime(
            out int pdwLastSendTime);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("B1DCA5CD-D5DA-4451-8E9E-DB5C59914EAD"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFASFContentInfo
    {
        [PreserveSig]
        HResult GetHeaderSize(
            [In] IMFMediaBuffer pIStartOfContent,
            out long cbHeaderSize);

        [PreserveSig]
        HResult ParseHeader(
            [In] IMFMediaBuffer pIHeaderBuffer,
            [In] long cbOffsetWithinHeader);

        [PreserveSig]
        HResult GenerateHeader(
            [In] IMFMediaBuffer pIHeader,
            out int pcbHeader);

        [PreserveSig]
        HResult GetProfile(
            out IMFASFProfile ppIProfile);

        [PreserveSig]
        HResult SetProfile(
            [In] IMFASFProfile pIProfile);

        [PreserveSig]
        HResult GeneratePresentationDescriptor(
            out IMFPresentationDescriptor ppIPresentationDescriptor);

        [PreserveSig]
        HResult GetEncodingConfigurationPropertyStore(
            [In] short wStreamNumber,
            out IPropertyStore ppIStore);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("53590F48-DC3B-4297-813F-787761AD7B3E"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFASFIndexer
    {
        [PreserveSig]
        HResult SetFlags(
            [In] MFAsfIndexerFlags dwFlags);

        [PreserveSig]
        HResult GetFlags(
            out MFAsfIndexerFlags pdwFlags);

        [PreserveSig]
        HResult Initialize(
            [In] IMFASFContentInfo pIContentInfo);

        [PreserveSig]
        HResult GetIndexPosition(
            [In] IMFASFContentInfo pIContentInfo,
            out long pcbIndexOffset);

        [PreserveSig]
        HResult SetIndexByteStreams(
            [In, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.IUnknown)] IMFByteStream[] ppIByteStreams,
            [In] int cByteStreams);

        [PreserveSig]
        HResult GetIndexByteStreamCount(
            out int pcByteStreams);

        [PreserveSig]
        HResult GetIndexStatus(
            [In, MarshalAs(UnmanagedType.LPStruct)] ASFIndexIdentifier pIndexIdentifier,
            [Out, MarshalAs(UnmanagedType.Bool)] out bool pfIsIndexed,
            IntPtr pbIndexDescriptor,
            ref int pcbIndexDescriptor);

        [PreserveSig]
        HResult SetIndexStatus(
            [In] IntPtr pbIndexDescriptor,
            [In] int cbIndexDescriptor,
            [In, MarshalAs(UnmanagedType.Bool)] bool fGenerateIndex);

        [PreserveSig]
        HResult GetSeekPositionForValue(
            [In, MarshalAs(UnmanagedType.LPStruct)] ConstPropVariant pvarValue,
            [In, MarshalAs(UnmanagedType.LPStruct)] ASFIndexIdentifier pIndexIdentifier,
            out long pcbOffsetWithinData,
            IntPtr phnsApproxTime,
            out int pdwPayloadNumberOfStreamWithinPacket);

        [PreserveSig]
        HResult GenerateIndexEntries(
            [In] IMFSample pIASFPacketSample);

        [PreserveSig]
        HResult CommitIndex(
            [In] IMFASFContentInfo pIContentInfo);

        [PreserveSig]
        HResult GetIndexWriteSpace(
            out long pcbIndexWriteSpace);

        [PreserveSig]
        HResult GetCompletedIndex(
            [In] IMFMediaBuffer pIIndexBuffer,
            [In] long cbOffsetWithinIndex);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("57BDD80A-9B38-4838-B737-C58F670D7D4F"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFASFMultiplexer
    {
        [PreserveSig]
        HResult Initialize(
            [In] IMFASFContentInfo pIContentInfo);

        [PreserveSig]
        HResult SetFlags(
            [In] MFASFMultiplexerFlags dwFlags);

        [PreserveSig]
        HResult GetFlags(
            out MFASFMultiplexerFlags pdwFlags);

        [PreserveSig]
        HResult ProcessSample(
            [In] short wStreamNumber,
            [In] IMFSample pISample,
            [In] long hnsTimestampAdjust);

        [PreserveSig]
        HResult GetNextPacket(
            out ASFStatusFlags pdwStatusFlags,
            out IMFSample ppIPacket);

        [PreserveSig]
        HResult Flush();

        [PreserveSig]
        HResult End(
            [In] IMFASFContentInfo pIContentInfo);

        [PreserveSig]
        HResult GetStatistics(
            [In] short wStreamNumber,
            out ASFMuxStatistics pMuxStats);

        [PreserveSig]
        HResult SetSyncTolerance(
            [In] int msSyncTolerance);
    }

    #endregion
}
