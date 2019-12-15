/* license

IMFReadWriteClassFactoryExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

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
    public static class IMFReadWriteClassFactoryExtensions
    {
        /// <summary>
        /// Creates an instance of the sink writer given a URL.
        /// </summary>
        /// <param name="factory">A valid IMFReadWriteClassFactory instance.</param>
        /// <param name="url">A string that specifies the name of the output file. The sink writer creates a new file with this name.</param>
        /// <param name="attributes">A instance to the IMFAttributes interface. You can use this parameter to configure the sink writer.</param>
        /// <param name="sourceReader">Receives the sink writer.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateInstanceFromURL(this IMFReadWriteClassFactory factory, string url, IMFAttributes attributes, out IMFSinkWriter sinkWriter)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            object tmp;

            HResult hr = factory.CreateInstanceFromURL(CLSID.CLSID_MFSinkWriter, url, attributes, typeof(IMFSinkWriterExtensions).GUID, out tmp);
            sinkWriter = hr.Succeeded() ? tmp as IMFSinkWriter : null;

            return hr;
        }

        /// <summary>
        /// Creates an instance of the source reader given a URL.
        /// </summary>
        /// <param name="factory">A valid IMFReadWriteClassFactory instance.</param>
        /// <param name="url">A string that specifies the input file for the source reader.</param>
        /// <param name="attributes">A instance to the IMFAttributes interface. You can use this parameter to configure the source reader.</param></param>
        /// <param name="sourceReader">Receives the source reader</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateInstanceFromURL(this IMFReadWriteClassFactory factory, string url, IMFAttributes attributes, out IMFSourceReader sourceReader)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            object tmp;

            HResult hr = factory.CreateInstanceFromURL(CLSID.CLSID_MFSourceReader, url, attributes, typeof(IMFSourceReader).GUID, out tmp);
            sourceReader = hr.Succeeded() ? tmp as IMFSourceReader : null;

            return hr;
        }

        /// <summary>
        /// Creates an instance of the sink writer, given an IMFByteStream instance.
        /// </summary>
        /// <param name="factory">A valid IMFReadWriteClassFactory instance.</param>
        /// <param name="mediaSink">A byte streamIndex used by the sink writer to write data.</param>
        /// <param name="attributes">A instance to the IMFAttributes interface. You can use this parameter to configure the sink writer.</param>
        /// <param name="sourceReader">Receives the sink writer.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateInstanceFromObject(this IMFReadWriteClassFactory factory, IMFByteStream byteStream, IMFAttributes attributes, out IMFSinkWriter sinkWriter)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            object tmp;

            HResult hr = factory.CreateInstanceFromObject(CLSID.CLSID_MFSinkWriter, byteStream, attributes, typeof(IMFSinkWriterExtensions).GUID, out tmp);
            sinkWriter = hr.Succeeded() ? tmp as IMFSinkWriter : null;

            return hr;
        }

        /// <summary>
        /// Creates an instance of the sink writer, given an IMFMediaSink instance.
        /// </summary>
        /// <param name="factory">A valid IMFReadWriteClassFactory instance.</param>
        /// <param name="mediaSink">An instance of IMFMediaSink used by the sink writer.</param>
        /// <param name="attributes">A instance to the IMFAttributes interface. You can use this parameter to configure the sink writer.</param>
        /// <param name="sourceReader">Receives the sink writer.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateInstanceFromObject(this IMFReadWriteClassFactory factory, IMFMediaSink mediaSink, IMFAttributes attributes, out IMFSinkWriter sinkWriter)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            object tmp;

            HResult hr = factory.CreateInstanceFromObject(CLSID.CLSID_MFSinkWriter, mediaSink, attributes, typeof(IMFSinkWriterExtensions).GUID, out tmp);
            sinkWriter = hr.Succeeded() ? tmp as IMFSinkWriter : null;

            return hr;
        }

        /// <summary>
        /// Creates an instance of the source reader, given an IMFByteStream instance.
        /// </summary>
        /// <param name="factory">A valid IMFReadWriteClassFactory instance.</param>
        /// <param name="mediaSink">A byte streamIndex used by the source reader to read data.</param>
        /// <param name="attributes">A instance to the IMFAttributes interface. You can use this parameter to configure the source reader.</param>
        /// <param name="sourceReader">Receives the source reader.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateInstanceFromObject(this IMFReadWriteClassFactory factory, IMFByteStream byteStream, IMFAttributes attributes, out IMFSourceReader sourceReader)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            object tmp;

            HResult hr = factory.CreateInstanceFromObject(CLSID.CLSID_MFSinkWriter, byteStream, attributes, typeof(IMFSinkWriterExtensions).GUID, out tmp);
            sourceReader = hr.Succeeded() ? tmp as IMFSourceReader : null;

            return hr;
        }

        /// <summary>
        /// Creates an instance of the source reader, given an IMFMediaSource instance.
        /// </summary>
        /// <param name="factory">A valid IMFReadWriteClassFactory instance.</param>
        /// <param name="mediaSink">An instance of IMFMediaSource used by the source reader.</param>
        /// <param name="attributes">A instance to the IMFAttributes interface. You can use this parameter to configure the source reader.</param>
        /// <param name="sourceReader">Receives the source reader.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateInstanceFromObject(this IMFReadWriteClassFactory factory, IMFMediaSource mediaSource, IMFAttributes attributes, out IMFSourceReader sourceReader)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            object tmp;

            HResult hr = factory.CreateInstanceFromObject(CLSID.CLSID_MFSinkWriter, mediaSource, attributes, typeof(IMFSinkWriterExtensions).GUID, out tmp);
            sourceReader = hr.Succeeded() ? tmp as IMFSourceReader : null;

            return hr;
        }
    }
}
