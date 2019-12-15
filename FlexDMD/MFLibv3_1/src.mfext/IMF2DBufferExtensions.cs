/* license

IMF2DBufferExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

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
    public static class IMF2DBufferExtensions
    {
        /// <summary>
        /// Copies data to this buffer from a byte array that has a contiguous format.
        /// </summary>
        /// <param name="value">A valid IMF2DBuffer instance.</param>
        /// <param name="sourceBuffer">The source byte array.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult ContiguousCopyFrom(this IMF2DBuffer buffer, byte[] sourceBuffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("value");

            if (sourceBuffer == null)
                throw new ArgumentNullException("destinationBuffer");

            using (GCPin pinnedArray = new GCPin(sourceBuffer))
            {
                return buffer.ContiguousCopyFrom(pinnedArray.PinnedAddress, sourceBuffer.Length);
            }
        }

        /// <summary>
        /// Copies data to this buffer from a byte array segment that has a contiguous format.
        /// </summary>
        /// <param name="value">A valid IMF2DBuffer instance.</param>
        /// <param name="sourceBuffer">A byte array segment.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult ContiguousCopyFrom(this IMF2DBuffer buffer, ArraySegment<byte> sourceBuffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("value");

            if (sourceBuffer.Array == null)
                throw new ArgumentNullException("sourceBuffer.Array");

            using (GCPin pinnedArray = new GCPin(sourceBuffer.Array))
            {
                IntPtr startAddress = pinnedArray.PinnedAddress + sourceBuffer.Offset;
                return buffer.ContiguousCopyFrom(startAddress, sourceBuffer.Count);
            }
        }

        /// <summary>
        /// Copies data to this buffer from a MemoryBuffer that has a contiguous format.
        /// </summary>
        /// <param name="value">A valid IMF2DBuffer instance.</param>
        /// <param name="sourceBuffer">A MemoryBuffer.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult ContiguousCopyFrom(this IMF2DBuffer buffer, MemoryBuffer sourceBuffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("value");

            if (sourceBuffer == null)
                throw new ArgumentNullException("sourceBuffer");

            return buffer.ContiguousCopyFrom(sourceBuffer.BufferPointer, (int)sourceBuffer.ByteLength);
        }

        /// <summary>
        /// Copies this buffer into a byte array, converting the data to contiguous format.
        /// </summary>
        /// <param name="value">A valid IMF2DBuffer instance.</param>
        /// <param name="destinationBuffer">The destination byte array.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult ContiguousCopyTo(this IMF2DBuffer buffer, byte[] destinationBuffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("value");

            if (destinationBuffer == null)
                throw new ArgumentNullException("destinationBuffer");

            using (GCPin pinnedArray = new GCPin(destinationBuffer))
            {
                return buffer.ContiguousCopyFrom(pinnedArray.PinnedAddress, destinationBuffer.Length);
            }
        }

        /// <summary>
        /// Copies this buffer into a byte array segment, converting the data to contiguous format.
        /// </summary>
        /// <param name="value">A valid IMF2DBuffer instance.</param>
        /// <param name="destinationBuffer">The destination byte array segment.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult ContiguousCopyTo(this IMF2DBuffer buffer, ArraySegment<byte> destinationBuffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("value");

            if (destinationBuffer.Array == null)
                throw new ArgumentNullException("destinationBuffer.Array");

            using (GCPin pinnedArray = new GCPin(destinationBuffer.Array))
            {
                IntPtr startAddress = pinnedArray.PinnedAddress + destinationBuffer.Offset;
                return buffer.ContiguousCopyFrom(startAddress, destinationBuffer.Count);
            }
        }

        /// <summary>
        /// Copies this buffer into a MemoryBuffer, converting the data to contiguous format.
        /// </summary>
        /// <param name="value">A valid IMF2DBuffer instance.</param>
        /// <param name="destinationBuffer">The destination MemoryBuffer.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult ContiguousCopyTo(this IMF2DBuffer buffer, MemoryBuffer destinationBuffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("value");

            if (destinationBuffer == null)
                throw new ArgumentNullException("destinationBuffer");

            return buffer.ContiguousCopyTo(destinationBuffer.BufferPointer, (int)destinationBuffer.ByteLength);
        }
    }
}
