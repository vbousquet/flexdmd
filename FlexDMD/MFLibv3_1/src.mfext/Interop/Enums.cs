/* license

Enums.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;

namespace MediaFoundation.Interop
{
    /// <summary>
    /// The STGC enumeration constants specify the conditions for performing the commit operation in the <see cref="IStorage.Commit"/> and <see cref="IStream.Commit"/> methods.
    /// </summary>
    [Flags]
    public enum STGC
    {
        /// <summary>You can specify this condition with <see cref="Consolidate"/>, or some combination of the other three flags in this list of elements. Use this value to increase the readability of code.</summary>
        Default = 0,
        /// <summary>The commit operation can overwrite existing data to reduce overall space requirements.</summary>
        Overwrite = 1,
        /// <summary>Prevents multiple users of a storage object from overwriting each other's changes.</summary>
        OnlyIfCurrent = 2,
        /// <summary>Commits the changes to a write-behind disk cache, but does not save the cache to the disk.</summary>
        DangerouslyCommitMerelyToDiskCache = 4,
        /// <summary>Indicates that a storage should be consolidated after it is committed, resulting in a smaller file on disk.</summary>
        Consolidate = 8
    }

    /// <summary>
    /// The STATFLAG enumeration values indicate whether the method should try to return a name in the <see cref="System.Runtime.InteropServices.ComTypes.STATSTG.pwcsName"/> member of the <see cref="System.Runtime.InteropServices.ComTypes.STATSTG"/> structure. The values are used in the <see cref="IStorage.Stat"/>, and <see cref="IStream.Stat"/> methods to save memory when the pwcsName member is not required.
    /// </summary>
    /// <unmanaged>STATFLAG_*</unmanaged>
    public enum STATFLAG
    {
        /// <summary>Requests that the statistics include the pwcsName member of the STATSTG structure.</summary>
        Default = 0,
        /// <summary>Requests that the statistics not include the pwcsName member of the STATSTG structure.</summary>
        NoName = 1,
        /// <summary>Not implemented.</summary>
        NoOpen = 2
    }

    /// <summary>
    /// The STGTY enumeration values are used in the type member of the STATSTG structure to indicate the type of the storage element. A storage element is a storage object, a streamIndex object, or a byte-array object (LOCKBYTES).
    /// </summary>
    public enum STGTY
    {
        None = 0,
        /// <summary>Indicates that the storage element is a storage object.</summary>
        Storage = 1,
        /// <summary>Indicates that the storage element is a streamIndex object.</summary>
        Stream = 2,
        /// <summary>Indicates that the storage element is a byte-array object.</summary>
        LockBytes = 3,
        /// <summary>Indicates that the storage element is a property storage object.</summary>
        Property = 4
    }

}
