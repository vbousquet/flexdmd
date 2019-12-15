/* license

IMFMediaEngineExExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

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
    public static class IMFMediaEngineExExtensions
    {
        /// <summary>
        /// Inserts an audio effect.
        /// </summary>
        /// <param name="mediaEngine">A valid IMFMediaBufferEx instance.</param>
        /// <param name="transform">A Media Foundation transform (MFT) that implements the audio effect.</param>
        /// <param name="optional">
        /// <list type="bullet">
        ///     <item>
        ///     <term>True</term>
        ///     <description>The effect is optional. If the Media Engine cannot add the effect, it ignores the effect and continues playback.</description>
        ///     </item>
        ///     <item>
        ///     <term>False</term>
        ///     <description>The effect is required. If the Media Engine object cannot add the effect, a playback error occurs.</description>
        ///     </item>
        /// </list>
        /// </param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult InsertAudioEffect(this IMFMediaEngineEx mediaEngine, IMFTransform transform, bool optional)
        {
            if (mediaEngine == null)
                throw new ArgumentNullException("mediaEngine");

            return mediaEngine.InsertAudioEffect(transform, optional);
        }

        /// <summary>
        /// Inserts an audio effect.
        /// </summary>
        /// <param name="mediaEngine">A valid IMFMediaBufferEx instance.</param>
        /// <param name="activate">An activation object that must create an MFT for the audio effect.</param>
        /// <param name="optional">
        /// <list type="bullet">
        ///     <item>
        ///     <term>True</term>
        ///     <description>The effect is optional. If the Media Engine cannot add the effect, it ignores the effect and continues playback.</description>
        ///     </item>
        ///     <item>
        ///     <term>False</term>
        ///     <description>The effect is required. If the Media Engine object cannot add the effect, a playback error occurs.</description>
        ///     </item>
        /// </list>
        /// </param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult InsertAudioEffect(this IMFMediaEngineEx mediaEngine, IMFActivate activate, bool optional)
        {
            if (mediaEngine == null)
                throw new ArgumentNullException("mediaEngine");

            return mediaEngine.InsertAudioEffect(activate, optional);
        }

        /// <summary>
        /// Inserts a video effect.
        /// </summary>
        /// <param name="mediaEngine">A valid IMFMediaBufferEx instance.</param>
        /// <param name="transform">A Media Foundation transform (MFT) that implements the video effect.</param>
        /// <param name="optional">
        /// <list type="bullet">
        ///     <item>
        ///     <term>True</term>
        ///     <description>The effect is optional. If the Media Engine cannot add the effect, it ignores the effect and continues playback.</description>
        ///     </item>
        ///     <item>
        ///     <term>False</term>
        ///     <description>The effect is required. If the Media Engine object cannot add the effect, a playback error occurs.</description>
        ///     </item>
        /// </list>
        /// </param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult InsertVideoEffect(this IMFMediaEngineEx mediaEngine, IMFTransform transform, bool optional)
        {
            if (mediaEngine == null)
                throw new ArgumentNullException("mediaEngine");

            return mediaEngine.InsertVideoEffect(transform, optional);
        }

        /// <summary>
        /// Inserts a video effect.
        /// </summary>
        /// <param name="mediaEngine">A valid IMFMediaBufferEx instance.</param>
        /// <param name="activate">An activation object that must create an MFT for the video effect.</param>
        /// <param name="optional">
        /// <list type="bullet">
        ///     <item>
        ///     <term>True</term>
        ///     <description>The effect is optional. If the Media Engine cannot add the effect, it ignores the effect and continues playback.</description>
        ///     </item>
        ///     <item>
        ///     <term>False</term>
        ///     <description>The effect is required. If the Media Engine object cannot add the effect, a playback error occurs.</description>
        ///     </item>
        /// </list>
        /// </param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult InsertVideoEffect(this IMFMediaEngineEx mediaEngine, IMFActivate activate, bool optional)
        {
            if (mediaEngine == null)
                throw new ArgumentNullException("mediaEngine");

            return mediaEngine.InsertVideoEffect(activate, optional);
        }

        /// <summary>
        /// Seeks to a new playback position using the specified <see cref="MF_MEDIA_ENGINE_SEEK_MODE"/>.
        /// </summary>
        /// <param name="mediaEngine">A valid IMFMediaBufferEx instance.</param>
        /// <param name="seekTime">The new playback position.</param>
        /// <param name="seekMode">Specifies if the seek is a normal seek or an approximate seek.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult SetCurrentTimeEx(this IMFMediaEngineEx mediaEngine, TimeSpan seekTime, MF_MEDIA_ENGINE_SEEK_MODE seekMode)
        {
            if (mediaEngine == null)
                throw new ArgumentNullException("mediaEngine");

            return mediaEngine.SetCurrentTimeEx(seekTime.TotalSeconds, seekMode);
        }
    }
}
