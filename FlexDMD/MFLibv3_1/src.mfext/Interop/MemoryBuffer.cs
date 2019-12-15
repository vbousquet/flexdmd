/* license

MemoryBuffer.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;
using System.Runtime.InteropServices;

namespace MediaFoundation.Interop
{
    /// <summary>
    /// A class based on SafeBuffer that allow to easily read or write into unmanaged memory.
    /// </summary>
    public class MemoryBuffer : SafeBuffer
    {
        /// <summary>
        /// Create an instance of a MemoryBuffer around unmanaged memory.
        /// </summary>
        /// <param name="bufferPtr">The unmanaged memory pointer.</param>
        /// <param name="bufferSize">The unmanaged memory size, in bytes.</param>
        /// <param name="ownBuffer">If true, this instance is responsible to free the unmanaged memory when the object is diposed. Otherwise, the unmanaged memory is not freed.</param>
        public MemoryBuffer(IntPtr bufferPtr, uint bufferSize, bool ownBuffer = false) : base(ownBuffer)
        {
            SetHandle(bufferPtr);
            Initialize(bufferSize);
        }

        /// <summary>
        /// Create an instance of a MemoryBuffer around unmanaged memory.
        /// </summary>
        /// <param name="bufferSize">The unmanaged memory size, in bytes.</param>
        public MemoryBuffer(uint bufferSize) : this(Marshal.AllocCoTaskMem((int)bufferSize), bufferSize, true) { }

        public IntPtr BufferPointer
        {
            get { return this.handle; }
        }

        /// <summary>
        /// Free the unmanaged memory.
        /// </summary>
        /// <returns></returns>
        protected override bool ReleaseHandle()
        {
            Marshal.FreeCoTaskMem(this.handle);
            this.handle = IntPtr.Zero;

            return true;
        }
    }
}
