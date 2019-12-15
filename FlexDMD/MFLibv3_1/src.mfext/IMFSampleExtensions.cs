/* license

IMFSampleExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;

namespace MediaFoundation
{
    public static class IMFSampleExtensions
    {
        /// <summary>
        /// Retrieves the duration of the sample.
        /// </summary>
        /// <param name="sample">A valid IMFSample instance.</param>
        /// <param name="sampleTime">Receives the sample duration.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetSampleDuration(this IMFSample sample, out TimeSpan sampleDuration)
        {
            if (sample == null)
                throw new ArgumentNullException("sample");

            long tmp;

            HResult hr = sample.GetSampleDuration(out tmp);
            sampleDuration = hr.Succeeded() ? TimeSpan.FromTicks(tmp) : default(TimeSpan);

            return hr;
        }

        /// <summary>
        /// Retrieves the presentation time of the sample.
        /// </summary>
        /// <param name="sample">A valid IMFSample instance.</param>
        /// <param name="sampleTime">Receives the presentation time.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetSampleTime(this IMFSample sample, out TimeSpan sampleTime)
        {
            if (sample == null)
                throw new ArgumentNullException("sample");

            long tmp;

            HResult hr = sample.GetSampleDuration(out tmp);
            sampleTime = hr.Succeeded() ? TimeSpan.FromTicks(tmp) : default(TimeSpan);

            return hr;
        }

        /// <summary>
        /// Sets the duration of the sample.
        /// </summary>
        /// <param name="sample">A valid IMFSample instance.</param>
        /// <param name="sampleTime">Duration of the sample.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult SetSampleDuration(this IMFSample sample, TimeSpan sampleDuration)
        {
            if (sample == null)
                throw new ArgumentNullException("sample");

            return sample.SetSampleDuration(sampleDuration.Ticks);
        }

        /// <summary>
        /// Sets the presentation time of the sample.
        /// </summary>
        /// <param name="sample">A valid IMFSample instance.</param>
        /// <param name="sampleTime">The presentation time.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult SetSampleTime(this IMFSample sample, TimeSpan sampleTime)
        {
            if (sample == null)
                throw new ArgumentNullException("sample");

            return sample.SetSampleTime(sampleTime.Ticks);
        }
    }
}
