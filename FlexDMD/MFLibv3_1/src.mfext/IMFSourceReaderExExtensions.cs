/* license

IMFSourceReaderExExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;

using MediaFoundation.Misc;
using MediaFoundation.ReadWrite;
using MediaFoundation.Transform;

namespace MediaFoundation
{
    public static class IMFSourceReaderExExtensions
    {
        /// <summary>
        /// Adds a transform, such as an audio or video effect, to a stream.
        /// </summary>
        /// <param name="sourceReader">A valid IMFSourceReaderEx instance.</param></param>
        /// <param name="streamIndex">The stream to configure.</param>
        /// <param name="transform">An instance of a Media Foundation transform (MFT).</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult AddTransformForStream(this IMFSourceReaderEx sourceReader, int streamIndex, IMFTransform transform)
        {
            if (sourceReader == null)
                throw new ArgumentNullException("sourceReader");

            return sourceReader.AddTransformForStream(streamIndex, transform);
        }

        /// <summary>
        /// Adds a transform, such as an audio or video effect, to a stream.
        /// </summary>
        /// <param name="sourceReader">A valid IMFSourceReaderEx instance.</param></param>
        /// <param name="streamIndex">The stream to configure.</param>
        /// <param name="transform">An instance of a Media Foundation transform (MFT).</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult AddTransformForStream(this IMFSourceReaderEx sourceReader, SourceReaderFirstStream streamIndex, IMFTransform transform)
        {
            if (sourceReader == null)
                throw new ArgumentNullException("sourceReader");

            return sourceReader.AddTransformForStream((int)streamIndex, transform);
        }

        /// <summary>
        /// Adds a transform, such as an audio or video effect, to a stream.
        /// </summary>
        /// <param name="sourceReader">A valid IMFSourceReaderEx instance.</param></param>
        /// <param name="streamIndex">The stream to configure.</param>
        /// <param name="activate">An instance of an MFT activation object.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult AddTransformForStream(this IMFSourceReaderEx sourceReader, int streamIndex, IMFActivate activate)
        {
            if (sourceReader == null)
                throw new ArgumentNullException("sourceReader");

            return sourceReader.AddTransformForStream(streamIndex, activate);
        }

        /// <summary>
        /// Adds a transform, such as an audio or video effect, to a stream.
        /// </summary>
        /// <param name="sourceReader">A valid IMFSourceReaderEx instance.</param></param>
        /// <param name="streamIndex">The stream to configure.</param>
        /// <param name="activate">An instance of an MFT activation object.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult AddTransformForStream(this IMFSourceReaderEx sourceReader, SourceReaderFirstStream streamIndex, IMFActivate activate)
        {
            if (sourceReader == null)
                throw new ArgumentNullException("sourceReader");

            return sourceReader.AddTransformForStream((int)streamIndex, activate);
        }

        /// <summary>
        /// Gets an instance of a Media Foundation transform (MFT) for a specified stream.
        /// </summary>
        /// <param name="sourceReader">A valid IMFSourceReaderEx instance.</param></param>
        /// <param name="streamIndex">The stream to query for the MFT. </param>
        /// <param name="transformIndex">The zero-based index of the MFT to retreive.</param>
        /// <param name="guidCategory">Receives a GUID that specifies the category of the MFT.</param>
        /// <param name="transform">Receives an instance of the IMFTransform interface for the MFT.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetTransformForStream(this IMFSourceReaderEx sourceReader, SourceReaderFirstStream streamIndex, int transformIndex, out Guid guidCategory, out IMFTransform transform)
        {
            if (sourceReader == null)
                throw new ArgumentNullException("sourceReader");

            return sourceReader.GetTransformForStream((int)streamIndex, transformIndex, out guidCategory, out transform);
        }

        /// <summary>
        /// Removes all of the Media Foundation transforms (MFTs) for a specified stream, with the exception of the decoder.
        /// </summary>
        /// <param name="sourceReader">A valid IMFSourceReaderEx instance.</param></param>
        /// <param name="streamIndex">The stream for which to remove the MFTs.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult RemoveAllTransformsForStream(this IMFSourceReaderEx sourceReader, SourceReaderFirstStream streamIndex)
        {
            if (sourceReader == null)
                throw new ArgumentNullException("sourceReader");

            return sourceReader.RemoveAllTransformsForStream((int)streamIndex);
        }

        /// <summary>
        /// Sets the native media type for a stream on the media source.
        /// </summary>
        /// <param name="sourceReader">A valid IMFSourceReaderEx instance.</param></param>
        /// <param name="streamIndex">The stream to set.</param>
        /// <param name="mediaType">An instance of the IMFMediaType interface for the media type.</param>
        /// <param name="streamFlags">Receives one or more members of the MF_SOURCE_READER_FLAG enumeration.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult SetNativeMediaType(this IMFSourceReaderEx sourceReader, SourceReaderFirstStream streamIndex, IMFMediaType mediaType, out MF_SOURCE_READER_FLAG streamFlags)
        {
            if (sourceReader == null)
                throw new ArgumentNullException("sourceReader");

            return sourceReader.SetNativeMediaType((int)streamIndex, mediaType, out streamFlags);
        }
    }
}
