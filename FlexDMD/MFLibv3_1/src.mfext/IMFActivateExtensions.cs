/* license

IMFActivateExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

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
    public static class IMFActivateExtensions
    {
        /// <summary>
        /// Creates the object associated with this activation object. 
        /// </summary>
        /// <typeparam name="T">The requested COM interface.</typeparam>
        /// <param name="activate">A valid IMFActivate instance.</param>
        /// <param name="activatedObject">The requested interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult ActivateObject<T>(this IMFActivate activate, out T activatedObject) where T : class
        {
            if (activate == null)
                throw new ArgumentNullException("activate");

            Type typeOfT = typeof(T);

            if (!typeOfT.IsInterface)
                throw new ArgumentOutOfRangeException("T", "T must be a COM visible interface.");

            object tmp;

            HResult hr = activate.ActivateObject(typeOfT.GUID, out tmp);
            activatedObject = hr.Succeeded() ? tmp as T : null;

            return hr;
        }
    }
}
