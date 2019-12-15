/* license

IMFByteStreamExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using MediaFoundation.Interop;
using MediaFoundation.Misc;

namespace MediaFoundation
{
    public static class IMFByteStreamExtensions
    {
        /// <summary>
        /// Reads data from a byte stream
        /// </summary>
        /// <param name="byteStream">A valid IMFByteStream instance.</param>
        /// <param name="buffer">A byte array that receives the data.</param>
        /// <param name="offset">An offset from the start of the buffer at which to begin storing the data read from the byte stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the byte stream.</param>
        /// <param name="bytesRead">The total number of bytes read into the buffer.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult Read(this IMFByteStream byteStream, byte[] buffer, int offset, int count, out int bytesRead)
        {
            return IMFByteStreamExtensions.Read<byte>(byteStream, buffer, offset, count, out bytesRead);
        }

        /// <summary>
        /// Reads data from a byte stream
        /// </summary>
        /// <typeparam name="T">A value type.</typeparam>
        /// <param name="byteStream">A valid IMFByteStream instance.</param>
        /// <param name="buffer">An array of T that receives the data.</param>
        /// <param name="offset">An index offset in buffer at which to begin storing the data read from the byte stream.</param>
        /// <param name="count">The maximum number of items to be read from the byte stream.</param>
        /// <param name="itemsRead">The total number of values read into the buffer.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult Read<T>(this IMFByteStream byteStream, T[] buffer, int offset, int count, out int itemsRead) where T : struct
        {
            if (buffer == null)
                throw new ArgumentNullException("value");

            if (offset > buffer.Length)
                throw new ArgumentNullException("offset");

            if ((offset + count) > buffer.Length)
                throw new ArgumentOutOfRangeException("count");

            ArraySegment<T> arraySegment = new ArraySegment<T>(buffer, offset, count);
            return IMFByteStreamExtensions.Read<T>(byteStream, arraySegment, out itemsRead);
        }

        /// <summary>
        /// Reads data from a byte stream
        /// </summary>
        /// <typeparam name="T">A value type.</typeparam>
        /// <param name="byteStream">A valid IMFByteStream instance.</param>
        /// <param name="buffer">An array segment of value types that receives the data.</param>
        /// <param name="itemsRead">The total number of values read into the array segment.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult Read<T>(this IMFByteStream byteStream, ArraySegment<T> buffer, out int itemsRead) where T : struct
        {
            if (byteStream == null)
                throw new ArgumentNullException("byteStream");

            int sizeOfT = Marshal.SizeOf(typeof(T));

            using (GCPin pin = new GCPin(buffer.Array))
            {
                IntPtr startAddress = pin.PinnedAddress + (buffer.Offset * sizeOfT);

                HResult hr = byteStream.Read(startAddress, buffer.Count * sizeOfT, out itemsRead);
                itemsRead /= sizeOfT;
                return hr;
            }
        }

        /// <summary>
        /// Reads data from a byte stream
        /// </summary>
        /// <typeparam name="T">A value type.</typeparam>
        /// <param name="byteStream">A valid IMFByteStream instance.</param>
        /// <param name="structure">Receives a structure read from the streamIndex.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult Read<T>(this IMFByteStream byteStream, out T structure) where T : struct
        {
            if (byteStream == null)
                throw new ArgumentNullException("byteStream");

            Type typeOfT = typeof(T);
            int sizeOfT = Marshal.SizeOf(typeOfT);

            unsafe
            {
                void* structPtr = stackalloc byte[sizeOfT];
                int bytesRead;

                HResult hr = byteStream.Read((IntPtr)structPtr, sizeOfT, out bytesRead);

                Debug.Assert(bytesRead == sizeOfT);
                structure = (T)Marshal.PtrToStructure((IntPtr)structPtr, typeOfT);

                return hr;
            }
        }

        /// <summary>
        /// Writes data to a byte stream.
        /// </summary>
        /// <param name="byteStream">A valid IMFByteStream instance.</param>
        /// <param name="buffer">A byte array that contains the data to write.</param>
        /// <param name="offset">An offset from the start of the buffer at which to begin reading the data written to the byte stream.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <param name="bytesWritten">The total number of bytes write into the buffer.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult Write(this IMFByteStream byteStream, byte[] buffer, int offset, int count, out int bytesWritten)
        {
            return IMFByteStreamExtensions.Write<byte>(byteStream, buffer, offset, count, out bytesWritten);
        }

        /// <summary>
        /// Writes data to a byte stream.
        /// </summary>
        /// <typeparam name="T">A value type.</typeparam>
        /// <param name="byteStream">A valid IMFByteStream instance.</param>
        /// <param name="buffer">An array of T that contains the data to write.</param>
        /// <param name="offset">An offset from the start of the buffer at which to begin reading the data written to the byte stream.</param>
        /// <param name="count">The number of items to write.</param>
        /// <param name="itemsWritten">Receives the number of items written.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult Write<T>(this IMFByteStream byteStream, T[] buffer, int offset, int count, out int itemsWritten) where T : struct
        {
            if (buffer == null)
                throw new ArgumentNullException("value");

            if (count > buffer.Length)
                throw new ArgumentOutOfRangeException("count");

            ArraySegment<T> arraySegment = new ArraySegment<T>(buffer, offset, count);
            return IMFByteStreamExtensions.Write<T>(byteStream, arraySegment, out itemsWritten);
        }

        /// <summary>
        /// Writes data to a byte stream.
        /// </summary>
        /// <typeparam name="T">A value type.</typeparam>
        /// <param name="byteStream">A valid IMFByteStream instance.</param>
        /// <param name="buffer">A value type array segment that contains the data to write.</param></param>
        /// <param name="itemsWritten">Receives the number of items bytesRead.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult Write<T>(this IMFByteStream byteStream, ArraySegment<T> buffer, out int itemsWritten) where T : struct
        {
            if (byteStream == null)
                throw new ArgumentNullException("byteStream");

            int sizeOfT = Marshal.SizeOf(typeof(T));

            using (GCPin pin = new GCPin(buffer.Array))
            {
                IntPtr startAddress = pin.PinnedAddress + buffer.Offset;

                HResult hr = byteStream.Write(startAddress, buffer.Count * sizeOfT, out itemsWritten);
                itemsWritten /= sizeOfT;
                return hr;
            }
        }

        /// <summary>
        /// Writes data to a byte stream.
        /// </summary>
        /// <typeparam name="T">A value type.</typeparam>
        /// <param name="byteStream">A valid IMFByteStream instance.</param>
        /// <param name="structure">A value type to write into the streamIndex.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult Write<T>(this IMFByteStream byteStream, T structure) where T : struct
        {
            if (byteStream == null)
                throw new ArgumentNullException("byteStream");

            int sizeOfT = Marshal.SizeOf(typeof(T));

            unsafe
            {
                void* structPtr = stackalloc byte[sizeOfT];
                int bytesWritten;

                Marshal.StructureToPtr(structure, (IntPtr)structPtr, false);
                
                HResult hr = byteStream.Write((IntPtr)structPtr, sizeOfT, out bytesWritten);
                Debug.Assert(bytesWritten == sizeOfT);

                return hr;
            }
        }

        /// <summary>
        /// Reads asynchronously data from a byte stream
        /// </summary>
        /// <param name="byteStream">A valid IMFByteStream instance.</param>
        /// <param name="buffer">A byte array that receives the data.</param>
        /// <param name="offset">An offset from the start of the buffer at which to begin storing the data read from the byte stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the byte stream.</param>
        /// <returns>A task that represents the asynchronous read operation. The task's result contain the number of bytes read.</returns>
        public static Task<int> ReadAsync(this IMFByteStream byteStream, byte[] buffer, int offset, int count)
        {
            return IMFByteStreamExtensions.ReadAsync<byte>(byteStream, buffer, offset, count);
        }

        /// <summary>
        /// Reads asynchronously data from a byte stream
        /// </summary>
        /// <typeparam name="T">A value type.</typeparam>
        /// <param name="byteStream">A valid IMFByteStream instance.</param>
        /// <param name="buffer">An array of T that receives the data.</param>
        /// <param name="offset">An index offset in buffer at which to begin storing the data read from the byte stream.</param>
        /// <param name="count">The maximum number of items to be read from the byte stream.</param>
        /// <returns>A task that represents the asynchronous read operation. The task's result contain the number of items read.</returns>
        public static Task<int> ReadAsync<T>(this IMFByteStream byteStream, T[] buffer, int offset, int count) where T : struct
        {
            if (buffer == null)
                throw new ArgumentNullException("value");

            if (offset > buffer.Length)
                throw new ArgumentNullException("offset");

            if ((offset + count) > buffer.Length)
                throw new ArgumentOutOfRangeException("count");

            ArraySegment<T> arraySegment = new ArraySegment<T>(buffer, offset, count);
            return IMFByteStreamExtensions.ReadAsync<T>(byteStream, arraySegment);
        }

        /// <summary>
        /// Reads asynchronously data from a byte stream
        /// </summary>
        /// <typeparam name="T">A value type.</typeparam>
        /// <param name="byteStream">A valid IMFByteStream instance.</param>
        /// <param name="buffer">An array segment of value types that receives the data.</param>
        /// <returns>A task that represents the asynchronous read operation. The task's result contain the number of items read.</returns>
        public static Task<int> ReadAsync<T>(this IMFByteStream byteStream, ArraySegment<T> buffer) where T : struct
        {
            if (byteStream == null)
                throw new ArgumentNullException("byteStream");

            int sizeOfT = Marshal.SizeOf(typeof(T));

            // Pin the array to get its addess and to avoid a GC move during the asynchronous operation
            GCPin pin = new GCPin(buffer.Array);

            // Get the start address - ArraySegment.Offset is exprimed in item offset
            IntPtr startAddress = pin.PinnedAddress + (buffer.Offset * sizeOfT);

            // Get the task for a byte pointer operation
            Task<int> result = byteStream.ReadAsync(startAddress, buffer.Count * sizeOfT);

            // return that Task with a continuation to do cleanup and correct the result
            return result.ContinueWith<int>((an) =>
            {
                // Release the GC handle
                pin.Dispose();

                // Result must be exprimed in items count , not in bytes
                return an.Result / sizeOfT;
            });
        }

        /// <summary>
        /// Reads asynchronously data from a byte stream
        /// </summary>
        /// <param name="byteStream">A valid IMFByteStream instance.</param>
        /// <param name="bufferPtr">A pointer designating the memory location that receives the data.</param>
        /// <param name="bufferSize">The maximum number of bytes to read from the byte stream.</param>
        /// <returns>A task that represents the asynchronous read operation. The task's result contain the number of bytes read.</returns>
        public static Task<int> ReadAsync(this IMFByteStream byteStream, IntPtr bufferPtr, int bufferSize)
        {
            if (byteStream == null)
                throw new ArgumentNullException("byteStream");

            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();

            HResult hrBegin = byteStream.BeginRead(bufferPtr, bufferSize, new MFAsyncCallback((ar) =>
            {
                int bytesRead = 0;
                try
                {
                    HResult hrEnd = byteStream.EndRead(ar, out bytesRead);
                    hrEnd.ThrowExceptionOnError();

                    tcs.SetResult(bytesRead);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }

            }), null);
            hrBegin.ThrowExceptionOnError();

            return tcs.Task;
        }

        /// <summary>
        /// Writes asynchronously data to a byte stream
        /// </summary>
        /// <param name="byteStream">A valid IMFByteStream instance.</param>
        /// <param name="buffer">A byte array that contain the data to write.</param>
        /// <param name="offset">An offset from the start of the buffer at which begin the data to write to the byte stream.</param>
        /// <param name="count">The maximum number of bytes to be written to the byte stream.</param>
        /// <returns>A task that represents the asynchronous write operation. The task's result contain the number of bytes written.</returns>
        public static Task<int> WriteAsync(this IMFByteStream byteStream, byte[] buffer, int offset, int count)
        {
            return IMFByteStreamExtensions.WriteAsync<byte>(byteStream, buffer, offset, count);
        }

        /// <summary>
        /// Writes asynchronously data to a byte stream
        /// </summary>
        /// <typeparam name="T">A value type.</typeparam>
        /// <param name="byteStream">A valid IMFByteStream instance.</param>
        /// <param name="buffer">An array of T that contain the data to write.</param>
        /// <param name="offset">An index offset in buffer at which to begin storing the data read from the byte stream.</param>
        /// <param name="count">The maximum number of items to be read from the byte stream.</param>
        /// <returns>A task that represents the asynchronous write operation. The task's result contain the number of items written.</returns>
        public static Task<int> WriteAsync<T>(this IMFByteStream byteStream, T[] buffer, int offset, int count) where T : struct
        {
            if (buffer == null)
                throw new ArgumentNullException("value");

            if (offset > buffer.Length)
                throw new ArgumentNullException("offset");

            if ((offset + count) > buffer.Length)
                throw new ArgumentOutOfRangeException("count");

            ArraySegment<T> arraySegment = new ArraySegment<T>(buffer, offset, count);
            return IMFByteStreamExtensions.WriteAsync<T>(byteStream, arraySegment);
        }

        /// <summary>
        /// Writes asynchronously data to a byte stream
        /// </summary>
        /// <typeparam name="T">A value type.</typeparam>
        /// <param name="byteStream">A valid IMFByteStream instance.</param>
        /// <param name="buffer">An array segment of value types that contain the data to write.</param>
        /// <returns>A task that represents the asynchronous write operation. The task's result contain the number of items written.</returns>
        public static Task<int> WriteAsync<T>(this IMFByteStream byteStream, ArraySegment<T> buffer) where T : struct
        {
            if (byteStream == null)
                throw new ArgumentNullException("byteStream");

            int sizeOfT = Marshal.SizeOf(typeof(T));

            // Pin the array to get its addess and to avoid a GC move during the asynchronous operation
            GCPin pin = new GCPin(buffer.Array);

            // Get the start address - ArraySegment.Offset is exprimed in item offset
            IntPtr startAddress = pin.PinnedAddress + (buffer.Offset * sizeOfT);

            // Get the task for a byte pointer operation
            Task<int> result = byteStream.WriteAsync(startAddress, buffer.Count * sizeOfT);

            // return that Task with a continuation to do cleanup and correct the result
            return result.ContinueWith<int>((an) =>
            {
                // Release the GC handle
                pin.Dispose();

                // Result must be exprimed in items count , not in bytes
                return an.Result / sizeOfT;
            });
        }

        /// <summary>
        /// Writes asynchronously data to a byte stream
        /// </summary>
        /// <param name="byteStream">A valid IMFByteStream instance.</param>
        /// <param name="bufferPtr">A pointer designating the memory location that contain the data to write.</param>
        /// <param name="bufferSize">The maximum number of bytes to write to the byte stream.</param>
        /// <returns>A task that represents the asynchronous write operation. The task's result contain the number of bytes written.</returns>
        public static Task<int> WriteAsync(this IMFByteStream byteStream, IntPtr bufferPtr, int bufferSize)
        {
            if (byteStream == null)
                throw new ArgumentNullException("byteStream");

            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();

            HResult hrBegin = byteStream.BeginWrite(bufferPtr, bufferSize, new MFAsyncCallback((ar) =>
            {
                int bytesWritten = 0;
                try
                {
                    HResult hrEnd = byteStream.EndWrite(ar, out bytesWritten);
                    hrEnd.ThrowExceptionOnError();

                    tcs.SetResult(bytesWritten);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }

            }), null);
            hrBegin.ThrowExceptionOnError();

            return tcs.Task;
        }


    }
}
