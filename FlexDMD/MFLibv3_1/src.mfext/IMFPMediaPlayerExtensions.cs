/* license

IMFPMediaPlayerExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;

using MediaFoundation.MFPlayer;
using MediaFoundation.Misc;
using MediaFoundation.Transform;

namespace MediaFoundation
{
    public static class IMFPMediaPlayerExtensions
    {
        /// <summary>
        /// Creates a media item from an object.
        /// </summary>
        /// <param name="mediaPlayer">A valid IMFPMediaPlayer instance.</param>
        /// <param name="mediaSource">An instance of a media source.</param>
        /// <param name="userData">Application-defined value to store in the media item.</param>
        /// <param name="mediaItem">Receives an instance of a IMFPMediaItem interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateMediaItemFromObject(this IMFPMediaPlayer mediaPlayer, IMFMediaSource mediaSource, IntPtr userData, out IMFPMediaItem mediaItem)
        {
            if (mediaPlayer == null)
                throw new ArgumentNullException("mediaSink");

            return mediaPlayer.CreateMediaItemFromObject(mediaSource, true, userData, out mediaItem);
        }

        /// <summary>
        /// Creates a media item from an object.
        /// </summary>
        /// <param name="mediaPlayer">A valid IMFPMediaPlayer instance.</param>
        /// <param name="mediaSink">An instance of a byte streamIndex.</param>
        /// <param name="userData">Application-defined value to store in the media item.</param>
        /// <param name="mediaItem">Receives an instance of a IMFPMediaItem interface.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateMediaItemFromObject(this IMFPMediaPlayer mediaPlayer, IMFByteStream byteStream, IntPtr userData, out IMFPMediaItem mediaItem)
        {
            if (byteStream == null)
                throw new ArgumentNullException("mediaSink");

            return mediaPlayer.CreateMediaItemFromObject(byteStream, true, userData, out mediaItem);
        }

        /// <summary>
        /// Gets the playback duration of the current media item.
        /// </summary>
        /// <param name="mediaPlayer">A valid IMFPMediaPlayer instance.</param>
        /// <param name="durationValue">Receives the duration.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetDuration(this IMFPMediaPlayer mediaPlayer, out TimeSpan durationValue)
        {
            if (mediaPlayer == null)
                throw new ArgumentNullException("mediaPlayer");

            using (PropVariant result = new PropVariant())
            {
                HResult hr = mediaPlayer.GetDuration(Guid.Empty, result);
                durationValue = hr.Succeeded() ? TimeSpan.FromTicks((long)result.GetULong()) : default(TimeSpan);
                
                return hr;
            }
        }

        /// <summary>
        /// Gets the current playback position.
        /// </summary>
        /// <param name="mediaPlayer">A valid IMFPMediaPlayer instance.</param>
        /// <param name="durationValue">Receives the playback position.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult GetPosition(this IMFPMediaPlayer mediaPlayer, out TimeSpan positionValue)
        {
            if (mediaPlayer == null)
                throw new ArgumentNullException("mediaPlayer");

            using (PropVariant result = new PropVariant())
            {
                HResult hr = mediaPlayer.GetPosition(Guid.Empty, result);
                positionValue = hr.Succeeded() ? TimeSpan.FromTicks((long)result.GetULong()) : default(TimeSpan);
                
                return hr;
            }
        }

        /// <summary>
        /// Applies an audio or video effect to playback.
        /// </summary>
        /// <param name="mediaPlayer">A valid IMFPMediaPlayer instance.</param>
        /// <param name="transform">A Media Foundation transform (MFT) that implements the effect.</param>
        /// <param name="optional">
        /// <list type="bullet">
        ///     <item>
        ///     <term>True</term>
        ///     <description>The effect is optional. If the MFPlay player object cannot add the effect, it ignores the effect and continues playback.</description>
        ///     </item>
        ///     <item>
        ///     <term>False</term>
        ///     <description>The effect is required. If the MFPlay player object cannot add the effect, a playback error occurs.</description>
        ///     </item>
        /// </list>
        /// </param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult InsertEffect(this IMFPMediaPlayer mediaPlayer, IMFTransform transform, bool optional)
        {
            if (mediaPlayer == null)
                throw new ArgumentNullException("mediaPlayer");

            return mediaPlayer.InsertEffect(transform, optional);
        }

        /// <summary>
        /// Applies an audio or video effect to playback.
        /// </summary>
        /// <param name="mediaPlayer">A valid IMFPMediaPlayer instance.</param>
        /// <param name="activate">An activation object that creates an MFT.</param>
        /// <param name="optional">
        /// <list type="bullet">
        ///     <item>
        ///     <term>True</term>
        ///     <description>The effect is optional. If the MFPlay player object cannot add the effect, it ignores the effect and continues playback.</description>
        ///     </item>
        ///     <item>
        ///     <term>False</term>
        ///     <description>The effect is required. If the MFPlay player object cannot add the effect, a playback error occurs.</description>
        ///     </item>
        /// </list>
        /// </param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult InsertEffect(this IMFPMediaPlayer mediaPlayer, IMFActivate activate, bool optional)
        {
            if (mediaPlayer == null)
                throw new ArgumentNullException("mediaPlayer");

            return mediaPlayer.InsertEffect(activate, optional);
        }

        /// <summary>
        /// Removes an effect that was added with the IMFPMediaPlayer.InsertEffect method.
        /// </summary>
        /// <param name="mediaPlayer">A valid IMFPMediaPlayer instance.</param>
        /// <param name="transform">The Media Foundation transform (MFT) previously added.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult RemoveEffect(this IMFPMediaPlayer mediaPlayer, IMFTransform transform)
        {
            if (mediaPlayer == null)
                throw new ArgumentNullException("mediaPlayer");

            return mediaPlayer.RemoveEffect(transform);
        }

        /// <summary>
        /// Removes an effect that was added with the IMFPMediaPlayer.InsertEffect method.
        /// </summary>
        /// <param name="mediaPlayer">A valid IMFPMediaPlayer instance.</param>
        /// <param name="activate">The activation object used to create the MFT.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult RemoveEffect(this IMFPMediaPlayer mediaPlayer, IMFActivate activate)
        {
            if (mediaPlayer == null)
                throw new ArgumentNullException("mediaPlayer");

            return mediaPlayer.RemoveEffect(activate);
        }

        /// <summary>
        /// Sets the playback position.
        /// </summary>
        /// <param name="mediaPlayer">A valid IMFPMediaPlayer instance.</param>
        /// <param name="positionValue">New playback position.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult SetPosition(this IMFPMediaPlayer mediaPlayer, TimeSpan positionValue)
        {
            if (mediaPlayer == null)
                throw new ArgumentNullException("mediaPlayer");

            using (PropVariant value = new PropVariant(positionValue.Ticks))
            {
                return mediaPlayer.SetPosition(Guid.Empty, value);
            }
        }
    }
}
