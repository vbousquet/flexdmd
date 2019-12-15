/* license

MFP.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;

namespace MediaFoundation.MFPlayer
{
    public static class MFP
    {
        /// <summary>
        /// Creates a new instance of the MFPlay player object.
        /// </summary>
        /// <param name="url">A string that contains the URL of a media file to open.</param>
        /// <param name="startPlayback">If true, playback starts automatically. If false, playback does not start until the application calls IMFMediaPlayer.Play.</param>
        /// <param name="creationOptions">Bitwise OR of zero of more flags from the MFP_CREATION_OPTIONS enumeration.</param>
        /// <param name="callback">An instance of the IMFPMediaPlayerCallback interface of a callback object, implemented by the application.</param>
        /// <param name="hWnd">A handle to a window where the video will appear. For audio-only playback, this parameter can be IntPtr.Zero.</param>
        /// <param name="mediaPlayer">Receives an instance of to the IMFPMediaPlayer interface.</param>
        /// <returns></returns>
        public static HResult CreateMediaPlayer(string url, bool startPlayback, MFP_CREATION_OPTIONS creationOptions, IMFPMediaPlayerCallback callback, IntPtr hWnd, out IMFPMediaPlayer mediaPlayer)
        {
            return MFExtern.MFPCreateMediaPlayer(url, startPlayback, creationOptions, callback, hWnd, out mediaPlayer);
        }
    }
}
