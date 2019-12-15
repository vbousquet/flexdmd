/* license

ComCallableStream.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;
using System.IO;
using System.Runtime.InteropServices;

using MediaFoundation.Misc;

namespace MediaFoundation.Interop
{
    /// <summary>
    /// An helper class that expose the COM interface IStream around a wrapped managed Stream.
    /// </summary>
    public class ComCallableStream : IStream, IDisposable
    {
        private Stream m_stream;
        private bool m_ownStream;

        /// <summary>
        /// Create an instance of ComCallableStream.
        /// </summary>
        /// <param name="streamIndex">A managed <see cref="System.IO.Stream"/> used by this instance to simulate the COM interface IStream.</param>
        /// <param name="ownStream">Indicate if the wrapper own the managed streamIndex. If true, the managed streamIndex is disposed with the wrapper instance.</param>
        /// <remarks>After the object creation, the managed Stream is accessible using the <see cref="BaseStream"/> property.</remarks>
        public ComCallableStream(Stream stream, bool ownStream = true)
        {
            if (stream == null)
                throw new ArgumentNullException("streamIndex");

            m_stream = stream;
            m_ownStream = ownStream;
        }

        /// <summary>
        /// Give access to the wrapped managed <see cref="System.IO.Stream"/>.
        /// </summary>
        public Stream BaseStream
        {
            get { return m_stream; }
        }

        /// <summary>
        /// Dispose the wrapped managed <see cref="System.IO.Stream"/>.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the wrapped managed <see cref="System.IO.Stream"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_stream != null && m_ownStream)
                {
                    m_stream.Dispose();
                }

                m_stream = null;
            }
        }

        /// <summary>
        /// The Read method reads a specified number of bytes from the streamIndex object into memory, starting at the current seek pointer.
        /// </summary>
        /// <param name="value">A byte array which the streamIndex data is read into.</param>
        /// <param name="bytesCount">The number of bytes of data to read from the streamIndex object.</param>
        /// <param name="itemsRead">A pointer to a <see cref="System.UInt32"/> variable that receives the actual number of bytes read from the streamIndex object.</param>
        /// <returns>An HRESULT.</returns>
        public HResult Read(byte[] buffer, int bytesCount, IntPtr bytesRead)
        {
            if (m_stream == null)
                return HResult.STG_E_REVERTED;

            if (buffer == null)
                return HResult.STG_E_INVALIDPOINTER;

            if (!m_stream.CanRead)
                return HResult.STG_E_ACCESSDENIED;

            if (bytesCount == 0)
                return HResult.S_OK;

            int read = m_stream.Read(buffer, 0, bytesCount);

            if (bytesRead != IntPtr.Zero)
                Marshal.WriteInt32(bytesRead, read);

            if (read < bytesCount)
                return HResult.S_FALSE;

            return HResult.S_OK;
        }

        /// <summary>
        /// The Write method writes a specified number of bytes into the streamIndex object starting at the current seek pointer.
        /// </summary>
        /// <param name="value">A byte array that contains the data that is to be itemsRead to the streamIndex. A valid array must be provided for this parameter even when <paramref name="bytesCount"/> is zero.</param>
        /// <param name="bytesCount">The number of bytes of data to attempt to write into the streamIndex. This value can be zero.</param>
        /// <param name="itemsWritten">A pointer to a <see cref="System.UInt32"/> variable where this method writes the actual number of bytes itemsRead to the streamIndex object. The caller can set this pointer to <see cref="System.IntPtr.Zero"/>, in which case this method does not provide the actual number of bytes itemsRead.</param>
        /// <returns>An HRESULT.</returns>
        public HResult Write(byte[] buffer, int bytesCount, IntPtr bytesWritten)
        {
            if (m_stream == null)
                return HResult.STG_E_REVERTED;

            if (buffer == null)
                return HResult.STG_E_INVALIDPOINTER;

            if (!m_stream.CanWrite)
                return HResult.STG_E_ACCESSDENIED;

            if (bytesCount == 0)
                return HResult.S_OK;

            long beforePos = m_stream.Position;
            m_stream.Write(buffer, 0, bytesCount);
            int written = (int)(m_stream.Position - beforePos);

            if (bytesWritten != IntPtr.Zero)
                Marshal.WriteInt32(bytesWritten, written);

            if (written < bytesCount)
                return HResult.S_FALSE; 

            return HResult.S_OK;
        }

        /// <summary>
        /// The Seek method changes the seek pointer to a new location. The new location is relative to either the beginning of the streamIndex, the end of the streamIndex, or the current seek pointer.
        /// </summary>
        /// <param name="offset">The displacement to be added to the location indicated by the <paramref name="origin"/> parameter. If <paramref name="origin"/> is <see cref="System.IO.SeekOrigin.Begin"/>, this is interpreted as an unsigned value rather than a signed value.</param>
        /// <param name="origin">The origin for the displacement specified in <paramref name="offset"/>. The origin can be the beginning of the file (<see cref="System.IO.SeekOrigin.Begin"/>), the current seek pointer (<see cref="System.IO.SeekOrigin.Current"/>), or the end of the file (<see cref="System.IO.SeekOrigin.End"/>). For more information about values, see the <see cref="System.IO.SeekOrigin"/> enumeration.</param>
        /// <param name="newPosition">A pointer to the location where this method writes the value of the new seek pointer from the beginning of the streamIndex. You can set this pointer to <see cref="System.IntPtr.Zero"/>. In this case, this method does not provide the new seek pointer.</param>
        /// <returns>An HRESULT.</returns>
        public HResult Seek(long offset, System.IO.SeekOrigin origin, IntPtr newPosition)
        {
            if (m_stream == null)
                return HResult.STG_E_REVERTED;

            if (!m_stream.CanSeek)
                return HResult.STG_E_INVALIDFUNCTION;

            if ((origin != SeekOrigin.Begin) && (origin != SeekOrigin.Current) && (origin != SeekOrigin.End))
                return HResult.STG_E_INVALIDFUNCTION;

            long newPos = m_stream.Seek(offset, (SeekOrigin)origin);

            if (newPosition != IntPtr.Zero)
                Marshal.WriteInt64(newPosition, newPos);

            return HResult.S_OK;
        }

        /// <summary>
        /// The SetSize method changes the size of the streamIndex object.
        /// </summary>
        /// <param name="newSize">Specifies the new size, in bytes, of the streamIndex.</param>
        /// <returns>An HRESULT.</returns>
        public HResult SetSize(long newSize)
        {
            if (m_stream == null)
                return HResult.STG_E_REVERTED;

            try
            {
                m_stream.SetLength(newSize);
            }
            catch (IOException)
            {
                return HResult.STG_E_INVALIDFUNCTION;
            }

            return HResult.S_OK;
        }

        /// <summary>
        /// The CopyTo method copies a specified number of bytes from the current seek pointer in the streamIndex to the current seek pointer in another streamIndex.
        /// </summary>
        /// <param name="otherStream">A pointer to the destination streamIndex. The streamIndex pointed to by <paramref name="otherStream"/> can be a new streamIndex or a clone of the source streamIndex.</param>
        /// <param name="bytesCount">The number of bytes to copy from the source streamIndex.</param>
        /// <param name="itemsRead">A pointer to the location where this method writes the actual number of bytes read from the source. You can set this pointer to <see cref="System.IntPtr.Zero"/>. In this case, this method does not provide the actual number of bytes read.</param>
        /// <param name="itemsWritten">A pointer to the location where this method writes the actual number of bytes itemsRead to the destination. You can set this pointer to <see cref="System.IntPtr.Zero"/>. In this case, this method does not provide the actual number of bytes itemsRead.</param>
        /// <returns>An HRESULT.</returns>
        public HResult CopyTo(IStream otherStream, long bytesCount, IntPtr bytesRead, IntPtr bytesWritten)
        {
            if (m_stream == null)
                return HResult.STG_E_REVERTED;

            if (otherStream == null)
                return HResult.STG_E_INVALIDPOINTER;

            if (!m_stream.CanRead)
                return HResult.STG_E_ACCESSDENIED;

            byte[] buffer = new byte[bytesCount];

            int read = m_stream.Read(buffer, 0, (int)bytesCount);

            int written = 0;

            unsafe
            {
                otherStream.Write(buffer, read, new IntPtr(&written));
            }

            if (bytesRead != IntPtr.Zero)
                Marshal.WriteInt64(bytesRead, read);

            if (bytesWritten != IntPtr.Zero)
                Marshal.WriteInt64(bytesWritten, written);

            return HResult.S_OK;
        }

        /// <summary>
        /// The Commit method ensures that any changes made to a streamIndex object open in transacted mode are reflected in the parent storage.
        /// </summary>
        /// <param name="commitFlags">Controls how the changes for the streamIndex object are committed. See the STGC enumeration for a definition of these values.</param>
        /// <returns>An HRESULT.</returns>
        /// <remarks>Since transacted IO are not supported by managed streams, this method just call <see cref="System.IO.Stream.Flush"/>.</remarks>
        public HResult Commit(Misc.STGC commitFlags)
        {
            if (m_stream == null)
                return HResult.STG_E_REVERTED;

            m_stream.Flush();

            return HResult.S_OK;
        }

        /// <summary>
        /// The Revert method discards all changes that have been made to a transacted streamIndex since the last IStream::Commit call.
        /// </summary>
        /// <returns>Always return HResult.S_OK.</returns>
        /// <remarks>Since transacted IO are not supported by managed streams, this method do nothing.</remarks>
        public HResult Revert()
        {
            if (m_stream == null)
                return HResult.STG_E_REVERTED;

            return HResult.S_OK;
        }

        /// <summary>
        /// The LockRegion method restricts access to a specified range of bytes in the streamIndex. Supporting this functionality is optional since some file systems do not provide it.
        /// </summary>
        /// <param name="offset">Integer that specifies the byte offset for the beginning of the range.</param>
        /// <param name="bytesCount">Integer that specifies the length of the range, in bytes, to be restricted.</param>
        /// <param name="lockType">Specifies the restrictions being requested on accessing the range. This parameter is ignored by this implementation.</param>
        /// <returns>An HRESULT.</returns>
        /// <remarks>This method only works if the wrapped streamIndex is a <see cref="System.IO.FileStream"/>. For other Stream kinds, the method tell to the caller that locking is not supported.</remarks>
        public HResult LockRegion(long offset, long bytesCount, int lockType)
        {
            if (m_stream == null)
                return HResult.STG_E_REVERTED;

            FileStream fileStream = m_stream as FileStream;
            if (fileStream == null)
                return HResult.STG_E_INVALIDFUNCTION;

            try
            {
                fileStream.Lock(offset, bytesCount);
            }
            catch (IOException)
            {
                return HResult.STG_E_LOCKVIOLATION;
            }

            return HResult.S_OK;
        }

        /// <summary>
        /// The UnlockRegion method removes the access restriction on a range of bytes previously restricted with <see cref="LockRegion"/>.
        /// </summary>
        /// <param name="offset">Specifies the byte offset for the beginning of the range.</param>
        /// <param name="bytesCount">Specifies, in bytes, the length of the range to be restricted.</param>
        /// <param name="lockType">Specifies the access restrictions previously placed on the range. This parameter is ignored by this implementation.</param>
        /// <returns>An HRESULT.</returns>
        /// <remarks>This method only works if the wrapped streamIndex is a <see cref="System.IO.FileStream"/>. For other Stream kinds, the method tell to the caller that locking is not supported.</remarks>
        public HResult UnlockRegion(long offset, long bytesCount, int lockType)
        {
            if (m_stream == null)
                return HResult.STG_E_REVERTED;

            FileStream fileStream = m_stream as FileStream;
            if (fileStream == null)
                return HResult.STG_E_INVALIDFUNCTION;

            try
            {
                fileStream.Unlock(offset, bytesCount);
            }
            catch (IOException)
            {
                return HResult.STG_E_LOCKVIOLATION;
            }

            return HResult.S_OK;
        }

        /// <summary>
        /// The Stat method retrieves the <see cref="System.Runtime.InteropServices.ComTypes.STATSTG"/> structure for this streamIndex.
        /// </summary>
        /// <param name="statstg">A <see cref="System.Runtime.InteropServices.ComTypes.STATSTG"/> structure where this method places information about this streamIndex object.</param>
        /// <param name="statFlag">Specifies that this method does not return some of the members in the STATSTG structure, thus saving a memory allocation operation. Values are taken from the <see cref="DirectShowLib2.ComTypes.STATFLAG"/> enumeration.</param>
        /// <returns>Always return HResult.E_NOTIMPL.</returns>
        /// <remarks>In this implementation, this method is unimplemented.</remarks>
        public HResult Stat(out Misc.STATSTG statstg, Misc.STATFLAG statFlag)
        {
            statstg = new MediaFoundation.Misc.STATSTG();

            statstg.type = MediaFoundation.Misc.STGTY.Stream;
            statstg.cbSize = m_stream.Length;

            if (m_stream.CanRead && m_stream.CanWrite)
                statstg.grfMode = 0x00000002; // STGM_READWRITE
            else if (m_stream.CanRead)
                statstg.grfMode = 0x00000000; // STGM_READ
            else if (m_stream.CanWrite)
                statstg.grfMode = 0x00000001; // STGM_WRITE

            if (statFlag == Misc.STATFLAG.Default)
            {
                FileStream baseStream = m_stream as FileStream;
                if (baseStream != null)
                {
                    statstg.pwcsName = Marshal.StringToCoTaskMemAuto(Path.GetFileName(baseStream.Name));
                }
            }

            return HResult.S_OK;
        }

        /// <summary>
        /// The Clone method creates a new streamIndex object with its own seek pointer that references the same bytes as the original streamIndex.
        /// </summary>
        /// <param name="clonedStream">When successful, pointer to the location of an IStream instance to the new streamIndex object. If an error occurs, this parameter is null.</param>
        /// <returns>Always return HResult.E_NOTIMPL.</returns>
        /// <remarks>In this implementation, this method is unimplemented.</remarks>
        public HResult Clone(out IStream clonedStream)
        {
            clonedStream = null;
            return HResult.E_NOTIMPL;
        }
    }
}
