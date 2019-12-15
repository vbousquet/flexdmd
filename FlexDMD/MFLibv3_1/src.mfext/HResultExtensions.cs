/* license

HResultExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

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
    public static class HResultExtensions
    {
        public static int ToInt32(this HResult hr)
        {
            return (int)hr;
        }

        public static bool Succeeded(this HResult hr)
        {
            return (hr >= 0);
        }

        public static bool Failed(this HResult hr)
        {
            return (hr < 0);
        }

        public static string GetDescription(this HResult hr)
        {
            return MFError.GetErrorText((int)hr);
        }

        public static void ThrowExceptionOnError(this HResult hr)
        {
            MFError.ThrowExceptionForHR(hr);
        }
    }
}
