/* license

IMFAttributesExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using MediaFoundation.Interop;
using MediaFoundation.Misc;

namespace MediaFoundation
{
    public static class IMFAttributesExtensions
    {
        /// <summary>
        /// Determines if a key exists in an IMFAttributes instance.
        /// </summary>
        /// <param name="attributes">A valid IMFAttributes instance.</param>
        /// <param name="guidKey">The key to test.</param>
        /// <returns>True if the instance have a value for the given key ; False otherwise.</returns>
        public static bool IsKeyExists(this IMFAttributes attributes, Guid guidKey)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            HResult hr = attributes.GetItem(guidKey, null);
            return (hr != HResult.MF_E_ATTRIBUTENOTFOUND);
        }

        /// <summary>
        /// Retrieves the value associated with a key.
        /// </summary>
        /// <param name="attributes">A valid IMFAttributes instance.</param>
        /// <param name="guidKey">The key that identifies the value to retrieve.</param>
        /// <param name="value">The value.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetItem(this IMFAttributes attributes, Guid guidKey, out object value)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            HResult hr = 0;
            value = null;

            using (PropVariant variant = new PropVariant())
            {
                hr = attributes.GetItem(guidKey, variant);
                if (hr.Succeeded())
                    value = variant.GetObject();
            }

            return hr;
        }

        /// <summary>
        /// Retrieves an attribute at the specified index.
        /// </summary>
        /// <param name="attributes">A valid IMFAttributes instance.</param>
        /// <param name="index">The index to retrieve.</param>
        /// <param name="guidKey">The key of this attribute.</param>
        /// <param name="value">The value of this attribute.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetItemByIndex(this IMFAttributes attributes, int index, out Guid guidKey, out object value)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            if (index < 0)
                throw new ArgumentOutOfRangeException("index");

            HResult hr = 0;
            value = null;

            using (PropVariant variant = new PropVariant())
            {
                hr = attributes.GetItemByIndex(index, out guidKey, variant);
                if (hr.Succeeded())
                    value = variant.GetObject();
            }

            return hr;
        }

        /// <summary>
        /// Retrieves a Boolean value associated with a key.
        /// </summary>
        /// <param name="attributes">A valid IMFAttributes instance.</param>
        /// <param name="guidKey">Guid that identifies which value to retrieve.</param>
        /// <param name="value">Receives the requested value.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        /// <remarks>Media Foundation attributes don't support boolean values. This method get them as a 32-bits unsigned integer and return true for any non-zero value and false otherwise.</remarks>
        public static HResult GetBoolean(this IMFAttributes attributes, Guid guidKey, out bool value)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            uint result;

            HResult hr = attributes.GetUINT32(guidKey, out result);
            value = hr.Succeeded() ? (result != 0) : default(bool);

            return hr;
        }

        /// <summary>
        /// Associates a Boolean value with a key.
        /// </summary>
        /// <param name="attributes">A valid IMFAttributes instance.</param>
        /// <param name="guidKey">Guid that identifies the value to set.</param>
        /// <param name="value">New value for this key.</param>
        /// <remarks>Media Foundation attributes don't support boolean values. This method set them as a 32-bits unsigned integer and store 1 for true and 0 for false.</remarks>
        public static HResult SetBoolean(this IMFAttributes attributes, Guid guidKey, bool value)
        {
            return attributes.SetUINT32(guidKey, (value ? 1u : 0u));
        }

        /// <summary>
        /// Retrieves a UInt32 value associated with a key.
        /// </summary>
        /// <param name="attributes">A valid IMFAttributes instance.</param>
        /// <param name="guidKey">Guid that identifies which value to retrieve.</param>
        /// <param name="value">Receives the requested value.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetUINT32(this IMFAttributes attributes, Guid guidKey, out uint value)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            int result = 0;

            HResult hr = attributes.GetUINT32(guidKey, out result);
            value = hr.Succeeded() ? (uint)result : default(uint);

            return hr;
        }

        /// <summary>
        /// Associates a UInt32 value with a key.
        /// </summary>
        /// <param name="attributes">A valid IMFAttributes instance.</param>
        /// <param name="guidKey">Guid that identifies the value to set.</param>
        /// <param name="value">New value for this key.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult SetUINT32(this IMFAttributes attributes, Guid guidKey, uint value)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            return attributes.SetUINT32(guidKey, unchecked((int)value));
        }

        /// <summary>
        /// Retrieves a UInt64 value associated with a key.
        /// </summary>
        /// <param name="attributes">A valid IMFAttributes instance.</param>
        /// <param name="guidKey">Guid that identifies which value to retrieve.</param>
        /// <param name="value">Receives the requested value.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetUINT64(this IMFAttributes attributes, Guid guidKey, out ulong value)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            long result = 0;

            HResult hr = attributes.GetUINT64(guidKey, out result);
            value = (ulong)result;

            return hr;
        }

        /// <summary>
        /// Associates a UInt64 value with a key.
        /// </summary>
        /// <param name="attributes">A valid IMFAttributes instance.</param>
        /// <param name="guidKey">Guid that identifies the value to set.</param>
        /// <param name="value">New value for this key.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult SetUINT64(this IMFAttributes attributes, Guid guidKey, ulong value)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            return attributes.SetUINT64(guidKey, unchecked((long)value));
        }

        /// <summary>
        /// Retrieves an address location value associated with a key.
        /// </summary>
        /// <param name="attributes">A valid IMFAttributes instance.</param>
        /// <param name="guidKey">Guid that identifies which value to retrieve.</param>
        /// <param name="value">Receives the requested value as an IntPtr.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        /// <remarks>Media Foundation attributes don't natively support pointer values. This method get them as a 64-bits unsigned integer and convert that value as a IntPtr.</remarks>
        public static HResult GetPointer(this IMFAttributes attributes, Guid guidKey, out IntPtr value)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            long result = 0;

            HResult hr = attributes.GetUINT64(guidKey, out result);
            value = new IntPtr(result);

            return hr;
        }

        /// <summary>
        /// Associates an address location value with a key.
        /// </summary>
        /// <param name="attributes">A valid IMFAttributes instance.</param>
        /// <param name="guidKey">Guid that identifies the value to set.</param>
        /// <param name="value">New value for this key as a IntPtr.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        /// <remarks>Media Foundation attributes don't natively support pointer values. This method store them as a 64-bits unsigned integer.</remarks>
        public static HResult SetPointer(this IMFAttributes attributes, Guid guidKey, IntPtr value)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            return attributes.SetUINT64(guidKey, value.ToInt64());
        }

        /// <summary>
        /// Retrieves a ratio value (numerator and denominator) associated with a key.
        /// </summary>
        /// <param name="attributes">A valid IMFAttributes instance.</param>
        /// <param name="guidKey">Guid that identifies which value to retrieve.</param>
        /// <param name="numerator">The numerator part of the ratio.</param>
        /// <param name="denominator">The denominator part of the ratio.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        /// <remarks>Numerator and denominator values are stored packed as 64-bits value.</remarks>
        public static HResult GetRatio(this IMFAttributes attributes, Guid guidKey, out uint numerator, out uint denominator)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            numerator = 0;
            denominator = 0;

            ulong value;

            HResult hr = attributes.GetUINT64(guidKey, out value);
            if (hr.Succeeded())
            {
                numerator = (uint)(value >> 32);
                denominator = (uint)(value & 0xffffffff);
            }

            return hr;
        }
        
        /// <summary>
        /// Associates a ratio value (numerator and denominator) with a key.
        /// </summary>
        /// <param name="attributes">A valid IMFAttributes instance.</param>
        /// <param name="guidKey">Guid that identifies the value to set.</param>
        /// <param name="numerator">The numerator part of the ratio.</param>
        /// <param name="denominator">The denominator part of the ratio.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        /// <remarks>Numerator and denominator values are stored packed as 64-bits value.</remarks>
        public static HResult SetRatio(this IMFAttributes attributes, Guid guidKey, uint numerator, uint denominator)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            return attributes.SetUINT64(guidKey, ((ulong)numerator << 32) | denominator);
        }


        /// <summary>
        /// Retrieves a size value (width and height) associated with a key.
        /// </summary>
        /// <param name="attributes">A valid IMFAttributes instance.</param>
        /// <param name="guidKey">Guid that identifies which value to retrieve.</param>
        /// <param name="width">The width part of the size.</param>
        /// <param name="height">The height part of the size.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        /// <remarks>Width and height values are stored packed as 64-bits value.</remarks>
        public static HResult GetSize(this IMFAttributes attributes, Guid guidKey, out uint width, out uint height)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            width = 0;
            height = 0;

            ulong value;

            HResult hr = attributes.GetUINT64(guidKey, out value);
            if (hr.Succeeded())
            {
                width = (uint)(value >> 32);
                height = (uint)(value & 0xffffffff);
            }

            return hr;
        }

        /// <summary>
        /// Associates a Ratio value with a key.
        /// </summary>
        /// <param name="attributes">A valid IMFAttributes instance.</param>
        /// <param name="guidKey">Guid that identifies the value to set.</param>
        /// <param name="width">The width part of the size.</param>
        /// <param name="height">The height part of the size.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        /// <remarks>Width and height values are stored packed as 64-bits value.</remarks>
        public static HResult SetSize(this IMFAttributes attributes, Guid guidKey, uint width, uint height)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            return attributes.SetUINT64(guidKey, ((ulong)width << 32) | height);
        }

        /// <summary>
        /// Retrieves a string associated with a key.
        /// </summary>
        /// <param name="attributes">A valid IMFAttributes instance.</param>
        /// <param name="guidKey">Guid that identifies which value to retrieve.</param>
        /// <param name="value">Receives the string.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetString(this IMFAttributes attributes, Guid guidKey, out string value)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            value = null;

            int stringLength;

            HResult hr = attributes.GetStringLength(guidKey, out stringLength);
            if (hr.Failed()) return hr;

            StringBuilder sb = new StringBuilder(stringLength + 1);

            hr = attributes.GetString(guidKey, sb, sb.Capacity, out stringLength);
            if (hr.Failed()) return hr;

            value = sb.ToString();

            return hr;
        }

        /// <summary>
        /// Retrieves a string associated with a key.
        /// </summary>
        /// <param name="attributes">A valid IMFAttributes instance.</param>
        /// <param name="guidKey">Guid that identifies which value to retrieve.</param>
        /// <param name="value">Receives the string.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetAllocatedString(this IMFAttributes attributes, Guid guidKey, out string value)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            int stringLength;

            return attributes.GetAllocatedString(guidKey, out value, out stringLength);
        }

        /// <summary>
        /// Retrieves a byte array associated with a key.
        /// </summary>
        /// <param name="attributes">A valid IMFAttributes instance.</param>
        /// <param name="guidKey">Guid that identifies which value to retrieve.</param>
        /// <param name="value">Receives the byte array.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetAllocatedBlob(this IMFAttributes attributes, Guid guidKey, out byte[] value)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            IntPtr resultPtr;
            int resultLength;

            HResult hr = attributes.GetAllocatedBlob(guidKey, out resultPtr, out resultLength);
            if (hr.Succeeded())
            {
                try
                {
                    value = new byte[resultLength];
                    Marshal.Copy(resultPtr, value, 0, resultLength);
                }
                finally
                {
                    Marshal.FreeCoTaskMem(resultPtr);
                }
            }
            else
            {
                value = null;
            }

            return hr;
        }

        /// <summary>
        /// Retrieves a <see cref="MemoryBuffer"/>  associated with a key.
        /// </summary>
        /// <param name="attributes">A valid IMFAttributes instance.</param>
        /// <param name="guidKey">Guid that identifies which value to retrieve.</param>
        /// <param name="value">Receives a MemoryBuffer wrapping the attribute content.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetAllocatedBlob(this IMFAttributes attributes, Guid guidKey, out MemoryBuffer value)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            IntPtr resultPtr;
            int resultLength;

            HResult hr = attributes.GetAllocatedBlob(guidKey, out resultPtr, out resultLength);
            value = hr.Succeeded() ? new MemoryBuffer(resultPtr, (uint)resultLength) : null;

            return hr;
        }


        /// <summary>
        /// Retrieves a COM instance associated with a key.
        /// </summary>
        /// <typeparam name="T">The requested COM interface.</typeparam>
        /// <param name="attributes">A valid IMFAttributes instance.</param>
        /// <param name="guidKey">Guid that identifies which value to retrieve.</param>
        /// <param name="obj">Receives the requested COM instance.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetUnknown<T>(this IMFAttributes attributes, Guid guidKey, out T obj) where T : class
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            Type typeOfT = typeof(T);

            if (!typeOfT.IsComInterface())
                throw new ArgumentOutOfRangeException("T", "T must be a COM visible interface.");

            object tmp;

            HResult hr = attributes.GetUnknown(guidKey, typeOfT.GUID, out tmp);
            obj = hr.Succeeded() ? tmp as T : null;

            return hr;
        }

        /// <summary>
        /// Writes the contents of an attribute store to a COM stream.
        /// </summary>
        /// <param name="attributes">A valid IMFAttributes instance.</param>
        /// <param name="stream">A COM stream where the attributes are write on.</param>
        /// <param name="options">On or more values of the MFAttributeSerializeOptions enumaration.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult SerializeAttributesToStream(this IMFAttributes attributes, IStream stream, MFAttributeSerializeOptions options = MFAttributeSerializeOptions.None)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            if (stream == null)
                throw new ArgumentNullException("stream");

            return MFExtern.MFSerializeAttributesToStream(attributes, options, stream);
        }

        /// <summary>
        /// Writes the contents of an attribute store to a managed stream.
        /// </summary>
        /// <param name="attributes">A valid IMFAttributes instance.</param>
        /// <param name="stream">A managed stream where the attributes are write on.</param>
        /// <param name="options">On or more values of the MFAttributeSerializeOptions enumaration.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult SerializeAttributesToStream(this IMFAttributes attributes, Stream stream, MFAttributeSerializeOptions options = MFAttributeSerializeOptions.None)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            if (stream == null)
                throw new ArgumentNullException("stream");

            if (!stream.CanWrite)
                throw new ArgumentException("stream must be writable.", "stream");

            using (ComCallableStream comStream = new ComCallableStream(stream, false))
            {
                return MFExtern.MFSerializeAttributesToStream(attributes, options, comStream);
            }
        }

        /// <summary>
        /// Load an attribute store from a COM stream.
        /// </summary>
        /// <param name="attributes">A valid IMFAttributes instance.</param>
        /// <param name="stream">A COM stream where the attributes are read from.</param>
        /// <param name="options">On or more values of the MFAttributeSerializeOptions enumaration.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult DeserializeAttributesFromStream(this IMFAttributes attributes, IStream stream, MFAttributeSerializeOptions options = MFAttributeSerializeOptions.None)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            if (stream == null)
                throw new ArgumentNullException("stream");

            return MFExtern.MFDeserializeAttributesFromStream(attributes, options, stream);
        }

        /// <summary>
        /// Load an attribute store from a managed stream.
        /// </summary>
        /// <param name="attributes">A valid IMFAttributes instance.</param>
        /// <param name="stream">A managed stream where the attributes are read from.</param>
        /// <param name="options">On or more values of the MFAttributeSerializeOptions enumaration.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult DeserializeAttributesFromStream(this IMFAttributes attributes, Stream stream, MFAttributeSerializeOptions options = MFAttributeSerializeOptions.None)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            if (stream == null)
                throw new ArgumentNullException("stream");

            if (!stream.CanRead)
                throw new ArgumentException("stream must be readable.", "stream");

            using (ComCallableStream comStream = new ComCallableStream(stream, false))
            {
                return MFExtern.MFDeserializeAttributesFromStream(attributes, options, comStream);
            }
        }

        /// <summary>
        /// Converts the attribute store's contents to a memory buffer.
        /// </summary>
        /// <param name="attributes">A valid IMFAttributes instance.</param>
        /// <param name="buffer">Receives a memory buffer filled with attribute data.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetAttributesAsBlob(this IMFAttributes attributes, out MemoryBuffer buffer)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            buffer = null;
            int sizeInBytes;

            HResult hr = MFExtern.MFGetAttributesAsBlobSize(attributes, out sizeInBytes);
            if (hr.Succeeded())
            {
                MemoryBuffer result = new MemoryBuffer((uint)sizeInBytes);

                hr = MFExtern.MFGetAttributesAsBlob(attributes, result.BufferPointer, (int)result.ByteLength);
                if (hr.Succeeded())
                    buffer = result;
                else
                    result.Dispose();
            }

            return hr;
        }

        /// <summary>
        /// Converts the attribute store's contents to a byte array.
        /// </summary>
        /// <param name="attributes">A valid IMFAttributes instance.</param>
        /// <param name="buffer">Receives a byte array filled with attribute data.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetAttributesAsBlob(this IMFAttributes attributes, out byte[] buffer)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            buffer = null;
            int sizeInBytes;

            HResult hr = MFExtern.MFGetAttributesAsBlobSize(attributes, out sizeInBytes);
            if (hr.Succeeded())
            {
                byte[] result = new byte[sizeInBytes];

                using (GCPin pin = new GCPin(result))
                {
                    hr = MFExtern.MFGetAttributesAsBlob(attributes, pin.PinnedAddress, result.Length);
                    if (hr.Succeeded())
                        buffer = result;
                }
            }

            return hr;
        }

        /// <summary>
        /// Initializes the attribute store's contents from a memory buffer.
        /// </summary>
        /// <param name="attributes">A valid IMFAttributes instance.</param>
        /// <param name="buffer">A memory buffer previously filled with <see cref="GetAttributesAsBlob"/>.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult InitializeAttributesFromBlob(this IMFAttributes attributes, MemoryBuffer buffer)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            if (buffer == null)
                throw new ArgumentNullException("buffer");

            return MFExtern.MFInitAttributesFromBlob(attributes, buffer.BufferPointer, (int)buffer.ByteLength);
        }

        /// <summary>
        /// Initializes the contents of an attribute store from a byte array.
        /// </summary>
        /// <param name="attributes">A valid IMFAttributes instance.</param>
        /// <param name="buffer">A byte array previously filled with <see cref="GetAttributesAsBlob"/>.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult InitializeAttributesFromBlob(this IMFAttributes attributes, byte[] buffer)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            if (buffer == null)
                throw new ArgumentNullException("buffer");

            using (GCPin pin = new GCPin(buffer))
            {
                return MFExtern.MFInitAttributesFromBlob(attributes, pin.PinnedAddress, buffer.Length);
            }
        }
    }
}
