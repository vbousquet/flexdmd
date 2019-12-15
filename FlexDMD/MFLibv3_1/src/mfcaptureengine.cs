/* license

mfcaptureengine.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

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
    #region Declarations

    [UnmanagedName("MF_CAPTURE_ENGINE_DEVICE_TYPE")]
    public enum MF_CAPTURE_ENGINE_DEVICE_TYPE
    {
        Audio = 0x00000000,
        Video = 0x00000001
    }

    [UnmanagedName("MF_CAPTURE_ENGINE_SINK_TYPE")]
    public enum MF_CAPTURE_ENGINE_SINK_TYPE
    {
        Record = 0x00000000,
        Preview = 0x00000001,
        Photo = 0x00000002
    }

    [UnmanagedName("MF_CAPTURE_ENGINE_STREAM_CATEGORY")]
    public enum MF_CAPTURE_ENGINE_STREAM_CATEGORY
    {
        VideoPreview = 0x00000000,
        VideoCapture = 0x00000001,
        PhotoIndependent = 0x00000002,
        PhotoDependent = 0x00000003,
        Audio = 0x00000004,
        Unsupported = 0x00000005
    }

    #endregion

    #region Interfaces

#if ALLOW_UNTESTED_INTERFACES
    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("e37ceed7-340f-4514-9f4d-9c2ae026100b")]
    public interface IMFCaptureEngineOnSampleCallback2 : IMFCaptureEngineOnSampleCallback
    {
        #region IMFCaptureEngineOnSampleCallback methods

        [PreserveSig]
        new HResult OnSample(
            IMFSample pSample
            );

        #endregion

        [PreserveSig]
        HResult OnSynchronizedEvent(
            IMFMediaEvent pEvent
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("f9e4219e-6197-4b5e-b888-bee310ab2c59")]
    public interface IMFCaptureSink2 : IMFCaptureSink
    {
        #region IMFCaptureEngineOnSampleCallback methods

        [PreserveSig]
        new HResult GetOutputMediaType(
            int dwSinkStreamIndex,
            out IMFMediaType ppMediaType
            );

        [PreserveSig]
        new HResult GetService(
            int dwSinkStreamIndex,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid rguidService,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppUnknown
            );

        [PreserveSig]
        new HResult AddStream(
            int dwSourceStreamIndex,
            IMFMediaType pMediaType,
            IMFAttributes pAttributes,
            out int pdwSinkStreamIndex
            );

        [PreserveSig]
        new HResult Prepare();

        [PreserveSig]
        new HResult RemoveAllStreams();

        #endregion

        [PreserveSig]
        HResult SetOutputMediaType(
            int dwStreamIndex,
            IMFMediaType pMediaType,
            IMFAttributes pEncodingAttributes
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("19f68549-ca8a-4706-a4ef-481dbc95e12c")]
    public interface IMFCapturePhotoConfirmation
    {
        [PreserveSig]
        HResult SetPhotoConfirmationCallback(
            IMFAsyncCallback pNotificationCallback
            );

        [PreserveSig]
        HResult SetPixelFormat(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid subtype
            );

        [PreserveSig]
        HResult GetPixelFormat(
            out Guid subtype
            );
    }
#endif

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("aeda51c0-9025-4983-9012-de597b88b089"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFCaptureEngineOnEventCallback
    {
        [PreserveSig]
        HResult OnEvent(
            IMFMediaEvent pEvent
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("52150b82-ab39-4467-980f-e48bf0822ecd"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFCaptureEngineOnSampleCallback
    {
        [PreserveSig]
        HResult OnSample(
            IMFSample pSample
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("72d6135b-35e9-412c-b926-fd5265f2a885"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFCaptureSink
    {
        [PreserveSig]
        HResult GetOutputMediaType(
            int dwSinkStreamIndex,
            out IMFMediaType ppMediaType
            );

        [PreserveSig]
        HResult GetService(
            int dwSinkStreamIndex,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid rguidService,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppUnknown
            );

        [PreserveSig]
        HResult AddStream(
            int dwSourceStreamIndex,
            IMFMediaType pMediaType,
            IMFAttributes pAttributes,
            out int pdwSinkStreamIndex
            );

        [PreserveSig]
        HResult Prepare();

        [PreserveSig]
        HResult RemoveAllStreams();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("3323b55a-f92a-4fe2-8edc-e9bfc0634d77"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFCaptureRecordSink : IMFCaptureSink
    {
        #region IMFCaptureSink Methods

        [PreserveSig]
        new HResult GetOutputMediaType(
            int dwSinkStreamIndex,
            out IMFMediaType ppMediaType
            );

        [PreserveSig]
        new HResult GetService(
            int dwSinkStreamIndex,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid rguidService,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppUnknown
            );

        [PreserveSig]
        new HResult AddStream(
            int dwSourceStreamIndex,
            IMFMediaType pMediaType,
            IMFAttributes pAttributes,
            out int pdwSinkStreamIndex
            );

        [PreserveSig]
        new HResult Prepare();

        [PreserveSig]
        new HResult RemoveAllStreams();

        #endregion

        [PreserveSig]
        HResult SetOutputByteStream(
            IMFByteStream pByteStream,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidContainerType
            );

        [PreserveSig]
        HResult SetOutputFileName(
            [MarshalAs(UnmanagedType.LPWStr)] string fileName
            );

        [PreserveSig]
        HResult SetSampleCallback(
            int dwStreamSinkIndex,
            IMFCaptureEngineOnSampleCallback pCallback
            );

        [PreserveSig]
        HResult SetCustomSink(
            IMFMediaSink pMediaSink
            );

        [PreserveSig]
        HResult GetRotation(
            int dwStreamIndex,
            out int pdwRotationValue
            );

        [PreserveSig]
        HResult SetRotation(
            int dwStreamIndex,
            int dwRotationValue
        );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("77346cfd-5b49-4d73-ace0-5b52a859f2e0"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFCapturePreviewSink : IMFCaptureSink
    {
        #region IMFCaptureSink Methods

        [PreserveSig]
        new HResult GetOutputMediaType(
            int dwSinkStreamIndex,
            out IMFMediaType ppMediaType
            );

        [PreserveSig]
        new HResult GetService(
            int dwSinkStreamIndex,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid rguidService,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppUnknown
            );

        [PreserveSig]
        new HResult AddStream(
            int dwSourceStreamIndex,
            IMFMediaType pMediaType,
            IMFAttributes pAttributes,
            out int pdwSinkStreamIndex
            );

        [PreserveSig]
        new HResult Prepare();

        [PreserveSig]
        new HResult RemoveAllStreams();

        #endregion

        [PreserveSig]
        HResult SetRenderHandle(
            IntPtr handle
            );

        [PreserveSig]
        HResult SetRenderSurface(
            [MarshalAs(UnmanagedType.IUnknown)] object pSurface
            );

        [PreserveSig]
        HResult UpdateVideo(
            [In] MFVideoNormalizedRect pSrc,
            [In] MFRect pDst,
            [In] MFInt pBorderClr
            );

        [PreserveSig]
        HResult SetSampleCallback(
            int dwStreamSinkIndex,
            IMFCaptureEngineOnSampleCallback pCallback
            );

        [PreserveSig]
        HResult GetMirrorState(
            [MarshalAs(UnmanagedType.Bool)] out bool pfMirrorState
            );

        [PreserveSig]
        HResult SetMirrorState(
            [MarshalAs(UnmanagedType.Bool)] bool fMirrorState
            );

        [PreserveSig]
        HResult GetRotation(
            int dwStreamIndex,
            out int pdwRotationValue
            );

        [PreserveSig]
        HResult SetRotation(
            int dwStreamIndex,
            int dwRotationValue
            );

        [PreserveSig]
        HResult SetCustomSink(
            IMFMediaSink pMediaSink
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("d2d43cc8-48bb-4aa7-95db-10c06977e777"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFCapturePhotoSink : IMFCaptureSink
    {
        #region IMFCaptureSink Methods

        [PreserveSig]
        new HResult GetOutputMediaType(
            int dwSinkStreamIndex,
            out IMFMediaType ppMediaType
            );

        [PreserveSig]
        new HResult GetService(
            int dwSinkStreamIndex,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid rguidService,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppUnknown
            );

        [PreserveSig]
        new HResult AddStream(
            int dwSourceStreamIndex,
            IMFMediaType pMediaType,
            IMFAttributes pAttributes,
            out int pdwSinkStreamIndex
            );

        [PreserveSig]
        new HResult Prepare();

        [PreserveSig]
        new HResult RemoveAllStreams();

        #endregion

        [PreserveSig]
        HResult SetOutputFileName(
            [MarshalAs(UnmanagedType.LPWStr)] string fileName
            );

        [PreserveSig]
        HResult SetSampleCallback(
            IMFCaptureEngineOnSampleCallback pCallback
            );

        [PreserveSig]
        HResult SetOutputByteStream(
            IMFByteStream pByteStream
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("439a42a8-0d2c-4505-be83-f79b2a05d5c4"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFCaptureSource
    {
        [PreserveSig]
        HResult GetCaptureDeviceSource(
            MF_CAPTURE_ENGINE_DEVICE_TYPE mfCaptureEngineDeviceType,
            out IMFMediaSource ppMediaSource
            );

        [PreserveSig]
        HResult GetCaptureDeviceActivate(
            MF_CAPTURE_ENGINE_DEVICE_TYPE mfCaptureEngineDeviceType,
            out IMFActivate ppActivate
            );

        [PreserveSig]
        HResult GetService(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid rguidService,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppUnknown
            );

        [PreserveSig]
        HResult AddEffect(
            int dwSourceStreamIndex,
            [MarshalAs(UnmanagedType.IUnknown)] object pUnknown
            );

        [PreserveSig]
        HResult RemoveEffect(
            int dwSourceStreamIndex,
            [MarshalAs(UnmanagedType.IUnknown)] object pUnknown
            );

        [PreserveSig]
        HResult RemoveAllEffects(
            int dwSourceStreamIndex
            );

        [PreserveSig]
        HResult GetAvailableDeviceMediaType(
            int dwSourceStreamIndex,
            int dwMediaTypeIndex,
            out IMFMediaType ppMediaType
            );

        [PreserveSig]
        HResult SetCurrentDeviceMediaType(
            int dwSourceStreamIndex,
            IMFMediaType pMediaType
            );

        [PreserveSig]
        HResult GetCurrentDeviceMediaType(
            int dwSourceStreamIndex,
            out IMFMediaType ppMediaType
            );

        [PreserveSig]
        HResult GetDeviceStreamCount(
            out int pdwStreamCount
            );

        [PreserveSig]
        HResult GetDeviceStreamCategory(
            int dwSourceStreamIndex,
            out MF_CAPTURE_ENGINE_STREAM_CATEGORY pStreamCategory
            );

        [PreserveSig]
        HResult GetMirrorState(
            int dwStreamIndex,
            [MarshalAs(UnmanagedType.Bool)] out bool pfMirrorState
            );

        [PreserveSig]
        HResult SetMirrorState(
            int dwStreamIndex,
            [MarshalAs(UnmanagedType.Bool)] bool fMirrorState
            );

        [PreserveSig]
        HResult GetStreamIndexFromFriendlyName(
            int uifriendlyName,
            out int pdwActualStreamIndex
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("a6bba433-176b-48b2-b375-53aa03473207"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFCaptureEngine
    {
        [PreserveSig]
        HResult Initialize(
            IMFCaptureEngineOnEventCallback pEventCallback,
            IMFAttributes pAttributes,
            [MarshalAs(UnmanagedType.IUnknown)] object pAudioSource,
            [MarshalAs(UnmanagedType.IUnknown)] object pVideoSource
            );

        [PreserveSig]
        HResult StartPreview();

        [PreserveSig]
        HResult StopPreview();

        [PreserveSig]
        HResult StartRecord();

        [PreserveSig]
        HResult StopRecord(
            [MarshalAs(UnmanagedType.Bool)] bool bFinalize,
            [MarshalAs(UnmanagedType.Bool)] bool bFlushUnprocessedSamples
            );

        [PreserveSig]
        HResult TakePhoto();

        [PreserveSig]
        HResult GetSink(
            MF_CAPTURE_ENGINE_SINK_TYPE mfCaptureEngineSinkType,
            out IMFCaptureSink ppSink
            );

        [PreserveSig]
        HResult GetSource(
            out IMFCaptureSource ppSource
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("8f02d140-56fc-4302-a705-3a97c78be779"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFCaptureEngineClassFactory
    {
        [PreserveSig]
        HResult CreateInstance(
                [In, MarshalAs(UnmanagedType.LPStruct)] Guid clsid,
                [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
                [MarshalAs(UnmanagedType.IUnknown)] out object ppvObject
                );

    }
    #endregion
}
