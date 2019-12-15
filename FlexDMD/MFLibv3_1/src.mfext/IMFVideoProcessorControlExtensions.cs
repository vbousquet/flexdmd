/* license

IMFVideoProcessorControlExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;
using System.Drawing;

using MediaFoundation.Misc;

namespace MediaFoundation
{
    public static class IMFVideoProcessorControlExtensions
    {
        public static HResult SetBorderColor(this IMFVideoProcessorControl videoProcessorControl, Color borderColor)
        {
            if (videoProcessorControl == null)
                throw new ArgumentNullException("videoProcessorControl");

            return videoProcessorControl.SetBorderColor(new MFARGB() { rgbRed = borderColor.R, rgbGreen = borderColor.G, rgbBlue = borderColor.B, rgbAlpha = borderColor.A });
        }

        public static HResult SetSourceRectangle(this IMFVideoProcessorControl videoProcessorControl, Rectangle sourceRectangle)
        {
            if (videoProcessorControl == null)
                throw new ArgumentNullException("videoProcessorControl");

            return videoProcessorControl.SetSourceRectangle(new MFRect(sourceRectangle));
        }

        public static HResult SetDestinationRectangle(this IMFVideoProcessorControl videoProcessorControl, Rectangle destinationRectangle)
        {
            if (videoProcessorControl == null)
                throw new ArgumentNullException("videoProcessorControl");

            return videoProcessorControl.SetDestinationRectangle(new MFRect(destinationRectangle));
        }

        public static HResult SetConstrictionSize(this IMFVideoProcessorControl videoProcessorControl, Size constrictionSize)
        {
            if (videoProcessorControl == null)
                throw new ArgumentNullException("videoProcessorControl");

            return videoProcessorControl.SetConstrictionSize(new MFSize(constrictionSize.Width, constrictionSize.Height));
        }
    }
}
