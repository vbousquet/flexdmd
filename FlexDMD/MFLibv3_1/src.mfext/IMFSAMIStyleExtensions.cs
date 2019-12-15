/* license

IMFSAMIStyleExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

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
    public static class IMFSAMIStyleExtensions
    {
        /// <summary>
        /// Gets a list of the style names defined in the SAMI file. 
        /// </summary>
        /// <param name="samiStyle">A valid IMFSAMIStyle instance.</param>
        /// <param name="styles">Receives an array of strings.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetStyles(this IMFSAMIStyle samiStyle, out string[] styles)
        {
            if (samiStyle == null)
                throw new ArgumentNullException("samiStyle");

            styles = null;

            using (PropVariant results = new PropVariant())
            {
                HResult hr = samiStyle.GetStyles(results);
                if (hr.Succeeded())
                {
                    styles = results.GetStringArray();
                }

                return hr;
            }
        }
    }
}
