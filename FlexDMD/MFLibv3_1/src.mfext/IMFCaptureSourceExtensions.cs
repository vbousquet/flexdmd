/* license

IMFCaptureSourceExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;
using System.Runtime.InteropServices;

using MediaFoundation.Transform;
using MediaFoundation.ReadWrite;

namespace MediaFoundation
{
    public static class IMFCaptureSourceExtensions
    {
        /// <summary>
        /// Adds an effect to a capture streamIndex.
        /// </summary>
        /// <param name="captureSource">A valid IMFCaptureSource instance.</param>
        /// <param name="sourceStreamIndex">The capture streamIndex.</param>
        /// <param name="transform">A Media Foundation transform instance.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult AddEffect(this IMFCaptureSource captureSource, int sourceStreamIndex, IMFTransform transform)
        {
            if (captureSource == null)
                throw new ArgumentNullException("captureSource");

            return captureSource.AddEffect(sourceStreamIndex, transform);
        }

        /// <summary>
        /// Adds an effect to a capture streamIndex.
        /// </summary>
        /// <param name="captureSource">A valid IMFCaptureSource instance.</param>
        /// <param name="sourceStream">A member of the <see cref="CaptureEngineStreams"/> enumeration.</param>
        /// <param name="transform">A Media Foundation transform instance.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult AddEffect(this IMFCaptureSource captureSource, CaptureEngineStreams sourceStream, IMFTransform transform)
        {
            if (captureSource == null)
                throw new ArgumentNullException("captureSource");

            return captureSource.AddEffect((int)sourceStream, transform);
        }

        /// <summary>
        /// Adds an effect to a capture streamIndex.
        /// </summary>
        /// <param name="captureSource">A valid IMFCaptureSource instance.</param>
        /// <param name="sourceStreamIndex">The capture streamIndex.</param>
        /// <param name="activate">An MFT activation instance.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult AddEffect(this IMFCaptureSource captureSource, int sourceStreamIndex, IMFActivate activate)
        {
            if (captureSource == null)
                throw new ArgumentNullException("captureSource");

            return captureSource.AddEffect(sourceStreamIndex, activate);
        }

        /// <summary>
        /// Adds an effect to a capture streamIndex.
        /// </summary>
        /// <param name="captureSource">A valid IMFCaptureSource instance.</param>
        /// <param name="sourceStream">A member of the <see cref="CaptureEngineStreams"/> enumeration.</param>
        /// <param name="activate">An MFT activation instance.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult AddEffect(this IMFCaptureSource captureSource, CaptureEngineStreams sourceStream, IMFActivate activate)
        {
            if (captureSource == null)
                throw new ArgumentNullException("captureSource");

            return captureSource.AddEffect((int)sourceStream, activate);
        }

        /// <summary>
        /// Gets a format that is supported by one of the capture streams.
        /// </summary>
        /// <param name="captureSource">A valid IMFCaptureSource instance.</param>
        /// <param name="sourceStream">A member of the <see cref="CaptureEngineStreams"/> enumeration.</param>
        /// <param name="mediaTypeIndex">The zero-based index of the media type to retrieve.</param>
        /// <param name="mediaType">Receives an instance of the IMFMediaType interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetAvailableDeviceMediaType(this IMFCaptureSource captureSource, CaptureEngineStreams sourceStream, int mediaTypeIndex, out IMFMediaType mediaType)
        {
            if (captureSource == null)
                throw new ArgumentNullException("captureSource");

            return captureSource.GetAvailableDeviceMediaType((int)sourceStream, mediaTypeIndex, out mediaType);
        }

        /// <summary>
        /// Gets the current media type for a capture streamIndex.
        /// </summary>
        /// <param name="captureSource">A valid IMFCaptureSource instance.</param>
        /// <param name="sourceStream">A member of the <see cref="CaptureEngineStreams"/> enumeration.</param>
        /// <param name="mediaType">Receives an instance of the IMFMediaType interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetCurrentDeviceMediaType(this IMFCaptureSource captureSource, CaptureEngineStreams sourceStream, out IMFMediaType mediaType)
        {
            if (captureSource == null)
                throw new ArgumentNullException("captureSource");

            return captureSource.GetCurrentDeviceMediaType((int)sourceStream, out mediaType);
        }

        /// <summary>
        /// Gets a pointer to the underlying Source Reader object.
        /// </summary>
        /// <param name="captureSource">A valid IMFCaptureSource instance.</param>
        /// <param name="sourceReader">Receives an instance of the underlying Source Reader.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetService(this IMFCaptureSource captureSource, out IMFSourceReader sourceReader)
        {
            if (captureSource == null)
                throw new ArgumentNullException("captureSource");

            object tmp;

            HResult hr = captureSource.GetService(Guid.Empty, typeof(IMFSourceReader).GUID, out tmp);
            sourceReader = hr.Succeeded() ? tmp as IMFSourceReader : null;

            return hr;
        }

        /// <summary>
        /// Gets the actual device streamIndex index translated from a friendly streamIndex name.
        /// </summary>
        /// <param name="captureSource">A valid IMFCaptureSource instance.</param>
        /// <param name="friendlyName">A member of the <see cref="CaptureEngineFriendlyName"/> enumeration.</param>
        /// <param name="actualStreamIndex">Receives the value of the streamIndex index that corresponds to the friendly name.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetStreamIndexFromFriendlyName(this IMFCaptureSource captureSource, CaptureEngineFriendlyName friendlyName, out int actualStreamIndex)
        {
            if (captureSource == null)
                throw new ArgumentNullException("captureSource");

            return captureSource.GetStreamIndexFromFriendlyName((int)friendlyName, out actualStreamIndex);
        }

        /// <summary>
        /// Removes all effects from a capture streamIndex.
        /// </summary>
        /// <param name="captureSource">A valid IMFCaptureSource instance.</param>
        /// <param name="sourceStream">A member of the <see cref="CaptureEngineStreams"/> enumeration.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult RemoveAllEffects(this IMFCaptureSource captureSource, CaptureEngineStreams sourceStream)
        {
            if (captureSource == null)
                throw new ArgumentNullException("captureSource");

            return captureSource.RemoveAllEffects((int)sourceStream);
        }

        /// <summary>
        /// Removes an effect from a capture streamIndex.
        /// </summary>
        /// <param name="captureSource">A valid IMFCaptureSource instance.</param>
        /// <param name="sourceStreamIndex">The capture streamIndex.</param>
        /// <param name="transform">A Media Foundation transform instance.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult RemoveEffect(this IMFCaptureSource captureSource, int sourceStreamIndex, IMFTransform transform)
        {
            if (captureSource == null)
                throw new ArgumentNullException("captureSource");

            return captureSource.RemoveEffect(sourceStreamIndex, transform);
        }

        /// <summary>
        /// Removes an effect from a capture streamIndex.
        /// </summary>
        /// <param name="captureSource">A valid IMFCaptureSource instance.</param>
        /// <param name="sourceStream">A member of the <see cref="CaptureEngineStreams"/> enumeration.</param>
        /// <param name="transform">A Media Foundation transform instance.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult RemoveEffect(this IMFCaptureSource captureSource, CaptureEngineStreams sourceStream, IMFTransform transform)
        {
            if (captureSource == null)
                throw new ArgumentNullException("captureSource");

            return captureSource.RemoveEffect((int)sourceStream, transform);
        }

        /// <summary>
        /// Removes an effect from a capture streamIndex.
        /// </summary>
        /// <param name="captureSource">A valid IMFCaptureSource instance.</param>
        /// <param name="sourceStreamIndex">The capture streamIndex.</param>
        /// <param name="activate">An MFT activation instance.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult RemoveEffect(this IMFCaptureSource captureSource, int sourceStreamIndex, IMFActivate activate)
        {
            if (captureSource == null)
                throw new ArgumentNullException("captureSource");

            return captureSource.RemoveEffect(sourceStreamIndex, activate);
        }

        /// <summary>
        /// Removes an effect from a capture streamIndex.
        /// </summary>
        /// <param name="captureSource">A valid IMFCaptureSource instance.</param>
        /// <param name="sourceStream">A member of the <see cref="CaptureEngineStreams"/> enumeration.</param>
        /// <param name="activate">An MFT activation instance.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult RemoveEffect(this IMFCaptureSource captureSource, CaptureEngineStreams sourceStream, IMFActivate activate)
        {
            if (captureSource == null)
                throw new ArgumentNullException("captureSource");

            return captureSource.RemoveEffect((int)sourceStream, activate);
        }

        /// <summary>
        /// Sets the output format for a capture streamIndex.
        /// </summary>
        /// <param name="captureSource">A valid IMFCaptureSource instance.</param>
        /// <param name="sourceStream">A member of the <see cref="CaptureEngineStreams"/> enumeration.</param>
        /// <param name="mediaType">An instance of the IMFMediaType interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult SetCurrentDeviceMediaType(this IMFCaptureSource captureSource, CaptureEngineStreams sourceStream, IMFMediaType mediaType)
        {
            if (captureSource == null)
                throw new ArgumentNullException("captureSource");

            return captureSource.SetCurrentDeviceMediaType((int)sourceStream, mediaType);
        }
    }
}
