/* license

IMFPresentationClockExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;

namespace MediaFoundation
{
    public static class IMFPresentationClockExtensions
    {
        /// <summary>
        /// Gets the latest clock time. 
        /// </summary>
        /// <param name="presentationClock">A valid IMFPresentationClock instance.</param>
        /// <param name="clockTime">Receives the latest clock time.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetTime(this IMFPresentationClock presentationClock, out TimeSpan clockTime)
        {
            if (presentationClock == null)
                throw new ArgumentNullException("presentationClock");

            long tmp;

            HResult hr = presentationClock.GetTime(out tmp);
            clockTime = hr.Succeeded() ? TimeSpan.FromTicks(tmp) : default(TimeSpan);

            return hr;
        }

        /// <summary>
        /// Starts the presentation clock.
        /// </summary>
        /// <param name="presentationClock">A valid IMFPresentationClock instance.</param>
        /// <param name="clockStartOffset">The initial starting time</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        /// <remarks>Use TimeSpan.MaxValue in the <paramref name="clockStartOffset"/> parameter to start the clock from its current position. Use this value if the clock is paused and you want to restart it from the same position.</remarks>
        public static HResult Start(this IMFPresentationClock presentationClock, TimeSpan clockStartOffset)
        {
            if (presentationClock == null)
                throw new ArgumentNullException("presentationClock");

            return presentationClock.Start(clockStartOffset.Ticks);
        }
    }
}
