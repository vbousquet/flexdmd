/* license

IMFClockExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;

namespace MediaFoundation
{
    public static class IMFClockExtensions
    {
        /// <summary>
        /// Retrieves the last clock time that was correlated with system time.
        /// </summary>
        /// <param name="clock">A valid IMFClock instance.</param>
        /// <param name="clockTime">Receives the last known clock time, in units of the clock's frequency.</param>
        /// <param name="systemTime">Receives the system time that corresponds to the clock time.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetCorrelatedTime(this IMFClock clock, out long clockTime, out TimeSpan systemTime)
        {
            if (clock == null)
                throw new ArgumentNullException("clock");

            long tmp;

            HResult hr = clock.GetCorrelatedTime(0, out clockTime, out tmp);
            systemTime = hr.Succeeded() ? TimeSpan.FromTicks(tmp) : default(TimeSpan);

            return hr;
        }

        /// <summary>
        /// Retrieves the current state of the clock.
        /// </summary>
        /// <param name="clock">A valid IMFClock instance.</param>
        /// <param name="clockState">Receives the clock state</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetState(this IMFClock clock, out MFClockState clockState)
        {
            if (clock == null)
                throw new ArgumentNullException("clock");

            return clock.GetState(0, out clockState);
        }
    }
}
