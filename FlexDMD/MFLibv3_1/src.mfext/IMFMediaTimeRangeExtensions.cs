/* license

IMFMediaTimeRangeExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;

namespace MediaFoundation
{
    public static class IMFMediaTimeRangeExtensions
    {
        /// <summary>
        /// Adds a new range to the list of time ranges.
        /// </summary>
        /// <param name="mediaTimeRange">A valid IMFMediaTimeRange instance.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult AddRange(this IMFMediaTimeRange mediaTimeRange, TimeSpan startTime, TimeSpan endTime)
        {
            if (mediaTimeRange == null)
                throw new ArgumentNullException("mediaTimeRange");

            return mediaTimeRange.AddRange(startTime.TotalSeconds, endTime.TotalSeconds);
        }

        /// <summary>
        /// Queries whether a specified time falls within any of the time ranges.
        /// </summary>
        /// <param name="mediaTimeRange">A valid IMFMediaTimeRange instance.</param>
        /// <param name="time">The time.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static bool ContainsTime(this IMFMediaTimeRange mediaTimeRange, TimeSpan time)
        {
            if (mediaTimeRange == null)
                throw new ArgumentNullException("mediaTimeRange");

            return mediaTimeRange.ContainsTime(time.TotalSeconds);
        }

        /// <summary>
        /// Gets the end time for a specified time range.
        /// </summary>
        /// <param name="mediaTimeRange">A valid IMFMediaTimeRange instance.</param>
        /// <param name="index">The zero-based index of the time range to query.</param>
        /// <param name="end">Receives the end time.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetEnd(this IMFMediaTimeRange mediaTimeRange, int index, TimeSpan end)
        {
            if (mediaTimeRange == null)
                throw new ArgumentNullException("mediaTimeRange");

            double result;

            HResult hr = mediaTimeRange.GetEnd(index, out result);
            end = hr.Succeeded() ? TimeSpan.FromSeconds(result) : default(TimeSpan);

            return hr;
        }

        /// <summary>
        /// Gets the start time for a specified time range.
        /// </summary>
        /// <param name="mediaTimeRange">A valid IMFMediaTimeRange instance.</param>
        /// <param name="index">The zero-based index of the time range to query.</param>
        /// <param name="end">Receives the start time.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetStart(this IMFMediaTimeRange mediaTimeRange, int index, TimeSpan start)
        {
            if (mediaTimeRange == null)
                throw new ArgumentNullException("mediaTimeRange");

            double result;

            HResult hr = mediaTimeRange.GetStart(index, out result);
            start = hr.Succeeded() ? TimeSpan.FromSeconds(result) : default(TimeSpan);

            return hr;
        }
    }
}
