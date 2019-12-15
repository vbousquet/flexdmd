/* license

IMFTopologyNodeExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;

using MediaFoundation.Transform;

namespace MediaFoundation
{
    public static class IMFTopologyNodeExtensions
    {
        public static HResult SetObject(this IMFTopologyNode topologyNode, IMFTransform transform)
        {
            if (topologyNode == null)
                throw new ArgumentNullException("topologyNode");

            return topologyNode.SetObject(transform);
        }

        public static HResult SetObject(this IMFTopologyNode topologyNode, IMFActivate activate)
        {
            if (topologyNode == null)
                throw new ArgumentNullException("topologyNode");

            return topologyNode.SetObject(activate);
        }

        public static HResult SetObject(this IMFTopologyNode topologyNode, IMFStreamSink streamSink)
        {
            if (topologyNode == null)
                throw new ArgumentNullException("topologyNode");

            return topologyNode.SetObject(streamSink);
        }

        public static HResult GetObject(this IMFTopologyNode topologyNode, out IMFTransform transform)
        {
            if (topologyNode == null)
                throw new ArgumentNullException("topologyNode");

            object tmp;

            HResult hr = topologyNode.GetObject(out tmp);
            transform = hr.Succeeded() ? tmp as IMFTransform : null;

            return hr;
        }

        public static HResult GetObject(this IMFTopologyNode topologyNode, out IMFActivate activate)
        {
            if (topologyNode == null)
                throw new ArgumentNullException("topologyNode");

            object tmp;

            HResult hr = topologyNode.GetObject(out tmp);
            activate = hr.Succeeded() ? tmp as IMFActivate : null;

            return hr;
        }

        public static HResult GetObject(this IMFTopologyNode topologyNode, out IMFStreamSink streamSink)
        {
            if (topologyNode == null)
                throw new ArgumentNullException("topologyNode");

            object tmp;

            HResult hr = topologyNode.GetObject(out tmp);
            streamSink = hr.Succeeded() ? tmp as IMFStreamSink : null;

            return hr;
        }
    }
}
