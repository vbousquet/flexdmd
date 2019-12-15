/* license

TypeExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;
using System.Reflection;

namespace MediaFoundation.Interop
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Determine if a type represents an interface decorated with the ComImport attribute.
        /// </summary>
        /// <param name="type">The type to test.</param>
        /// <returns>True if the type represents an interface decorated with the ComImport attribute; False otherwise.</returns>
        public static bool IsComInterface(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return (type.IsInterface && ((type.Attributes & TypeAttributes.Import) != 0));
        }
    }
}
