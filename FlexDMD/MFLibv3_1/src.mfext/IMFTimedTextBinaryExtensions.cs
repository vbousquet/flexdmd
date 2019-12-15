/* license

IMFTimedTextBinaryExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;

using MediaFoundation.Interop;
using MediaFoundation.Misc;

namespace MediaFoundation
{
#if ALLOW_UNTESTED_INTERFACES

    public static class IMFTimedTextBinaryExtensions
    {
        /// <summary>
        /// Gets the data content of the timed-text object.
        /// </summary>
        /// <param name="timedTextBinary">A valid IMFTimedTextBinary instance.</param>
        /// <param name="data">Return the timed-text data as a MemoryBuffer.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetData(this IMFTimedTextBinary timedTextBinary, out MemoryBuffer data)
        {
            if (timedTextBinary == null)
                throw new ArgumentNullException("timedTextBinary");

            data = null;

            IntPtr resultPtr;
            int resultSize;

            HResult hr = timedTextBinary.GetData(out resultPtr, out resultSize);
            if (hr.Succeeded())
            {
                data = new MemoryBuffer(resultPtr, (uint)resultSize);
            }

            return hr;
        }
    }

#endif
}
