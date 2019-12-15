/* license

IMFGetServiceExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;

namespace MediaFoundation
{
    public static class IMFGetServiceExtensions
    {
        /// <summary>
        /// Retrieves a service interface.
        /// </summary>
        /// <typeparam name="T">The COM interface being requested.</typeparam>
        /// <param name="getService">A valid IMFGetService instance.</param>
        /// <param name="guidService">The service identifier (SID) of the service.</param>
        /// <param name="service">Receives the interface instance.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetService<T>(this IMFGetService getService, Guid guidService, out T service) where T : class
        {
            if (getService == null)
                throw new ArgumentNullException("getService");

            Type typeOfT = typeof(T);

            if (!typeOfT.IsInterface)
                throw new ArgumentOutOfRangeException("T", "T must be a COM visible interface.");

            object tmp;

            HResult hr = getService.GetService(guidService, typeOfT.GUID, out tmp);
            service = hr.Succeeded() ? tmp as T : default(T);

            return hr;
        }
    }
}
