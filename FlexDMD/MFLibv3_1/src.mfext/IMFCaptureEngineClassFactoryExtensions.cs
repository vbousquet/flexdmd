/* license

IMFCaptureEngineClassFactoryExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

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
    public static class IMFCaptureEngineClassFactoryExtensions
    {
        /// <summary>
        /// Creates an instance of the capture engine.
        /// </summary>
        /// <param name="factory">A valid IMFCaptureEngineClassFactory instance.</param>
        /// <param name="engine">Receives an instance of the capture engine.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateInstance(this IMFCaptureEngineClassFactory factory, out IMFCaptureEngine engine)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            object tmp;

            HResult hr = factory.CreateInstance(CLSID.CLSID_MFCaptureEngine, typeof(IMFCaptureEngine).GUID, out tmp);
            engine = hr.Succeeded() ? tmp as IMFCaptureEngine : null;

            return hr;
        }
    }
}
