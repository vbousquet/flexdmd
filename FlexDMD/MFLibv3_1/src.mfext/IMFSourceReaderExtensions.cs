/* license

IMFSourceReaderExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;

using MediaFoundation.Misc;
using MediaFoundation.ReadWrite;

namespace MediaFoundation
{
    public static class IMFSourceReaderExtensions
    {
        /// <summary>
        /// Flushes one or more streams.
        /// </summary>
        /// <param name="sourceReader">A valid IMFSourceReader instance.</param></param>
        /// <param name="streamIndex">The streamIndex(s) to flush.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult Flush(this IMFSourceReader sourceReader, SourceReaderStreams stream)
        {
            if (sourceReader == null)
                throw new ArgumentNullException("sourceReader");

            return sourceReader.Flush((int)stream);
        }

        /// <summary>
        /// Gets the current media type for a streamIndex.
        /// </summary>
        /// <param name="sourceReader">A valid IMFSourceReader instance.</param></param>
        /// <param name="streamIndex">The streamIndex to query.</param>
        /// <param name="mediaType">Receives an instance of the IMFMediaType interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetCurrentMediaType(this IMFSourceReader sourceReader, SourceReaderFirstStream streamIndex, out IMFMediaType mediaType)
        {
            if (sourceReader == null)
                throw new ArgumentNullException("sourceReader");

            return sourceReader.GetCurrentMediaType((int)streamIndex, out mediaType);
        }

        /// <summary>
        /// Gets a format that is supported natively by the media source.
        /// </summary>
        /// <param name="sourceReader">A valid IMFSourceReader instance.</param></param>
        /// <param name="streamIndex">The streamIndex to query.</param>
        /// <param name="mediaTypeIndex">The zero-based index of the media type to retrieve.</param>
        /// <param name="mediaType">Receives an instance of the IMFMediaType interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetNativeMediaType(this IMFSourceReader sourceReader, SourceReaderFirstStream streamIndex, int mediaTypeIndex, out IMFMediaType mediaType)
        {
            if (sourceReader == null)
                throw new ArgumentNullException("sourceReader");

            return sourceReader.GetNativeMediaType((int)streamIndex, mediaTypeIndex, out mediaType);
        }

        /// <summary>
        /// Gets an attribute from the underlying media source.
        /// </summary>
        /// <param name="sourceReader">A valid IMFSourceReader instance.</param></param>
        /// <param name="streamIndex">The streamIndex to query.</param>
        /// <param name="mediaTypeIndex">The zero-based index of the media type to retrieve.</param>
        /// <param name="mediaType">Receives an instance of the IMFMediaType interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetPresentationAttribute(this IMFSourceReader sourceReader, SourceReaderFirstStreamOrMediaSource streamIndex, Guid guidAttribute, PropVariant attribute)
        {
            if (sourceReader == null)
                throw new ArgumentNullException("sourceReader");

            return sourceReader.GetPresentationAttribute((int)streamIndex, guidAttribute, attribute);
        }

        /// <summary>
        /// Queries the underlying media source or decoder for an interface.
        /// </summary>
        /// <typeparam name="T">The COM interface being requested.</typeparam></typeparam>
        /// <param name="sourceReader">A valid IMFSourceReader instance.</param></param></param>
        /// <param name="streamIndex">The streamIndex or object to query.</param>
        /// <param name="guidService">A service identifier GUID.</param>
        /// <param name="service">Receives an instance of the requested interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetServiceForStream<T>(this IMFSourceReader sourceReader, int streamIndex, Guid guidService, out T service) where T : class
        {
            if (sourceReader == null)
                throw new ArgumentNullException("sourceReader");

            Type typeOfT = typeof(T);

            if (!typeOfT.IsInterface)
                throw new ArgumentOutOfRangeException("T", "T must be a COM visible interface.");

            object tmp;

            HResult hr = sourceReader.GetServiceForStream(streamIndex, guidService, typeOfT.GUID, out tmp);
            service = hr.Succeeded() ? tmp as T : null;

            return hr;
        }

        /// <summary>
        /// Queries the underlying media source or decoder for an interface.
        /// </summary>
        /// <typeparam name="T">The COM interface being requested.</typeparam></typeparam>
        /// <param name="sourceReader">A valid IMFSourceReader instance.</param></param></param>
        /// <param name="streamIndex">The streamIndex or object to query.</param>
        /// <param name="guidService">A service identifier GUID.</param>
        /// <param name="service">Receives an instance of the requested interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetServiceForStream<T>(this IMFSourceReader sourceReader, SourceReaderFirstStreamOrMediaSource streamIndex, Guid guidService, out T service) where T : class
        {
            if (sourceReader == null)
                throw new ArgumentNullException("sourceReader");

            Type typeOfT = typeof(T);

            if (!typeOfT.IsInterface)
                throw new ArgumentOutOfRangeException("T", "T must be a COM visible interface.");

            object tmp;

            HResult hr = sourceReader.GetServiceForStream((int)streamIndex, guidService, typeOfT.GUID, out tmp);
            service = hr.Succeeded() ? tmp as T : null;

            return hr;
        }

        /// <summary>
        /// Queries whether a stream is selected.
        /// </summary>
        /// <param name="sourceReader">A valid IMFSourceReader instance.</param></param></param>
        /// <param name="streamIndex">The stream to query.</param>
        /// <param name="mediaType">Receives True if the stream is selected and will generate data otherwise False if the stream is not selected and will not generate data.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetStreamSelection(this IMFSourceReader sourceReader, SourceReaderFirstStream streamIndex, out bool selected)
        {
            if (sourceReader == null)
                throw new ArgumentNullException("sourceReader");

            return sourceReader.GetStreamSelection((int)streamIndex, out selected);
        }

        /// <summary>
        /// Reads the next sample from the media source.
        /// </summary>
        /// <param name="sourceReader">A valid IMFSourceReader instance.</param></param></param>
        /// <param name="streamIndex">The stream to pull data from.</param>
        /// <param name="controlFlags">One or more members of the MF_SOURCE_READER_CONTROL_FLAG enumeration.</param>
        /// <param name="actualStreamIndex">Receives the zero-based index of the stream.</param>
        /// <param name="streamFlags">Receives one or more members of the MF_SOURCE_READER_FLAG enumeration.</param>
        /// <param name="timestamp">Receives the time stamp of the sample, or the time of the stream event indicated in <paramref name="streamFlags"/>.</param>
        /// <param name="sample">Receives an instance of the IMFSample interface or the value null.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult ReadSample(this IMFSourceReader sourceReader, int streamIndex, MF_SOURCE_READER_CONTROL_FLAG controlFlags, out int actualStreamIndex, out  MF_SOURCE_READER_FLAG streamFlags, out TimeSpan timestamp, out IMFSample sample)
        {
            if (sourceReader == null)
                throw new ArgumentNullException("sourceReader");

            long tmp = 0;

            HResult hr = sourceReader.ReadSample(streamIndex, controlFlags, out actualStreamIndex, out streamFlags, out tmp, out sample);
            timestamp = hr.Succeeded() ? TimeSpan.FromTicks(tmp) : default(TimeSpan);

            return hr;
        }

        /// <summary>
        /// Reads the next sample from the media source.
        /// </summary>
        /// <param name="sourceReader">A valid IMFSourceReader instance.</param></param></param>
        /// <param name="streamIndex">The stream to pull data from.</param>
        /// <param name="controlFlags">One or more members of the MF_SOURCE_READER_CONTROL_FLAG enumeration.</param>
        /// <param name="actualStreamIndex">Receives the zero-based index of the stream.</param>
        /// <param name="streamFlags">Receives one or more members of the MF_SOURCE_READER_FLAG enumeration.</param>
        /// <param name="timestamp">Receives the time stamp of the sample, or the time of the stream event indicated in <paramref name="streamFlags"/>.</param>
        /// <param name="sample">Receives an instance of the IMFSample interface or the value null.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult ReadSample(this IMFSourceReader sourceReader, SourceReaderStreams streamIndex, MF_SOURCE_READER_CONTROL_FLAG controlFlags, out int actualStreamIndex, out  MF_SOURCE_READER_FLAG streamFlags, out TimeSpan timestamp, out IMFSample sample)
        {
            if (sourceReader == null)
                throw new ArgumentNullException("sourceReader");

            long tmp = 0;

            HResult hr = sourceReader.ReadSample((int)streamIndex, controlFlags, out actualStreamIndex, out streamFlags, out tmp, out sample);
            timestamp = hr.Succeeded() ? TimeSpan.FromTicks(tmp) : default(TimeSpan);

            return hr;
        }

        /// <summary>
        /// Sets the media type for a stream.
        /// </summary>
        /// <param name="sourceReader">A valid IMFSourceReader instance.</param></param></param>
        /// <param name="streamIndex">The stream to configure.</param>
        /// <param name="mediaType">An instance of the IMFMediaType interface of the media type.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult SetCurrentMediaType(this IMFSourceReader sourceReader, int streamIndex, IMFMediaType mediaType)
        {
            if (sourceReader == null)
                throw new ArgumentNullException("sourceReader");

            return sourceReader.SetCurrentMediaType(streamIndex, null, mediaType);
        }

        /// <summary>
        /// Sets the media type for a stream.
        /// </summary>
        /// <param name="sourceReader">A valid IMFSourceReader instance.</param></param></param>
        /// <param name="streamIndex">The stream to configure.</param>
        /// <param name="mediaType">An instance of the IMFMediaType interface of the media type.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult SetCurrentMediaType(this IMFSourceReader sourceReader, SourceReaderFirstStream streamIndex, IMFMediaType mediaType)
        {
            if (sourceReader == null)
                throw new ArgumentNullException("sourceReader");

            return sourceReader.SetCurrentMediaType((int)streamIndex, null, mediaType);
        }

        /// <summary>
        /// Seeks to a new position in the media source.
        /// </summary>
        /// <param name="sourceReader">A valid IMFSourceReader instance.</param></param></param>
        /// <param name="position">The position from which playback will be started.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult SetCurrentPosition(this IMFSourceReader sourceReader, TimeSpan position)
        {
            if (sourceReader == null)
                throw new ArgumentNullException("sourceReader");

            using(PropVariant value = new PropVariant(position.Ticks))
            {
                return sourceReader.SetCurrentPosition(Guid.Empty, value);
            }
        }

        /// <summary>
        /// Selects or deselects one or more streams.
        /// </summary>
        /// <param name="sourceReader">A valid IMFSourceReader instance.</param></param></param>
        /// <param name="streamIndex">The stream to set.</param>
        /// <param name="selected">Specify True to select streams or False to deselect streams. If a stream is deselected, it will not generate data.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult SetStreamSelection(this IMFSourceReader sourceReader, SourceReaderStreams streamIndex, bool selected)
        {
            if (sourceReader == null)
                throw new ArgumentNullException("sourceReader");

            return sourceReader.SetStreamSelection((int)streamIndex, selected);
        }
    }
}
