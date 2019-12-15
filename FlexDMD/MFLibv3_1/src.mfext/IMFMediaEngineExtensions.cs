/* license

IMFMediaEngineExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;

namespace MediaFoundation
{
    public static class IMFMediaEngineExtensions
    {
        /// <summary>
        /// Seeks to a new playback position.
        /// </summary>
        /// <param name="mediaEngine">A valid IMFMediaBuffer instance.</param>
        /// <param name="seekTime">The new playback position.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult SetCurrentTime(this IMFMediaEngine mediaEngine, TimeSpan seekTime)
        {
            if (mediaEngine == null)
                throw new ArgumentNullException("mediaEngine");

            return mediaEngine.SetCurrentTime(seekTime.TotalSeconds);
        }
    }
}
