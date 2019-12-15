/* license

IMFMediaTypeExtensions.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

﻿using System;
using System.Runtime.InteropServices;

using MediaFoundation.Misc;

namespace MediaFoundation
{
    public static class IMFMediaTypeExtensions
    {
        /// <summary>
        /// Converts an audio media type to a WaveFormatEx structure.
        /// </summary>
        /// <param name="mediaType">A valid IMFMediaType instance.</param>
        /// <param name="flags">One member of the <see cref="MFWaveFormatExConvertFlags"/> enumaration.</param>
        /// <param name="waveFormatEx">Receives a WaveFormatEx structure representing this audio media type.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateWaveFormatEx(this IMFMediaType mediaType, MFWaveFormatExConvertFlags flags, out WaveFormatEx waveFormatEx)
        {
            if (mediaType == null)
                throw new ArgumentNullException("mediaType");

            int structSize;
            return MFExtern.MFCreateWaveFormatExFromMFMediaType(mediaType, out waveFormatEx, out structSize, flags);
        }

        /// <summary>
        /// Initializes the media type with a WaveFormatEx structure.
        /// </summary>
        /// <param name="mediaType">A valid IMFMediaType instance.</param>
        /// <param name="waveFormatEx">A WaveFormatEx structure that describe the media type.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult InitializeFromWaveFormatEx(this IMFMediaType mediaType, WaveFormatEx waveFormatEx)
        {
            if (mediaType == null)
                throw new ArgumentNullException("mediaType");

            if (waveFormatEx == null)
                throw new ArgumentNullException("waveFormatEx");

            return MFExtern.MFInitMediaTypeFromWaveFormatEx(mediaType, waveFormatEx, Marshal.SizeOf(waveFormatEx));
        }


        /// <summary>
        /// Initializes the media type with a MFVideoFormat structure.
        /// </summary>
        /// <param name="mediaType">A valid IMFMediaType instance.</param>
        /// <param name="videoFormat">A MFVideoFormat structure that describe the media type.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult InitializeFromMFVideoFormat(this IMFMediaType mediaType, MFVideoFormat videoFormat)
        {
            if (mediaType == null)
                throw new ArgumentNullException("mediaType");

            if (videoFormat == null)
                throw new ArgumentNullException("videoFormat");

            return MFExtern.MFInitMediaTypeFromMFVideoFormat(mediaType, videoFormat, Marshal.SizeOf(videoFormat));
        }

        /// <summary>
        /// Initializes the media type with a DirectShow AMMediaType structure.
        /// </summary>
        /// <param name="mediaType">A valid IMFMediaType instance.</param>
        /// <param name="amMediaType">An AMMediaType structure that describe the media type.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult InitializeFromAMMediaType(this IMFMediaType mediaType, AMMediaType amMediaType)
        {
            if (mediaType == null)
                throw new ArgumentNullException("mediaType");

            if (amMediaType == null)
                throw new ArgumentNullException("amMediaType");

            return MFExtern.MFInitMediaTypeFromAMMediaType(mediaType, amMediaType);
        }

        /// <summary>
        /// Initializes the media type with a VideoInfoHeader structure.
        /// </summary>
        /// <param name="mediaType">A valid IMFMediaType instance.</param>
        /// <param name="vih2">A VideoInfoHeader structure that describe the media type.</param>
        /// <param name="subType">Define the media type sub type.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult InitializeFromVideoInfoHeader(this IMFMediaType mediaType, VideoInfoHeader vih, Guid subType)
        {
            if (mediaType == null)
                throw new ArgumentNullException("mediaType");

            if (vih == null)
                throw new ArgumentNullException("vih2");

            return MFExtern.MFInitMediaTypeFromVideoInfoHeader(mediaType, vih, Marshal.SizeOf(vih), subType);
        }

        /// <summary>
        /// Initializes the media type with a VideoInfoHeader2 structure.
        /// </summary>
        /// <param name="mediaType">A valid IMFMediaType instance.</param>
        /// <param name="vih2">A VideoInfoHeader structure that describe the media type.</param>
        /// <param name="subType">Define the media type sub type.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult InitializeFromVideoInfoHeader2(this IMFMediaType mediaType, VideoInfoHeader2 vih2, Guid subType)
        {
            if (mediaType == null)
                throw new ArgumentNullException("mediaType");

            if (vih2 == null)
                throw new ArgumentNullException("vih2");

            return MFExtern.MFInitMediaTypeFromVideoInfoHeader2(mediaType, vih2, Marshal.SizeOf(vih2), subType);
        }

        /// <summary>
        /// Wrap this media type into another media type. 
        /// </summary>
        /// <param name="mediaType">A valid IMFMediaType instance.</param>
        /// <param name="majorType">Define the major type of the new media type.</param>
        /// <param name="subType">Define the subtype of the new media type.</param>
        /// <param name="wrap">Receives a new media type that wrap the current instance.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult WrapMediaType(this IMFMediaType mediaType, Guid majorType, Guid subType, out IMFMediaType wrap)
        {
            if (mediaType == null)
                throw new ArgumentNullException("mediaType");

            return MFExtern.MFWrapMediaType(mediaType, majorType, subType, out wrap);
        }

        /// <summary>
        /// Retrieve the media type wrapped into this instance.
        /// </summary>
        /// <param name="mediaType">A valid IMFMediaType instance.</param>
        /// <param name="original">Receives the original media type.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult UnwrapMediaType(this IMFMediaType mediaType, out IMFMediaType original)
        {
            if (mediaType == null)
                throw new ArgumentNullException("mediaType");

            return MFExtern.MFUnwrapMediaType(mediaType, out original);
        }

        /// <summary>
        /// Converts an audio media type to a WaveFormatEx structure.
        /// </summary>
        /// <param name="mediaType">A valid IMFMediaType instance.</param>
        /// <param name="waveFormatEx">Receives a WaveFormatEx structure representing this audio media type.</param>
        /// <returns>If this function succeeds, it returns the S_OK member. Otherwise, it returns another HResult's member that describe the error.</returns>
        public static HResult CreateWaveFormatEx(this IMFMediaType mediaType, out WaveFormatEx waveFormat)
        {
            if (mediaType == null)
                throw new ArgumentNullException("mediaType");

            int structSize;
            return MFExtern.MFCreateWaveFormatExFromMFMediaType(mediaType, out waveFormat, out structSize, MFWaveFormatExConvertFlags.Normal);
        }

        /// <summary>
        /// Compares this media type to a partial media type.
        /// </summary>
        /// <param name="mediaType">A valid IMFMediaType instance.</param>
        /// <param name="partialMediaType">A partial media type.</param>
        /// <returns>True if <paramref name="partialMediaType"/> have the same major type and if all attributes from the partial type exists in the current instance with the same value ; False otherwise.</returns>
        public static bool CompareToPartialMediaType(this IMFMediaType mediaType, IMFMediaType partialMediaType)
        {
            if (mediaType == null)
                throw new ArgumentNullException("mediaType");

            return MFExtern.MFCompareFullToPartialMediaType(mediaType, partialMediaType);
        }



    }
}
