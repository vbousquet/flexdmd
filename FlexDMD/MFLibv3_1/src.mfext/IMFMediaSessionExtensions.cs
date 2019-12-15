/* license

IMFMediaSessionExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

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
    public static class IMFMediaSessionExtensions
    {
        /// <summary>
        /// Starts the Media Session.
        /// </summary>
        /// <param name="mediaSession">A valid IMFMediaSession instance.</param>
        /// <param name="startPosition">The starting position for playback.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult Start(this IMFMediaSession mediaSession, TimeSpan startPosition)
        {
            if (mediaSession == null)
                throw new ArgumentNullException("mediaSession");

            using (PropVariant start = new PropVariant(startPosition.Ticks))
            {
                return mediaSession.Start(Guid.Empty, start);
            }
        }

        /// <summary>
        /// Starts the Media Session.
        /// </summary>
        /// <param name="mediaSession">A valid IMFMediaSession instance.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        /// <remarks>This method starts the media session at its beginning.</remarks>
        public static HResult Start(this IMFMediaSession mediaSession)
        {
            return Start(mediaSession, TimeSpan.FromTicks(0));
        }
    }
}
