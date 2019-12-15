/* license

IMFMetadataExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

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
    public static class IMFMetadataExtensions
    {
        /// <summary>
        /// Gets a list of the languages in which metadata is available.
        /// </summary>
        /// <param name="metadata">A valid IMFMetadata instance.</param>
        /// <param name="languages">Receives the list of languages as an array strings. Each string in the array is an RFC 1766-compliant language tag.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetAllLanguages(this IMFMetadata metadata, out string[] languages)
        {
            if (metadata == null)
                throw new ArgumentNullException("metadata");

            languages = null;

            using (PropVariant results = new PropVariant())
            {
                HResult hr = metadata.GetAllLanguages(results);
                if (hr.Succeeded())
                {
                    languages = results.GetStringArray();
                }

                return hr;
            }
        }

        /// <summary>
        /// Gets a list of all the metadata property names on this object.
        /// </summary>
        /// <param name="metadata">A valid IMFMetadata instance.</param>
        /// <param name="propertyNames">Receives an array strings.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetAllPropertyNames(this IMFMetadata metadata, out string[] propertyNames)
        {
            if (metadata == null)
                throw new ArgumentNullException("metadata");

            propertyNames = null;

            using(PropVariant results = new PropVariant())
            {
                HResult hr = metadata.GetAllPropertyNames(results);
                if (hr.Succeeded())
                {
                    propertyNames = results.GetStringArray();
                }

                return hr;
            }
        }
    }
}
