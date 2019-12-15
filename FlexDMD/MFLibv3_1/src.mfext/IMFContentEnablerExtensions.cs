/* license

IMFContentEnablerExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;
using System.Runtime.InteropServices;

namespace MediaFoundation
{
    public static class IMFContentEnablerExtensions
    {
        public static HResult GetEnableData(this IMFContentEnabler contentEnabler, out byte[] data)
        {
            if (contentEnabler == null)
                throw new ArgumentNullException("contentEnabler");

            IntPtr dataPtr;
            int dataLength;

            HResult hr = contentEnabler.GetEnableData(out dataPtr, out dataLength);
            if (hr.Succeeded())
            {
                try
                {
                    data = new byte[dataLength];
                    Marshal.Copy(dataPtr, data, 0, dataLength);
                }
                finally
                {
                    Marshal.FreeCoTaskMem(dataPtr);
                }
            }
            else
            {
                data = null;
            }

            return hr;
        }
    }
}
