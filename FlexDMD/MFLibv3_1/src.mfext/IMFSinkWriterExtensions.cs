/* license

IMFSinkWriterExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;

using MediaFoundation.ReadWrite;

namespace MediaFoundation
{
    public static class IMFSinkWriterExtensions
    {
        /// <summary>
        /// Queries the underlying media sink or encoder for an interface.
        /// </summary>
        /// <typeparam name="T">The COM interface being requested.</typeparam>
        /// <param name="sinkWriter">A valid IMFSinkWriter instance.</param>
        /// <param name="streamIndex">The zero-based index of a streamIndex to query or -1 to query the media sink itself.</param>
        /// <param name="guidService">A service identifier GUID.</param>
        /// <param name="service">Receives an instance of the requested interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetServiceForStream<T>(this IMFSinkWriter sinkWriter, int streamIndex, Guid guidService, out T service) where T : class
        {
            if (sinkWriter == null)
                throw new ArgumentNullException("sinkWriter");

            Type typeOfT = typeof(T);

            if (!typeOfT.IsInterface)
                throw new ArgumentOutOfRangeException("T", "T must be a COM visible interface.");

            object tmp;

            HResult hr = sinkWriter.GetServiceForStream(streamIndex, guidService, typeOfT.GUID, out tmp);
            service = hr.Succeeded() ? tmp as T : null;

            return hr;
        }

        /// <summary>
        /// Indicates a gap in an input streamIndex.
        /// </summary>
        /// <param name="sinkWriter">A valid IMFSinkWriter instance.</param>
        /// <param name="streamIndex">The zero-based index of the streamIndex.</param>
        /// <param name="timestamp">The position in the streamIndex where the gap in the data occurs, relative to the start of the streamIndex.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult SendStreamTick(this IMFSinkWriter sinkWriter, int streamIndex, TimeSpan timestamp)
        {
            if (sinkWriter == null)
                throw new ArgumentNullException("sinkWriter");

            return sinkWriter.SendStreamTick(streamIndex, timestamp.Ticks);
        }
    }
}
