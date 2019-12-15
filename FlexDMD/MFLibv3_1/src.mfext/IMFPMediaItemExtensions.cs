/* license

IMFPMediaItemExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;

using MediaFoundation.MFPlayer;
using MediaFoundation.Misc;

namespace MediaFoundation
{
    public static class IMFPMediaItemExtensions
    {
        /// <summary>
        /// Gets the duration of the media item.
        /// </summary>
        /// <param name="mediaItem">A valid IMFPMediaItem instance.</param>
        /// <param name="durationValue">Receives the duration.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetDuration(this IMFPMediaItem mediaItem, out TimeSpan durationValue)
        {
            if (mediaItem == null)
                throw new ArgumentNullException("mediaItem");

            using (PropVariant result = new PropVariant())
            {
                HResult hr = mediaItem.GetDuration(Guid.Empty, result);
                durationValue = hr.Succeeded() ? TimeSpan.FromTicks((long)result.GetULong()) : default(TimeSpan);

                return hr;
            }
        }

        /// <summary>
        /// Gets the start and stop times for the media item.
        /// </summary>
        /// <param name="mediaItem">A valid IMFPMediaItem instance.</param>
        /// <param name="startValue">Receives the start position.</param>
        /// <param name="stopValue">Receives the stop position.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetStartStopPosition(this IMFPMediaItem mediaItem, out TimeSpan startValue, out TimeSpan stopValue)
        {
            if (mediaItem == null)
                throw new ArgumentNullException("mediaItem");

            using (PropVariant resultStart = new PropVariant(), resultStop = new PropVariant())
            {
                HResult hr = mediaItem.GetStartStopPosition(Guid.Empty, resultStart, Guid.Empty, resultStop);
                if (hr.Succeeded())
                {
                    startValue = TimeSpan.FromTicks((long)resultStart.GetULong());
                    stopValue = TimeSpan.FromTicks((long)resultStop.GetULong());
                }
                else
                {
                    startValue = default(TimeSpan);
                    stopValue = default(TimeSpan);
                }

                return hr;
            }
        }

        /// <summary>
        /// Sets the start and stop time for the media item.
        /// </summary>
        /// <param name="mediaItem">A valid IMFPMediaItem instance.</param>
        /// <param name="startValue">Start position.</param>
        /// <param name="stopValue">Stop position.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult SetStartStopPosition(this IMFPMediaItem mediaItem, TimeSpan startValue, TimeSpan stopValue)
        {
            if (mediaItem == null)
                throw new ArgumentNullException("mediaItem");

            using (PropVariant resultStart = new PropVariant(startValue.Ticks), resultStop = new PropVariant(stopValue.Ticks))
            {
                return mediaItem.SetStartStopPosition(Guid.Empty, resultStart, Guid.Empty, resultStop);
            }
        }

        /// <summary>
        /// Sets a media sink for the media item. A media sink is an object that consumes the data from one or more streams. 
        /// </summary>
        /// <param name="mediaItem">A valid IMFPMediaItem instance.</param>
        /// <param name="streamIndex">Zero-based index of a streamIndex on the media source.</param>
        /// <param name="streamSink">An instance to a streamIndex sink.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult SetStreamSink(this IMFPMediaItem mediaItem, int streamIndex, IMFStreamSink streamSink)
        {
            if (mediaItem == null)
                throw new ArgumentNullException("mediaItem");

            return mediaItem.SetStreamSink(streamIndex, streamSink);
        }

        /// <summary>
        /// Sets a media sink for the media item. A media sink is an object that consumes the data from one or more streams. 
        /// </summary>
        /// <param name="mediaItem">A valid IMFPMediaItem instance.</param>
        /// <param name="streamIndex">Zero-based index of a streamIndex on the media source.</param>
        /// <param name="activate">An instance of an activation object that creates the media sink.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult SetStreamSink(this IMFPMediaItem mediaItem, int streamIndex, IMFActivate activate)
        {
            if (mediaItem == null)
                throw new ArgumentNullException("mediaItem");

            return mediaItem.SetStreamSink(streamIndex, activate);
        }
    }
}
