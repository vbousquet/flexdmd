/* license

mfmp2dlna.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

// This file is a mess.  While technically part of MF, I'm in no hurry
// to try to get this to work.

using System;
using System.Runtime.InteropServices;
using System.Security;

using MediaFoundation.Misc;

namespace MediaFoundation
{
    [UnmanagedName("CLSID_MPEG2DLNASink"),
    ComImport,
    Guid("fa5fe7c5-6a1d-4b11-b41f-f959d6c76500")]
    public class MPEG2DLNASink
    {
    }

    [StructLayout(LayoutKind.Sequential), UnmanagedName("MFMPEG2DLNASINKSTATS")]
    public struct MFMPEG2DLNASINKSTATS
    {
        long cBytesWritten;
        bool fPAL;
        int fccVideo;
        int dwVideoWidth;
        int dwVideoHeight;
        long cVideoFramesReceived;
        long cVideoFramesEncoded;
        long cVideoFramesSkipped;
        long cBlackVideoFramesEncoded;
        long cVideoFramesDuplicated;
        int cAudioSamplesPerSec;
        int cAudioChannels;
        long cAudioBytesReceived;
        long cAudioFramesEncoded;
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("0C012799-1B61-4C10-BDA9-04445BE5F561")]
    public interface IMFDLNASinkInit
    {
        [PreserveSig]
        HResult Initialize(
            IMFByteStream pByteStream,
            [MarshalAs(UnmanagedType.Bool)] bool fPal
        );
    }

}
