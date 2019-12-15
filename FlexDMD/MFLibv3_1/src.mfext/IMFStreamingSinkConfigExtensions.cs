/* license

IMFStreamingSinkConfigExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;

namespace MediaFoundation
{
    public static class IMFStreamingSinkConfigExtensions
    {
        /// <summary>
        /// Called by the streaming media client before the Media Session starts streaming to specify the byte offset or the time offset.
        /// </summary>
        /// <param name="streamingSinkConfig">A valid IMFStreamingSinkConfig instance.</param>
        /// <param name="byteOffset">A seeking byte offset.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult StartStreaming(this IMFStreamingSinkConfig streamingSinkConfig, long byteOffset)
        {
            if (streamingSinkConfig == null)
                throw new ArgumentNullException("streamingSinkConfig");

            return streamingSinkConfig.StartStreaming(true, byteOffset);
        }

        /// <summary>
        /// Called by the streaming media client before the Media Session starts streaming to specify the byte offset or the time offset.
        /// </summary>
        /// <param name="streamingSinkConfig">A valid IMFStreamingSinkConfig instance.</param>
        /// <param name="timePosition">A seeking time offset.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult StartStreaming(this IMFStreamingSinkConfig streamingSinkConfig, TimeSpan timePosition)
        {
            if (streamingSinkConfig == null)
                throw new ArgumentNullException("streamingSinkConfig");

            return streamingSinkConfig.StartStreaming(false, timePosition.Ticks);
        }
    }
}
