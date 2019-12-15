/* license

PropVariantExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;
using System.Runtime.InteropServices;

using MediaFoundation.Misc;

namespace MediaFoundation.Interop
{
    public static class PropVariantExtensions
    {
        /// <summary>
        /// Convert a PropVariant to a managed object
        /// </summary>
        /// <param name="variant">The PropVariant to convert.</param>
        /// <returns>A managed object corresponding to the given PropVariant or null if the PropVariant type is unsupported.</returns>
        public static object GetObject(this ConstPropVariant variant)
        {
            if (variant == null)
                throw new ArgumentNullException("variant");

            switch (variant.GetVariantType())
            {
                case ConstPropVariant.VariantType.None: 
                    return null;

                case ConstPropVariant.VariantType.Short:
                    return variant.GetShort();

                case ConstPropVariant.VariantType.Int32:
                    return variant.GetInt();

                case ConstPropVariant.VariantType.Float:
                    return variant.GetFloat();

                case ConstPropVariant.VariantType.Double:
                    return variant.GetDouble();

                case ConstPropVariant.VariantType.IUnknown:
                    return variant.GetIUnknown();

                case ConstPropVariant.VariantType.UByte:
                    return variant.GetUByte();

                case ConstPropVariant.VariantType.UShort:
                    return variant.GetUShort();

                case ConstPropVariant.VariantType.UInt32:
                    return variant.GetUInt();

                case ConstPropVariant.VariantType.UInt64:
                    return variant.GetULong();

                case ConstPropVariant.VariantType.String:
                    return variant.GetString();

                case ConstPropVariant.VariantType.Guid:
                    return variant.GetGuid();

                case ConstPropVariant.VariantType.Blob:
                    return variant.GetBlob();

                case ConstPropVariant.VariantType.StringArray:
                    return variant.GetStringArray();
            }

            return null;
        }
    }
}
