/* license

Misc.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

using System;
using System.Runtime.InteropServices;
using System.IO;

namespace MediaFoundation.Misc
{
    #region Declarations

    [UnmanagedName("MPEG1VIDEOINFO"), StructLayout(LayoutKind.Sequential)]
    public class Mpeg1VideoInfo
    {
        public VideoInfoHeader hdr;
        public int dwStartTimeCode;
        public int cbSequenceHeader;
        public byte[] bSequenceHeader;
    }

    [UnmanagedName("AMINTERLACE_*"), Flags]
    public enum AMInterlace
    {
        None = 0,
        IsInterlaced = 0x00000001,
        OneFieldPerSample = 0x00000002,
        Field1First = 0x00000004,
        Unused = 0x00000008,
        FieldPatternMask = 0x00000030,
        FieldPatField1Only = 0x00000000,
        FieldPatField2Only = 0x00000010,
        FieldPatBothRegular = 0x00000020,
        FieldPatBothIrregular = 0x00000030,
        DisplayModeMask = 0x000000c0,
        DisplayModeBobOnly = 0x00000000,
        DisplayModeWeaveOnly = 0x00000040,
        DisplayModeBobOrWeave = 0x00000080,
    }

    [UnmanagedName("AMCOPYPROTECT_*")]
    public enum AMCopyProtect
    {
        None = 0,
        RestrictDuplication = 0x00000001
    }

    [UnmanagedName("From AMCONTROL_*"), Flags]
    public enum AMControl
    {
        None = 0,
        Used = 0x00000001,
        PadTo4x3 = 0x00000002,
        PadTo16x9 = 0x00000004,
    }

    [Flags, UnmanagedName("SPEAKER_* defines")]
    public enum WaveMask
    {
        None = 0x0,
        FrontLeft = 0x1,
        FrontRight = 0x2,
        FrontCenter = 0x4,
        LowFrequency = 0x8,
        BackLeft = 0x10,
        BackRight = 0x20,
        FrontLeftOfCenter = 0x40,
        FrontRightOfCenter = 0x80,
        BackCenter = 0x100,
        SideLeft = 0x200,
        SideRight = 0x400,
        TopCenter = 0x800,
        TopFrontLeft = 0x1000,
        TopFrontCenter = 0x2000,
        TopFrontRight = 0x4000,
        TopBackLeft = 0x8000,
        TopBackCenter = 0x10000,
        TopBackRight = 0x20000
    }

    [UnmanagedName("MFVideoPadFlags")]
    public enum MFVideoPadFlags
    {
        PAD_TO_None = 0,
        PAD_TO_4x3 = 1,
        PAD_TO_16x9 = 2
    }

    [UnmanagedName("MFVideoSrcContentHintFlags")]
    public enum MFVideoSrcContentHintFlags
    {
        None = 0,
        F16x9 = 1,
        F235_1 = 2
    }

    [Flags, UnmanagedName("STGC")]
    public enum STGC
    {
        Default = 0,
        Overwrite = 1,
        OnlyIfCurrent = 2,
        DangerouslyCommitMerelyToDiskCache = 4,
        Consolidate = 8
    }

    [UnmanagedName("STATFLAG")]
    public enum STATFLAG
    {
        Default = 0,
        NoName = 1,
        NoOpen = 2
    }

    [UnmanagedName("STGTY")]
    public enum STGTY
    {
        None = 0,
        Storage = 1,
        Stream = 2,
        LockBytes = 3,
        Property = 4
    }

    [UnmanagedName("MT_CUSTOM_VIDEO_PRIMARIES"), StructLayout(LayoutKind.Sequential)]
    public struct MT_CustomVideoPrimaries
    {
        public float fRx;
        public float fRy;
        public float fGx;
        public float fGy;
        public float fBx;
        public float fBy;
        public float fWx;
        public float fWy;
    }

    /// <summary>
    /// When you are done with an instance of this class,
    /// it should be released with FreeAMMediaType() to avoid leaking
    /// </summary>
    [UnmanagedName("AM_MEDIA_TYPE"), StructLayout(LayoutKind.Sequential)]
    public class AMMediaType
    {
        public Guid majorType;
        public Guid subType;
        [MarshalAs(UnmanagedType.Bool)]
        public bool fixedSizeSamples;
        [MarshalAs(UnmanagedType.Bool)]
        public bool temporalCompression;
        public int sampleSize;
        public Guid formatType;
        public IntPtr unkPtr; // IUnknown Pointer
        public int formatSize;
        public IntPtr formatPtr; // Pointer to a buff determined by formatType
    }

    [UnmanagedName("VIDEOINFOHEADER"), StructLayout(LayoutKind.Sequential)]
    public class VideoInfoHeader
    {
        public MFRect SrcRect;
        public MFRect TargetRect;
        public int BitRate;
        public int BitErrorRate;
        public long AvgTimePerFrame;
        public BitmapInfoHeader BmiHeader;  // Custom marshaler?
    }

    [UnmanagedName("VIDEOINFOHEADER2"), StructLayout(LayoutKind.Sequential)]
    public class VideoInfoHeader2
    {
        public MFRect SrcRect;
        public MFRect TargetRect;
        public int BitRate;
        public int BitErrorRate;
        public long AvgTimePerFrame;
        public AMInterlace InterlaceFlags;
        public AMCopyProtect CopyProtectFlags;
        public int PictAspectRatioX;
        public int PictAspectRatioY;
        public AMControl ControlFlags;
        public int Reserved2;
        public BitmapInfoHeader BmiHeader;  // Custom marshaler?
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4), UnmanagedName("PROPERTYKEY")]
    public class PropertyKey
    {
        public Guid fmtid;
        public int pID;

        public PropertyKey()
        {
        }

        public PropertyKey(Guid f, int p)
        {
            fmtid = f;
            pID = p;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4), UnmanagedName("STATSTG")]
    public struct STATSTG
    {
        public IntPtr pwcsName;
        public STGTY type;
        public long cbSize;
        public long mtime;
        public long ctime;
        public long atime;
        public int grfMode;
        public int grfLocksSupported;
        public Guid clsid;
        public int grfStateBits;
        public int reserved;
    }

    #endregion

    #region Generic Interfaces

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("71604b0f-97b0-4764-8577-2f13e98a1422")]
    public interface INamedPropertyStore
    {
        [PreserveSig]
        HResult GetNamedValue(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszName,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalCookie = "INamedPropertyStore.GetNamedValue", MarshalTypeRef = typeof(PVMarshaler))] PropVariant pValue
        );

        [PreserveSig]
        HResult SetNamedValue(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszName,
            [In, MarshalAs(UnmanagedType.LPStruct)] ConstPropVariant propvar);

        [PreserveSig]
        HResult GetNameCount(
            out int pdwCount);

        [PreserveSig]
        HResult GetNameAt(
            int iProp,
            [MarshalAs(UnmanagedType.BStr)] out string pbstrName);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99")]
    public interface IPropertyStore
    {
        [PreserveSig]
        HResult GetCount(
            out int cProps
            );

        [PreserveSig]
        HResult GetAt(
            [In] int iProp,
            [Out] PropertyKey pkey
            );

        [PreserveSig]
        HResult GetValue(
            [In, MarshalAs(UnmanagedType.LPStruct)] PropertyKey key,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalCookie = "IPropertyStore.GetValue", MarshalTypeRef = typeof(PVMarshaler))] PropVariant pv
            );

        [PreserveSig]
        HResult SetValue(
            [In, MarshalAs(UnmanagedType.LPStruct)] PropertyKey key,
            [In, MarshalAs(UnmanagedType.LPStruct)] ConstPropVariant propvar
            );

        [PreserveSig]
        HResult Commit();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("0000000c-0000-0000-C000-000000000046")]
    public interface ISequentialStream
    {
        [PreserveSig]
        HResult Read(
            [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] buffer,
            [In] int bytesCount,
            [In] IntPtr bytesRead
            );

        [PreserveSig]
        HResult Write(
            [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] buffer,
            [In] int bytesCount,
            [In] IntPtr bytesWritten
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("0c733a30-2a1c-11ce-ade5-00aa0044773d")]
    public interface IStream : ISequentialStream
    {
        #region ISequentialStream Methods

        [PreserveSig]
        new HResult Read(
            [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] buffer,
            [In] int bytesCount,
            [In] IntPtr bytesRead
            );

        [PreserveSig]
        new HResult Write(
            [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] buffer,
            [In] int bytesCount,
            [In] IntPtr bytesWritten
            );

        #endregion

        [PreserveSig]
        HResult Seek(
            [In] long offset,
            [In] SeekOrigin origin,
            [In] IntPtr newPosition
            );

        [PreserveSig]
        HResult SetSize(
            [In] long newSize
            );

        [PreserveSig]
        HResult CopyTo(
            [In] IStream otherStream,
            [In] long bytesCount,
            [In] IntPtr bytesRead,
            [In] IntPtr bytesWritten
            );

        [PreserveSig]
        HResult Commit(
            [In] STGC commitFlags
            );

        [PreserveSig]
        HResult Revert();

        [PreserveSig]
        HResult LockRegion(
            [In] long offset,
            [In] long bytesCount,
            [In] int lockType
            );

        [PreserveSig]
        HResult UnlockRegion(
            [In] long offset,
            [In] long bytesCount,
            [In] int lockType
            );

        [PreserveSig]
        HResult Stat(
            [Out] out STATSTG statstg,
            [In] STATFLAG statFlag
            );

        [PreserveSig]
        HResult Clone(
            [Out] out IStream clonedStream
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("0000010c-0000-0000-C000-000000000046"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPersist
    {
        [PreserveSig]
        HResult GetClassID(out Guid pClassID);
    }

    #endregion
}
