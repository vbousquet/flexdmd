/* license

IMFMediaSourceExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;

using MediaFoundation.Misc;

namespace MediaFoundation
{
    public static class IMFMediaSourceExtensions
    {
        /// <summary>
        /// Starts, seeks, or restarts the media source by specifying where to start playback.
        /// </summary>
        /// <param name="mediaSource">A valid IMFMediaSource instance.</param>
        /// <param name="presentationDescriptor">An IMFPresentationDescriptor instance of the media source's presentation descriptor.</param>
        /// <param name="startPosition">Specifies where to start playback.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult Start(this IMFMediaSource mediaSource, IMFPresentationDescriptor presentationDescriptor, TimeSpan startPosition)
        {
            if (mediaSource == null)
                throw new ArgumentNullException("mediaSource");

            using (PropVariant start = new PropVariant(startPosition.Ticks))
            {
                return mediaSource.Start(presentationDescriptor, Guid.Empty, start);
            }
        }

        /// <summary>
        /// Starts, seeks, or restarts the media source by specifying where to start playback.
        /// </summary>
        /// <param name="mediaSource">A valid IMFMediaSource instance.</param>
        /// <param name="presentationDescriptor">An IMFPresentationDescriptor instance of the media source's presentation descriptor.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        /// <remarks>This method starts the media source at its beginning.</remarks>
        public static HResult Start(this IMFMediaSource mediaSource, IMFPresentationDescriptor presentationDescriptor)
        {
            if (mediaSource == null)
                throw new ArgumentNullException("mediaSource");

            using (PropVariant start = new PropVariant(0))
            {
                return mediaSource.Start(presentationDescriptor, Guid.Empty, start);
            }
        }
    }
}
