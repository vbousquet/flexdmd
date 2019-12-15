/* license

IMFMediaBufferExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;
using System.IO;

namespace MediaFoundation
{
    public static class IMFMediaBufferExtensions
    {
        /// <summary>
        /// Gives the caller access to the memory in the value, for reading or writing.
        /// </summary>
        /// <param name="mediaBuffer">A valid IMFMediaBuffer instance.</param>
        /// <param name="streamIndex">Receives a memory streamIndex that wrap the media value memory.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        /// <remarks>The media value must be unlocked with IMFMediaBuffer.Unlock.</remarks>
        public static HResult Lock(this IMFMediaBuffer mediaBuffer, out UnmanagedMemoryStream stream)
        {
            if (mediaBuffer == null)
                throw new ArgumentNullException("mediaBuffer");

            stream = null;
            IntPtr bufferPtr;
            int maxLength, currentLength;

            HResult hr = mediaBuffer.Lock(out bufferPtr, out maxLength, out currentLength);
            if (hr.Succeeded())
            {
                unsafe
                {
                    stream = new UnmanagedMemoryStream((byte*)bufferPtr, currentLength, maxLength, FileAccess.ReadWrite);
                }
            }

            return hr;
        }

    }
}
