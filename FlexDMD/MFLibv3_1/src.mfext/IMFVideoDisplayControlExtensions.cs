/* license

IMFVideoDisplayControlExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;
using System.IO;
using System.Runtime.InteropServices;

using MediaFoundation.EVR;
using MediaFoundation.Misc;

namespace MediaFoundation
{
    public static class IMFVideoDisplayControlExtensions
    {
        /// <summary>
        /// Gets a copy of the current image being displayed by the video renderer. 
        /// </summary>
        /// <param name="videoDisplayControl">A valid IMFVideoDisplayControl instance.</param>
        /// <param name="bih">A BitmapInfoHeader class that receives a description of the bitmap. </param>
        /// <param name="dib">Receives byte array that contains a packed Windows device-independent bitmap (DIB).</param>
        /// <param name="timeStamp">Receives the time stamp of the captured image.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetCurrentImage(this IMFVideoDisplayControl videoDisplayControl, BitmapInfoHeader bih, out byte[] dib, out TimeSpan timeStamp)
        {
            if (videoDisplayControl == null)
                throw new ArgumentNullException("videoDisplayControl");

            IntPtr dibPtr;
            int dibLength;
            long time;

            HResult hr = videoDisplayControl.GetCurrentImage(bih, out dibPtr, out dibLength, out time);
            if (hr.Succeeded())
            {
                try
                {
                    dib = new byte[dibLength];
                    Marshal.Copy(dibPtr, dib, 0, dibLength);
                    timeStamp = TimeSpan.FromTicks(time);
                }
                finally
                {
                    Marshal.FreeCoTaskMem(dibPtr);
                }
            }
            else
            {
                dib = null;
                timeStamp = TimeSpan.MinValue;
            }

            return hr;
        }
    }
}
