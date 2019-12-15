/* license

IMFSeekInfoExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

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
    public static class IMFSeekInfoExtensions
    {
        public static HResult GetNearestKeyFrames(this IMFSeekInfo seekInfo, TimeSpan startPosition, out TimeSpan previousKeyFrame, out TimeSpan nextKeyFrame)
        {
            if (seekInfo == null)
                throw new ArgumentNullException("seekInfo");

            PropVariant start = new PropVariant(startPosition.Ticks);
            PropVariant previous = new PropVariant();
            PropVariant next = new PropVariant();

            try
            {
                HResult hr = seekInfo.GetNearestKeyFrames(Guid.Empty, start, previous, next);

                previousKeyFrame = TimeSpan.FromTicks(previous.GetLong());
                nextKeyFrame = TimeSpan.FromTicks(next.GetLong());

                return hr;
            }
            finally
            {
                start.Dispose();
                previous.Dispose();
                next.Dispose();
            }
        }
    }
}
