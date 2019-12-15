/* license

MF.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MediaFoundation.EVR;
using MediaFoundation.Interop;
using MediaFoundation.Misc;
using MediaFoundation.ReadWrite;
using MediaFoundation.Transform;

namespace MediaFoundation
{
    public static class MF
    {
        #region ********** Toolkit methods ************************************

        private static int? s_mfVersion = null;

        /// <summary>
        /// Indicate if Media Foundation is available by the computer executing that method.
        /// </summary>
        /// <returns>true if Media Foundation is supported; false otherwise</returns>
        /// <remarks>
        /// <para>
        /// On desktop OS, Media Foundation is only supported on Windows Vista or higher.
        /// On server OS, Media Foundation is only supported on Windows 2008 or higher as an installable feature.
        /// </para>
        /// <para>
        /// The following specific Windows editions don't support Media Foundation without installing the corresponding Media Feature Pack:
        /// <list type="bullet">
        /// <description>Windows N editions</description>
        /// <description>Windows KN editions</description>
        /// </list>
        /// </para>
        /// </remarks>
        public static bool IsSupported()
        {
            return GetIsSupported();
        }

        /// <summary>
        /// Initializes Media Foundation.
        /// </summary>
        /// <param name="startupFlags">Media Foundation startup flags.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult Startup(MFStartup startupFlags)
        {
            if (s_mfVersion == null)
                s_mfVersion = GetMediaFoundationVersion();

            return Startup(s_mfVersion.Value, startupFlags);
        }

        /// <summary>
        /// Initializes Media Foundation.
        /// </summary>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult Startup()
        {
            return Startup(MFStartup.Full);
        }

        /// <summary>
        /// Initializes Media Foundation.
        /// </summary>
        /// <param name="version">Media Foundation version number (0x10070 for Windows Vista or 0x20070 for higher OS versions).</param>
        /// <param name="startupFlags">Media Foundation startup flags.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult Startup(int version, MFStartup startupFlags)
        {
            HResult hr = MFExtern.MFStartup(version, startupFlags);
            return hr;
        }

        /// <summary>
        /// Shuts down the Media Foundation platform.
        /// </summary>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult Shutdown()
        {
            HResult hr = MFExtern.MFShutdown();
            return hr;
        }

        /// <summary>
        /// Retrieves the MIME types that are registered for the source resolver.
        /// </summary>
        /// <param name="supportedMimeTypes">Receives the the supported MIME types.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetSupportedMimeTypes(out string[] supportedMimeTypes)
        {
            supportedMimeTypes = null;

            using (PropVariant result = new PropVariant())
            {
                HResult hr = MFExtern.MFGetSupportedMimeTypes(result);
                if (hr.Succeeded())
                    supportedMimeTypes = result.GetStringArray();

                return hr;
            }
        }

        /// <summary>
        /// Retrieves the URL schemes that are registered for the source resolver.
        /// </summary>
        /// <param name="supportedSchemes">Receives the the supported URL schemes.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetSupportedSchemes(out string[] supportedSchemes)
        {
            supportedSchemes = null;

            using (PropVariant result = new PropVariant())
            {
                HResult hr = MFExtern.MFGetSupportedSchemes(result);
                if (hr.Succeeded())
                    supportedSchemes = result.GetStringArray();

                return hr;
            }
        }

        /// <summary>
        /// Blocks the <see cref="Shutdown"/> method.
        /// </summary>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult LockPlatform()
        {
            return MFExtern.MFLockPlatform();
        }

        /// <summary>
        /// Unlocks the Media Foundation platform after it was locked by a call to the <see cref="LockPlatform"/> method.
        /// </summary>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult UnlockPlatform()
        {
            return MFExtern.MFUnlockPlatform();
        }

        #endregion

        #region ********** IMFAttributes methods ******************************

        /// <summary>
        /// Creates an empty attribute store.
        /// </summary>
        /// <param name="initialSize">The initial number of elements allocated for the attribute store.</param>
        /// <param name="attributes">Receives an instance of the IMFAttributes interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        /// <remarks>The attribute store grows as needed.</remarks>
        public static HResult CreateAttributes(int initialSize, out IMFAttributes attributes)
        {
            if (initialSize < 0)
                throw new ArgumentOutOfRangeException("initialSize");

            HResult hr = MFExtern.MFCreateAttributes(out attributes, initialSize);
            return hr;
        }

        /// <summary>
        /// Creates an empty attribute store.
        /// </summary>
        /// <param name="initialSize">The initial number of elements allocated for the attribute store.</param>
        /// <returns>Return an instance of the IMFAttributes interface</returns>
        /// <remarks>The attribute store grows as needed.</remarks>
        public static IMFAttributes CreateAttributes(int initialSize = 0)
        {
            if (initialSize < 0)
                throw new ArgumentOutOfRangeException("initialSize");

            IMFAttributes result = null;

            HResult hr = MFExtern.MFCreateAttributes(out result, initialSize);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates an attribute store and initialize it with values.
        /// </summary>
        /// <param name="initialValues">The initial values.</param>
        /// <returns>Return an instance of the IMFAttributes interface filled with the initial values</returns>
        /// <remarks>The attribute store grows as needed.</remarks>
        public static IMFAttributes CreateAttributes(KeyValuePair<Guid, object>[] initialValues)
        {
            if (initialValues == null)
                throw new ArgumentNullException("initialValues");

            IMFAttributes result = CreateAttributes(initialValues.Length);

            HResult hr = 0;

            foreach (KeyValuePair<Guid, object> value in initialValues)
            {
                if (value.Value is bool)
                {
                    hr = result.SetBoolean(value.Key, (bool)value.Value);
                }
                else if (value.Value is uint)
                {
                    hr = result.SetUINT32(value.Key, (uint)value.Value);
                }
                else if (value.Value is ulong)
                {
                    hr = result.SetUINT64(value.Key, (ulong)value.Value);
                }
                else if (value.Value is TimeSpan)
                {
                    hr = result.SetUINT64(value.Key, (ulong)((TimeSpan)value.Value).Ticks);
                }
                else if (value.Value is double)
                {
                    hr = result.SetDouble(value.Key, (double)value.Value);
                }
                else if (value.Value is Guid)
                {
                    hr = result.SetGUID(value.Key, (Guid)value.Value);
                }
                else if (value.Value is string)
                {
                    hr = result.SetString(value.Key, (string)value.Value);
                }
                else if (value.Value is byte[])
                {
                    byte[] arrayValue = (byte[])value.Value;
                    hr = result.SetBlob(value.Key, arrayValue, arrayValue.Length);
                }
                else if (Marshal.IsComObject(value.Value))
                {
                    hr = result.SetUnknown(value.Key, value.Value);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("value", "Values must be of the following types: Boolean, UInt32, UInt64, TimeSpan, Double, Guid, String, byte[] or a COM visible object.");
                }

                hr.ThrowExceptionOnError();
            }

            return result;
        }

        /// <summary>
        /// Creates an attribute store and initialize it with values.
        /// </summary>
        /// <param name="initialValues">The initial values.</param>
        /// <returns>Return an instance of the IMFAttributes interface filled with the initial values</returns>
        /// <remarks>The attribute store grows as needed.</remarks>
        public static IMFAttributes CreateAttributes(IDictionary<Guid, object> initialValues)
        {
            if (initialValues == null)
                throw new ArgumentNullException("initialValues");

            IMFAttributes result = CreateAttributes(initialValues.Count);

            HResult hr = 0;

            foreach (Guid key in initialValues.Keys)
            {
                object value = initialValues[key];

                if (value is bool)
                {
                    hr = result.SetBoolean(key, (bool)value);
                }
                else if (value is uint)
                {
                    hr = result.SetUINT32(key, (uint)value);
                }
                else if (value is ulong)
                {
                    hr = result.SetUINT64(key, (ulong)value);
                }
                else if (value is TimeSpan)
                {
                    hr = result.SetUINT64(key, (ulong)((TimeSpan)value).Ticks);
                }
                else if (value is double)
                {
                    hr = result.SetDouble(key, (double)value);
                }
                else if (value is Guid)
                {
                    hr = result.SetGUID(key, (Guid)value);
                }
                else if (value is string)
                {
                    hr = result.SetString(key, (string)value);
                }
                else if (value is byte[])
                {
                    byte[] arrayValue = (byte[])value;
                    hr = result.SetBlob(key, arrayValue, arrayValue.Length);
                    hr.ThrowExceptionOnError();
                }
                else if (Marshal.IsComObject(value))
                {
                    hr = result.SetUnknown(key, value);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("initialValues", "Initial values must be of the following types: Boolean, UInt32, UInt64, TimeSpan, Double, Guid, String, byte[] or a COM visible object.");
                }

                hr.ThrowExceptionOnError();
            }

            return result;
        }

        /// <summary>
        /// Clone an attribute store.
        /// </summary>
        /// <param name="from">The source attribute store.</param>
        /// <returns>Returns a duplicated attribute store</returns>
        public static IMFAttributes CloneAttributes(IMFAttributes from)
        {
            if (from == null)
                throw new ArgumentNullException("from");

            IMFAttributes result = CreateAttributes();

            HResult hr = from.CopyAllItems(result);
            hr.ThrowExceptionOnError();

            return result;
        }

        #endregion

        #region ********** IMFByteStream methods ******************************

        /// <summary>
        /// Creates a Microsoft Media Foundation byte stream that wraps a COM stream.
        /// </summary>
        /// <param name="comStream">The COM stream to be wrapped.</param>
        /// <param name="byteStream">Receives an instance of a IMFByteStream interface that wrap the COM stream.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateMFByteStreamOnStream(IStream comStream, out IMFByteStream byteStream)
        {
            return MFExtern.MFCreateMFByteStreamOnStream(comStream, out byteStream);
        }

        /// <summary>
        /// Creates a Microsoft Media Foundation byte stream that wraps a COM stream.
        /// </summary>
        /// <param name="comStream">The COM stream to be wrapped.</param>
        /// <returns>Returns an instance of a IMFByteStream interface that wrap the COM stream</returns>
        public static IMFByteStream CreateMFByteStreamOnStream(IStream comStream)
        {
            if (comStream == null)
                throw new ArgumentNullException("comStream");

            IMFByteStream result = null;

            HResult hr = MFExtern.MFCreateMFByteStreamOnStream(comStream, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates a Microsoft Media Foundation byte stream that wraps a managed stream.
        /// </summary>
        /// <param name="stream">The managed stream to be wrapped.</param>
        /// <param name="byteStream">Receives an instance of a IMFByteStream interface that wrap the managed stream.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateMFByteStreamOnStream(Stream stream, out IMFByteStream byteStream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            IStream comStream = new ComCallableStream(stream);

            return MFExtern.MFCreateMFByteStreamOnStream(comStream, out byteStream);
        }

        /// <summary>
        /// Creates a Microsoft Media Foundation byte stream that wraps a managed stream.
        /// </summary>
        /// <param name="stream">The managed stream to be wrapped.</param>
        /// <returns>Returns an instance of a IMFByteStream interface that wrap the managed stream</returns>
        public static IMFByteStream CreateMFByteStreamOnStream(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            IStream comStream = new ComCallableStream(stream);
            return CreateMFByteStreamOnStream(comStream);
        }

        public static HResult CreateStreamOnMFByteStreamEx(IMFByteStream byteStream, Guid riid, out object obj)
        {
            return MFExtern.MFCreateStreamOnMFByteStreamEx(byteStream, riid, out obj);
        }

        /// <summary>
        /// Returns a COM stream that wraps a Microsoft Media Foundation byte stream.
        /// </summary>
        /// <param name="byteStream">The Media Foundation byte stream to be wrapped.</param>
        /// <param name="comStream">Receives an instance of a IStream interface that wrap the byte stream.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateStreamOnMFByteStream(IMFByteStream byteStream, out IStream comStream)
        {
            return MFExtern.MFCreateStreamOnMFByteStream(byteStream, out comStream);
        }

        /// <summary>
        /// Returns a COM stream that wraps a Microsoft Media Foundation byte stream.
        /// </summary>
        /// <param name="byteStream">The Media Foundation byte stream to be wrapped.</param>
        /// <returns>Returns an instance of a IStream interface that wrap the byte stream</returns>
        public static IStream CreateStreamOnMFByteStream(IMFByteStream byteStream)
        {
            if (byteStream == null)
                throw new ArgumentNullException("byteStream");

            IStream result = null;

            HResult hr = MFExtern.MFCreateStreamOnMFByteStream(byteStream, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Returns a managed stream that wraps a Microsoft Media Foundation byte stream.
        /// </summary>
        /// <param name="byteStream">The Media Foundation byte stream to be wrapped.</param>
        /// <param name="stream">Receives a managed stream that wrap the byte stream.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateManagedStreamOnMFByteStream(IMFByteStream byteStream, out Stream stream)
        {
            if (byteStream == null)
                throw new ArgumentNullException("byteStream");

            stream = null;
            IStream comStream;

            HResult hr = MFExtern.MFCreateStreamOnMFByteStream(byteStream, out comStream);
            if (hr.Succeeded())
                stream = new RuntimeCallableStream(comStream, true);

            return hr;
        }

        /// <summary>
        /// Returns a managed stream that wraps a Microsoft Media Foundation byte stream.
        /// </summary>
        /// <param name="byteStream">The Media Foundation byte stream to be wrapped.</param>
        /// <returns>Returns a managed stream that wrap the byte stream</returns>
        public static Stream CreateManagedStreamOnMFByteStream(IMFByteStream byteStream)
        {
            if (byteStream == null)
                throw new ArgumentNullException("byteStream");

            IStream result = null;

            HResult hr = MFExtern.MFCreateStreamOnMFByteStream(byteStream, out result);
            hr.ThrowExceptionOnError();

            return new RuntimeCallableStream(result, true);
        }

        /// <summary>
        /// Creates a byte stream from a file.
        /// </summary>
        /// <param name="filePath">The file path and name.</param>
        /// <param name="accessMode">The requested access mode.</param>
        /// <param name="openMode">The behavior of the function if the file already exists or does not exist.</param>
        /// <param name="flags">The other file opening flags.</param>
        /// <param name="byteStream">Receives an instance to the IMFByteStream interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateFile(string filePath, MFFileAccessMode accessMode, MFFileOpenMode openMode, MFFileFlags flags, out IMFByteStream byteStream)
        {
            HResult hr = MFExtern.MFCreateFile(accessMode, openMode, flags, filePath, out byteStream);
            return hr;
        }

        /// <summary>
        /// Creates a byte stream from a file.
        /// </summary>
        /// <param name="filePath">The file path and name.</param>
        /// <param name="accessMode">The requested access mode.</param>
        /// <param name="openMode">The behavior of the function if the file already exists or does not exist.</param>
        /// <param name="flags">The other file opening flags.</param>
        /// <returns>Returns an instance to the IMFByteStream interface</returns>
        public static IMFByteStream CreateFile(string filePath, MFFileAccessMode accessMode, MFFileOpenMode openMode, MFFileFlags flags)
        {
            IMFByteStream result = null;

            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");

            HResult hr = MFExtern.MFCreateFile(accessMode, openMode, flags, filePath, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Asynchronously creates a byte stream from a file.
        /// </summary>
        /// <param name="filePath">The file path and name.</param>
        /// <param name="accessMode">The requested access mode.</param>
        /// <param name="openMode">The behavior of the function if the file already exists or does not exist.</param>
        /// <param name="flags">The other file opening flags.</param>
        /// <returns>A task responsible of the byte stream creation</returns>
        public static Task<IMFByteStream> CreateFileAsync(string filePath, MFFileAccessMode accessMode, MFFileOpenMode openMode, MFFileFlags flags)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");

            TaskCompletionSource<IMFByteStream> tcs = new TaskCompletionSource<IMFByteStream>();
            object cancelToken = null;

            HResult hrBegin = MFExtern.MFBeginCreateFile(accessMode, openMode, flags, filePath, new MFAsyncCallback((ar) =>
            {
                IMFByteStream result;

                try
                {
                    HResult hrEnd = MFExtern.MFEndCreateFile(ar, out result);
                    hrEnd.ThrowExceptionOnError();

                    ComClass.SafeRelease(ref cancelToken);

                    tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }

            }), null, out cancelToken);
            hrBegin.ThrowExceptionOnError();

            return tcs.Task;
        }

        /// <summary>
        /// Asynchronously creates a byte stream from a file with cancellation.
        /// </summary>
        /// <param name="filePath">The file path and name.</param>
        /// <param name="accessMode">The requested access mode.</param>
        /// <param name="openMode">The behavior of the function if the file already exists or does not exist.</param>
        /// <param name="flags">The other file opening flags.</param>
        /// <param name="token">A cancellation token to end the asynchronous byte stream creation.</param>
        /// <returns>A task responsible of the byte stream creation</returns>
        public static Task<IMFByteStream> CreateFileAsync(string filePath, MFFileAccessMode accessMode, MFFileOpenMode openMode, MFFileFlags flags, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");

            TaskCompletionSource<IMFByteStream> tcs = new TaskCompletionSource<IMFByteStream>();
            object cancelToken = null;

            CancellationTokenRegistration ctReg = token.Register(() =>
            {
                HResult hrCancel = MFExtern.MFCancelCreateFile(cancelToken);
            });

            HResult hrBegin = MFExtern.MFBeginCreateFile(accessMode, openMode, flags, filePath, new MFAsyncCallback((ar) =>
            {
                IMFByteStream result;

                try
                {
                    if (!token.IsCancellationRequested)
                    {
                        HResult hrEnd = MFExtern.MFEndCreateFile(ar, out result);
                        hrEnd.ThrowExceptionOnError();

                        tcs.SetResult(result);
                    }
                    else
                    {
                        tcs.SetResult(null);
                    }

                    ComClass.SafeRelease(ref cancelToken);
                    ctReg.Dispose();
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }

            }), null, out cancelToken);
            hrBegin.ThrowExceptionOnError();

            return tcs.Task;
        }

        /// <summary>
        /// Creates a byte stream that is backed by a temporary local file.
        /// </summary>
        /// <param name="accessMode">The requested access mode.</param>
        /// <param name="openMode">The behavior of the function if the file already exists or does not exist.</param>
        /// <param name="flags">The other file opening flags.</param>
        /// <param name="byteStream">Receives an instance to the IMFByteStream interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateTempFile(MFFileAccessMode accessMode, MFFileOpenMode openMode, MFFileFlags flags, out IMFByteStream byteStream)
        {
            return MFExtern.MFCreateTempFile(accessMode, openMode, flags, out byteStream);
        }

        /// <summary>
        /// Creates a byte stream that is backed by a temporary local file.
        /// </summary>
        /// <param name="accessMode">The requested access mode.</param>
        /// <param name="openMode">The behavior of the function if the file already exists or does not exist.</param>
        /// <param name="flags">The other file opening flags.</param>
        /// <returns>Returns an instance to the IMFByteStream interface</returns>
        public static IMFByteStream CreateTempFile(MFFileAccessMode accessMode, MFFileOpenMode openMode, MFFileFlags flags)
        {
            IMFByteStream result = null;

            HResult hr = MFExtern.MFCreateTempFile(accessMode, openMode, flags, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates a wrapper for a byte stream.
        /// </summary>
        /// <param name="stream">An instance of the original byte stream.</param>
        /// <param name="streamWrapper">Receives an instance of a byte stream wrapper.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        /// <remarks>The returned wrapper call directly through to the original byte stream methods, except for the IMFByteStream::Close method. Calling Close on the wrapper closes the wrapper object, but leaves the original byte stream open.</remarks>
        public static HResult CreateMFByteStreamWrapper(IMFByteStream stream, out IMFByteStream streamWrapper)
        {
            return MFExtern.MFCreateMFByteStreamWrapper(stream, out streamWrapper);
        }

        #endregion

        #region ********** IMFMediaType methods *******************************

        /// <summary>
        /// Creates an empty media type.
        /// </summary>
        /// <param name="mediaType">Receives an instance to the IMFMediaType interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateMediaType(out IMFMediaType mediaType)
        {
            return MFExtern.MFCreateMediaType(out mediaType);
        }

        /// <summary>
        /// Creates an empty media type.
        /// </summary>
        /// <returns>Returns an instance to the IMFMediaType interface</returns>
        public static IMFMediaType CreateMediaType()
        {
            IMFMediaType result;

            HResult hr = CreateMediaType(out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Converts a Media Foundation audio media type to a WaveFormatEx structure.
        /// </summary>
        /// <param name="mediaType">An instance of the media type.</param>
        /// <param name="flags">WaveFormatEx type.</param>
        /// <param name="waveFormatEx">Receives the converted WaveFormatEx structure.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateWaveFormatExFromMFMediaType(IMFMediaType mediaType, MFWaveFormatExConvertFlags flags, out WaveFormatEx waveFormatEx)
        {
            int structSize;
            return MFExtern.MFCreateWaveFormatExFromMFMediaType(mediaType, out waveFormatEx, out structSize, flags);
        }

        /// <summary>
        /// Converts a Media Foundation audio media type to a WaveFormatEx structure.
        /// </summary>
        /// <param name="mediaType">An instance of the media type.</param>
        /// <param name="waveFormatEx">Receives the converted WaveFormatEx structure.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        /// <remarks>This method create a normal WaveFormatEx structure (ie. not an extensible one)</remarks>
        public static HResult CreateWaveFormatExFromMFMediaType(IMFMediaType mediaType, out WaveFormatEx waveFormatEx)
        {
            int structSize;
            return MFExtern.MFCreateWaveFormatExFromMFMediaType(mediaType, out waveFormatEx, out structSize, MFWaveFormatExConvertFlags.Normal);
        }   
        
        /// <summary>
        /// Create a media type from an MFVideoFormat structure.
        /// </summary>
        /// <param name="videoFormat">An MFVideoFormat structure that describes the media type. The caller must fill in the structure members before calling this function.</param>
        /// <param name="mediaType">Receives an instance of the media type.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateMediaTypeFromMFVideoFormat(MFVideoFormat videoFormat, out IMFMediaType mediaType)
        {
            IMFMediaType result = null;

            HResult hr = MFExtern.MFCreateMediaType(out mediaType);
            if (hr.Succeeded())
            {
                hr = MFExtern.MFInitMediaTypeFromMFVideoFormat(result, videoFormat, Marshal.SizeOf(videoFormat));
                if (hr.Succeeded())
                {
                    mediaType = result;
                    return hr;
                }
            }

            ComClass.SafeRelease(ref result);
            return hr;
        }

        /// <summary>
        /// Create a media type from an MFVideoFormat structure.
        /// </summary>
        /// <param name="videoFormat">An MFVideoFormat structure that describes the media type. The caller must fill in the structure members before calling this function.</param>
        /// <returns>Returns an instance of the media type</returns>
        public static IMFMediaType CreateMediaTypeFromMFVideoFormat(MFVideoFormat videoFormat)
        {
            IMFMediaType result;

            HResult hr = CreateMediaTypeFromMFVideoFormat(videoFormat, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Create a media type from an AMMediaType structure.
        /// </summary>
        /// <param name="amMediaType">An AMMediaType structure that describes the media type. The caller must fill in the structure members before calling this function.</param>
        /// <param name="mediaType">Receives an instance of the media type.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateMediaTypeFromAMMediaType(AMMediaType amMediaType, out IMFMediaType mediaType)
        {
            IMFMediaType result = null;

            HResult hr = MFExtern.MFCreateMediaType(out mediaType);
            if (hr.Succeeded())
            {
                hr = MFExtern.MFInitMediaTypeFromAMMediaType(result, amMediaType);
                if (hr.Succeeded())
                {
                    mediaType = result;
                    return hr;
                }
            }

            ComClass.SafeRelease(ref result);
            return hr;
        }

        /// <summary>
        /// Create a media type from an AMMediaType structure.
        /// </summary>
        /// <param name="amMediaType">An AMMediaType structure that describes the media type. The caller must fill in the structure members before calling this function.</param>
        /// <returns>Returns an instance of the media type</returns>
        public static IMFMediaType CreateMediaTypeFromAMMediaType(AMMediaType amMediaType)
        {
            IMFMediaType result;

            HResult hr = CreateMediaTypeFromAMMediaType(amMediaType, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Create a media type from an VideoInfoHeader structure.
        /// </summary>
        /// <param name="vih">An VideoInfoHeader structure that describes the media type. The caller must fill in the structure members before calling this function.</param>
        /// <param name="subType">The video subtype.</param>
        /// <param name="mediaType">Receives an instance of the media type.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateMediaTypeFromVideoInfoHeader(VideoInfoHeader vih, Guid subType, out IMFMediaType mediaType)
        {
            IMFMediaType result = null;

            HResult hr = MFExtern.MFCreateMediaType(out mediaType);
            if (hr.Succeeded())
            {
                hr = MFExtern.MFInitMediaTypeFromVideoInfoHeader(result, vih, Marshal.SizeOf(vih), subType);
                if (hr.Succeeded())
                {
                    mediaType = result;
                    return hr;
                }
            }

            ComClass.SafeRelease(ref result);
            return hr;
        }

        /// <summary>
        /// Create a media type from an VideoInfoHeader structure.
        /// </summary>
        /// <param name="vih">An VideoInfoHeader structure that describes the media type. The caller must fill in the structure members before calling this function.</param>
        /// <param name="subType">The video subtype.</param>
        /// <returns>Returns an instance of the media type</returns>
        public static IMFMediaType CreateMediaTypeFromVideoInfoHeader(VideoInfoHeader vih, Guid subType)
        {
            IMFMediaType result;

            HResult hr = CreateMediaTypeFromVideoInfoHeader(vih, subType, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Create a media type from an VideoInfoHeader2 structure.
        /// </summary>
        /// <param name="vih2">An VideoInfoHeader2 structure that describes the media type. The caller must fill in the structure members before calling this function.</param>
        /// <param name="subType">The video subtype.</param>
        /// <param name="mediaType">Receives an instance of the media type.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateMediaTypeFromVideoInfoHeader2(VideoInfoHeader2 vih2, Guid subType, out IMFMediaType mediaType)
        {
            IMFMediaType result = null;

            HResult hr = MFExtern.MFCreateMediaType(out mediaType);
            if (hr.Succeeded())
            {
                hr = MFExtern.MFInitMediaTypeFromVideoInfoHeader2(result, vih2, Marshal.SizeOf(vih2), subType);
                if (hr.Succeeded())
                {
                    mediaType = result;
                    return hr;
                }
            }

            ComClass.SafeRelease(ref result);
            return hr;
        }

        /// <summary>
        /// Create a media type from an VideoInfoHeader2 structure.
        /// </summary>
        /// <param name="vih2">An VideoInfoHeader2 structure that describes the media type. The caller must fill in the structure members before calling this function.</param>
        /// <param name="subType">The video subtype.</param>
        /// <returns>Returns an instance of the media type</returns>
        public static IMFMediaType CreateMediaTypeFromVideoInfoHeader2(VideoInfoHeader2 vih2, Guid subType)
        {
            IMFMediaType result;

            HResult hr = CreateMediaTypeFromVideoInfoHeader2(vih2, subType, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates an MFVideoFormat structure from a video media type.
        /// </summary>
        /// <param name="mediaType">An instance of a video media type.</param>
        /// <param name="videoFormat">Receives a MFVideoFormat structure.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateMFVideoFormatFromMFMediaType(IMFMediaType mediaType, out MFVideoFormat videoFormat)
        {
            int structSize;

            return MFExtern.MFCreateMFVideoFormatFromMFMediaType(mediaType, out videoFormat, out structSize);
        }

        /// <summary>
        /// Returns the FOURCC or D3DFORMAT value for an uncompressed video format.
        /// </summary>
        /// <param name="videoFormat">A MFVideoFormat structure.</param>
        /// <returns>Returns a FOURCC or D3DFORMAT value that identifies the video format.</returns>
        /// <remarks>If the video format is compressed or not recognized, the return value is zero</remarks>
        public static int GetUncompressedVideoFormat(MFVideoFormat videoFormat)
        {
            return MFExtern.MFGetUncompressedVideoFormat(videoFormat);
        }

        /// <summary>
        /// Initializes a DirectShow AMMediaType structure from a Media Foundation media type.
        /// </summary>
        /// <param name="mediaType">An instance of the media type to convert.</param>
        /// <param name="guidFormatBlockType">The format type GUID.</param>
        /// <param name="amMediaType">An AMMediaType structure to initialize.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        /// <remarks>If the value of <paramref name="guidFormatBlockType"/> is Guid.Empty, the function attempts to deduce the correct format block, based on the major type and subtype</remarks>
        public static HResult InitAMMediaTypeFromMFMediaType(IMFMediaType mediaType, Guid guidFormatBlockType, AMMediaType amMediaType)
        {
            return MFExtern.MFInitAMMediaTypeFromMFMediaType(mediaType, guidFormatBlockType, amMediaType);
        }

        /// <summary>
        /// Creates a DirectShow AMMediaType structure from a Media Foundation media type.
        /// </summary>
        /// <param name="mediaType">An instance of the media type to convert.</param>
        /// <param name="guidFormatBlockType">The format type GUID.</param>
        /// <param name="amMediaType">Receives an AMMediaType structure.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        /// <remarks>If the value of <paramref name="guidFormatBlockType"/> is Guid.Empty, the function attempts to deduce the correct format block, based on the major type and subtype</remarks>
        public static HResult CreateAMMediaTypeFromMFMediaType(IMFMediaType mediaType, Guid guidFormatBlockType, out AMMediaType amMediaType)
        {
            return MFExtern.MFCreateAMMediaTypeFromMFMediaType(mediaType, guidFormatBlockType, out amMediaType);
        }

        /// <summary>
        /// Initializes a media type from a WaveFormatEx structure. 
        /// </summary>
        /// <param name="mediaType">An instance of the media type to initialize.</param>
        /// <param name="waveFormatEx">A WaveFormatEx structure that describes the media type. The caller must fill in the structure members before calling this function.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult InitMediaTypeFromWaveFormatEx(IMFMediaType mediaType, WaveFormatEx waveFormatEx)
        {
            return MFExtern.MFInitMediaTypeFromWaveFormatEx(mediaType, waveFormatEx, Marshal.SizeOf(waveFormatEx));
        }

        /// <summary>
        /// Initializes a media type from a MFVideoFormat structure. 
        /// </summary>
        /// <param name="mediaType">An instance of the media type to initialize.</param>
        /// <param name="videoFormat">A MFVideoFormat structure that describes the media type. The caller must fill in the structure members before calling this function.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult InitMediaTypeFromMFVideoFormat(IMFMediaType mediaType, MFVideoFormat videoFormat)
        {
            return MFExtern.MFInitMediaTypeFromMFVideoFormat(mediaType, videoFormat, Marshal.SizeOf(videoFormat));
        }

        /// <summary>
        /// Initializes a media type from a AMMediaType structure. 
        /// </summary>
        /// <param name="mediaType">An instance of the media type to initialize.</param>
        /// <param name="amMediaType">An AMMediaType structure that describes the media type. The caller must fill in the structure members before calling this function.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult InitMediaTypeFromAMMediaType(IMFMediaType mediaType, AMMediaType amMediaType)
        {
            return MFExtern.MFInitMediaTypeFromAMMediaType(mediaType, amMediaType);
        }

        /// <summary>
        /// Initializes a media type from a VideoInfoHeader structure.
        /// </summary>
        /// <param name="mediaType">An instance of the media type to initialize.</param>
        /// <param name="vih">A VideoInfoHeader structure that describes the media type. The caller must fill in the structure members before calling this function.</param>
        /// <param name="subType">The format type GUID.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        /// <remarks>If the value of <paramref name="subType"/> is Guid.Empty, the function attempts to deduce the correct format block</remarks>
        public static HResult InitMediaTypeFromVideoInfoHeader(IMFMediaType mediaType, VideoInfoHeader vih, Guid subType)
        {
            return MFExtern.MFInitMediaTypeFromVideoInfoHeader(mediaType, vih, Marshal.SizeOf(vih), subType);
        }

        /// <summary>
        /// Initializes a media type from a VideoInfoHeader2 structure.
        /// </summary>
        /// <param name="mediaType">An instance of the media type to initialize.</param>
        /// <param name="vih">A VideoInfoHeader2 structure that describes the media type. The caller must fill in the structure members before calling this function.</param>
        /// <param name="subType">The format type GUID.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        /// <remarks>If the value of <paramref name="subType"/> is Guid.Empty, the function attempts to deduce the correct format block</remarks>
        public static HResult InitMediaTypeFromVideoInfoHeader2(IMFMediaType mediaType, VideoInfoHeader2 vih2, Guid subType)
        {
            return MFExtern.MFInitMediaTypeFromVideoInfoHeader2(mediaType, vih2, Marshal.SizeOf(vih2), subType);
        }

        /// <summary>
        /// Retrieves a media type that was wrapped in another media type by the <see cref="WrapMediaType"/> method.
        /// </summary>
        /// <param name="wrap">An instance of the IMFMediaType interface of the media type that was retrieved by <see cref="WrapMediaType"/>.</param>
        /// <param name="original">Receives an instance to the IMFMediaType interface of the original media type.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult UnwrapMediaType(IMFMediaType wrap, out IMFMediaType original)
        {
            return MFExtern.MFUnwrapMediaType(wrap, out original);
        }

        /// <summary>
        /// Creates a media type that wraps another media type.
        /// </summary>
        /// <param name="original">An instance of the IMFMediaType interface of the media type to wrap in a new media type.</param>
        /// <param name="majorType">A Guid that specifies the major type for the new media type.</param>
        /// <param name="subType">A Guid that specifies the subtype for the new media type.</param>
        /// <param name="wrap">Receives an instance of the IMFMediaType interface of the new media type that wraps the original media type.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult WrapMediaType(IMFMediaType original, Guid majorType, Guid subType, out IMFMediaType wrap)
        {
            return MFExtern.MFWrapMediaType(original, majorType, subType, out wrap);
        }

        /// <summary>
        /// Creates a video media type from a BitmapInfoHeader structure.
        /// </summary>
        /// <param name="bitMapInfoHeader">The BitmapInfoHeader structure to convert.</param>
        /// <param name="pixelAspectRatioX">The X dimension of the pixel aspect ratio.</param>
        /// <param name="pixelAspectRatioY">The Y dimension of the pixel aspect ratio.</param>
        /// <param name="interlaceMode">A member of the MFVideoInterlaceMode enumeration, specifying how the video is interlaced.</param>
        /// <param name="videoFlags">A bitwise OR of flags from the MFVideoFlags enumeration.</param>
        /// <param name="framesPerSecondNumerator">The numerator of the frame rate in frames per second.</param>
        /// <param name="framesPerSecondDenominator">The denominator of the frame rate in frames per second.</param>
        /// <param name="maxBitRate">The approximate data rate of the video stream, in bits per second. If the rate is unknown, set this parameter to zero.</param>
        /// <param name="videoMediaType">Receives an instance of the IMFVideoMediaType interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateVideoMediaTypeFromBitMapInfoHeaderEx(BitmapInfoHeader bitMapInfoHeader, int pixelAspectRatioX, int pixelAspectRatioY, MFVideoInterlaceMode interlaceMode, MFVideoFlags videoFlags, int framesPerSecondNumerator, int framesPerSecondDenominator, int maxBitRate, out IMFVideoMediaType videoMediaType)
        {
            return MFExtern.MFCreateVideoMediaTypeFromBitMapInfoHeaderEx(bitMapInfoHeader, Marshal.SizeOf(bitMapInfoHeader), pixelAspectRatioX, pixelAspectRatioY, interlaceMode, videoFlags, framesPerSecondNumerator, framesPerSecondDenominator, maxBitRate, out  videoMediaType);
        }

        /// <summary>
        /// Creates a Media Foundation media type from another format representation.
        /// </summary>
        /// <param name="guidRepresentation">A Guid that specifies which format representation to convert.</param>
        /// <param name="representationPtr">Pointer to a buffer that contains the format representation to convert. The layout of the buffer depends on the value of <paramref name="guidRepresentation"/>.</param>
        /// <param name="mediaType">Receives an instance of the IMFMediaType interface. </param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateMediaTypeFromRepresentation(Guid guidRepresentation, IntPtr representationPtr, out IMFMediaType mediaType)
        {
            return MFExtern.MFCreateMediaTypeFromRepresentation(guidRepresentation, representationPtr, out mediaType);
        }

        /// <summary>
        /// Compares a full media type to a partial media type.
        /// </summary>
        /// <param name="fullMediaType">An instance of the IMFMediaType interface of the full media type.</param>
        /// <param name="partialMediaType">An instance of the IMFMediaType interface of the partial media type.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static bool CompareFullToPartialMediaType(IMFMediaType fullMediaType, IMFMediaType partialMediaType)
        {
            return MFExtern.MFCompareFullToPartialMediaType(fullMediaType, partialMediaType);
        }

        /// <summary>
        /// Initializes a media type from a DirectShow Mpeg1VideoInfo structure.
        /// </summary>
        /// <param name="mediaType">An instance of the media type to initialize.</param>
        /// <param name="mpeg1VideoInfo">A Mpeg1VideoInfo structure that describes the media type. The caller must fill in the structure members before calling this function.</param>
        /// <param name="subtype">The format type GUID.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        /// <remarks>If the value of <paramref name="subType"/> is Guid.Empty, the function attempts to deduce the correct format block.</remarks>
        public static HResult InitMediaTypeFromMPEG1VideoInfo(IMFMediaType mediaType, Mpeg1VideoInfo mpeg1VideoInfo, Guid subtype)
        {
            return MFExtern.MFInitMediaTypeFromMPEG1VideoInfo(mediaType, mpeg1VideoInfo, Marshal.SizeOf(mpeg1VideoInfo), subtype);
        }

        /// <summary>
        /// Initializes a media type from a DirectShow Mpeg2VideoInfo structure.
        /// </summary>
        /// <param name="mediaType">An instance of the media type to initialize.</param>
        /// <param name="mpeg2VideoInfo">A Mpeg2VideoInfo structure that describes the media type. The caller must fill in the structure members before calling this function.</param>
        /// <param name="subtype">The format type GUID.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        /// <remarks>If the value of <paramref name="subType"/> is Guid.Empty, the function attempts to deduce the correct format block.</remarks>
        public static HResult InitMediaTypeFromMPEG2VideoInfo(IMFMediaType mediaType, Mpeg2VideoInfo mpeg2VideoInfo, Guid subtype)
        {
            return MFExtern.MFInitMediaTypeFromMPEG2VideoInfo(mediaType, mpeg2VideoInfo, Marshal.SizeOf(mpeg2VideoInfo), subtype);
        }

        public static HResult CreateMediaTypeFromProperties(object properties, out IMFMediaType mediaType)
        {
            return MFExtern.MFCreateMediaTypeFromProperties(properties, out mediaType);
        }

        public static HResult CreatePropertiesFromMediaType(IMFMediaType mediaType, Guid riid, out object properties)
        {
            return MFExtern.MFCreatePropertiesFromMediaType(mediaType, riid, out properties);
        }

        public static HResult CreatePropertiesFromMediaType<T>(IMFMediaType mediaType, out T properties) where T : class
        {
            if (!typeof(T).IsComInterface())
                throw new ArgumentOutOfRangeException("T", "T must be a COM visible interface.");

            object tmp;

            HResult hr = MFExtern.MFCreatePropertiesFromMediaType(mediaType, typeof(T).GUID, out tmp);
            properties = hr.Succeeded() ? tmp as T : null;

            return hr;
        }

        /// <summary>
        /// Clone a media type.
        /// </summary>
        /// <param name="from">The source media type.</param>
        /// <returns>Returns a duplicated media type.</returns>
        public static IMFMediaType CloneMediaType(IMFMediaType from)
        {
            if (from == null)
                throw new ArgumentNullException("from");

            IMFMediaType result = CreateMediaType();

            HResult hr = from.CopyAllItems(result);
            hr.ThrowExceptionOnError();

            return result;
        }

        #endregion

        #region ********** IPropertyStore methods *****************************

        /// <summary>
        /// Creates an in-memory property store.
        /// </summary>
        /// <param name="propertyStore">Receives an instance of an empty property store.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreatePropertyStore(out IPropertyStore propertyStore)
        {
            object result = null;

            HResult hr = NativeMethods.PSCreateMemoryPropertyStore(typeof(IPropertyStore).GUID, out result);
            propertyStore = hr.Succeeded() ? result as IPropertyStore : null;

            return hr;
        }

        /// <summary>
        /// Creates an in-memory property store.
        /// </summary>
        /// <param name="propertyStore">Receives an instance of an empty property store.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreatePropertyStore(out INamedPropertyStore propertyStore)
        {
            object result = null;

            HResult hr = NativeMethods.PSCreateMemoryPropertyStore(typeof(INamedPropertyStore).GUID, out result);
            propertyStore = hr.Succeeded() ? result as INamedPropertyStore : null;

            return hr;
        }

        /// <summary>
        /// Creates an in-memory property store.
        /// </summary>
        /// <returns>Returns an instance of an empty property store.</returns>
        public static IPropertyStore CreatePropertyStore()
        {
            IPropertyStore result = null;

            HResult hr = CreatePropertyStore(out result);
            if (hr.Succeeded())
                return result;
            else
                return null;
        }

        #endregion

        #region ********** IMFSourceResolver methods **************************

        /// <summary>
        /// Creates the source resolver, which is used to create a media source from a URL or byte stream.
        /// </summary>
        /// <param name="sourceResolver">Receives an instance of the source resolver's IMFSourceResolver interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateSourceResolver(out IMFSourceResolver sourceResolver)
        {
            return MFExtern.MFCreateSourceResolver(out sourceResolver);
        }

        /// <summary>
        /// Creates the source resolver, which is used to create a media source from a URL or byte stream.
        /// </summary>
        /// <returns>Returns an instance of the source resolver's IMFSourceResolver interface.</returns>
        public static IMFSourceResolver CreateSourceResolver()
        {
            IMFSourceResolver result;

            HResult hr = MFExtern.MFCreateSourceResolver(out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        private static readonly Guid CLSID_MFDShowSourceResolver = new Guid(0x14d7a407, 0x396b, 0x44b3, 0xbe, 0x85, 0x51, 0x99, 0xa0, 0xf0, 0xf8, 0x0a);

        /// <summary>
        /// Creates the DirectShow source resolver.
        /// </summary>
        /// <returns>Returns an instance of the DirectShow source resolver.</returns>
        /// <remarks>
        /// <para>The DirectShow source resolver is an undocumented component of Windows Media Player that is available since Windows Vista. Use it at your own risk.</para>
        /// <para>This source resolver can wrap a DirectShow source filter as a Media Foundation media source. This allow reading file formats not supported natively by Media Foundation but by DirectShow.</para>
        /// </remarks>
        public static IMFSourceResolver CreateDirectShowSourceResolver()
        {
            return ComClass.CreateInstance<IMFSourceResolver>(CLSID_MFDShowSourceResolver);
        }

        #endregion

        #region ********** IMFAsyncResult methods *****************************

        /// <summary>
        /// Creates an asynchronous result object. Use this function if you are implementing an asynchronous method.
        /// </summary>
        /// <param name="storedObject">The object to store in the asynchronous result.</param>
        /// <param name="callback">An instance of the IMFAsyncCallback interface that is implemented by the caller of the asynchronous method.</param>
        /// <param name="state">A state object that is provided by the caller of the asynchronous method. This parameter can be null.</param>
        /// <param name="asyncResult">Receives an instance of the IMFAsyncResult interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateAsyncResult(object storedObject, IMFAsyncCallback callback, object state, out IMFAsyncResult asyncResult)
        {
            return MFExtern.MFCreateAsyncResult(storedObject, callback, state, out asyncResult);
        }

        /// <summary>
        /// Invokes a callback method to complete an asynchronous operation.
        /// </summary>
        /// <param name="asyncResult">An instance of the IMFAsyncResult interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult InvokeCallback(IMFAsyncResult asyncResult)
        {
            return MFExtern.MFInvokeCallback(asyncResult);
        }

        #endregion

        #region ********** IMFPresentationDescriptor methods ******************

        /// <summary>
        /// Creates a presentation descriptor.
        /// </summary>
        /// <param name="streamDescriptors">Array of IMFStreamDescriptor interface. Each instance represents a stream descriptor for one stream in the presentation.</param>
        /// <param name="presentationDescriptor">Receives an instance of an IMFPresentationDescriptor interface of the presentation descriptor.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreatePresentationDescriptor(IMFStreamDescriptor[] streamDescriptors, out IMFPresentationDescriptor presentationDescriptor)
        {
            if (streamDescriptors == null)
                throw new ArgumentNullException("streamDescriptors");

            return MFExtern.MFCreatePresentationDescriptor(streamDescriptors.Length, streamDescriptors, out presentationDescriptor);
        }

        /// <summary>
        /// Serializes a presentation descriptor to a memory buffer.
        /// </summary>
        /// <param name="presentationDescriptor">A valid IMFPresentationDescriptor instance.</param>
        /// <param name="buffer">Receives a memory buffer containing the serialized presentation descriptor.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult SerializePresentationDescriptor(IMFPresentationDescriptor presentationDescriptor, out MemoryBuffer buffer)
        {
            if (presentationDescriptor == null)
                throw new ArgumentNullException("presentationDescriptor");

            IntPtr bufferPtr;
            int bufferSize;

            HResult hr = MFExtern.MFSerializePresentationDescriptor(presentationDescriptor, out bufferSize, out bufferPtr);
            buffer = hr.Succeeded() ? new MemoryBuffer(bufferPtr, (uint)bufferSize) : null;

            return hr;
        }

        /// <summary>
        /// Serializes a presentation descriptor to a byte array.
        /// </summary>
        /// <param name="presentationDescriptor">A valid IMFPresentationDescriptor instance.</param>
        /// <param name="buffer">Receives a byte array containing the serialized presentation descriptor.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult SerializePresentationDescriptor(IMFPresentationDescriptor presentationDescriptor, out byte[] buffer)
        {
            if (presentationDescriptor == null)
                throw new ArgumentNullException("presentationDescriptor");

            HResult hr = 0;
            buffer = null;
            MemoryBuffer tmpBuffer = null;

            try
            {
                hr = SerializePresentationDescriptor(presentationDescriptor, out tmpBuffer);
                if (hr.Succeeded())
                {
                    buffer = new byte[tmpBuffer.ByteLength];
                    Marshal.Copy(tmpBuffer.BufferPointer, buffer, 0, buffer.Length);
                }
            }
            finally
            {
                if (tmpBuffer != null)
                    tmpBuffer.Dispose();
            }

            return hr;
        }

        /// <summary>
        /// Deserializes a presentation descriptor from a memory buffer.
        /// </summary>
        /// <param name="buffer">A memory buffer that contains the serialized presentation descriptor.</param>
        /// <param name="presentationDescriptor">Receives an IMFPresentationDescriptor interface of the presentation descriptor.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult DeserializePresentationDescriptor(MemoryBuffer buffer, out IMFPresentationDescriptor presentationDescriptor)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            return MFExtern.MFDeserializePresentationDescriptor((int)buffer.ByteLength, buffer.BufferPointer, out presentationDescriptor); 
        }

        /// <summary>
        /// Deserializes a presentation descriptor from a byte array.
        /// </summary>
        /// <param name="buffer">A byte array that contains the serialized presentation descriptor.</param>
        /// <param name="presentationDescriptor">Receives an IMFPresentationDescriptor interface of the presentation descriptor.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult DeserializePresentationDescriptor(byte[] buffer, out IMFPresentationDescriptor presentationDescriptor)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            using (GCPin pin = new GCPin(buffer))
            {
                return MFExtern.MFDeserializePresentationDescriptor(buffer.Length, pin.PinnedAddress, out presentationDescriptor);
            }
        }

        /// <summary>
        /// Creates a presentation descriptor from an ASF profile object.
        /// </summary>
        /// <param name="profile">The IMFASFProfile interface of the ASF profile object.</param>
        /// <param name="presentationDescriptor">Receives an IMFPresentationDescriptor interface of the presentation descriptor.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreatePresentationDescriptorFromASFProfile(IMFASFProfile profile, out IMFPresentationDescriptor presentationDescriptor)
        {
            return MFExtern.MFCreatePresentationDescriptorFromASFProfile(profile, out presentationDescriptor);
        }

        #endregion

        #region ********** IMFMediaBuffer methods *****************************

        /// <summary>
        /// Allocates system memory and creates a media buffer to manage it.
        /// </summary>
        /// <param name="maxLength">Size of the buffer, in bytes.</param>
        /// <param name="buffer">Receives an instance of the IMFMediaBuffer interface of the media buffer.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateMemoryBuffer(int maxLength, out IMFMediaBuffer buffer)
        {
            return MFExtern.MFCreateMemoryBuffer(maxLength, out buffer);
        }

        /// <summary>
        /// Allocates system memory and creates a media buffer to manage it.
        /// </summary>
        /// <param name="maxLength">Size of the buffer, in bytes.</param>
        /// <returns>Returns an instance of the IMFMediaBuffer interface of the media buffer.</returns>
        public static IMFMediaBuffer CreateMemoryBuffer(int maxLength)
        {
            IMFMediaBuffer result;

            HResult hr = MFExtern.MFCreateMemoryBuffer(maxLength, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Allocates system memory with a specified byte alignment and creates a media buffer to manage the memory.
        /// </summary>
        /// <param name="maxLength">Size of the buffer, in bytes.</param>
        /// <param name="aligment">A member of the <see cref="MemoryAlignment"/> enumeration that define the required memory alignment.</param>
        /// <param name="buffer">Receives an instance of the IMFMediaBuffer interface of the media buffer.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateAlignedMemoryBuffer(int maxLength, MemoryAlignment aligment, out IMFMediaBuffer buffer)
        {
            return MFExtern.MFCreateAlignedMemoryBuffer(maxLength, (int)aligment, out buffer);
        }

        /// <summary>
        /// Allocates system memory with a specified byte alignment and creates a media buffer to manage the memory.
        /// </summary>
        /// <param name="maxLength">Size of the buffer, in bytes.</param>
        /// <param name="aligment">A member of the <see cref="MemoryAlignment"/> enumeration that define the required memory alignment.</param>
        /// <returns>Returns an instance of the IMFMediaBuffer interface of the media buffer.</returns>
        public static IMFMediaBuffer CreateAlignedMemoryBuffer(int maxLength, MemoryAlignment aligment)
        {
            IMFMediaBuffer result;

            HResult hr = MFExtern.MFCreateAlignedMemoryBuffer(maxLength, (int)aligment, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates a media buffer that wraps an existing media buffer. The new media buffer points to the same memory as the original media buffer, or to an offset from the start of the memory.
        /// </summary>
        /// <param name="buffer">An instance of the IMFMediaBuffer interface of the original media buffer.</param>
        /// <param name="offset">The start of the new buffer, as an offset in bytes from the start of the original buffer.</param>
        /// <param name="length">The size of the new buffer. The value of <paramref name="offset"/> + <paramref name="length"/> must be less than or equal to the size of valid data the original buffer.</param>
        /// <param name="wrappedBuffer">Receives an instance of the IMFMediaBuffer interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateMediaBufferWrapper(IMFMediaBuffer buffer, int offset, int length, out IMFMediaBuffer wrappedBuffer)
        {
            return MFExtern.MFCreateMediaBufferWrapper(buffer, offset, length, out wrappedBuffer);
        }

        /// <summary>
        /// Creates a media buffer that wraps an existing media buffer. The new media buffer points to the same memory as the original media buffer, or to an offset from the start of the memory.
        /// </summary>
        /// <param name="buffer">An instance of the IMFMediaBuffer interface of the original media buffer.</param>
        /// <param name="offset">The start of the new buffer, as an offset in bytes from the start of the original buffer.</param>
        /// <param name="length">The size of the new buffer. The value of <paramref name="offset"/> + <paramref name="length"/> must be less than or equal to the size of valid data the original buffer.</param>
        /// <returns>Returns an instance of the IMFMediaBuffer interface.</returns>
        public static IMFMediaBuffer CreateMediaBufferWrapper(IMFMediaBuffer buffer, int offset, int length)
        {
            IMFMediaBuffer result;

            HResult hr = MFExtern.MFCreateMediaBufferWrapper(buffer, offset, length, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates a media buffer object that manages a Windows Imaging Component (WIC) bitmap.
        /// </summary>
        /// <param name="riid">Set this parameter to {00000121-a8f2-4877-ba0a-fd2b6645fb94} (IID of IWICBitmap).</param>
        /// <param name="wicBitmap">An instance of the bitmap surface. The bitmap surface must be a WIC bitmap that exposes the IWICBitmap interface.</param>
        /// <param name="mediaBuffer">Receives an instance of the IMFMediaBuffer interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateWICBitmapBuffer(Guid riid, object wicBitmap, out IMFMediaBuffer mediaBuffer)
        {
            return MFExtern.MFCreateWICBitmapBuffer(riid, wicBitmap, out mediaBuffer);
        }

        private static readonly Guid IID_IWICBitmap = new Guid(0x00000121, 0xa8f2, 0x4877, 0xba, 0x0a, 0xfd, 0x2b, 0x66, 0x45, 0xfb, 0x94);

        /// <summary>
        /// Creates a media buffer object that manages a Windows Imaging Component (WIC) bitmap.
        /// </summary>
        /// <param name="wicBitmap">An instance of the bitmap surface. The bitmap surface must be a WIC bitmap that exposes the IWICBitmap interface.</param>
        /// <param name="mediaBuffer">Receives an instance of the IMFMediaBuffer interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateWICBitmapBuffer(object wicBitmap, out IMFMediaBuffer mediaBuffer)
        {
            return MFExtern.MFCreateWICBitmapBuffer(IID_IWICBitmap, wicBitmap, out mediaBuffer);
        }

        /// <summary>
        /// Creates a media buffer object that manages a Windows Imaging Component (WIC) bitmap.
        /// </summary>
        /// <param name="wicBitmap">A pointer to the bitmap surface. The bitmap surface must be a WIC bitmap that exposes the IWICBitmap interface.</param>
        /// <param name="mediaBuffer">Receives an instance of the IMFMediaBuffer interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateWICBitmapBuffer(IntPtr wicBitmap, out IMFMediaBuffer mediaBuffer)
        {
            return NativeMethods.MFCreateWICBitmapBuffer(IID_IWICBitmap, wicBitmap, out mediaBuffer);
        }

        /// <summary>
        /// Creates a system-memory buffer object to hold 2D image data.
        /// </summary>
        /// <param name="width">Width of the image, in pixels.</param>
        /// <param name="height">Height of the image, in pixels.</param>
        /// <param name="fourCC">A FOURCC code or D3DFORMAT value that specifies the video format.</param>
        /// <param name="bottomUp">If true, the buffer's IMF2DBuffer.ContiguousCopyTo method copies the buffer into a bottom-up format. The bottom-up format is compatible with GDI for uncompressed RGB images. If this parameter is false, the ContiguousCopyTo method copies the buffer into a top-down format, which is compatible with DirectX.</param>
        /// <param name="mediaBuffer">Receives an instance of the IMFMediaBuffer interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult Create2DMediaBuffer(int width, int height, int fourCC, bool bottomUp, out IMFMediaBuffer mediaBuffer)
        {
            return MFExtern.MFCreate2DMediaBuffer(width, height, fourCC, bottomUp, out mediaBuffer);
        }

        /// <summary>
        /// Creates a system-memory buffer object to hold 2D image data.
        /// </summary>
        /// <param name="width">Width of the image, in pixels.</param>
        /// <param name="height">Height of the image, in pixels.</param>
        /// <param name="fourCC">A FOURCC code or D3DFORMAT value that specifies the video format.</param>
        /// <param name="bottomUp">If true, the buffer's IMF2DBuffer.ContiguousCopyTo method copies the buffer into a bottom-up format. The bottom-up format is compatible with GDI for uncompressed RGB images. If this parameter is false, the ContiguousCopyTo method copies the buffer into a top-down format, which is compatible with DirectX.</param>
        /// <param name="mediaBuffer">Receives an instance of the IMF2DBuffer interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult Create2DMediaBuffer(int width, int height, int fourCC, bool bottomUp, out IMF2DBuffer mediaBuffer)
        {
            IMFMediaBuffer result;

            HResult hr = MFExtern.MFCreate2DMediaBuffer(width, height, fourCC, bottomUp, out result);
            mediaBuffer = result as IMF2DBuffer;

            return hr;
        }

#if ALLOW_UNTESTED_INTERFACES
        /// <summary>
        /// Creates a system-memory buffer object to hold 2D image data.
        /// </summary>
        /// <param name="width">Width of the image, in pixels.</param>
        /// <param name="height">Height of the image, in pixels.</param>
        /// <param name="fourCC">A FOURCC code or D3DFORMAT value that specifies the video format.</param>
        /// <param name="bottomUp">If true, the buffer's IMF2DBuffer.ContiguousCopyTo method copies the buffer into a bottom-up format. The bottom-up format is compatible with GDI for uncompressed RGB images. If this parameter is false, the ContiguousCopyTo method copies the buffer into a top-down format, which is compatible with DirectX.</param>
        /// <param name="mediaBuffer">Receives an instance of the IMF2DBuffer2 interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult Create2DMediaBuffer(int width, int height, int fourCC, bool bottomUp, out IMF2DBuffer2 mediaBuffer)
        {
            IMFMediaBuffer result;

            HResult hr = MFExtern.MFCreate2DMediaBuffer(width, height, fourCC, bottomUp, out result);
            mediaBuffer = result as IMF2DBuffer2;

            return hr;
        }
#endif

        /// <summary>
        /// Creates a system-memory buffer object to hold 2D image data.
        /// </summary>
        /// <param name="width">Width of the image, in pixels.</param>
        /// <param name="height">Height of the image, in pixels.</param>
        /// <param name="fourCC">A FOURCC code or D3DFORMAT value that specifies the video format.</param>
        /// <param name="bottomUp">If true, the buffer's IMF2DBuffer.ContiguousCopyTo method copies the buffer into a bottom-up format. The bottom-up format is compatible with GDI for uncompressed RGB images. If this parameter is false, the ContiguousCopyTo method copies the buffer into a top-down format, which is compatible with DirectX.</param>
        /// <returns>Returns an instance of the IMF2DBuffer2 interface.</returns>
        public static IMFMediaBuffer Create2DMediaBuffer(int width, int height, int fourCC, bool bottomUp)
        {
            IMFMediaBuffer result;

            HResult hr = MFExtern.MFCreate2DMediaBuffer(width, height, fourCC, bottomUp, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Allocates a system-memory buffer that is optimal for a specified media type.
        /// </summary>
        /// <param name="mediaType">An instance of the IMFMediaType interface of the media type.</param>
        /// <param name="duration">The sample duration. This value is required for audio formats.</param>
        /// <param name="minLength">The minimum size of the buffer, in bytes. The actual buffer size might be larger. Specify zero to allocate the default buffer size for the media type.</param>
        /// <param name="minAlignment">The minimum memory alignment for the buffer. Specify zero to use the default memory alignment.</param>
        /// <param name="mediaBuffer">Receives an instance of the IMFMediaBuffer interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateMediaBufferFromMediaType(IMFMediaType mediaType, long duration, int minLength, int minAlignment, out IMFMediaBuffer mediaBuffer)
        {
            return MFExtern.MFCreateMediaBufferFromMediaType(mediaType, duration, minLength, minAlignment, out mediaBuffer);
        }

        /// <summary>
        /// Allocates a system-memory buffer that is optimal for a specified media type.
        /// </summary>
        /// <param name="mediaType">An instance of the IMFMediaType interface of the media type.</param>
        /// <param name="duration">The sample duration. This value is required for audio formats.</param>
        /// <param name="minLength">The minimum size of the buffer, in bytes. The actual buffer size might be larger. Specify zero to allocate the default buffer size for the media type.</param>
        /// <param name="minAlignment">The minimum memory alignment for the buffer. Specify zero to use the default memory alignment.</param>
        /// <param name="mediaBuffer">Receives an instance of the IMFMediaBuffer interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateMediaBufferFromMediaType(IMFMediaType mediaType, TimeSpan duration, int minLength, int minAlignment, out IMFMediaBuffer mediaBuffer)
        {
            return MFExtern.MFCreateMediaBufferFromMediaType(mediaType, duration.Ticks, minLength, minAlignment, out mediaBuffer);
        }

#if ALLOW_UNTESTED_INTERFACES

        private static readonly Guid IID_ID3D11Texture2D = new Guid(0x6f15aaf2, 0xd208, 0x4e89, 0x9a, 0xb4, 0x48, 0x95, 0x35, 0xd3, 0x4f, 0x9c);

        /// <summary>
        /// Creates a media buffer to manage a Microsoft DirectX Graphics Infrastructure (DXGI) surface.
        /// </summary>
        /// <param name="surface">An instance of the DXGI surface.</param>
        /// <param name="subresourceIndex">The zero-based index of a subresource of the surface. The media buffer object is associated with this subresource.</param>
        /// <param name="bottomUpWhenLinear">If true, the buffer's IMF2DBuffer.ContiguousCopyTo method copies the buffer into a bottom-up format. The bottom-up format is compatible with GDI for uncompressed RGB images. If this parameter is false, the ContiguousCopyTo method copies the buffer into a top-down format, which is compatible with Direct3D.</param>
        /// <param name="mediaBuffer">Receives an instance of the IMFMediaBuffer interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateDXGISurfaceBuffer(object surface, int subresourceIndex, bool bottomUpWhenLinear, out IMFMediaBuffer mediaBuffer)
        {
            return MFExtern.MFCreateDXGISurfaceBuffer(IID_ID3D11Texture2D, surface, subresourceIndex, bottomUpWhenLinear, out mediaBuffer);
        }

        /// <summary>
        /// Creates a media buffer to manage a Microsoft DirectX Graphics Infrastructure (DXGI) surface.
        /// </summary>
        /// <param name="surface">A pointer to the DXGI surface.</param>
        /// <param name="subresourceIndex">The zero-based index of a subresource of the surface. The media buffer object is associated with this subresource.</param>
        /// <param name="bottomUpWhenLinear">If true, the buffer's IMF2DBuffer.ContiguousCopyTo method copies the buffer into a bottom-up format. The bottom-up format is compatible with GDI for uncompressed RGB images. If this parameter is false, the ContiguousCopyTo method copies the buffer into a top-down format, which is compatible with Direct3D.</param>
        /// <param name="mediaBuffer">Receives an instance of the IMFMediaBuffer interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateDXGISurfaceBuffer(IntPtr surface, int subresourceIndex, bool bottomUpWhenLinear, out IMFMediaBuffer mediaBuffer)
        {
            return NativeMethods.MFCreateDXGISurfaceBuffer(IID_ID3D11Texture2D, surface, subresourceIndex, bottomUpWhenLinear, out mediaBuffer);
        }
#endif

#if ALLOW_UNTESTED_INTERFACES

        private static readonly Guid IID_IDirect3DSurface9 = new Guid(0xcfbaf3a, 0x9ff6, 0x429a, 0x99, 0xb3, 0xa2, 0x79, 0x6a, 0xf8, 0xb8, 0x9b);

        /// <summary>
        /// Creates a media buffer object that manages a Direct3D 9 surface. 
        /// </summary>
        /// <param name="surface">An instance of the Direct3D 9 surface.</param>
        /// <param name="bottomUpWhenLinear">If true, the buffer's IMF2DBuffer.ContiguousCopyTo method copies the buffer into a bottom-up format. The bottom-up format is compatible with GDI for uncompressed RGB images. If this parameter is false, the ContiguousCopyTo method copies the buffer into a top-down format, which is compatible with Direct3D.</param>
        /// <param name="mediaBuffer">Receives an instance of the IMFMediaBuffer interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateDXSurfaceBuffer(object surface, bool bottomUpWhenLinear, out IMFMediaBuffer mediaBuffer)
        {
            return MFExtern.MFCreateDXSurfaceBuffer(IID_IDirect3DSurface9, surface, bottomUpWhenLinear, out mediaBuffer);
        }

        /// <summary>
        /// Creates a media buffer object that manages a Direct3D 9 surface. 
        /// </summary>
        /// <param name="surface">A pointer to the Direct3D 9 surface.</param>
        /// <param name="bottomUpWhenLinear">If true, the buffer's IMF2DBuffer.ContiguousCopyTo method copies the buffer into a bottom-up format. The bottom-up format is compatible with GDI for uncompressed RGB images. If this parameter is false, the ContiguousCopyTo method copies the buffer into a top-down format, which is compatible with Direct3D.</param>
        /// <param name="mediaBuffer">Receives an instance of the IMFMediaBuffer interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateDXSurfaceBuffer(IntPtr surface, bool bottomUpWhenLinear, out IMFMediaBuffer mediaBuffer)
        {
            return NativeMethods.MFCreateDXSurfaceBuffer(IID_IDirect3DSurface9, surface, bottomUpWhenLinear, out mediaBuffer);
        }
#endif
        #endregion

        #region ********** Timer methods **************************************

        /// <summary>
        /// Retrieves the timer interval for the <see cref="AddPeriodicCallback"/> method.
        /// </summary>
        /// <param name="periodicity">Receives the timer interval, in milliseconds.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetTimerPeriodicity(out int periodicity)
        {
            return MFExtern.MFGetTimerPeriodicity(out periodicity);
        }

        /// <summary>
        /// Retrieves the timer interval for the <see cref="AddPeriodicCallback"/> method.
        /// </summary>
        /// <param name="periodicity">Receives the timer interval.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetTimerPeriodicity(out TimeSpan periodicity)
        {
            int milliseconds;

            HResult hr = MFExtern.MFGetTimerPeriodicity(out milliseconds);
            periodicity = hr.Succeeded() ? TimeSpan.FromMilliseconds(milliseconds) : default(TimeSpan);

            return hr;
        }

        /// <summary>
        /// Returns the system time.
        /// </summary>
        /// <returns>Returns the system time.</returns>
        public static TimeSpan GetSystemTime()
        {
            return TimeSpan.FromTicks(MFExtern.MFGetSystemTime());
        }

        /// <summary>
        /// Sets a callback function to be called at a fixed interval.
        /// </summary>
        /// <param name="callback">An instance of the the callback function as a delegate.</param>
        /// <param name="context">A caller-provided context object or null. This parameter is passed to the callback function.</param>
        /// <param name="key">Receives a key that can be used to cancel the callback. To cancel the callback, call <see cref="RemovePeriodicCallback"/> and pass this key as the key parameter.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult AddPeriodicCallback(MFExtern.MFPERIODICCALLBACK callback, object context, out int key)
        {
            return MFExtern.MFAddPeriodicCallback(callback, context, out key);
        }

        /// <summary>
        /// Sets a callback function to be called at a fixed interval.
        /// </summary>
        /// <typeparam name="T">Type of the context object</typeparam>
        /// <param name="action">An action that have to be called at a fixed interval.</param>
        /// <param name="context">A caller-provided context object or null. This parameter is passed to the callback function.</param>
        /// <param name="key">Receives a key that can be used to cancel the callback. To cancel the callback, call <see cref="RemovePeriodicCallback"/> and pass this key as the key parameter.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult AddPeriodicCallback<T>(Action<T> action, T context, out int key) where T : class
        {
            MFExtern.MFPERIODICCALLBACK callback = new MFExtern.MFPERIODICCALLBACK((obj) => action(obj as T));
            return MFExtern.MFAddPeriodicCallback(callback, context, out key);
        }

        /// <summary>
        /// Cancels a callback function that was set by the <see cref="AddPeriodicCallback"/> method.
        /// </summary>
        /// <param name="key">Key that identifies the callback. This value is retrieved by the <see cref="AddPeriodicCallback"/> method.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult RemovePeriodicCallback(int key)
        {
            return MFExtern.MFRemovePeriodicCallback(key);
        }

        #endregion

        #region ********** Work Queues methods ********************************

        /// <summary>
        /// Creates a new work queue.
        /// </summary>
        /// <param name="workQueue">Receives an identifier for the work queue.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult AllocateWorkQueue(out int workQueue)
        {
            return MFExtern.MFAllocateWorkQueue(out workQueue);
        }

        /// <summary>
        /// Creates a new work queue. This function extends the capabilities of the <see cref="AllocateWorkQueue"/> method by making it possible to create a work queue that has a message loop.
        /// </summary>
        /// <param name="workQueueType">A member of the MFASYNC_WORKQUEUE_TYPE enumeration, specifying the type of work queue to create.</param>
        /// <param name="workQueue">Receives an identifier for the work queue that was created.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult AllocateWorkQueueEx(MFASYNC_WORKQUEUE_TYPE workQueueType, out int workQueue)
        {
            return MFExtern.MFAllocateWorkQueueEx(workQueueType, out workQueue);
        }

        /// <summary>
        /// Locks a work queue.
        /// </summary>
        /// <param name="workQueue">The identifier for the work queue. The identifier is returned by the <see cref="AllocateWorkQueue"/> method.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult LockWorkQueue(int workQueue)
        {
            return MFExtern.MFLockWorkQueue(workQueue);
        }

        /// <summary>
        /// Unlocks a work queue.
        /// </summary>
        /// <param name="workQueue">The identifier for the work queue. The identifier is returned by the <see cref="AllocateWorkQueue"/> method.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult UnlockWorkQueue(int workQueue)
        {
            return MFExtern.MFUnlockWorkQueue(workQueue);
        }

        /// <summary>
        /// Puts an asynchronous operation on a work queue.
        /// </summary>
        /// <param name="workQueue">The identifier for the work queue. This value can specify one of the standard Media Foundation work queues, or a work queue created by the application.</param>
        /// <param name="callback">An instance of the IMFAsyncCallback interface.</param>
        /// <param name="state">A state object defined by the caller. This parameter can be null. You can use this object to hold state information.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult PutWorkItem(int workQueue, IMFAsyncCallback callback, object state)
        {
            return MFExtern.MFPutWorkItem(workQueue, callback, state);
        }

        /// <summary>
        /// Puts an asynchronous operation on a work queue.
        /// </summary>
        /// <param name="workQueue">The identifier for the work queue. This value can specify one of the standard Media Foundation work queues, or a work queue created by the application.</param>
        /// <param name="result">An instance of the IMFAsyncResult interface of an asynchronous result object.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult PutWorkItemEx(int workQueue, IMFAsyncResult result)
        {
            return MFExtern.MFPutWorkItemEx(workQueue, result);
        }

        /// <summary>
        /// Puts an asynchronous operation on a work queue, with a specified priority.
        /// </summary>
        /// <param name="workQueue">he identifier for the work queue. This value can specify one of the standard Media Foundation work queues, or a work queue created by the application.</param>
        /// <param name="priority">The priority of the work item. Work items are performed in order of priority.</param>
        /// <param name="callback">An instance of the IMFAsyncCallback interface.</param>
        /// <param name="state">A state object defined by the caller. This parameter can be null. You can use this object to hold state information.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult PutWorkItem2(int workQueue, int priority, IMFAsyncCallback callback, object state)
        {
            return MFExtern.MFPutWorkItem2(workQueue, priority, callback, state);
        }

        /// <summary>
        /// Puts an asynchronous operation on a work queue, with a specified priority.
        /// </summary>
        /// <param name="workQueue">The identifier for the work queue. This value can specify one of the standard Media Foundation work queues, or a work queue created by the application.</param>
        /// <param name="priority">The priority of the work item. Work items are performed in order of priority.</param>
        /// <param name="result">An instance of the IMFAsyncResult interface of an asynchronous result object.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult PutWorkItemEx2(int workQueue, int priority, IMFAsyncResult result)
        {
            return MFExtern.MFPutWorkItemEx2(workQueue, priority, result);
        }

        /// <summary>
        /// Schedules an asynchronous operation to be completed after a specified interval.
        /// </summary>
        /// <param name="callback">An instance of the IMFAsyncCallback interface.</param>
        /// <param name="state">A state object defined by the caller. This parameter can be null. You can use this object to hold state information.</param>
        /// <param name="timeout">Time-out interval, in milliseconds. Set this parameter to a negative value. The callback is invoked after −Timeout milliseconds. For example, if Timeout is −5000, the callback is invoked after 5000 milliseconds.</param>
        /// <param name="key">Receives a key that can be used to cancel the timer.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult ScheduleWorkItem(IMFAsyncCallback callback, object state, long timeout, out long key)
        {
            return MFExtern.MFScheduleWorkItem(callback, state, timeout, out key);
        }

        /// <summary>
        /// Schedules an asynchronous operation to be completed after a specified interval.
        /// </summary>
        /// <param name="callback">An instance of the IMFAsyncCallback interface.</param>
        /// <param name="state">A state object defined by the caller. This parameter can be null. You can use this object to hold state information.</param>
        /// <param name="timeout">Time-out interval. Set this parameter to a negative value. The callback is invoked after −Timeout milliseconds. For example, if Timeout is −5000, the callback is invoked after 5000 milliseconds.</param>
        /// <param name="key">Receives a key that can be used to cancel the timer.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult ScheduleWorkItem(IMFAsyncCallback callback, object state, TimeSpan timeout, out long key)
        {
            return MFExtern.MFScheduleWorkItem(callback, state, (long)timeout.TotalMilliseconds, out key);
        }

        /// <summary>
        /// Schedules an asynchronous operation to be completed after a specified interval.
        /// </summary>
        /// <param name="result">An instance of the IMFAsyncResult interface of an asynchronous result object.</param>
        /// <param name="timeout">Time-out interval, in milliseconds. Set this parameter to a negative value. The callback is invoked after −Timeout milliseconds. For example, if Timeout is −5000, the callback is invoked after 5000 milliseconds.</param>
        /// <param name="key">Receives a key that can be used to cancel the timer.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult ScheduleWorkItemEx(IMFAsyncResult result, long timeout, out long key)
        {
            return MFExtern.MFScheduleWorkItemEx(result, timeout, out key);
        }

        /// <summary>
        /// Schedules an asynchronous operation to be completed after a specified interval.
        /// </summary>
        /// <param name="result">An instance of the IMFAsyncResult interface of an asynchronous result object.</param>
        /// <param name="timeout">Time-out interval. Set this parameter to a negative value. The callback is invoked after −Timeout milliseconds. For example, if Timeout is −5000, the callback is invoked after 5000 milliseconds.</param>
        /// <param name="key">Receives a key that can be used to cancel the timer.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult ScheduleWorkItemEx(IMFAsyncResult result, TimeSpan timeout, out long key)
        {
            return MFExtern.MFScheduleWorkItemEx(result, (long)timeout.TotalMilliseconds, out key);
        }

        /// <summary>
        /// Attempts to cancel an asynchronous operation that was scheduled with <see cref="ScheduleWorkItem"/> or <see cref="ScheduleWorkItemEx"/>.
        /// </summary>
        /// <param name="key">The key that was received in the key parameter of the <see cref="ScheduleWorkItem"/>, <see cref="ScheduleWorkItemEx"/>, or <see cref="PutWaitingWorkItem"/> methods.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CancelWorkItem(long key)
        {
            return MFExtern.MFCancelWorkItem(key);
        }

        public static HResult BeginRegisterWorkQueueWithMMCSS(int workQueueId, string className, int taskId, IMFAsyncCallback doneCallback, object doneState)
        {
            return MFExtern.MFBeginRegisterWorkQueueWithMMCSS(workQueueId, className, taskId, doneCallback, doneState);
        }

        public static HResult BeginRegisterWorkQueueWithMMCSSEx(int workQueueId, string className, int taskId, int priority, IMFAsyncCallback doneCallback, object doneState)
        {
            return MFExtern.MFBeginRegisterWorkQueueWithMMCSSEx(workQueueId, className, taskId, priority, doneCallback, doneState);
        }

        public static HResult EndRegisterWorkQueueWithMMCSS(IMFAsyncResult result, out int taskId)
        {
            return MFExtern.MFEndRegisterWorkQueueWithMMCSS(result, out taskId);
        }

        public static HResult BeginUnregisterWorkQueueWithMMCSS(int workQueueId, IMFAsyncCallback doneCallback, object doneState)
        {
            return MFExtern.MFBeginUnregisterWorkQueueWithMMCSS(workQueueId, doneCallback, doneState);
        }

        public static HResult EndUnregisterWorkQueueWithMMCSS(IMFAsyncResult result)
        {
            return MFExtern.MFEndUnregisterWorkQueueWithMMCSS(result);
        }

        /// <summary>
        /// Retrieves the Multimedia Class Scheduler Service (MMCSS) class currently associated with this work queue.
        /// </summary>
        /// <param name="workQueueId">Identifier for the work queue. The identifier is retrieved by the <see cref="AllocateWorkQueue"/> method.</param>
        /// <param name="className">Receives the name of the MMCSS class.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetWorkQueueMMCSSClass(int workQueueId, out string className)
        {
            className = null;
            MFInt bufferSize = new MFInt(0);

            HResult hr = MFExtern.MFGetWorkQueueMMCSSClass(workQueueId, null, bufferSize);
            if (hr == HResult.MF_E_BUFFERTOOSMALL)
            {
                StringBuilder sb = new StringBuilder(bufferSize + 1);

                hr = MFExtern.MFGetWorkQueueMMCSSClass(workQueueId, sb, bufferSize);
                if (hr.Succeeded())
                    className = sb.ToString();
            }

            return hr;
        }

        public static HResult GetWorkQueueMMCSSTaskId(int workQueueId, out int taskId)
        {
            return MFExtern.MFGetWorkQueueMMCSSTaskId(workQueueId, out taskId);
        }

        public static HResult PutWaitingWorkItem(IntPtr hEvent, int priority, IMFAsyncResult asyncResult, out long key)
        {
            return MFExtern.MFPutWaitingWorkItem(hEvent, priority, asyncResult, out key);
        }

        public static HResult PutWaitingWorkItem(ManualResetEvent mre, int priority, IMFAsyncResult asyncResult, out long key)
        {
            return MFExtern.MFPutWaitingWorkItem(mre.SafeWaitHandle.DangerousGetHandle(), priority, asyncResult, out key);
        }

        public static HResult PutWaitingWorkItem(AutoResetEvent are, int priority, IMFAsyncResult asyncResult, out long key)
        {
            return MFExtern.MFPutWaitingWorkItem(are.SafeWaitHandle.DangerousGetHandle(), priority, asyncResult, out key);
        }

        public static HResult AllocateSerialWorkQueue(int workQueueId, out int serialWorkQueueId)
        {
            return MFExtern.MFAllocateSerialWorkQueue(workQueueId, out serialWorkQueueId);
        }

        public static HResult RegisterPlatformWithMMCSS(string taskName, ref int taskId, int priority)
        {
            return MFExtern.MFRegisterPlatformWithMMCSS(taskName, ref taskId, priority);
        }

        public static HResult UnregisterPlatformFromMMCSS()
        {
            return MFExtern.MFUnregisterPlatformFromMMCSS();
        }

        public static HResult LockSharedWorkQueue(string taskName, int basePriority, ref int taskId, out int id)
        {
            return MFExtern.MFLockSharedWorkQueue(taskName, basePriority, ref taskId, out id);
        }

        public static HResult GetWorkQueueMMCSSPriority(int workQueueId, out int priority)
        {
            return MFExtern.MFGetWorkQueueMMCSSPriority(workQueueId, out priority);
        }


        #endregion

        #region ********** Protected Media Path (PMP) methods *****************

        /// <summary>
        /// Creates an instance of the Media Session inside a Protected Media Path (PMP) process.
        /// </summary>
        /// <param name="creationFlags">A member of the MFPMPSessionCreationFlags enumeration that specifies how to create the session object. </param>
        /// <param name="configuration">An instance of the IMFAttributes interface. This parameter can be null.</param>
        /// <param name="mediaSession">Receives an instance of the PMP Media Session's IMFMediaSession interface.</param>
        /// <param name="enablerActivate">Receives an instance of the IMFActivate interface or the value null. </param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreatePMPMediaSession(MFPMPSessionCreationFlags creationFlags, IMFAttributes configuration, out IMFMediaSession mediaSession, out IMFActivate enablerActivate)
        {
            return MFExtern.MFCreatePMPMediaSession(creationFlags, configuration, out mediaSession, out enablerActivate);
        }

        /// <summary>
        /// Creates the protected media path (PMP) server object.
        /// </summary>
        /// <param name="creationFlags">A member of the MFPMPSessionCreationFlags enumeration that specifies how to create the PMP session.</param>
        /// <param name="pmpServer">Receives an instance of the IMFPMPServer interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreatePMPServer(MFPMPSessionCreationFlags creationFlags, out IMFPMPServer pmpServer)
        {
            return MFExtern.MFCreatePMPServer(creationFlags, out pmpServer);
        }

        /// <summary>
        /// Queries whether a media presentation requires the Protected Media Path (PMP).
        /// </summary>
        /// <param name="presentationDescriptor">An instance of the IMFPresentationDescriptor interface of a presentation descriptor. The presentation descriptor is created by the media source, and describes the presentation.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult RequireProtectedEnvironment(IMFPresentationDescriptor presentationDescriptor)
        {
            return MFExtern.MFRequireProtectedEnvironment(presentationDescriptor);
        }

        #endregion

        #region ********** ASF related methods ********************************

        /// <summary>
        /// Creates the ASF Header Object object.
        /// </summary>
        /// <param name="contentInfo">Receives an instance of the IMFASFContentInfo interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateASFContentInfo(out IMFASFContentInfo contentInfo)
        {
            return MFExtern.MFCreateASFContentInfo(out contentInfo);
        }

        /// <summary>
        /// Creates the ASF Header Object object.
        /// </summary>
        /// <returns>Returns an instance of the IMFASFContentInfo interface.</returns>
        public static IMFASFContentInfo CreateASFContentInfo()
        {
            IMFASFContentInfo result;

            HResult hr = MFExtern.MFCreateASFContentInfo(out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates the ASF Splitter.
        /// </summary>
        /// <param name="splitter">Receives an instance of the IMFASFSplitter interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateASFSplitter(out IMFASFSplitter splitter)
        {
            return MFExtern.MFCreateASFSplitter(out splitter);
        }

        /// <summary>
        /// Creates the ASF Splitter.
        /// </summary>
        /// <returns>Returns an instance of the IMFASFSplitter interface.</returns>
        public static IMFASFSplitter CreateASFSplitter()
        {
            IMFASFSplitter result;

            HResult hr = MFExtern.MFCreateASFSplitter(out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates the ASF profile object.
        /// </summary>
        /// <param name="profile">Receives an instance of the IMFASFProfile interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateASFProfile(out IMFASFProfile profile)
        {
            return MFExtern.MFCreateASFProfile(out profile);
        }

        /// <summary>
        /// Creates the ASF profile object.
        /// </summary>
        /// <returns>Returns an instance of the IMFASFProfile interface.</returns>
        public static IMFASFProfile CreateASFProfile()
        {
            IMFASFProfile result;

            HResult hr = MFExtern.MFCreateASFProfile(out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates the ASF Indexer object.
        /// </summary>
        /// <param name="indexer">Receives an instance of the IMFASFIndexer interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateASFIndexer(out IMFASFIndexer indexer)
        {
            return MFExtern.MFCreateASFIndexer(out indexer);
        }

        /// <summary>
        /// Creates the ASF Indexer object.
        /// </summary>
        /// <returns>Returns an instance of the IMFASFIndexer interface.</returns>
        public static IMFASFIndexer CreateASFIndexer()
        {
            IMFASFIndexer result;

            HResult hr = MFExtern.MFCreateASFIndexer(out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates the ASF stream selector.
        /// </summary>
        /// <param name="asfProfile">An instance of the IMFASFProfile interface.</param>
        /// <param name="selector">Receives an instance of the IMFASFStreamSelector interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateASFStreamSelector(IMFASFProfile asfProfile, out IMFASFStreamSelector selector)
        {
            return MFExtern.MFCreateASFStreamSelector(asfProfile, out selector);
        }

        /// <summary>
        /// Creates the ASF stream selector.
        /// </summary>
        /// <param name="asfProfile">An instance of the IMFASFProfile interface.</param>
        /// <returns>Returns an instance of the IMFASFStreamSelector interface.</returns>
        public static IMFASFStreamSelector CreateASFStreamSelector(IMFASFProfile asfProfile)
        {
            IMFASFStreamSelector result;

            HResult hr = MFExtern.MFCreateASFStreamSelector(asfProfile, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates an ASF profile object from a presentation descriptor.
        /// </summary>
        /// <param name="presentationDescriptor">An instance of the IMFPresentationDescriptor interface of the presentation descriptor that contains the profile information.</param>
        /// <param name="profile">Receives an instance of the IMFASFProfile interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateASFProfileFromPresentationDescriptor(IMFPresentationDescriptor presentationDescriptor, out IMFASFProfile profile)
        {
            return MFExtern.MFCreateASFProfileFromPresentationDescriptor(presentationDescriptor, out profile);
        }

        /// <summary>
        /// Creates an ASF profile object from a presentation descriptor.
        /// </summary>
        /// <param name="presentationDescriptor">An instance of the IMFPresentationDescriptor interface of the presentation descriptor that contains the profile information.</param>
        /// <returns>Returns an instance of the IMFASFProfile interface.</returns>
        public static IMFASFProfile CreateASFProfileFromPresentationDescriptor(IMFPresentationDescriptor presentationDescriptor)
        {
            IMFASFProfile result;

            HResult hr = MFExtern.MFCreateASFProfileFromPresentationDescriptor(presentationDescriptor, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates an activation object for the ASF streaming sink.
        /// </summary>
        /// <param name="byteStream">An instance of a byte stream object in which the ASF media sink writes the streamed content.</param>
        /// <param name="mediaSink">Receives an instance of the IMFMediaSink interface of the ASF streaming-media sink object.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        /// <remarks>The ASF streaming sink enables an application to write streaming Advanced Systems Format (ASF) packets to an HTTP byte stream.</remarks>
        public static HResult CreateASFStreamingMediaSink(IMFByteStream byteStream, out IMFMediaSink mediaSink)
        {
            return MFExtern.MFCreateASFStreamingMediaSink(byteStream, out mediaSink);
        }

        /// <summary>
        /// Creates an activation object for the ASF streaming sink.
        /// </summary>
        /// <param name="byteStream">An instance of a byte stream object in which the ASF media sink writes the streamed content.</param>
        /// <returns>Returns an instance of the IMFMediaSink interface of the ASF streaming-media sink object.</returns>
        /// <remarks>The ASF streaming sink enables an application to write streaming Advanced Systems Format (ASF) packets to an HTTP byte stream.</remarks>
        public static IMFMediaSink CreateASFStreamingMediaSink(IMFByteStream byteStream)
        {
            IMFMediaSink result;

            HResult hr = MFExtern.MFCreateASFStreamingMediaSink(byteStream, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates an activation object for the ASF streaming sink.
        /// </summary>
        /// <param name="byteStreamActivate">An instance of the IMFActivate interface of an activation object for a byte-stream object that exposes the IMFByteStream interface. The ASF streaming sink will write data to this byte stream.</param>
        /// <param name="contentInfo">An instance of an ASF ContentInfo Object that contains the properties that describe the ASF content.</param>
        /// <param name="activate">Receives an instance of the IMFActivate interface of the activation object that is used to create the ASF streaming-media sink.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateASFStreamingMediaSinkActivate(IMFActivate byteStreamActivate, IMFASFContentInfo contentInfo, out IMFActivate activate)
        {
            return MFExtern.MFCreateASFStreamingMediaSinkActivate(byteStreamActivate, contentInfo, out activate);
        }

        /// <summary>
        /// Creates an activation object for the ASF streaming sink.
        /// </summary>
        /// <param name="byteStreamActivate">An instance of the IMFActivate interface of an activation object for a byte-stream object that exposes the IMFByteStream interface. The ASF streaming sink will write data to this byte stream.</param>
        /// <param name="contentInfo">An instance of an ASF ContentInfo Object that contains the properties that describe the ASF content.</param>
        /// <returns>Returns an instance of the IMFActivate interface of the activation object that is used to create the ASF streaming-media sink.</returns>
        public static IMFActivate CreateASFStreamingMediaSinkActivate(IMFActivate byteStreamActivate, IMFASFContentInfo contentInfo)
        {
            IMFActivate result;

            HResult hr = MFExtern.MFCreateASFStreamingMediaSinkActivate(byteStreamActivate, contentInfo, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates a byte stream to access the index in an ASF stream.
        /// </summary>
        /// <param name="contentByteStream">An instance of the IMFByteStream interface of a byte stream that contains the ASF stream.</param>
        /// <param name="indexStartOffset">Byte offset of the index within the ASF stream. To get this value, call IMFASFIndexer.GetIndexPosition.</param>
        /// <param name="indexByteStream">Receives an instance of the IMFByteStream interface. Use this interface to read from the index or write to the index.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateASFIndexerByteStream(IMFByteStream contentByteStream, long indexStartOffset, out IMFByteStream indexByteStream)
        {
            return MFExtern.MFCreateASFIndexerByteStream(contentByteStream, indexStartOffset, out indexByteStream);
        }

        /// <summary>
        /// Creates the ASF Multiplexer.
        /// </summary>
        /// <param name="multiplexer">Receives an instance of the IMFASFMultiplexer interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateASFMultiplexer(out IMFASFMultiplexer multiplexer)
        {
            return MFExtern.MFCreateASFMultiplexer(out multiplexer);
        }

        /// <summary>
        /// Creates the ASF Multiplexer.
        /// </summary>
        /// <returns>Returns an instance of the IMFASFMultiplexer interface.</returns>
        public static IMFASFMultiplexer CreateASFMultiplexer()
        {
            IMFASFMultiplexer result;

            HResult hr = MFExtern.MFCreateASFMultiplexer(out  result);
            hr.ThrowExceptionOnError();

            return result;
        }

        #endregion

        #region ********** Source Reader & Sink Writter methods ***************

        /// <summary>
        /// Creates the source reader from a media source.
        /// </summary>
        /// <param name="mediaSource">An instance of the IMFMediaSource interface of a media source.</param>
        /// <param name="sourceReader">Receives an instance of the IMFSourceReader interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateSourceReaderFromMediaSource(IMFMediaSource mediaSource, out IMFSourceReader sourceReader)
        {
            return MFExtern.MFCreateSourceReaderFromMediaSource(mediaSource, null, out sourceReader);
        }

        /// <summary>
        /// Creates the source reader from a media source.
        /// </summary>
        /// <param name="mediaSource">An instance of the IMFMediaSource interface of a media source.</param>
        /// <param name="attributes">An instance of the IMFAttributes interface. You can use this parameter to configure the source reader.</param>
        /// <param name="sourceReader">Receives an instance of the IMFSourceReader interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateSourceReaderFromMediaSource(IMFMediaSource mediaSource, IMFAttributes attributes, out IMFSourceReader sourceReader)
        {
            return MFExtern.MFCreateSourceReaderFromMediaSource(mediaSource, attributes, out sourceReader);
        }

        /// <summary>
        /// Creates the sink writer from a URL or byte stream.
        /// </summary>
        /// <param name="outputURL">A string that contains the URL of the output file. This parameter can be null.</param>
        /// <param name="byteStream">An instance of the IMFByteStream interface of a byte stream. This parameter can be null.</param>
        /// <param name="attributes">An instance of the IMFAttributes interface. You can use this parameter to configure the sink writer.</param>
        /// <param name="sinkWriter">Receives an instance of the IMFSinkWriter interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateSinkWriterFromURL(string outputURL, IMFByteStream byteStream, IMFAttributes attributes, out IMFSinkWriter sinkWriter)
        {
            return MFExtern.MFCreateSinkWriterFromURL(outputURL, byteStream, attributes, out sinkWriter);
        }

        /// <summary>
        /// Creates the sink writer from a byte stream.
        /// </summary>
        /// <param name="byteStream">An instance of the IMFByteStream interface of a byte stream. This parameter can be null.</param>
        /// <param name="attributes">An instance of the IMFAttributes interface. You can use this parameter to configure the sink writer.</param>
        /// <param name="sinkWriter">Receives an instance of the IMFSinkWriter interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateSinkWriterFromURL(IMFByteStream byteStream, IMFAttributes attributes, out IMFSinkWriter sinkWriter)
        {
            return MFExtern.MFCreateSinkWriterFromURL(null, byteStream, attributes, out sinkWriter);
        }

        /// <summary>
        /// Creates the sink writer from a URL.
        /// </summary>
        /// <param name="outputURL">A string that contains the URL of the output file. This parameter can be null.</param>
        /// <param name="attributes">An instance of the IMFAttributes interface. You can use this parameter to configure the sink writer.</param>
        /// <param name="sinkWriter">Receives an instance of the IMFSinkWriter interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateSinkWriterFromURL(string outputURL, IMFAttributes attributes, out IMFSinkWriter sinkWriter)
        {
            return MFExtern.MFCreateSinkWriterFromURL(outputURL, null, attributes, out sinkWriter);
        }

        /// <summary>
        /// Creates the source reader from a byte stream.
        /// </summary>
        /// <param name="byteStream">An instance of the IMFByteStream interface of a byte stream. This byte stream will provide the source data for the source reader.</param>
        /// <param name="attributes">An instance of the IMFAttributes interface. You can use this parameter to configure the source reader.</param>
        /// <param name="sourceReader">Receives an instance of the IMFSourceReader interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateSourceReaderFromByteStream(IMFByteStream byteStream, IMFAttributes attributes, out IMFSourceReader sourceReader)
        {
            return MFExtern.MFCreateSourceReaderFromByteStream(byteStream, attributes, out sourceReader);
        }

        /// <summary>
        /// Creates the sink writer from a media sink.
        /// </summary>
        /// <param name="mediaSink">An instance of the IMFMediaSink interface of a media sink.</param>
        /// <param name="attributes">An instance of the IMFAttributes interface. You can use this parameter to configure the sink writer. This parameter can be null.</param>
        /// <param name="sinkWriter">Receives an instance of the IMFSinkWriter interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateSinkWriterFromMediaSink(IMFMediaSink mediaSink, IMFAttributes attributes, out IMFSinkWriter sinkWriter)
        {
            return MFExtern.MFCreateSinkWriterFromMediaSink(mediaSink, attributes, out sinkWriter);
        }

        /// <summary>
        /// Creates the sink writer from a media sink.
        /// </summary>
        /// <param name="mediaSink">An instance of the IMFMediaSink interface of a media sink. </param>
        /// <param name="sinkWriter">Receives an instance of the IMFSinkWriter interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateSinkWriterFromMediaSink(IMFMediaSink mediaSink, out IMFSinkWriter sinkWriter)
        {
            return MFExtern.MFCreateSinkWriterFromMediaSink(mediaSink, null, out sinkWriter);
        }

        /// <summary>
        /// Creates the source reader from a URL.
        /// </summary>
        /// <param name="url">The URL of a media file to open.</param>
        /// <param name="attributes">An instance of the IMFAttributes interface. You can use this parameter to configure the source reader. This parameter can be null.</param>
        /// <param name="sourceReader">Receives an instance of the IMFSourceReader interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateSourceReaderFromURL(string url, IMFAttributes attributes, out IMFSourceReader sourceReader)
        {
            return MFExtern.MFCreateSourceReaderFromURL(url, attributes, out sourceReader);
        }

        /// <summary>
        /// Creates the source reader from a URL.
        /// </summary>
        /// <param name="url">The URL of a media file to open.</param>
        /// <param name="sourceReader">Receives an instance of the IMFSourceReader interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateSourceReaderFromURL(string url, out IMFSourceReader sourceReader)
        {
            return MFExtern.MFCreateSourceReaderFromURL(url, null, out sourceReader);
        }

        #endregion

        #region ********** Transcode methods **********************************

        /// <summary>
        /// Creates the transcode sink activation object.
        /// </summary>
        /// <param name="activate">Receives an instance of the IMFActivate interface. This interface is used to create the file sink instance from the activation object.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateTranscodeSinkActivate(out IMFActivate activate)
        {
            return MFExtern.MFCreateTranscodeSinkActivate(out activate);
        }

        /// <summary>
        /// Creates the transcode sink activation object.
        /// </summary>
        /// <returns>Returns an instance of the IMFActivate interface. This interface is used to create the file sink instance from the activation object.</returns>
        public static IMFActivate CreateTranscodeSinkActivate()
        {
            IMFActivate result;

            HResult hr = MFExtern.MFCreateTranscodeSinkActivate(out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates an empty transcode profile object.
        /// </summary>
        /// <param name="transcodeProfile">Receives an instance of the IMFTranscodeProfile interface of the transcode profile object.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateTranscodeProfile(out IMFTranscodeProfile transcodeProfile)
        {
            return MFExtern.MFCreateTranscodeProfile(out transcodeProfile);
        }

        /// <summary>
        /// Creates an empty transcode profile object.
        /// </summary>
        /// <returns>Returns an instance of the IMFTranscodeProfile interface of the transcode profile object.</returns>
        public static IMFTranscodeProfile CreateTranscodeProfile()
        {
            IMFTranscodeProfile result;

            HResult hr = MFExtern.MFCreateTranscodeProfile(out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates a partial transcode topology.
        /// </summary>
        /// <param name="mediaSource">An instance of a media source that encapsulates the source file to be transcoded.</param>
        /// <param name="outputFilePath">A string that contains the name and path of the output file to be generated.</param>
        /// <param name="profile">An instance of the transcode profile that contains the configuration settings for the audio stream, the video stream, and the container to which the file is written.</param>
        /// <param name="transcodeTopo">Receives an instance of the IMFTopology interface of the transcode topology object.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateTranscodeTopology(IMFMediaSource mediaSource, string outputFilePath, IMFTranscodeProfile profile, out IMFTopology transcodeTopo)
        {
            return MFExtern.MFCreateTranscodeTopology(mediaSource, outputFilePath, profile, out transcodeTopo);
        }

        /// <summary>
        /// Gets a list of output formats from an audio encoder.
        /// </summary>
        /// <param name="guidSubType">Specifies the subtype of the output media.</param>
        /// <param name="mftFlags">Bitwise OR of zero or more flags from the MFT_EnumFlag enumeration.</param>
        /// <param name="codecConfig">An instance of the IMFAttributes interface of an attribute store. The attribute store specifies the encoder configuration settings. This parameter can be null.</param>
        /// <param name="availableTypes">Receives an instance of the IMFCollection interface of a collection object that contains a list of preferred audio media types. The collection contains IMFMediaType instances.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult TranscodeGetAudioOutputAvailableTypes(Guid guidSubType, MFT_EnumFlag mftFlags, IMFAttributes codecConfig, out IMFCollection availableTypes)
        {
            return MFExtern.MFTranscodeGetAudioOutputAvailableTypes(guidSubType, mftFlags, codecConfig, out availableTypes);
        }

        /// <summary>
        /// Creates a topology for transcoding to a byte stream.
        /// </summary>
        /// <param name="mediaSource">An instance of the IMFMediaSource interface of a media source. The media source provides that source content for transcoding.</param>
        /// <param name="outputStream">An instance of the IMFByteStream interface of a byte stream. The transcoded output will be written to this byte stream.</param>
        /// <param name="profile">An instance of the IMFTranscodeProfile interface of a transcoding profile.</param>
        /// <param name="transcodeTopo">Receives an instance of the IMFTopology interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateTranscodeTopologyFromByteStream(IMFMediaSource mediaSource, IMFByteStream outputStream, IMFTranscodeProfile profile, out IMFTopology transcodeTopo)
        {
            return MFExtern.MFCreateTranscodeTopologyFromByteStream(mediaSource, outputStream, profile, out transcodeTopo);
        }

        #endregion

        #region ********** Media Source methods *******************************

        /// <summary>
        /// Enumerates a list of audio or video capture devices.
        /// </summary>
        /// <param name="attributes">An instance of an attribute store that contains search criteria.</param>
        /// <param name="sourceActivate">Receives an array of IMFActivate interface instances. Each item represents an activation object for a media source.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult EnumDeviceSources(IMFAttributes attributes, out IMFActivate[] sourceActivate)
        {
            int sourceCount;
            return MFExtern.MFEnumDeviceSources(attributes, out sourceActivate, out sourceCount);
        }

        /// <summary>
        /// Enumerates a list of audio capture devices.
        /// </summary>
        /// <param name="sourceActivate">Receives an array of IMFActivate interface instances. Each item represents an activation object for a media source.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult EnumAudioDeviceSources(out IMFActivate[] sourceActivate)
        {
            HResult hr = 0;
            IMFAttributes attributes;
            int sourceCount;

            hr = MF.CreateAttributes(1, out attributes);
            hr.ThrowExceptionOnError();

            hr = attributes.SetGUID(MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE, CLSID.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_GUID);
            hr.ThrowExceptionOnError();

            return MFExtern.MFEnumDeviceSources(attributes, out sourceActivate, out sourceCount);
        }

        /// <summary>
        /// Enumerates a list of audio capture devices.
        /// </summary>
        /// <param name="role">Specifies the device role for an audio capture device.</param>
        /// <param name="sourceActivate">Receives an array of IMFActivate interface instances. Each item represents an activation object for a media source.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult EnumAudioDeviceSources(ERole role, out IMFActivate[] sourceActivate)
        {
            HResult hr = 0;
            IMFAttributes attributes;
            int sourceCount;

            hr = MF.CreateAttributes(2, out attributes);
            hr.ThrowExceptionOnError();

            hr = attributes.SetGUID(MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE, CLSID.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_GUID);
            hr.ThrowExceptionOnError();

            hr = attributes.SetUINT32(MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_ROLE, (int)role);
            hr.ThrowExceptionOnError();

            return MFExtern.MFEnumDeviceSources(attributes, out sourceActivate, out sourceCount);
        }

        /// <summary>
        /// Enumerates a list of video capture devices.
        /// </summary>
        /// <param name="sourceActivate">Receives an array of IMFActivate interface instances. Each item represents an activation object for a media source.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult EnumVideoDeviceSources(out IMFActivate[] sourceActivate)
        {
            HResult hr = 0;
            IMFAttributes attributes;
            int sourceCount;

            hr = MF.CreateAttributes(1, out attributes);
            hr.ThrowExceptionOnError();

            hr = attributes.SetGUID(MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE, CLSID.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID);
            hr.ThrowExceptionOnError();

            return MFExtern.MFEnumDeviceSources(attributes, out sourceActivate, out sourceCount);
        }

        /// <summary>
        /// Enumerates a list of video capture devices.
        /// </summary>
        /// <param name="videoDeviceCategory">Specifies the device category for a video capture device.</param>
        /// <param name="sourceActivate">Receives an array of IMFActivate interface instances. Each item represents an activation object for a media source.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult EnumVideoDeviceSources(Guid videoDeviceCategory, out IMFActivate[] sourceActivate)
        {
            HResult hr = 0;
            IMFAttributes attributes;
            int sourceCount;

            hr = MF.CreateAttributes(2, out attributes);
            hr.ThrowExceptionOnError();

            hr = attributes.SetGUID(MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE, CLSID.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID);
            hr.ThrowExceptionOnError();

            hr = attributes.SetGUID(MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_CATEGORY, videoDeviceCategory);
            hr.ThrowExceptionOnError();

            return MFExtern.MFEnumDeviceSources(attributes, out sourceActivate, out sourceCount);
        }

        /// <summary>
        /// Creates a media source for a hardware capture device.
        /// </summary>
        /// <param name="attributes">An instance of the IMFAttributes interface of an attribute store, which is used to select the device.</param>
        /// <param name="mediaSource">Receives an instance of the media source's IMFMediaSource interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateDeviceSource(IMFAttributes attributes, out IMFMediaSource mediaSource)
        {
            return MFExtern.MFCreateDeviceSource(attributes, out mediaSource);
        }

        /// <summary>
        /// Creates a media source for a audio capture device.
        /// </summary>
        /// <param name="audioRole">Specifies the device role for an audio capture device.</param>
        /// <param name="mediaSource">Receives an instance of the media source's IMFMediaSource interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateAudioDeviceSource(ERole audioRole,  out IMFMediaSource mediaSource)
        {
            HResult hr = 0;
            IMFAttributes attributes;

            hr = MF.CreateAttributes(2, out attributes);
            hr.ThrowExceptionOnError();

            hr = attributes.SetGUID(MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE, CLSID.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_GUID);
            hr.ThrowExceptionOnError();

            hr = attributes.SetUINT32(MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_ROLE, (int)audioRole);
            hr.ThrowExceptionOnError();

            return MFExtern.MFCreateDeviceSource(attributes, out mediaSource);
        }

        /// <summary>
        /// Creates a media source for a audio capture device.
        /// </summary>
        /// <param name="endpointId">Specifies the audio endpoint ID of the audio capture device.</param>
        /// <param name="mediaSource">Receives an instance of the media source's IMFMediaSource interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateAudioDeviceSource(string endpointId, out IMFMediaSource mediaSource)
        {
            HResult hr = 0;
            IMFAttributes attributes;

            hr = MF.CreateAttributes(2, out attributes);
            hr.ThrowExceptionOnError();

            hr = attributes.SetGUID(MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE, CLSID.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_GUID);
            hr.ThrowExceptionOnError();

            hr = attributes.SetString(MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_ENDPOINT_ID, endpointId);
            hr.ThrowExceptionOnError();

            return MFExtern.MFCreateDeviceSource(attributes, out mediaSource);
        }

        /// <summary>
        /// Creates a media source for a video capture device.
        /// </summary>
        /// <param name="symbolicLink">Specifies the symbolic link to the device.</param>
        /// <param name="mediaSource">Receives an instance of the media source's IMFMediaSource interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateVideoDeviceSource(string symbolicLink, out IMFMediaSource mediaSource)
        {
            HResult hr = 0;
            IMFAttributes attributes;

            hr = MF.CreateAttributes(2, out attributes);
            hr.ThrowExceptionOnError();

            hr = attributes.SetGUID(MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE, CLSID.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID);
            hr.ThrowExceptionOnError();

            hr = attributes.SetString(MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_SYMBOLIC_LINK, symbolicLink);
            hr.ThrowExceptionOnError();

            return MFExtern.MFCreateDeviceSource(attributes, out mediaSource);
        }

        /// <summary>
        /// Creates an activation object that represents a hardware capture device.
        /// </summary>
        /// <param name="attributes">An instance of the IMFAttributes interface of an attribute store, which is used to select the device.</param>
        /// <param name="activate">Receives an instance of the IMFActivate interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateDeviceSourceActivate(IMFAttributes attributes, out IMFActivate activate)
        {
            return MFExtern.MFCreateDeviceSourceActivate(attributes, out activate);
        }

        /// <summary>
        /// Creates an activation object that represents an audio capture device.
        /// </summary>
        /// <param name="audioRole">Specifies the device role for an audio capture device.</param>
        /// <param name="activate">Receives an instance of the IMFActivate interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateAudioDeviceSourceActivate(ERole audioRole, out IMFActivate activate)
        {
            HResult hr = 0;
            IMFAttributes attributes;

            hr = MF.CreateAttributes(2, out attributes);
            hr.ThrowExceptionOnError();

            hr = attributes.SetGUID(MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE, CLSID.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_GUID);
            hr.ThrowExceptionOnError();

            hr = attributes.SetUINT32(MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_ROLE, (int)audioRole);
            hr.ThrowExceptionOnError();

            return MFExtern.MFCreateDeviceSourceActivate(attributes, out activate);
        }

        /// <summary>
        /// Creates an activation object that represents an audio capture device.
        /// </summary>
        /// <param name="endpointId">Specifies the audio endpoint ID of the audio capture device.</param>
        /// <param name="activate">Receives an instance of the IMFActivate interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateAudioDeviceSourceActivate(string endpointId, out IMFActivate activate)
        {
            HResult hr = 0;
            IMFAttributes attributes;

            hr = MF.CreateAttributes(2, out attributes);
            hr.ThrowExceptionOnError();

            hr = attributes.SetGUID(MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE, CLSID.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_GUID);
            hr.ThrowExceptionOnError();

            hr = attributes.SetString(MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_ENDPOINT_ID, endpointId);
            hr.ThrowExceptionOnError();

            return MFExtern.MFCreateDeviceSourceActivate(attributes, out activate);
        }

        /// <summary>
        /// Creates an activation object that represents a video capture device.
        /// </summary>
        /// <param name="symbolicLink">Specifies the symbolic link to the device.</param>
        /// <param name="activate">Receives an instance of the IMFActivate interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateVideoDeviceSourceActivate(string symbolicLink, out IMFActivate activate)
        {
            HResult hr = 0;
            IMFAttributes attributes;

            hr = MF.CreateAttributes(2, out attributes);
            hr.ThrowExceptionOnError();

            hr = attributes.SetGUID(MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE, CLSID.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID);
            hr.ThrowExceptionOnError();

            hr = attributes.SetString(MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_SYMBOLIC_LINK, symbolicLink);
            hr.ThrowExceptionOnError();

            return MFExtern.MFCreateDeviceSourceActivate(attributes, out activate);
        }

        /// <summary>
        /// Creates a media source that aggregates a collection of media sources.
        /// </summary>
        /// <param name="sourceCollection">An instance of the IMFCollection interface of the collection object that contains a list of media sources.</param>
        /// <param name="aggregateSource">Receives an instance of the IMFMediaSource interface of the aggregated media source.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateAggregateSource(IMFCollection sourceCollection, out IMFMediaSource aggregateSource)
        {
            return MFExtern.MFCreateAggregateSource(sourceCollection, out aggregateSource);
        }

        /// <summary>
        /// Creates a media source that aggregates two media sources.
        /// </summary>
        /// <param name="source1">An instance of the IMFMediaSource interface of the first media source.</param>
        /// <param name="source2">An instance of the IMFMediaSource interface of the second media source.</param>
        /// <param name="aggregateSource">Receives an instance of the IMFMediaSource interface of the aggregated media source.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateAggregateSource(IMFMediaSource source1, IMFMediaSource source2, out IMFMediaSource aggregateSource)
        {
            HResult hr = 0;
            IMFCollection sourceCollection;

            hr = MF.CreateCollection(out sourceCollection);
            hr.ThrowExceptionOnError();

            hr = sourceCollection.AddElement(source1);
            hr.ThrowExceptionOnError();

            hr = sourceCollection.AddElement(source2);
            hr.ThrowExceptionOnError();

            hr = MFExtern.MFCreateAggregateSource(sourceCollection, out aggregateSource);
            hr.ThrowExceptionOnError();

            ComClass.SafeRelease(ref sourceCollection);

            return hr;
        }

        #endregion

        #region ********** Media Sink methods *********************************

        /// <summary>
        /// Creates an activation object for the Streaming Audio Renderer.
        /// </summary>
        /// <param name="activate">Receives an instance of the IMFActivate interface. Use this interface to create the audio renderer.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateAudioRendererActivate(out IMFActivate activate)
        {
            return MFExtern.MFCreateAudioRendererActivate(out activate);
        }

        /// <summary>
        /// Creates an activation object for the Streaming Audio Renderer.
        /// </summary>
        /// <returns>Returns an instance of the IMFActivate interface. Use this interface to create the audio renderer.</returns>
        public static IMFActivate CreateAudioRendererActivate()
        {
            IMFActivate result;

            HResult hr = MFExtern.MFCreateAudioRendererActivate(out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates the Streaming Audio Renderer.
        /// </summary>
        /// <param name="audioAttributes">An instance of the IMFAttributes interface, which is used to configure the audio renderer.</param>
        /// <param name="mediaSink">Receives an instance of the audio renderer's IMFMediaSink interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateAudioRenderer(IMFAttributes audioAttributes, out IMFMediaSink mediaSink)
        {
            return MFExtern.MFCreateAudioRenderer(audioAttributes, out mediaSink);
        }

        /// <summary>
        /// Creates the Streaming Audio Renderer.
        /// </summary>
        /// <param name="audioAttributes">An instance of the IMFAttributes interface, which is used to configure the audio renderer.</param>
        /// <returns>Returns an instance of the audio renderer's IMFMediaSink interface.</returns>
        public static IMFMediaSink CreateAudioRenderer(IMFAttributes audioAttributes)
        {
            IMFMediaSink result;

            HResult hr = MFExtern.MFCreateAudioRenderer(audioAttributes, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates an activation object for the enhanced video renderer (EVR) media sink.
        /// </summary>
        /// <param name="hwndVideo">Handle to the window where the video will be displayed.</param>
        /// <param name="activate">Receives an instance of the IMFActivate interface. Use this interface to create the EVR.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateVideoRendererActivate(IntPtr hwndVideo, out IMFActivate activate)
        {
            return MFExtern.MFCreateVideoRendererActivate(hwndVideo, out activate);
        }

        /// <summary>
        /// Creates an activation object for the enhanced video renderer (EVR) media sink.
        /// </summary>
        /// <param name="hwndVideo">Handle to the window where the video will be displayed.</param>
        /// <returns>Returns an instance of the IMFActivate interface. Use this interface to create the EVR.</returns>
        public static IMFActivate CreateVideoRendererActivate(IntPtr hwndVideo)
        {
            IMFActivate result;

            HResult hr = MFExtern.MFCreateVideoRendererActivate(hwndVideo, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates an instance of the enhanced video renderer (EVR) media sink.
        /// </summary>
        /// <param name="riidRenderer">Interface identifier (IID) of the requested interface on the EVR.</param>
        /// <param name="renderer">Receives an instance of the requested interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateVideoRenderer(Guid riidRenderer, out object renderer)
        {
            return MFExtern.MFCreateVideoRenderer(riidRenderer, out renderer);
        }

        /// <summary>
        /// Creates an instance of the enhanced video renderer (EVR) media sink.
        /// </summary>
        /// <typeparam name="T">Type of the renderer interface</typeparam>
        /// <param name="renderer">Receives an instance of the requested interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateVideoRenderer<T>(out T renderer) where T : class
        {
            if (!typeof(T).IsComInterface())
                throw new ArgumentOutOfRangeException("T", "T must be a COM visible interface.");

            object tmp;

            HResult hr = MFExtern.MFCreateVideoRenderer(typeof(T).GUID, out tmp);
            renderer = hr.Succeeded() ? tmp as T : null; 

            return hr;
        }

        /// <summary>
        /// Creates an instance of the enhanced video renderer (EVR) media sink.
        /// </summary>
        /// <param name="mediaSink">Receives an instance of the EVR's IMFMediaSink interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateVideoRenderer(out IMFMediaSink mediaSink)
        {
            return CreateVideoRenderer<IMFMediaSink>(out mediaSink);
        }

        /// <summary>
        /// Creates an instance of the enhanced video renderer (EVR) media sink.
        /// </summary>
        /// <typeparam name="T">Type of the renderer interface</typeparam>
        /// <returns>Returns an instance of the requested interface.</returns>
        public static T CreateVideoRenderer<T>() where T : class
        {
            if (!typeof(T).IsComInterface())
                throw new ArgumentOutOfRangeException("T", "T must be a COM visible interface.");

            object tmp;

            HResult hr = MFExtern.MFCreateVideoRenderer(typeof(T).GUID, out tmp);
            hr.ThrowExceptionOnError();

            return tmp as T;
        }

        /// <summary>
        /// Creates an instance of the enhanced video renderer (EVR) media sink.
        /// </summary>
        /// <returns>Returns an instance of the EVR's IMFMediaSink interface.</returns>
        public static IMFMediaSink CreateVideoRenderer()
        {
            return CreateVideoRenderer<IMFMediaSink>();
        }

        /// <summary>
        /// Creates the default video presenter for the enhanced video renderer (EVR).
        /// </summary>
        /// <param name="owner">An instance of the owner of the object. This parameter can be null.</param>
        /// <param name="riidDevice">Interface identifier (IID) of the video device interface that will be used for processing the video. Currently the only supported value is IID_IDirect3DDevice9.</param>
        /// <param name="riid">IID of the requested interface on the video presenter. The video presenter exposes the IMFVideoPresenter interface.</param>
        /// <param name="videoPresenter">Receives an instance of the requested interface on the video presenter.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateVideoPresenter(object owner, Guid riidDevice, Guid riid, out object videoPresenter)
        {
            return MFExtern.MFCreateVideoPresenter(owner, riidDevice, riid, out videoPresenter);
        }

        private static readonly Guid IID_IDirect3DDevice9 = new Guid(0xD0223B96, 0xBF7A, 0x43fd, 0x92, 0xbd, 0xA4, 0x3B, 0x0D, 0x82, 0xB9, 0xEB);

        /// <summary>
        /// Creates the default video presenter for the enhanced video renderer (EVR).
        /// </summary>
        /// <param name="videoPresenter">Receives an instance of the IMFVideoPresenter interface on the video presenter.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateVideoPresenter(out IMFVideoPresenter videoPresenter)
        {
            object tmp;

            HResult hr = MFExtern.MFCreateVideoPresenter(null, IID_IDirect3DDevice9, typeof(IMFVideoPresenter).GUID, out tmp);
            videoPresenter = hr.Succeeded() ? tmp as IMFVideoPresenter : null;

            return hr;
        }

        /// <summary>
        /// Creates the default video presenter for the enhanced video renderer (EVR).
        /// </summary>
        /// <returns>Returns an instance of the IMFVideoPresenter interface on the video presenter.</returns>
        public static IMFVideoPresenter CreateVideoPresenter()
        {
            IMFVideoPresenter result;

            HResult hr = MF.CreateVideoPresenter(out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates the default video mixer for the enhanced video renderer (EVR).
        /// </summary>
        /// <param name="owner">An instance of the owner of the object. This parameter can be null.</param>
        /// <param name="riidDevice">Interface identifier (IID) of the video device interface that will be used for processing the video. Currently the only supported value is IID_IDirect3DDevice9.</param>
        /// <param name="riid">IID of the requested interface on the video presenter. The video presenter exposes the IMFTransform interface.</param>
        /// <param name="videoMixer">Receives an instance of the requested interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateVideoMixer(object owner, Guid riidDevice, Guid riid, out object videoMixer)
        {
            return MFExtern.MFCreateVideoMixer(owner, riidDevice, riid, out videoMixer);
        }

        /// <summary>
        /// Creates the default video mixer for the enhanced video renderer (EVR).
        /// </summary>
        /// <param name="videoMixer">Receives an instance of the IMFTransform interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateVideoMixer(out IMFTransform videoMixer)
        {
            object tmp;

            HResult hr = MFExtern.MFCreateVideoMixer(null, IID_IDirect3DDevice9, typeof(IMFVideoPresenter).GUID, out tmp);
            videoMixer = hr.Succeeded() ? tmp as IMFTransform : null;

            return hr;
        }

        /// <summary>
        /// Creates the default video mixer for the enhanced video renderer (EVR).
        /// </summary>
        /// <returns>Returns an instance of the IMFTransform interface.</returns>
        public static IMFTransform CreateVideoMixer()
        {
            IMFTransform result;

            HResult hr = MF.CreateVideoMixer(out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates the default video mixer and video presenter for the enhanced video renderer (EVR).
        /// </summary>
        /// <param name="mixerOwner">An instance of the owner of the video mixer. This parameter can be null.</param>
        /// <param name="presenterOwner">An instance of the owner of the video presenter. This parameter can be null.</param>
        /// <param name="riidMixer">Interface identifier (IID) of the requested interface on the video mixer. The video mixer exposes the IMFTransform interface.</param>
        /// <param name="riidPresenter">IID of the requested interface on the video presenter. The video presenter exposes the IMFVideoPresenter interface.</param>
        /// <param name="videoMixer">Receives an instance of the requested interface on the video mixer.</param>
        /// <param name="videoPresenter">Receives an instance of the requested interface on the video presenter.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateVideoMixerAndPresenter(object mixerOwner, object presenterOwner, Guid riidMixer, Guid riidPresenter, out object videoMixer, out object videoPresenter)
        {
            return MFExtern.MFCreateVideoMixerAndPresenter(mixerOwner, presenterOwner, riidMixer, out videoMixer, riidPresenter, out videoPresenter);
        }

        /// <summary>
        /// Creates the default video mixer and video presenter for the enhanced video renderer (EVR).
        /// </summary>
        /// <param name="videoMixer">Receives an instance of the IMFTransform interface on the video mixer.</param>
        /// <param name="videoPresenter">Receives an instance of the IMFVideoPresenter interface on the video presenter.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateVideoMixerAndPresenter(out IMFTransform videoMixer, out IMFVideoPresenter videoPresenter)
        {
            object tmp1, tmp2;

            HResult hr = MFExtern.MFCreateVideoMixerAndPresenter(null, null, typeof(IMFTransform).GUID, out tmp1, typeof(IMFVideoPresenter).GUID, out tmp2);
            if (hr.Succeeded())
            {
                videoMixer = tmp1 as IMFTransform;
                videoPresenter = tmp2 as IMFVideoPresenter;
            }
            else
            {
                videoMixer = null;
                videoPresenter = null;
            }

            return hr;
        }

        /// <summary>
        /// Creates the MP3 media sink.
        /// </summary>
        /// <param name="targetByteStream">An instance of the IMFByteStream interface of a byte stream. The media sink writes the MP3 file to this byte stream. The byte stream must be writable.</param>
        /// <param name="mediaSink">Receives an instance of the IMFMediaSink interface of the MP3 media sink.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateMP3MediaSink(IMFByteStream targetByteStream, out IMFMediaSink mediaSink)
        {
            return MFExtern.MFCreateMP3MediaSink(targetByteStream, out mediaSink);
        }

        /// <summary>
        /// Creates the MP3 media sink.
        /// </summary>
        /// <param name="targetByteStream">An instance of the IMFByteStream interface of a byte stream. The media sink writes the MP3 file to this byte stream. The byte stream must be writable.</param>
        /// <returns>Returns an instance of the IMFMediaSink interface of the MP3 media sink.</returns>
        public static IMFMediaSink CreateMP3MediaSink(IMFByteStream targetByteStream)
        {
            IMFMediaSink result;

            HResult hr = MFExtern.MFCreateMP3MediaSink(targetByteStream, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates an activation object for the sample grabber media sink.
        /// </summary>
        /// <param name="mediaType">An instance of the IMFMediaType interface, defining the media type for the sample grabber's input stream.</param>
        /// <param name="sampleGrabberSinkCallback">An instance of the IMFSampleGrabberSinkCallback interface of a callback object. The caller must implement this interface.</param>
        /// <param name="activate">Receives an instance of the IMFActivate interface. Use this interface to complete the creation of the sample grabber.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateSampleGrabberSinkActivate(IMFMediaType mediaType, IMFSampleGrabberSinkCallback sampleGrabberSinkCallback, out IMFActivate activate)
        {
            return MFExtern.MFCreateSampleGrabberSinkActivate(mediaType, sampleGrabberSinkCallback, out activate);
        }

        /// <summary>
        /// Creates an activation object for the sample grabber media sink.
        /// </summary>
        /// <param name="mediaType">An instance of the IMFMediaType interface, defining the media type for the sample grabber's input stream.</param>
        /// <param name="sampleGrabberSinkCallback">An instance of the IMFSampleGrabberSinkCallback interface of a callback object. The caller must implement this interface.</param>
        /// <returns>Returns an instance of the IMFActivate interface. Use this interface to complete the creation of the sample grabber.</returns>
        public static IMFActivate CreateSampleGrabberSinkActivate(IMFMediaType mediaType, IMFSampleGrabberSinkCallback sampleGrabberSinkCallback)
        {
            IMFActivate result;

            HResult hr = MFExtern.MFCreateSampleGrabberSinkActivate(mediaType, sampleGrabberSinkCallback, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates the ASF media sink.
        /// </summary>
        /// <param name="byteStream">An instance of a byte stream that will be used to write the ASF stream.</param>
        /// <param name="mediaSink">Receives an instance of the IMFMediaSink interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateASFMediaSink(IMFByteStream byteStream, out IMFMediaSink mediaSink)
        {
            return MFExtern.MFCreateASFMediaSink(byteStream, out mediaSink);
        }

        /// <summary>
        /// Creates the ASF media sink.
        /// </summary>
        /// <param name="byteStream">An instance of a byte stream that will be used to write the ASF stream.</param>
        /// <returns>Returns an instance of the IMFMediaSink interface.</returns>
        public static IMFMediaSink CreateASFMediaSink(IMFByteStream byteStream)
        {
            IMFMediaSink result;

            HResult hr = MFExtern.MFCreateASFMediaSink(byteStream, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates an activation object that can be used to create the ASF media sink.
        /// </summary>
        /// <param name="fileName">A string that contains the output file name.</param>
        /// <param name="contentInfo">An instance of the IMFASFContentInfo interface of an initialized ASF Header Object object. Use this interface to configure the ASF media sink.</param>
        /// <param name="activate">Receives an instance of the IMFActivate interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateASFMediaSinkActivate(string fileName, IMFASFContentInfo contentInfo, out IMFActivate activate)
        {
            return MFExtern.MFCreateASFMediaSinkActivate(fileName, contentInfo, out activate);
        }

        /// <summary>
        /// Creates an activation object that can be used to create the ASF media sink.
        /// </summary>
        /// <param name="fileName">A string that contains the output file name.</param>
        /// <param name="contentInfo">An instance of the IMFASFContentInfo interface of an initialized ASF Header Object object. Use this interface to configure the ASF media sink.</param>
        /// <returns>Returns an instance of the IMFActivate interface.</returns>
        public static IMFActivate CreateASFMediaSinkActivate(string fileName, IMFASFContentInfo contentInfo)
        {
            IMFActivate result;

            HResult hr = MFExtern.MFCreateASFMediaSinkActivate(fileName, contentInfo, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates a media sink for authoring MP4 files.
        /// </summary>
        /// <param name="byteStream">An instance of the IMFByteStream interface of a byte stream. The media sink writes the MP4 file to this byte stream. The byte stream must be writable and support seeking.</param>
        /// <param name="videoMediaType">An instance of the IMFMediaType interface of a video media type.</param>
        /// <param name="audioMediaType">An instance of the IMFMediaType interface of a audio media type.</param>
        /// <param name="mediaSink">Receives an instance of the MP4 media sink's IMFMediaSink interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateMPEG4MediaSink(IMFByteStream byteStream, IMFMediaType videoMediaType, IMFMediaType audioMediaType, out IMFMediaSink mediaSink)
        {
            return MFExtern.MFCreateMPEG4MediaSink(byteStream, videoMediaType, audioMediaType, out mediaSink);
        }

        /// <summary>
        /// Creates a media sink for authoring MP4 files.
        /// </summary>
        /// <param name="byteStream">An instance of the IMFByteStream interface of a byte stream. The media sink writes the MP4 file to this byte stream. The byte stream must be writable and support seeking.</param>
        /// <param name="videoMediaType">An instance of the IMFMediaType interface of a video media type.</param>
        /// <param name="audioMediaType">An instance of the IMFMediaType interface of a audio media type.</param>
        /// <returns>Returns an instance of the MP4 media sink's IMFMediaSink interface.</returns>
        public static IMFMediaSink CreateMPEG4MediaSink(IMFByteStream byteStream, IMFMediaType videoMediaType, IMFMediaType audioMediaType)
        {
            IMFMediaSink result;

            HResult hr = MFExtern.MFCreateMPEG4MediaSink(byteStream, videoMediaType, audioMediaType, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates a media sink for authoring 3GP files.
        /// </summary>
        /// <param name="byteStream">An instance of the IMFByteStream interface of a byte stream. The media sink writes the 3GP file to this byte stream. The byte stream must be writable and support seeking.</param>
        /// <param name="videoMediaType">An instance of the IMFMediaType interface of a video media type.</param>
        /// <param name="audioMediaType">An instance of the IMFMediaType interface of a audio media type.</param>
        /// <param name="mediaSink">Receives an instance of the 3GP media sink's IMFMediaSink interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult Create3GPMediaSink(IMFByteStream byteStream, IMFMediaType videoMediaType, IMFMediaType audioMediaType, out IMFMediaSink mediaSink)
        {
            return MFExtern.MFCreate3GPMediaSink(byteStream, videoMediaType, audioMediaType, out mediaSink);
        }

        /// <summary>
        /// Creates a media sink for authoring 3GP files.
        /// </summary>
        /// <param name="byteStream">An instance of the IMFByteStream interface of a byte stream. The media sink writes the 3GP file to this byte stream. The byte stream must be writable and support seeking.</param>
        /// <param name="videoMediaType">An instance of the IMFMediaType interface of a video media type.</param>
        /// <param name="audioMediaType">An instance of the IMFMediaType interface of a audio media type.</param>
        /// <returns>Returns an instance of the 3GP media sink's IMFMediaSink interface.</returns>
        public static IMFMediaSink Create3GPMediaSink(IMFByteStream byteStream, IMFMediaType videoMediaType, IMFMediaType audioMediaType)
        {
            IMFMediaSink result;

            HResult hr = MFExtern.MFCreate3GPMediaSink(byteStream, videoMediaType, audioMediaType, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates an instance of the AC-3 media sink.
        /// </summary>
        /// <param name="targetByteStream">An instance of the IMFByteStream interface of a byte stream. The media sink writes the AC-3 file to this byte stream. The byte stream must be writable.</param>
        /// <param name="audioMediaType">An instance of the IMFMediaType interface. This parameter specifies the media type for the AC-3 audio stream.</param>
        /// <param name="mediaSink">Receives an instance of the IMFMediaSink interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateAC3MediaSink(IMFByteStream targetByteStream, IMFMediaType audioMediaType, out IMFMediaSink mediaSink)
        {
            return MFExtern.MFCreateAC3MediaSink(targetByteStream, audioMediaType, out mediaSink);
        }

        /// <summary>
        /// Creates an instance of the AC-3 media sink.
        /// </summary>
        /// <param name="targetByteStream">An instance of the IMFByteStream interface of a byte stream. The media sink writes the AC-3 file to this byte stream. The byte stream must be writable.</param>
        /// <param name="audioMediaType">An instance of the IMFMediaType interface. This parameter specifies the media type for the AC-3 audio stream.</param>
        /// <returns>Returns an instance of the IMFMediaSink interface.</returns>
        public static IMFMediaSink CreateAC3MediaSink(IMFByteStream targetByteStream, IMFMediaType audioMediaType)
        {
            IMFMediaSink result;

            HResult hr = MFExtern.MFCreateAC3MediaSink(targetByteStream, audioMediaType, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates an instance of the audio data transport stream (ADTS) media sink.
        /// </summary>
        /// <param name="targetByteStream">An instance of the IMFByteStream interface of a byte stream. The media sink writes the ADTS stream to this byte stream. The byte stream must be writable.</param>
        /// <param name="audioMediaType">An instance of the IMFMediaType interface. This parameter specifies the media type for the ADTS audio stream.</param>
        /// <param name="mediaSink">Receives an instance of the IMFMediaSink interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateADTSMediaSink(IMFByteStream targetByteStream, IMFMediaType audioMediaType, out IMFMediaSink mediaSink)
        {
            return MFExtern.MFCreateADTSMediaSink(targetByteStream, audioMediaType, out mediaSink);
        }

        /// <summary>
        /// Creates an instance of the audio data transport stream (ADTS) media sink.
        /// </summary>
        /// <param name="targetByteStream">An instance of the IMFByteStream interface of a byte stream. The media sink writes the ADTS stream to this byte stream. The byte stream must be writable.</param>
        /// <param name="audioMediaType">An instance of the IMFMediaType interface. This parameter specifies the media type for the ADTS audio stream.</param>
        /// <returns>Returns an instance of the IMFMediaSink interface.</returns>
        public static IMFMediaSink CreateADTSMediaSink(IMFByteStream targetByteStream, IMFMediaType audioMediaType)
        {
            IMFMediaSink result;

            HResult hr = MFExtern.MFCreateADTSMediaSink(targetByteStream, audioMediaType, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates a generic media sink that wraps a multiplexer Microsoft Media Foundation transform (MFT).
        /// </summary>
        /// <param name="guidOutputSubType">The subtype GUID of the output type for the MFT.</param>
        /// <param name="outputAttributes">A list of format attributes for the MFT output type. This parameter is optional and can be null.</param>
        /// <param name="outputByteStream">An instance of the IMFByteStream interface of a byte stream. The output from the MFT is written to this byte stream. This parameter can be null.</param>
        /// <param name="muxSink">Receives an instance of the IMFMediaSink interface of the media sink.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateMuxSink(Guid guidOutputSubType, IMFAttributes outputAttributes, IMFByteStream outputByteStream, out IMFMediaSink muxSink)
        {
            return MFExtern.MFCreateMuxSink(guidOutputSubType, outputAttributes, outputByteStream, out muxSink);
        }

        /// <summary>
        /// Creates a media sink for authoring fragmented MP4 files.
        /// </summary>
        /// <param name="byteStream">An instance of the IMFByteStream interface of a byte stream. The media sink writes the MP4 file to this byte stream. The byte stream must be writable and support seeking.</param>
        /// <param name="videoMediaType">An instance of the IMFMediaType interface of the video media type.</param>
        /// <param name="audioMediaType">An instance of the IMFMediaType interface of the audio media type.</param>
        /// <param name="mediaSink">Receives an instance of the MP4 media sink's IMFMediaSink interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateFMPEG4MediaSink(IMFByteStream byteStream, IMFMediaType videoMediaType, IMFMediaType audioMediaType, out IMFMediaSink mediaSink)
        {
            return MFExtern.MFCreateFMPEG4MediaSink(byteStream, videoMediaType, audioMediaType, out mediaSink);
        }

        /// <summary>
        /// Creates a media sink for authoring fragmented MP4 files.
        /// </summary>
        /// <param name="byteStream">An instance of the IMFByteStream interface of a byte stream. The media sink writes the MP4 file to this byte stream. The byte stream must be writable and support seeking.</param>
        /// <param name="videoMediaType">An instance of the IMFMediaType interface of the video media type.</param>
        /// <param name="audioMediaType">An instance of the IMFMediaType interface of the audio media type.</param>
        /// <returns>Returns an instance of the MP4 media sink's IMFMediaSink interface.</returns>
        public static IMFMediaSink CreateFMPEG4MediaSink(IMFByteStream byteStream, IMFMediaType videoMediaType, IMFMediaType audioMediaType)
        {
            IMFMediaSink result;

            HResult hr = MFExtern.MFCreateFMPEG4MediaSink(byteStream, videoMediaType, audioMediaType, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

#if ALLOW_UNTESTED_INTERFACES

        /// <summary>
        /// Creates an Audio-Video Interleaved (AVI) Sink.
        /// </summary>
        /// <param name="byteStream">An instance of the byte stream that will be used to write the AVI file.</param>
        /// <param name="videoMediaType">An instance of the IMFMediaType interface of the video media type.</param>
        /// <param name="audioMediaType">An instance of the IMFMediaType interface of the audio media type.</param>
        /// <param name="mediaSink">Receives an instance of the IMFMediaSink Interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateAVIMediaSink(IMFByteStream byteStream, IMFMediaType videoMediaType, IMFMediaType audioMediaType, out IMFMediaSink mediaSink)
        {
            return MFExtern.MFCreateAVIMediaSink(byteStream, videoMediaType, audioMediaType, out mediaSink);
        }

        /// <summary>
        /// Creates an Audio-Video Interleaved (AVI) Sink.
        /// </summary>
        /// <param name="byteStream">An instance of the byte stream that will be used to write the AVI file.</param>
        /// <param name="videoMediaType">An instance of the IMFMediaType interface of the video media type.</param>
        /// <param name="audioMediaType">An instance of the IMFMediaType interface of the audio media type.</param>
        /// <returns>Returns an instance of the IMFMediaSink Interface.</returns>
        public static IMFMediaSink CreateAVIMediaSink(IMFByteStream byteStream, IMFMediaType videoMediaType, IMFMediaType audioMediaType)
        {
            IMFMediaSink result;

            HResult hr = MFExtern.MFCreateAVIMediaSink(byteStream, videoMediaType, audioMediaType, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates an WAVE archive sink. The WAVE archive sink takes audio and writes it to an .wav file. 
        /// </summary>
        /// <param name="byteStream">An instance of the byte stream that will be used to write the .wav file.</param>
        /// <param name="audioMediaType">An instance of the audio media type.</param>
        /// <param name="mediaSink">Receives an instance of the IMFMediaSink interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateWAVEMediaSink(IMFByteStream byteStream, IMFMediaType audioMediaType, out IMFMediaSink mediaSink)
        {
            return MFExtern.MFCreateWAVEMediaSink(byteStream, audioMediaType, out mediaSink);
        }

        /// <summary>
        /// Creates an WAVE archive sink. The WAVE archive sink takes audio and writes it to an .wav file. 
        /// </summary>
        /// <param name="byteStream">An instance of the byte stream that will be used to write the .wav file.</param>
        /// <param name="audioMediaType">An instance of the audio media type.</param>
        /// <returns>Returns an instance of the IMFMediaSink interface.</returns>
        public static IMFMediaSink CreateWAVEMediaSink(IMFByteStream byteStream, IMFMediaType audioMediaType)
        {
            IMFMediaSink result;

            HResult hr = MFExtern.MFCreateWAVEMediaSink(byteStream, audioMediaType, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

#endif
        #endregion

        #region ********** Transform methods **********************************

        /// <summary>
        /// Creates an activation object that can be used to create a Windows Media Audio (WMA) encoder.
        /// </summary>
        /// <param name="mediaType">An instance of the IMFMediaType interface. This parameter specifies the encoded output format.</param>
        /// <param name="encodingConfigurationProperties">An instance of the IPropertyStore interface of a property store that contains encoding parameters.</param>
        /// <param name="activate">Receives an instance of the IMFActivate interface. Use this interface to create the encoder.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateWMAEncoderActivate(IMFMediaType mediaType, IPropertyStore encodingConfigurationProperties, out IMFActivate activate)
        {
            return MFExtern.MFCreateWMAEncoderActivate(mediaType, encodingConfigurationProperties, out activate);
        }

        /// <summary>
        /// Creates an activation object that can be used to create a Windows Media Audio (WMA) encoder.
        /// </summary>
        /// <param name="mediaType">An instance of the IMFMediaType interface. This parameter specifies the encoded output format.</param>
        /// <param name="encodingConfigurationProperties">An instance of the IPropertyStore interface of a property store that contains encoding parameters.</param>
        /// <returns>Returns an instance of the IMFActivate interface. Use this interface to create the encoder.</returns>
        public static IMFActivate CreateWMAEncoderActivate(IMFMediaType mediaType, IPropertyStore encodingConfigurationProperties)
        {
            IMFActivate result;

            HResult hr = MFExtern.MFCreateWMAEncoderActivate(mediaType, encodingConfigurationProperties, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates an activation object that can be used to create a Windows Media Video (WMV) encoder. 
        /// </summary>
        /// <param name="mediaType">An instance of the IMFMediaType interface. This parameter specifies the encoded output format.</param>
        /// <param name="encodingConfigurationProperties">An instance of the IPropertyStore interface of a property store that contains encoding parameters.</param>
        /// <param name="activate">Receives an instance of the IMFActivate interface. Use this interface to create the encoder.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateWMVEncoderActivate(IMFMediaType mediaType, IPropertyStore encodingConfigurationProperties, out IMFActivate activate)
        {
            return MFExtern.MFCreateWMVEncoderActivate(mediaType, encodingConfigurationProperties, out activate);
        }

        /// <summary>
        /// Creates an activation object that can be used to create a Windows Media Video (WMV) encoder. 
        /// </summary>
        /// <param name="mediaType">An instance of the IMFMediaType interface. This parameter specifies the encoded output format.</param>
        /// <param name="encodingConfigurationProperties">An instance of the IPropertyStore interface of a property store that contains encoding parameters.</param>
        /// <returns>Returns an instance of the IMFActivate interface. Use this interface to create the encoder.</returns>
        public static IMFActivate CreateWMVEncoderActivate(IMFMediaType mediaType, IPropertyStore encodingConfigurationProperties)
        {
            IMFActivate result;

            HResult hr = MFExtern.MFCreateWMVEncoderActivate(mediaType, encodingConfigurationProperties, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates an instance of the sample copier transform.
        /// </summary>
        /// <param name="copierMFT">Receives an instance of the IMFTransform interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateSampleCopierMFT(out IMFTransform copierMFT)
        {
            return MFExtern.MFCreateSampleCopierMFT(out copierMFT);
        }

        /// <summary>
        /// Creates an instance of the sample copier transform.
        /// </summary>
        /// <returns>Returns an instance of the IMFTransform interface.</returns>
        public static IMFTransform CreateSampleCopierMFT()
        {
            IMFTransform result;

            HResult hr = MFExtern.MFCreateSampleCopierMFT(out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        #endregion

        #region ********** Topology methods ***********************************

        /// <summary>
        /// Creates a topology node.
        /// </summary>
        /// <param name="nodeType">The type of node to create, specified as a member of the MFTopologyType enumeration.</param>
        /// <param name="node">Receives an instance of the node's IMFTopologyNode interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateTopologyNode(MFTopologyType nodeType, out IMFTopologyNode node)
        {
            return MFExtern.MFCreateTopologyNode(nodeType, out node);
        }

        /// <summary>
        /// Creates a topology node.
        /// </summary>
        /// <param name="nodeType">The type of node to create, specified as a member of the MFTopologyType enumeration.</param>
        /// <returns>Returns an instance of the node's IMFTopologyNode interface.</returns>
        public static IMFTopologyNode CreateTopologyNode(MFTopologyType nodeType)
        {
            IMFTopologyNode result;

            HResult hr = MFExtern.MFCreateTopologyNode(nodeType, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates a topology object.
        /// </summary>
        /// <param name="topology">Receives an instance of the IMFTopology interface of the topology object.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateTopology(out IMFTopology topology)
        {
            return MFExtern.MFCreateTopology(out topology);
        }

        /// <summary>
        /// Creates a topology object.
        /// </summary>
        /// <returns>Returns an instance of the IMFTopology interface of the topology object.</returns>
        public static IMFTopology CreateTopology()
        {
            IMFTopology result;

            HResult hr = MFExtern.MFCreateTopology(out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Gets the media type for a stream associated with a topology node.
        /// </summary>
        /// <param name="node">An instance of the IMFTopologyNode interface.</param>
        /// <param name="streamIndex">The identifier of the stream to query.</param>
        /// <param name="output">If true, the function gets an output type otherwise the function gets an input type.</param>
        /// <param name="mediaType">Receives an instance of the IMFMediaType interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetTopoNodeCurrentType(IMFTopologyNode node, int streamIndex, bool output, out IMFMediaType mediaType)
        {
            return MFExtern.MFGetTopoNodeCurrentType(node, streamIndex, output, out mediaType);
        }

        /// <summary>
        /// Creates a new instance of the topology loader.
        /// </summary>
        /// <param name="topoLoader">Receives an instance of the IMFTopoLoader interface of the topology loader.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateTopoLoader(out IMFTopoLoader topoLoader)
        {
            return MFExtern.MFCreateTopoLoader(out topoLoader);
        }

        /// <summary>
        /// Creates a new instance of the topology loader.
        /// </summary>
        /// <returns>Returns an instance of the IMFTopoLoader interface of the topology loader.</returns>
        public static IMFTopoLoader CreateTopoLoader()
        {
            IMFTopoLoader result;

            HResult hr = MFExtern.MFCreateTopoLoader(out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        #endregion

        #region ********** Misc methods ***************************************

        /// <summary>
        /// Creates a presentation time source that is based on the system time.
        /// </summary>
        /// <param name="systemTimeSource">Receives an instance of the object's IMFPresentationTimeSource interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateSystemTimeSource(out IMFPresentationTimeSource systemTimeSource)
        {
            return MFExtern.MFCreateSystemTimeSource(out systemTimeSource);
        }

        /// <summary>
        /// Creates a presentation time source that is based on the system time.
        /// </summary>
        /// <returns>Returns an instance of the object's IMFPresentationTimeSource interface.</returns>
        public static IMFPresentationTimeSource CreateSystemTimeSource()
        {
            IMFPresentationTimeSource result;

            HResult hr = MFExtern.MFCreateSystemTimeSource(out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates an empty collection object.
        /// </summary>
        /// <param name="collection">Receives an instance of the collection object's IMFCollection interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateCollection(out IMFCollection collection)
        {
            return MFExtern.MFCreateCollection(out collection);
        }

        /// <summary>
        /// Creates an empty collection object.
        /// </summary>
        /// <returns>Returns an instance of the collection object's IMFCollection interface.</returns>
        public static IMFCollection CreateCollection()
        {
            IMFCollection result;

            HResult hr = MFExtern.MFCreateCollection(out result);
            hr.ThrowExceptionOnError();

            return result;
        }
        
        /// <summary>
        /// Creates a stream descriptor.
        /// </summary>
        /// <param name="streamIdentifier">Stream identifier.</param>
        /// <param name="mediaTypes">An array of IMFMediaType interface instances. These instances are used to initialize the media type handler for the stream descriptor.</param>
        /// <param name="descriptor">Receives an instance of the IMFStreamDescriptor interface of the new stream descriptor.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateStreamDescriptor(int streamIdentifier, IMFMediaType[] mediaTypes, out IMFStreamDescriptor descriptor)
        {
            if (mediaTypes == null)
                throw new ArgumentNullException("mediaTypes");

            return MFExtern.MFCreateStreamDescriptor(streamIdentifier, mediaTypes.Length, mediaTypes, out descriptor);
        }

        /// <summary>
        /// Creates an event queue.
        /// </summary>
        /// <param name="mediaEventQueue">Receives an instance of the IMFMediaEventQueue interface of the event queue.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateEventQueue(out IMFMediaEventQueue mediaEventQueue)
        {
            return MFExtern.MFCreateEventQueue(out mediaEventQueue);
        }

        /// <summary>
        /// Creates an event queue.
        /// </summary>
        /// <returns>Returns an instance of the IMFMediaEventQueue interface of the event queue.</returns>
        public static IMFMediaEventQueue CreateEventQueue()
        {
            IMFMediaEventQueue result;

            HResult hr = MFExtern.MFCreateEventQueue(out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates a media event object.
        /// </summary>
        /// <param name="mediaEventType">The event type.</param>
        /// <param name="extendedType">The extended type.</param>
        /// <param name="hrStatus">The event status.</param>
        /// <param name="value">The value associated with the event, if any. This parameter can be null.</param>
        /// <param name="mediaEvent">Receives an instance of the IMFMediaEvent interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateMediaEvent(MediaEventType mediaEventType, Guid extendedType, HResult hrStatus, ConstPropVariant value, out IMFMediaEvent mediaEvent)
        {
            return MFExtern.MFCreateMediaEvent(mediaEventType, extendedType, hrStatus, value, out mediaEvent);
        }

        /// <summary>
        /// Creates a media event object.
        /// </summary>
        /// <param name="mediaEventType">The event type.</param>
        /// <param name="extendedType">The extended type.</param>
        /// <param name="hrStatus">The event status.</param>
        /// <param name="value">The value associated with the event, if any. This parameter can be null.</param>
        /// <param name="mediaEvent">Receives an instance of the IMFMediaEvent interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateMediaEvent(MediaEventType mediaEventType, Guid extendedType, HResult hrStatus, object value, out IMFMediaEvent mediaEvent)
        {
            PropVariant propVariant = value != null ? new PropVariant(value) : null;

            try
            {
                return MFExtern.MFCreateMediaEvent(mediaEventType, extendedType, hrStatus, propVariant, out mediaEvent);
            }
            finally
            {
                if (propVariant != null)
                    propVariant.Dispose();
            }
        }

        /// <summary>
        /// Creates an empty media sample.
        /// </summary>
        /// <param name="sample">Receives an instance of the IMFSample interface of the media sample.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateSample(out IMFSample sample)
        {
            return MFExtern.MFCreateSample(out sample);
        }

        /// <summary>
        /// Creates an empty media sample.
        /// </summary>
        /// <returns>Returns an instance of the IMFSample interface of the media sample.</returns>
        public static IMFSample CreateSample()
        {
            IMFSample result;

            HResult hr = MFExtern.MFCreateSample(out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates a media sample that manages a Direct3D surface.
        /// </summary>
        /// <param name="surface">An instance of the Direct3D surface. This parameter can be null.</param>
        /// <param name="sample">Receives an instance of the sample's IMFSample interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateVideoSampleFromSurface(object surface, out IMFSample sample)
        {
            return MFExtern.MFCreateVideoSampleFromSurface(surface, out sample);
        }

        /// <summary>
        /// Creates a media sample that manages a Direct3D surface.
        /// </summary>
        /// <param name="surface">A pointer to the IUnknown interface of the Direct3D surface. This parameter can be null.</param>
        /// <param name="sample">Receives an instance of the sample's IMFSample interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateVideoSampleFromSurface(IntPtr surface, out IMFSample sample)
        {
            return NativeMethods.MFCreateVideoSampleFromSurface(surface, out sample);
        }

        /// <summary>
        /// Queries an object for a specified service interface.
        /// </summary>
        /// <param name="obj">An instance of the object to query.</param>
        /// <param name="guidService">The service identifier (SID) of the service.</param>
        /// <param name="riid">The interface identifier (IID) of the interface being requested.</param>
        /// <param name="serviceObject">Receives the interface instance.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetService(object obj, Guid guidService, Guid riid, out object serviceObject)
        {
            return MFExtern.MFGetService(obj, guidService, riid, out serviceObject);
        }

        /// <summary>
        /// Queries an object for a specified service interface.
        /// </summary>
        /// <typeparam name="T">The type of the requested interface.</typeparam>
        /// <param name="obj">An instance of the object to query.</param>
        /// <param name="guidService">The service identifier (SID) of the service.</param>
        /// <param name="serviceObject">Receives the interface instance.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetService<T>(object obj, Guid guidService, out T serviceObject) where T : class
        {
            if (!Marshal.IsComObject(obj))
                throw new ArgumentOutOfRangeException("obj", "obj must be a COM object.");

            if (!typeof(T).IsComInterface())
                throw new ArgumentOutOfRangeException("T", "T must be a COM visible interface.");

            object tmp;

            HResult hr = MFExtern.MFGetService(obj, guidService, typeof(T).GUID, out tmp);
            serviceObject = hr.Succeeded() ? tmp as T : null;

            return hr;
        }

        /// <summary>
        /// Creates the Media Session in the application's process.
        /// </summary>
        /// <param name="configuration">An instance of the IMFAttributes interface. This parameter can be null.</param>
        /// <param name="mediaSession">Receives an instance of the Media Session's IMFMediaSession interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateMediaSession(IMFAttributes configuration, out IMFMediaSession mediaSession)
        {
            return MFExtern.MFCreateMediaSession(configuration, out mediaSession);
        }

        /// <summary>
        /// Creates the Media Session in the application's process.
        /// </summary>
        /// <param name="configuration">An instance of the IMFAttributes interface. This parameter can be null.</param>
        /// <returns>Returns an instance of the Media Session's IMFMediaSession interface.</returns>
        public static IMFMediaSession CreateMediaSession(IMFAttributes configuration)
        {
            IMFMediaSession result;

            HResult hr = MFExtern.MFCreateMediaSession(configuration, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates the presentation clock.
        /// </summary>
        /// <param name="presentationClock">Receives an instance of the clock's IMFPresentationClock interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreatePresentationClock(out IMFPresentationClock presentationClock)
        {
            return MFExtern.MFCreatePresentationClock(out presentationClock);
        }

        /// <summary>
        /// Creates the presentation clock.
        /// </summary>
        /// <returns>Returns an instance of the clock's IMFPresentationClock interface.</returns>
        public static IMFPresentationClock CreatePresentationClock()
        {
            IMFPresentationClock result;

            HResult hr = MFExtern.MFCreatePresentationClock(out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates a media-type handler that supports a single media type at a time.
        /// </summary>
        /// <param name="handler">Receives an instance of the IMFMediaTypeHandler interface of the media-type handler.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateSimpleTypeHandler(out IMFMediaTypeHandler handler)
        {
            return MFExtern.MFCreateSimpleTypeHandler(out handler);
        }

        /// <summary>
        /// Creates a media-type handler that supports a single media type at a time.
        /// </summary>
        /// <returns>Returns an instance of the IMFMediaTypeHandler interface of the media-type handler.</returns>
        public static IMFMediaTypeHandler CreateSimpleTypeHandler()
        {
            IMFMediaTypeHandler result;

            HResult hr = MFExtern.MFCreateSimpleTypeHandler(out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates a PropVariant that can be used to seek within a sequencer source presentation.
        /// </summary>
        /// <param name="id">Sequencer element identifier. This value specifies the segment in which to begin playback.</param>
        /// <param name="offset">Starting position within the segment, in 100-nanosecond units.</param>
        /// <param name="segmentOffset">Receives an instance of a PropVariant. The method fills in the PropVariant with the information needed for performing a seek operation.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateSequencerSegmentOffset(int id, long offset, out PropVariant segmentOffset)
        {
            segmentOffset = new PropVariant();

            return MFExtern.MFCreateSequencerSegmentOffset(id, offset, segmentOffset);
        }

        /// <summary>
        /// Creates a PropVariant that can be used to seek within a sequencer source presentation.
        /// </summary>
        /// <param name="id">Sequencer element identifier. This value specifies the segment in which to begin playback.</param>
        /// <param name="offset">Starting position within the segment</param>
        /// <param name="segmentOffset">Receives an instance of a PropVariant. The method fills in the PropVariant with the information needed for performing a seek operation.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateSequencerSegmentOffset(int id, TimeSpan offset, out PropVariant segmentOffset)
        {
            return CreateSequencerSegmentOffset(id, offset.Ticks, out segmentOffset);
        }

        /// <summary>
        /// Creates a PropVariant that can be used to seek within a sequencer source presentation.
        /// </summary>
        /// <param name="id">Sequencer element identifier. This value specifies the segment in which to begin playback.</param>
        /// <param name="offset">Starting position within the segment, in 100-nanosecond units.</param>
        /// <returns>Returns an instance of a PropVariant. The method fills in the PropVariant with the information needed for performing a seek operation.</returns>
        public static PropVariant CreateSequencerSegmentOffset(int id, long offset)
        {
            PropVariant result = new PropVariant();

            HResult hr = MFExtern.MFCreateSequencerSegmentOffset(id, offset, result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates a PropVariant that can be used to seek within a sequencer source presentation.
        /// </summary>
        /// <param name="id">Sequencer element identifier. This value specifies the segment in which to begin playback.</param>
        /// <param name="offset">Starting position within the segment.</param>
        /// <returns>Returns an instance of a PropVariant. The method fills in the PropVariant with the information needed for performing a seek operation.</returns>
        public static PropVariant CreateSequencerSegmentOffset(int id, TimeSpan offset)
        {
            return CreateSequencerSegmentOffset(id, offset.Ticks);
        }

        /// <summary>
        /// Converts a Media Foundation media buffer into a buffer that is compatible with DirectX Media Objects (DMOs).
        /// </summary>
        /// <param name="sample">An instance of the IMFSample interface of the sample that contains the Media Foundation buffer. This parameter can be null.</param>
        /// <param name="mediaBuffer">An instance of the IMFMediaBuffer interface of the Media Foundation buffer.</param>
        /// <param name="offset">Offset in bytes from the start of the Media Foundation buffer. This offset defines where the DMO buffer starts. If this parameter is zero, the DMO buffer starts at the beginning of the Media Foundation buffer.</param>
        /// <param name="legacyMediaBuffer">Receives an instance of a DMO IMediaBuffer interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateLegacyMediaBufferOnMFMediaBuffer(IMFSample sample, IMFMediaBuffer mediaBuffer, int offset, out object legacyMediaBuffer)
        {
            return MFExtern.MFCreateLegacyMediaBufferOnMFMediaBuffer(sample, mediaBuffer, offset, out legacyMediaBuffer);
        }

        /// <summary>
        /// Creates the sequencer source.
        /// </summary>
        /// <param name="sequencerSource">Receives an instance of the IMFSequencerSource interface of the sequencer source.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateSequencerSource(out IMFSequencerSource sequencerSource)
        {
            return MFExtern.MFCreateSequencerSource(null, out sequencerSource);
        }

        /// <summary>
        /// Creates the sequencer source.
        /// </summary>
        /// <returns>Returns an instance of the IMFSequencerSource interface of the sequencer source.</returns>
        public static IMFSequencerSource CreateSequencerSource()
        {
            IMFSequencerSource result;

            HResult hr = MFExtern.MFCreateSequencerSource(null, out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Calculates the minimum surface stride for a video format.
        /// </summary>
        /// <param name="format">FOURCC code or D3DFORMAT value that specifies the video format.</param>
        /// <param name="width">Width of the image, in pixels.</param>
        /// <param name="stride">Receives the minimum surface stride, in pixels.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetStrideForBitmapInfoHeader(int format, int width, out int stride)
        {
            return MFExtern.MFGetStrideForBitmapInfoHeader(format, width, out stride);
        }

        /// <summary>
        /// Retrieves the image size for a video format. Given a BitmapInfoHeader structure, this function calculates the correct value of the ImageSize member.
        /// </summary>
        /// <param name="bih">A BitmapInfoHeader structure that describes the format.</param>
        /// <param name="imageSize">Receives the image size, in bytes.</param>
        /// <param name="known">Receives the value true if the function recognizes the video format. Otherwise, receives the value false.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CalculateBitmapImageSize(BitmapInfoHeader bih, out int imageSize, out bool known)
        {
            return MFExtern.MFCalculateBitmapImageSize(bih, Marshal.SizeOf(bih), out imageSize, out known);
        }

        /// <summary>
        /// Converts a video frame rate into a frame duration.
        /// </summary>
        /// <param name="numerator">The numerator of the frame rate.</param>
        /// <param name="denominator">The denominator of the frame rate.</param>
        /// <param name="averageTimePerFrame">Receives the average duration of a video frame, in 100-nanosecond units.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult FrameRateToAverageTimePerFrame(int numerator, int denominator, out long averageTimePerFrame)
        {
            return MFExtern.MFFrameRateToAverageTimePerFrame(numerator, denominator, out averageTimePerFrame);
        }

        /// <summary>
        /// Converts a video frame rate into a frame duration.
        /// </summary>
        /// <param name="numerator">The numerator of the frame rate.</param>
        /// <param name="denominator">The denominator of the frame rate.</param>
        /// <param name="averageTimePerFrame">Receives the average duration of a video frame.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult FrameRateToAverageTimePerFrame(int numerator, int denominator, out TimeSpan averageTimePerFrame)
        {
            long result;

            HResult hr = MFExtern.MFFrameRateToAverageTimePerFrame(numerator, denominator, out result);
            averageTimePerFrame = hr.Succeeded() ? TimeSpan.FromTicks(result) : default(TimeSpan);

            return hr;
        }

        /// <summary>
        /// Copies an image or image plane from one buffer to another.
        /// </summary>
        /// <param name="destination">Pointer to the start of the first row of pixels in the destination buffer.</param>
        /// <param name="destinationStride">Stride of the destination buffer, in bytes.</param>
        /// <param name="source">Pointer to the start of the first row of pixels in the source image.</param>
        /// <param name="sourceStride">Stride of the source image, in bytes.</param>
        /// <param name="widthInBytes">Width of the image, in bytes.</param>
        /// <param name="lines">Number of rows of pixels to copy.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CopyImage(IntPtr destination, int destinationStride, IntPtr source, int sourceStride, int widthInBytes, int lines)
        {
            return MFExtern.MFCopyImage(destination, destinationStride, source, sourceStride, widthInBytes, lines);
        }

        /// <summary>
        /// Gets an instance of the Microsoft Media Foundation plug-in manager.
        /// </summary>
        /// <param name="pluginControl">Receives an instance of the IMFPluginControl interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetPluginControl(out IMFPluginControl pluginControl)
        {
            return MFExtern.MFGetPluginControl(out pluginControl);
        }

        /// <summary>
        /// Gets an instance of the Microsoft Media Foundation plug-in manager.
        /// </summary>
        /// <returns>Returns an instance of the IMFPluginControl interface.</returns>
        public static IMFPluginControl GetPluginControl()
        {
            IMFPluginControl result;

            HResult hr = MFExtern.MFGetPluginControl(out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates a generic activation object for Media Foundation transforms (MFTs).
        /// </summary>
        /// <param name="activate">Receives an instance of the IMFActivate interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateTransformActivate(out IMFActivate activate)
        {
            return MFExtern.MFCreateTransformActivate(out activate);
        }

        /// <summary>
        /// Creates a generic activation object for Media Foundation transforms (MFTs).
        /// </summary>
        /// <returns>Returns an instance of the IMFActivate interface.</returns>
        public static IMFActivate CreateTransformActivate()
        {
            IMFActivate result;

            HResult hr = MFExtern.MFCreateTransformActivate(out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates a credential cache object. An application can use this object to implement a custom credential manager.
        /// </summary>
        /// <param name="credentialCache">Receives an instance of the IMFNetCredentialCache interface of the new credential cache object.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateCredentialCache(out IMFNetCredentialCache credentialCache)
        {
            return MFExtern.MFCreateCredentialCache(out credentialCache);
        }

        /// <summary>
        /// Queries whether a FOURCC code or D3DFORMAT value is a YUV format.
        /// </summary>
        /// <param name="format">FOURCC code or D3DFORMAT value.</param>
        /// <returns>Returns true if the value specifies a YUV format. Otherwise, returns false.</returns>
        public static bool IsFormatYUV(int format)
        {
            return MFExtern.MFIsFormatYUV(format);
        }

        /// <summary>
        /// Retrieves the image size, in bytes, for an uncompressed video format.
        /// </summary>
        /// <param name="format">FOURCC code or D3DFORMAT value that specifies the video format.</param>
        /// <param name="width">Width of the image, in pixels.</param>
        /// <param name="height">Height of the image, in pixels.</param>
        /// <param name="planeSize">Receives the size of one frame, in bytes. If the format is compressed or is not recognized, this value is zero.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetPlaneSize(int format, int width, int height, out int planeSize)
        {
            return MFExtern.MFGetPlaneSize(format, width, height, out planeSize);
        }

        /// <summary>
        /// Converts an array of 16-bit floating-point numbers into an array of 32-bit floating-point numbers.
        /// </summary>
        /// <param name="destination">An array of float values. The array must contain at least <paramref name="count"/> elements.</param>
        /// <param name="source">An array of 16-bit floating-point values, typed as Int16 values. The array must contain at least <paramref name="count"/> elements.</param>
        /// <param name="count">Number of elements in the <paramref name="source"/> array to convert.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult ConvertFromFP16Array(float[] destination, short[] source, int count)
        {
            return MFExtern.MFConvertFromFP16Array(destination, source, count);
        }

        /// <summary>
        /// Converts an array of 32-bit floating-point numbers into an array of 16-bit floating-point numbers.
        /// </summary>
        /// <param name="destination">An array of 16-bit floating-point values, typed as Int16 values. The array must contain at least <paramref name="count"/> elements.</param>
        /// <param name="source">An array of float values. The array must contain at least <paramref name="count"/> elements.</param>
        /// <param name="count">Number of elements in the <paramref name="source"/> array to convert.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult ConvertToFP16Array(short[] destination, float[] source, int count)
        {
            return MFExtern.MFConvertToFP16Array(destination, source, count);
        }

        /// <summary>
        /// Creates an object that allocates video samples.
        /// </summary>
        /// <param name="sampleAllocator">Receives a pointer to the IMFVideoSampleAllocator interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateVideoSampleAllocator(out IMFVideoSampleAllocator sampleAllocator)
        {
            object tmp;

            HResult hr = MFExtern.MFCreateVideoSampleAllocator(typeof(IMFVideoSampleAllocator).GUID, out tmp);
            sampleAllocator = hr.Succeeded() ? tmp as IMFVideoSampleAllocator : null;

            return hr;
        }

        /// <summary>
        /// Creates an object that allocates video samples.
        /// </summary>
        /// <param name="sampleAllocatorCallback">Receives a pointer to the IMFVideoSampleAllocatorCallback interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateVideoSampleAllocator(out IMFVideoSampleAllocatorCallback sampleAllocatorCallback)
        {
            object tmp;

            HResult hr = MFExtern.MFCreateVideoSampleAllocator(typeof(IMFVideoSampleAllocatorCallback).GUID, out tmp);
            sampleAllocatorCallback = hr.Succeeded() ? tmp as IMFVideoSampleAllocatorCallback : null;

            return hr;
        }

        /// <summary>
        /// Creates an object that allocates video samples that are compatible with Microsoft DirectX Graphics Infrastructure (DXGI).
        /// </summary>
        /// <param name="sampleAllocatorCallback">Receives a pointer to the IMFVideoSampleAllocator interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateVideoSampleAllocatorEx(out IMFVideoSampleAllocator sampleAllocator)
        {
            object tmp;

            HResult hr = MFExtern.MFCreateVideoSampleAllocatorEx(typeof(IMFVideoSampleAllocator).GUID, out tmp);
            sampleAllocator = hr.Succeeded() ? tmp as IMFVideoSampleAllocator : null;

            return hr;
        }

        /// <summary>
        /// Creates an object that allocates video samples that are compatible with Microsoft DirectX Graphics Infrastructure (DXGI).
        /// </summary>
        /// <param name="sampleAllocatorCallback">Receives a pointer to the IMFVideoSampleAllocatorEx interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateVideoSampleAllocatorEx(out IMFVideoSampleAllocatorEx sampleAllocatorCallback)
        {
            object tmp;

            HResult hr = MFExtern.MFCreateVideoSampleAllocatorEx(typeof(IMFVideoSampleAllocatorEx).GUID, out tmp);
            sampleAllocatorCallback = hr.Succeeded() ? tmp as IMFVideoSampleAllocatorEx : null;

            return hr;
        }

        /// <summary>
        /// Creates an object that allocates video samples that are compatible with Microsoft DirectX Graphics Infrastructure (DXGI).
        /// </summary>
        /// <param name="sampleAllocatorCallback">Receives a pointer to the IMFVideoSampleAllocatorCallback interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateVideoSampleAllocatorEx(out IMFVideoSampleAllocatorCallback sampleAllocatorCallback)
        {
            object tmp;

            HResult hr = MFExtern.MFCreateVideoSampleAllocatorEx(typeof(IMFVideoSampleAllocatorCallback).GUID, out tmp);
            sampleAllocatorCallback = hr.Succeeded() ? tmp as IMFVideoSampleAllocatorCallback : null;

            return hr;
        }

        /// <summary>
        /// Calculates ((a * b) + d) / c, where each term is a 64-bit signed value.
        /// </summary>
        /// <param name="a">A multiplier.</param>
        /// <param name="b">Another multiplier.</param>
        /// <param name="c">The divisor.</param>
        /// <param name="d">The rounding factor.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static long MulDiv(long a, long b, long c, long d)
        {
            return MFExtern.MFllMulDiv(a, b, c, d);
        }

        /// <summary>
        /// Shuts down a Media Foundation object and releases all resources associated with the object.
        /// </summary>
        /// <param name="obj">An instance of a shutdownable object.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        /// <remarks>This is an helper method that wraps the IMFShutdown.Shutdown method. The method queries the object for the IMFShutdown interface and, if successful, calls Shutdown on the object.</remarks>
        public static HResult ShutdownObject(object obj)
        {
            return MFExtern.MFShutdownObject(obj);
        }

        /// <summary>
        /// Creates a default proxy locator.
        /// </summary>
        /// <param name="protocol">The name of the protocol.</param>
        /// <param name="proxyConfig">An instance of the IPropertyStore interface of a property store that contains the proxy configuration in the MFNETSOURCE_PROXYSETTINGS property.</param>
        /// <param name="proxyLocator">Receives an instance of the IMFNetProxyLocator interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateProxyLocator(string protocol, IPropertyStore proxyConfig, out IMFNetProxyLocator proxyLocator)
        {
            return MFExtern.MFCreateProxyLocator(protocol, proxyConfig, out proxyLocator);
        }

        /// <summary>
        /// Creates the scheme handler for the network source.
        /// </summary>
        /// <param name="riid">Interface identifier (IID) of the interface to retrieve.</param>
        /// <param name="schemeHandler">Receives an instance of the requested interface. The scheme handler exposes the IMFSchemeHandler interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateNetSchemePlugin(Guid riid, out object schemeHandler)
        {
            return MFExtern.MFCreateNetSchemePlugin(riid, out schemeHandler);
        }

        /// <summary>
        /// Creates the scheme handler for the network source.
        /// </summary>
        /// <param name="schemeHandler">Receives an instance of the IMFSchemeHandler interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateNetSchemePlugin(out IMFSchemeHandler schemeHandler)
        {
            object tmp;

            HResult hr = MFExtern.MFCreateNetSchemePlugin(typeof(IMFSchemeHandler).GUID, out tmp);
            schemeHandler = hr.Succeeded() ? tmp as IMFSchemeHandler : null;

            return hr;
        }

        /// <summary>
        /// Validates the size of a buffer for a video format block.
        /// </summary>
        /// <param name="formatType">GUID that specifies the type of format block.</param>
        /// <param name="blockPtr">Pointer to a buffer that contains the format block.</param>
        /// <param name="blockSize">Size of the pBlock buffer, in bytes.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult ValidateMediaTypeSize(Guid formatType, IntPtr blockPtr, int blockSize)
        {
            return MFExtern.MFValidateMediaTypeSize(formatType, blockPtr, blockSize);
        }

        /// <summary>
        /// Retrieves the image size, in bytes, for an uncompressed video format.
        /// </summary>
        /// <param name="guidSubtype">Media subtype for the video format.</param>
        /// <param name="width">Width of the image, in pixels.</param>
        /// <param name="height">Height of the image, in pixels.</param>
        /// <param name="imageSize">Receives the size of each frame, in bytes. If the format is compressed or is not recognized, the value is zero.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CalculateImageSize(Guid guidSubtype, int width, int height, out int imageSize)
        {
            return MFExtern.MFCalculateImageSize(guidSubtype, width, height, out imageSize);
        }

        /// <summary>
        /// Calculates the frame rate, in frames per second, from the average duration of a video frame.
        /// </summary>
        /// <param name="averageTimePerFrame">The average duration of a video frame, in 100-nanosecond units.</param>
        /// <param name="numerator">Receives the numerator of the frame rate.</param>
        /// <param name="denominator">Receives the denominator of the frame rate.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult AverageTimePerFrameToFrameRate(long averageTimePerFrame, out int numerator, out int denominator)
        {
            return MFExtern.MFAverageTimePerFrameToFrameRate(averageTimePerFrame, out numerator, out denominator);
        }

        /// <summary>
        /// Calculates the frame rate, in frames per second, from the average duration of a video frame.
        /// </summary>
        /// <param name="averageTimePerFrame">The average duration of a video frame.</param>
        /// <param name="numerator">Receives the numerator of the frame rate.</param>
        /// <param name="denominator">Receives the denominator of the frame rate.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult AverageTimePerFrameToFrameRate(TimeSpan averageTimePerFrame, out int numerator, out int denominator)
        {
            return MFExtern.MFAverageTimePerFrameToFrameRate(averageTimePerFrame.Ticks, out numerator, out denominator);
        }

        /// <summary>
        /// Creates the remote desktop plug-in object. Use this object if the application is running in a Terminal Services client session.
        /// </summary>
        /// <param name="plugin">Receives an instance of the IMFRemoteDesktopPlugin interface of the plug-in object.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateRemoteDesktopPlugin(out IMFRemoteDesktopPlugin plugin)
        {
            return MFExtern.MFCreateRemoteDesktopPlugin(out plugin);
        }

        /// <summary>
        /// Creates the remote desktop plug-in object. Use this object if the application is running in a Terminal Services client session.
        /// </summary>
        /// <returns>Returns an instance of the IMFRemoteDesktopPlugin interface of the plug-in object.</returns>
        public static IMFRemoteDesktopPlugin CreateRemoteDesktopPlugin()
        {
            IMFRemoteDesktopPlugin result;

            HResult hr = MFExtern.MFCreateRemoteDesktopPlugin(out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates the default implementation of the quality manager.
        /// </summary>
        /// <param name="qualityManager">Receives an instance of the quality manager's IMFQualityManager interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateStandardQualityManager(out IMFQualityManager qualityManager)
        {
            return MFExtern.MFCreateStandardQualityManager(out qualityManager);
        }

        /// <summary>
        /// Creates the default implementation of the quality manager.
        /// </summary>
        /// <returns>Returns an instance of the quality manager's IMFQualityManager interface.</returns>
        public static IMFQualityManager CreateStandardQualityManager()
        {
            IMFQualityManager result;

            HResult hr = MFExtern.MFCreateStandardQualityManager(out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Creates an IMFTrackedSample object that tracks the reference counts on a video media sample.
        /// </summary>
        /// <param name="trackedSample">Receives an instance of the IMFTrackedSample interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateTrackedSample(out IMFTrackedSample trackedSample)
        {
            return MFExtern.MFCreateTrackedSample(out trackedSample);
        }

        /// <summary>
        /// Creates an IMFTrackedSample object that tracks the reference counts on a video media sample.
        /// </summary>
        /// <returns>Returns an instance of the IMFTrackedSample interface.</returns>
        public static IMFTrackedSample CreateTrackedSample()
        {
            IMFTrackedSample result;

            HResult hr = MFExtern.MFCreateTrackedSample(out result);
            hr.ThrowExceptionOnError();

            return result;
        }

        /// <summary>
        /// Converts a Microsoft Direct3D 9 format identifier to a Microsoft DirectX Graphics Infrastructure (DXGI) format identifier.
        /// </summary>
        /// <param name="dx9Format">The D3DFORMAT value or FOURCC code to convert.</param>
        /// <returns>Returns a DXGI_FORMAT value.</returns>
        public static int MapDX9FormatToDXGIFormat(int dx9Format)
        {
            return MFExtern.MFMapDX9FormatToDXGIFormat(dx9Format);
        }

        /// <summary>
        /// Converts a Microsoft DirectX Graphics Infrastructure (DXGI) format identifier to a Microsoft Direct3D 9 format identifier.
        /// </summary>
        /// <param name="dx11Format">The DXGI_FORMAT value to convert.</param>
        /// <returns>Returns a D3DFORMAT value or FOURCC code.</returns>
        public static int MapDXGIFormatToDX9Format(int dx11Format)
        {
            return MFExtern.MFMapDXGIFormatToDX9Format(dx11Format);
        }

        /// <summary>
        /// Unlocks the shared Microsoft DirectX Graphics Infrastructure (DXGI) Device Manager.
        /// </summary>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult UnlockDXGIDeviceManager()
        {
            return MFExtern.MFUnlockDXGIDeviceManager();
        }

        /// <summary>
        /// Registers a scheme handler in the caller's process.
        /// </summary>
        /// <param name="scheme">A string that contains the scheme. The scheme includes the trailing ':' character; for example, "http:".</param>
        /// <param name="activate">An instance of the IMFActivate interface of an activation object.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult RegisterLocalSchemeHandler(string scheme, IMFActivate activate)
        {
            return MFExtern.MFRegisterLocalSchemeHandler(scheme, activate);
        }

        /// <summary>
        /// Registers a byte-stream handler in the caller's process.
        /// </summary>
        /// <param name="fileExtension">A string that contains the file name extension for this handler.</param>
        /// <param name="mimeType">A string that contains the MIME type for this handler.</param>
        /// <param name="activate">An instance of the IMFActivate interface of an activation object.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult RegisterLocalByteStreamHandler(string fileExtension, string mimeType, IMFActivate activate)
        {
            return MFExtern.MFRegisterLocalByteStreamHandler(fileExtension, mimeType, activate);
        }

        /// <summary>
        /// Creates an activation object for a Windows Runtime class.
        /// </summary>
        /// <param name="activatableClassId">The class identifier that is associated with the activatable runtime class.</param>
        /// <param name="configuration">An instance of an optional IPropertySet object, which is used to configure the Windows Runtime class. This parameter can be null.</param>
        /// <param name="riid">The interface identifier (IID) of the interface being requested.</param>
        /// <param name="obj">Receives an instance of the requested interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateMediaExtensionActivate(string activatableClassId, object configuration, Guid riid, out object obj)
        {
            return MFExtern.MFCreateMediaExtensionActivate(activatableClassId, configuration, riid, out obj);
        }

        /// <summary>
        /// Creates an activation object for a Windows Runtime class.
        /// </summary>
        /// <param name="activatableClassId">The class identifier that is associated with the activatable runtime class.</param>
        /// <param name="configuration">An instance of an optional IPropertySet object, which is used to configure the Windows Runtime class. This parameter can be null.</param>
        /// <param name="classFactory">Receives an instance of the IClassFactory interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateMediaExtensionActivate(string activatableClassId, object configuration, out IClassFactory classFactory)
        {

            object tmp;

            HResult hr = MFExtern.MFCreateMediaExtensionActivate(activatableClassId, configuration, typeof(IClassFactory).GUID, out tmp);
            classFactory = hr.Succeeded() ? tmp as IClassFactory : null;

            return hr;
        }

        /// <summary>
        /// Creates an activation object for a Windows Runtime class.
        /// </summary>
        /// <param name="activatableClassId">The class identifier that is associated with the activatable runtime class.</param>
        /// <param name="configuration">An instance of an optional IPropertySet object, which is used to configure the Windows Runtime class. This parameter can be null.</param>
        /// <param name="activate">Receives an instance of the IMFActivate interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateMediaExtensionActivate(string activatableClassId, object configuration, out IMFActivate activate)
        {

            object tmp;

            HResult hr = MFExtern.MFCreateMediaExtensionActivate(activatableClassId, configuration, typeof(IMFActivate).GUID, out tmp);
            activate = hr.Succeeded() ? tmp as IMFActivate : null;

            return hr;
        }

        /// <summary>
        /// Gets the class identifier for a content protection system.
        /// </summary>
        /// <param name="guidProtectionSystemID">The GUID that identifies the content protection system.</param>
        /// <param name="clsid">Receives the class identifier to the content protection system.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetContentProtectionSystemCLSID(Guid guidProtectionSystemID, out Guid clsid)
        {
            return MFExtern.MFGetContentProtectionSystemCLSID(guidProtectionSystemID, out clsid);
        }

#if ALLOW_UNTESTED_INTERFACES

        /// <summary>
        /// Creates an instance of the Microsoft DirectX Graphics Infrastructure (DXGI) Device Manager.
        /// </summary>
        /// <param name="resetToken">Receives a token that identifies this instance of the DXGI Device Manager. Use this token when calling IMFDXGIDeviceManager.ResetDevice. </param>
        /// <param name="deviceManager">Receives an instance of the IMFDXGIDeviceManager interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateDXGIDeviceManager(out int resetToken, out IMFDXGIDeviceManager deviceManager)
        {
            return MFExtern.MFCreateDXGIDeviceManager(out resetToken, out deviceManager);
        }

        /// <summary>
        /// Locks the shared Microsoft DirectX Graphics Infrastructure (DXGI) Device Manager.
        /// </summary>
        /// <param name="resetToken">Receives a token that identifies this instance of the DXGI Device Manager. Use this token when calling IMFDXGIDeviceManager.ResetDevice.</param>
        /// <param name="deviceManager">Receives an instance of the IMFDXGIDeviceManager interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult LockDXGIDeviceManager(out int resetToken, out IMFDXGIDeviceManager deviceManager)
        {
            return MFExtern.MFLockDXGIDeviceManager(out resetToken, out deviceManager);
        }

        /// <summary>
        /// Creates an IMFProtectedEnvironmentAccess object that allows content protection systems to perform a handshake with the protected environment.
        /// </summary>
        /// <param name="protectedEnvironmentAccess">Receives an instance of the IMFProtectedEnvironmentAccess interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateProtectedEnvironmentAccess(out IMFProtectedEnvironmentAccess protectedEnvironmentAccess)
        {
            return MFExtern.MFCreateProtectedEnvironmentAccess(out protectedEnvironmentAccess);
        }

        /// <summary>
        /// Loads a dynamic link library that is signed for the protected environment.
        /// </summary>
        /// <param name="fileName">The name of the dynamic link library to load. This dynamic link library must be signed for the protected environment.</param>
        /// <param name="signedLibrary">Receives an instance of the IMFSignedLibrary interface for the library.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult LoadSignedLibrary(string fileName, out IMFSignedLibrary signedLibrary)
        {
            return MFExtern.MFLoadSignedLibrary(fileName, out signedLibrary);
        }

        /// <summary>
        /// Returns an IMFSystemId object for retrieving system id data.
        /// </summary>
        /// <param name="systemId">Receives an instance of the IMFSystemId interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetSystemId(out IMFSystemId systemId)
        {
            return MFExtern.MFGetSystemId(out systemId);
        }

        /// <summary>
        /// Creates an IMFContentProtectionDevice interface for the specified media protection system.
        /// </summary>
        /// <param name="protectionSystemId">The idenfier of the media protection system for which you want to create the IMFContentProtectionDevice interface.</param>
        /// <param name="contentProtectionDevice">Receives an instance of the created IMFContentProtectionDevice interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateContentProtectionDevice(Guid protectionSystemId, out IMFContentProtectionDevice contentProtectionDevice)
        {
            return MFExtern.MFCreateContentProtectionDevice(protectionSystemId, out contentProtectionDevice);
        }

        /// <summary>
        /// Checks whether a hardware security processor is supported for the specified media protection system.
        /// </summary>
        /// <param name="protectionSystemId">The identifier of the protection system that you want to check.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CheckContentProtectionDevice(Guid protectionSystemId)
        {
            return MFExtern.MFCheckContentProtectionDevice(protectionSystemId);
        }

        /// <summary>
        /// Checks whether a hardware security processor is supported for the specified media protection system.
        /// </summary>
        /// <param name="protectionSystemId">The identifier of the protection system that you want to check.</param>
        /// <param name="isSupported">True if the hardware security processor is supported for the specified protection system; otherwise false.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult IsContentProtectionDeviceSupported(Guid protectionSystemId, out bool isSupported)
        {
            return MFExtern.MFIsContentProtectionDeviceSupported(protectionSystemId, out isSupported);
        }

        /// <summary>
        /// Creates an IMFContentDecryptorContext interface for the specified media protection system.
        /// </summary>
        /// <param name="guidMediaProtectionSystemId">The identifier of the media protection system for which you want to create an IMFContentDecryptorContext interface.</param>
        /// <param name="dxgiManager">An instance of the IMFDXGIDeviceManager interface that you want to use for sharing the Direct3D 11 device.</param>
        /// <param name="contentProtectionDevice">The IMFContentProtectionDevice interface for the specified media protection system.</param>
        /// <param name="contentDecryptorContext">Receives an instance of the created IMFContentDecryptorContext interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateContentDecryptorContext(Guid guidMediaProtectionSystemId, IMFDXGIDeviceManager dxgiManager, IMFContentProtectionDevice contentProtectionDevice, out IMFContentDecryptorContext contentDecryptorContext)
        {
            return MFExtern.MFCreateContentDecryptorContext(guidMediaProtectionSystemId, dxgiManager, contentProtectionDevice, out contentDecryptorContext);
        }

        /// <summary>
        /// Gets the merit value of a hardware codec.
        /// </summary>
        /// <param name="mft">An instance of the Media Foundation transform (MFT) that represents the codec.</param>
        /// <param name="mftClsid">The class identifier (CLSID) of the MFT.</param>
        /// <param name="merit">Receives the merit value.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static unsafe HResult GetMFTMerit(object mft, Guid mftClsid, out int merit)
        {
            return MFExtern.MFGetMFTMerit(mft, sizeof(Guid), (IntPtr)((void*)&mftClsid), out merit);
        }

        /// <summary>
        /// Gets the merit value of a hardware codec.
        /// </summary>
        /// <param name="mft">An instance of the Media Foundation transform (MFT) that represents the codec.</param>
        /// <param name="symbolLink">A string that contains the symbol link for the underlying hardware device.</param>
        /// <param name="merit">Receives the merit value.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetMFTMerit(object mft, string symbolLink, out int merit)
        {
            if (string.IsNullOrEmpty(symbolLink))
                throw new ArgumentNullException("symbolLink");

            IntPtr symbolLinkPtr = Marshal.StringToCoTaskMemUni(symbolLink);

            try
            {
                return MFExtern.MFGetMFTMerit(mft, (symbolLink.Length + 1) * 2, symbolLinkPtr, out merit);
            }
            finally
            {
                Marshal.FreeCoTaskMem(symbolLinkPtr);
            }
        }

        /// <summary>
        /// Gets the local system ID.
        /// </summary>
        /// <param name="verifier">Application-specific verifier value.</param>
        /// <param name="id">Returned ID string.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetLocalId(byte[] verifier, out string id)
        {
            if (verifier == null)
                throw new ArgumentNullException("verifier");

            IntPtr idPtr;
            id = null;

            using (GCPin pinned = new GCPin(verifier))
            {
                HResult hr = MFExtern.MFGetLocalId(pinned.PinnedAddress, verifier.Length, out idPtr);
                if (hr.Succeeded())
                {
                    id = Marshal.PtrToStringUni(idPtr);
                    Marshal.FreeCoTaskMem(idPtr);
                }

                return hr;
            }
        }

#endif

        #endregion

        #region Private methods

        private static bool GetIsSupported()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Version osVersion = Environment.OSVersion.Version;

                //TODO : Add a check for Windows Servers.
                if (osVersion.Major >= 6)
                {
                    // Windows Vista or Seven
                    if (osVersion.Minor == 0 || osVersion.Minor == 1)
                    {
                        // Windows Vista & Seven N or KN edtions are not shipped with 'mfplat.dll'
                        // http://blogs.msdn.com/b/chuckw/archive/2010/08/13/quot-who-moved-my-windows-media-cheese-quot.aspx
                        return File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "mfplat.dll"));
                    }
                    else // Windows 8 or higher
                    {
                        HResult hr = MFExtern.MFStartup(0x20070, MFStartup.Lite);
                        if (hr.Succeeded()) MFExtern.MFShutdown();

                        return hr.Succeeded();
                    }
                }
            }

            return false;
        }

        private static int GetMediaFoundationVersion()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    if (Environment.OSVersion.Version.Minor == 0)
                        return 0x10070; // Windows Vista
                    else
                        return 0x20070; // Windows Seven or Higher
                }
            }

            throw new NotSupportedException("Windows Vista or higher is need.");
        }

        #endregion
    }

    public enum MemoryAlignment
    {
        AlignTo1Byte = 0,
        AlignTo2Bytes = 1,
        AlignTo4Bytes = 3,
        AlignTo8Bytes = 7,
        AlignTo16Bytes = 15,
        AlignTo32Bytes = 31,
        AlignTo64Bytes = 63,
        AlignTo128Bytes = 127,
        AlignTo256Bytes = 255,
        AlignTo512Bytes = 511,
    }

    internal static partial class NativeMethods
    {
        [DllImport("propsys.dll", ExactSpelling = true), SuppressUnmanagedCodeSecurity]
        public static extern HResult PSCreateMemoryPropertyStore([In, MarshalAs(UnmanagedType.LPStruct)] Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out object ppv);

        [DllImport("evr.dll", ExactSpelling = true), SuppressUnmanagedCodeSecurity]
        public static extern HResult MFCreateVideoSampleFromSurface(IntPtr pUnkSurface, out IMFSample ppSample);

        [DllImport("mfplat.dll", ExactSpelling = true)]
        public static extern HResult MFCreateWICBitmapBuffer([MarshalAs(UnmanagedType.LPStruct)] [In] Guid riid, IntPtr punkSurface, out IMFMediaBuffer ppBuffer);

		[DllImport("mfplat.dll", ExactSpelling = true)]
		public static extern HResult MFCreateDXGISurfaceBuffer([MarshalAs(UnmanagedType.LPStruct)] [In] Guid riid, IntPtr punkSurface, int uSubresourceIndex, [MarshalAs(UnmanagedType.Bool)] bool fBottomUpWhenLinear, out IMFMediaBuffer ppBuffer);

        [DllImport("evr.dll", ExactSpelling = true)]
        public static extern HResult MFCreateDXSurfaceBuffer([MarshalAs(UnmanagedType.LPStruct)] [In] Guid riid, IntPtr punkSurface, [MarshalAs(UnmanagedType.Bool)] [In] bool fBottomUpWhenLinear, out IMFMediaBuffer ppBuffer);

    }
}
