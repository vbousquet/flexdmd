/* license

Enums.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;

namespace MediaFoundation
{
    public enum CaptureEngineStreams
    {
        /// <summary>The first image streamIndex.</summary>
        FirstSourcePhotoStream = unchecked((int)0xfffffffb),
        /// <summary>The first video streamIndex.</summary>
        FirstSourceVideoStream = unchecked((int)0xfffffffc),
        /// <summary>The first audio streamIndex.</summary>
        FirstSourceAudioStream = unchecked((int)0xfffffffd),
    }

    public enum CaptureEngineFriendlyName
    {
        /// <summary>The first image streamIndex.</summary>
        FirstSourcePhotoStream = unchecked((int)0xfffffffb),
        /// <summary>The first video streamIndex.</summary>
        FirstSourceVideoStream = unchecked((int)0xfffffffc),
        /// <summary>The first audio streamIndex.</summary>
        FirstSourceAudioStream = unchecked((int)0xfffffffd),
/*
        /// <summary>The prefered video streamIndex for recording.</summary>
        PreferredSourceVideoStreamForRecord = unchecked((int)0xffffffff), // Unknown value
        /// <summary>The prefered video streamIndex for preview.</summary>
        PreferredSourceVideoStreamForPreview = unchecked((int)0xffffffff), // Unknown value
        /// <summary>The first independent image streamIndex.</summary>
        FirstSourceIndependentPhotoStream = unchecked((int)0xffffffff), // Unknown value
*/
    }

    public enum SourceReaderStreams
    {
        /// <summary>The first video streamIndex.</summary>
        FirstVideoStream = unchecked((int)0xfffffffc),
        /// <summary>The first audio streamIndex.</summary>
        FirstAudioStream = unchecked((int)0xfffffffd),
        /// <summary>The first image streamIndex.</summary>
        AllStreams = unchecked((int)0xfffffffe),
    }

    public enum SourceReaderFirstStream
    {
        /// <summary>The first video streamIndex.</summary>
        FirstVideoStream = unchecked((int)0xfffffffc),
        /// <summary>The first audio streamIndex.</summary>
        FirstAudioStream = unchecked((int)0xfffffffd),
    }

    public enum SourceReaderFirstStreamOrMediaSource
    {
        /// <summary>The first video streamIndex.</summary>
        FirstVideoStream = unchecked((int)0xfffffffc),
        /// <summary>The first audio streamIndex.</summary>
        FirstAudioStream = unchecked((int)0xfffffffd),
        /// <summary>The media source.</summary>
        MediaSource = unchecked((int)0xffffffff),
    }

}
