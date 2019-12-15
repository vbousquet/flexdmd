/* license

IMFSourceResolverExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;
using System.Diagnostics;

using MediaFoundation.Misc;

namespace MediaFoundation
{
    public static class IMFSourceResolverExtensions
    {
        /// <summary>
        /// Creates a media source from a URL.
        /// </summary>
        /// <param name="sourceResolver">A valid IMFSourceResolver instance.</param>
        /// <param name="url">A string that contains the URL to resolve.</param>
        /// <param name="flags">One or more members of the MFResolution enumeration.</param>
        /// <param name="properties">An instance of the IPropertyStore interface of a property store. The method passes the property store to the scheme handler or byte-stream handler that creates the object.</param>
        /// <param name="mediaSource">Receives a media source that can handle the media file targeted by <paramref name="url"/>.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateObjectFromURL(this IMFSourceResolver sourceResolver, string url, MFResolution flags, IPropertyStore properties, out IMFMediaSource mediaSource)
        {
            if (sourceResolver == null)
                throw new ArgumentNullException("sourceResolver");

            flags &= ~MFResolution.ByteStream;
            flags |= MFResolution.MediaSource;

            MFObjectType objectType;
            object tmp;

            HResult hr = sourceResolver.CreateObjectFromURL(url, flags, properties, out objectType, out tmp);
            mediaSource = hr.Succeeded() ? tmp as IMFMediaSource : null;

            return hr;
        }

        /// <summary>
        /// Creates a media source from a Uri.
        /// </summary>
        /// <param name="sourceResolver">A valid IMFSourceResolver instance.</param>
        /// <param name="url">A Uri that contains the URL to resolve.</param>
        /// <param name="flags">One or more members of the MFResolution enumeration.</param>
        /// <param name="properties">An instance of the IPropertyStore interface of a property store. The method passes the property store to the scheme handler or byte-stream handler that creates the object.</param>
        /// <param name="mediaSource">Receives a media source that can handle the media file targeted by <paramref name="url"/>.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateObjectFromURL(this IMFSourceResolver sourceResolver, Uri url, MFResolution flags, IPropertyStore properties, out IMFMediaSource mediaSource)
        {
            return CreateObjectFromURL(sourceResolver, url.ToString(), flags, properties, out mediaSource);
        }

        /// <summary>
        /// Creates a byte stream from a URL.
        /// </summary>
        /// <param name="sourceResolver">A valid IMFSourceResolver instance.</param>
        /// <param name="url">A string that contains the URL to resolve.</param>
        /// <param name="flags">One or more members of the MFResolution enumeration.</param>
        /// <param name="properties">An instance of the IPropertyStore interface of a property store. The method passes the property store to the scheme handler or byte-stream handler that creates the object.</param>
        /// <param name="byteStream">Receives a byte stream that can handle the media file targeted by <paramref name="url"/>.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateObjectFromURL(this IMFSourceResolver sourceResolver, string url, MFResolution flags, IPropertyStore properties, out IMFByteStream byteStream)
        {
            if (sourceResolver == null)
                throw new ArgumentNullException("sourceResolver");

            flags &= ~MFResolution.MediaSource;
            flags |= MFResolution.ByteStream;

            MFObjectType objectType;
            object tmp;

            HResult hr = sourceResolver.CreateObjectFromURL(url, flags, properties, out objectType, out tmp);
            byteStream = hr.Succeeded() ? tmp as IMFByteStream : null;

            return hr;
        }

        /// <summary>
        /// Creates a byte stream from a URL.
        /// </summary>
        /// <param name="sourceResolver">A valid IMFSourceResolver instance.</param>
        /// <param name="url">A Uri that contains the URL to resolve.</param>
        /// <param name="flags">One or more members of the MFResolution enumeration.</param>
        /// <param name="properties">An instance of the IPropertyStore interface of a property store. The method passes the property store to the scheme handler or byte-stream handler that creates the object.</param>
        /// <param name="byteStream">Receives a byte stream that can handle the media file targeted by <paramref name="url"/>.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateObjectFromURL(this IMFSourceResolver sourceResolver, Uri url, MFResolution flags, IPropertyStore properties, out IMFByteStream byteStream)
        {
            return CreateObjectFromURL(sourceResolver, url.ToString(), flags, properties, out byteStream);
        }

        /// <summary>
        /// Creates a media source from a byte stream.
        /// </summary>
        /// <param name="sourceResolver">A valid IMFSourceResolver instance.</param>
        /// <param name="byteStream">An instance of the byte stream's IMFByteStream interface.</param>
        /// <param name="url">A string that contains the URL of the byte stream. The URL is optional and can be null.</param>
        /// <param name="flags">One or more members of the MFResolution enumeration.</param>
        /// <param name="properties">An instance of the IPropertyStore interface of a property store. The method passes the property store to the scheme handler or byte-stream handler that creates the object.</param>
        /// <param name="mediaSource">Receives a media source that can handle the provided byte stream.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateObjectFromByteStream(this IMFSourceResolver sourceResolver, IMFByteStream byteStream, string url, MFResolution flags, IPropertyStore properties, out IMFMediaSource mediaSource)
        {
            if (sourceResolver == null)
                throw new ArgumentNullException("sourceResolver");

            flags &= ~MFResolution.ByteStream;
            flags |= MFResolution.MediaSource;

            MFObjectType objectType;
            object tmp;

            HResult hr = sourceResolver.CreateObjectFromByteStream(byteStream, url, flags, properties, out objectType, out tmp);
            mediaSource = hr.Succeeded() ? tmp as IMFMediaSource : null;

            return hr;
        }

        /// <summary>
        /// Creates a media source from a byte stream.
        /// </summary>
        /// <param name="sourceResolver">A valid IMFSourceResolver instance.</param>
        /// <param name="byteStream">An instance of the byte stream's IMFByteStream interface.</param>
        /// <param name="url">A Uri that contains the URL of the byte stream. The URL is optional and can be null.</param>
        /// <param name="flags">One or more members of the MFResolution enumeration.</param>
        /// <param name="properties">An instance of the IPropertyStore interface of a property store. The method passes the property store to the scheme handler or byte-stream handler that creates the object.</param>
        /// <param name="mediaSource">Receives a media source that can handle the provided byte stream.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateObjectFromByteStream(this IMFSourceResolver sourceResolver, IMFByteStream byteStream, Uri url, MFResolution flags, IPropertyStore properties, out IMFMediaSource mediaSource)
        {
            return CreateObjectFromByteStream(sourceResolver, byteStream, url.ToString(), flags, properties, out mediaSource);
        }

        /// <summary>
        /// Creates a media source from a byte stream.
        /// </summary>
        /// <param name="sourceResolver">A valid IMFSourceResolver instance.</param>
        /// <param name="byteStream">An instance of the byte stream's IMFByteStream interface.</param>
        /// <param name="flags">One or more members of the MFResolution enumeration.</param>
        /// <param name="properties">An instance of the IPropertyStore interface of a property store. The method passes the property store to the scheme handler or byte-stream handler that creates the object.</param>
        /// <param name="mediaSource">Receives a media source that can handle the provided byte stream.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateObjectFromByteStream(this IMFSourceResolver sourceResolver, IMFByteStream byteStream, MFResolution flags, IPropertyStore properties, out IMFMediaSource mediaSource)
        {
            return CreateObjectFromByteStream(sourceResolver, byteStream, (string)null, flags, properties, out mediaSource);
        }
    }
}
