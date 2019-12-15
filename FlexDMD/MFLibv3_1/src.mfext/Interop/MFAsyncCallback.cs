/* license

MFAsyncCallback.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;
using System.Diagnostics;

namespace MediaFoundation.Interop
{
    /// <summary>
    /// An implementation of the IMFAsyncCallback interface that wrap managed delegate
    /// </summary>
    public class MFAsyncCallback : IMFAsyncCallback
    {
        public delegate void OnInvoke(IMFAsyncResult asyncResult);

        private OnInvoke m_onInvokeMethod;

        public MFAsyncCallback(OnInvoke onInvokeMethod)
        {
            if (onInvokeMethod == null)
                throw new ArgumentNullException("onInvokeMethod");

            m_onInvokeMethod = onInvokeMethod;
        }

        #region IMFAsyncCallback Members

        public HResult GetParameters(out MFASync pdwFlags, out MFAsyncCallbackQueue pdwQueue)
        {
            pdwFlags = 0;
            pdwQueue = 0;

            return HResult.E_NOTIMPL;
        }

        public HResult Invoke(IMFAsyncResult pAsyncResult)
        {
            m_onInvokeMethod(pAsyncResult);
            return HResult.S_OK;
        }

        #endregion
    }
}
