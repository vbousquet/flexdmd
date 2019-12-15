/* license

IMFByteStreamTimeSeekExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;

namespace MediaFoundation
{
#if ALLOW_UNTESTED_INTERFACES
    public static class IMFByteStreamTimeSeekExtensions
    {
        public static HResult TimeSeek(this IMFByteStreamTimeSeek timeSeek, TimeSpan timePosition)
        {
            if (timeSeek == null)
                throw new ArgumentNullException("timeSeek");

            return timeSeek.TimeSeek(timePosition.Ticks);
        }

        public static HResult GetTimeSeekResult(this IMFByteStreamTimeSeek timeSeek, out TimeSpan startTime, out TimeSpan stopTime, out TimeSpan duration)
        {
            if (timeSeek == null)
                throw new ArgumentNullException("timeSeek");

            long start, stop, dur;

            HResult hr = timeSeek.GetTimeSeekResult(out start, out stop, out dur);

            startTime = TimeSpan.FromTicks(start);
            stopTime = TimeSpan.FromTicks(stop);
            duration = TimeSpan.FromTicks(dur);

            return hr;
        }
    }
#endif 
}
