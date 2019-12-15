/* license

IMFPresentationDescriptorExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;
using System.Runtime.InteropServices;

using MediaFoundation.Interop;

namespace MediaFoundation
{
    public static class IMFPresentationDescriptorExtensions
    {
        /// <summary>
        /// Determines if the media presentation requires the Protected Media Path (PMP).
        /// </summary>
        /// <param name="presentationDescriptor">A valid IMFPresentationDescriptor instance.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult RequireProtectedEnvironment(this IMFPresentationDescriptor presentationDescriptor)
        {
            if (presentationDescriptor == null)
                throw new ArgumentNullException("presentationDescriptor");

            return MFExtern.MFRequireProtectedEnvironment(presentationDescriptor);
        }

    }
}
