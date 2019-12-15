/* license

IMFCaptureSinkExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;
using System.Runtime.InteropServices;

using MediaFoundation.ReadWrite;

namespace MediaFoundation
{
    public static class IMFCaptureSinkExtensions
    {
        /// <summary>
        /// Connects a streamIndex from the capture source to this capture sink.
        /// </summary>
        /// <param name="captureSink">A valid IMFCaptureSink instance.</param>
        /// <param name="streamIndex">A value from the <see cref="CaptureEngineStreams"/> enumeration.</param>
        /// <param name="mediaType">An IMFMediaType instance that specifies the desired format of the output streamIndex.</param>
        /// <param name="attributes">An IMFAttributes instance or null.</param>
        /// <param name="sinkStreamIndex">Receives the index of the new streamIndex on the capture sink.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult AddStream(this IMFCaptureSink captureSink, CaptureEngineStreams streamIndex, IMFMediaType  mediaType, IMFAttributes attributes, out int sinkStreamIndex)
        {
            if (captureSink == null)
                throw new ArgumentNullException("captureSink");

            return captureSink.AddStream((int)streamIndex, mediaType, attributes, out sinkStreamIndex);
        }

        /// <summary>
        /// Queries the underlying Sink Writer object.
        /// </summary>
        /// <param name="captureSink">A valid IMFCaptureSink instance.</param>
        /// <param name="sinkStreamIndex">The zero-based index of the streamIndex to query. The index is returned in the sinkStreamIndex parameter of the <see cref="IMFCaptureSink.AddStream"/> method.</param>
        /// <param name="sourceReader">Receives an instance of the underlying Sink Writer.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetService(this IMFCaptureSink captureSink, int sinkStreamIndex, out IMFSinkWriter sinkWriter)
        {
            if (captureSink == null)
                throw new ArgumentNullException("captureSink");

            object tmp;

            HResult hr = captureSink.GetService(sinkStreamIndex, Guid.Empty, typeof(IMFSinkWriterExtensions).GUID, out tmp);
            sinkWriter = hr.Succeeded() ? tmp as IMFSinkWriter : null;

            return hr;
        }
    }
}
