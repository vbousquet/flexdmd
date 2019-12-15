/* license

RuntimeCallableStream.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

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
    ///  An helper class that expose a managed Stream around a wrapped IStream COM interface.
    /// </summary>
    public class RuntimeCallableStream : Stream
    {
        private IStream m_comStream;
        private bool m_ownStream;
        private Misc.STATSTG? m_STATSTG = null;

        public RuntimeCallableStream(IStream comStream, bool ownStream = true) : base()
        {
            m_comStream = comStream;
            m_ownStream = ownStream;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { }

            if (m_ownStream && m_comStream != null)
            {
                Marshal.ReleaseComObject(m_comStream);
            }

            m_comStream = null;
        }

        public override bool CanRead
        {
            get { return this.GetCanRead(); }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return this.GetCanWrite(); }
        }

        public override long Length
        {
            get { return this.GetLength(); }
        }

        public override long Position
        {
            get
            {
                return this.Seek(0, SeekOrigin.Current);
            }
            set
            {
                this.Seek(value, SeekOrigin.Current);
            }
        }

        public override void Flush()
        {
            HResult hr = m_comStream.Commit(Misc.STGC.Default);
            hr.ThrowExceptionOnError();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            IntPtr resultPtr = Marshal.AllocCoTaskMem(sizeof(long));
            long result;

            try
            {
                HResult hr = m_comStream.Seek(offset, origin, resultPtr);
                hr.ThrowExceptionOnError();

                result = Marshal.ReadInt64(resultPtr);
            }
            finally
            {
                Marshal.FreeCoTaskMem(resultPtr);
            }

            return result;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("value", "value is null");

            if (count < 0 || offset < 0)
                throw new ArgumentException("offset or count is negative.");

            if (offset + count > buffer.Length)
                throw new IndexOutOfRangeException("The sum of offset and count is larger than the value length.");

            IntPtr byteReadPtr = Marshal.AllocCoTaskMem(sizeof(int));
            int byteRead;

            // If offset is zero, read directly into value
            byte[] tempBuffer = (offset == 0) ? buffer : new byte[count];

            try
            {
                HResult hr = m_comStream.Read(tempBuffer, count, byteReadPtr);
                hr.ThrowExceptionOnError();

                byteRead = Marshal.ReadInt32(byteReadPtr);
            }
            finally
            {
                Marshal.FreeCoTaskMem(byteReadPtr);
            }

            if (offset != 0) Buffer.BlockCopy(tempBuffer, 0, buffer, offset, byteRead);

            return byteRead;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("value", "value is null");

            if (count < 0 || offset < 0)
                throw new ArgumentException("offset or count is negative.");

            if (offset + count > buffer.Length)
                throw new IndexOutOfRangeException("The sum of offset and count is larger than the value length.");

            byte[] tempBuffer;

            // If offset is zero, write directly from value
            if (offset == 0)
            {
                tempBuffer = buffer;
            }
            else
            {
                tempBuffer = new byte[count]; 
                Buffer.BlockCopy(buffer, offset, tempBuffer, 0, count);
            }

            HResult hr = m_comStream.Write(tempBuffer, count, IntPtr.Zero);
            hr.ThrowExceptionOnError();
        }

        private void UpdateCachedSTATSTG()
        {
            Misc.STATSTG stat;

            HResult hr = m_comStream.Stat(out stat, Misc.STATFLAG.NoName);
            hr.ThrowExceptionOnError();

            m_STATSTG = stat;
        }

        private bool GetCanRead()
        {
            if (m_STATSTG == null)
                this.UpdateCachedSTATSTG();

            int accessPart = m_STATSTG.Value.grfMode & 0x00000002;
            return (accessPart == 0 || accessPart == 2); // STGM_READ || STGM_READWRITE
        }

        private bool GetCanWrite()
        {
            if (m_STATSTG == null)
                this.UpdateCachedSTATSTG();

            int accessPart = m_STATSTG.Value.grfMode & 0x00000002;
            return (accessPart == 1 || accessPart == 2); // STGM_WRITE || STGM_READWRITE
        }

        private long GetLength()
        {
            this.UpdateCachedSTATSTG();
            return m_STATSTG.Value.cbSize;
        }

    }
}
