/* license

MFUtils.cs - Part of MediaFoundationLib, which provide access to MediaFoundation interfaces via .NET

Copyright (C) 2015, by the Administrators of the Media Foundation .NET SourceForge Project
http://mfnet.sourceforge.net

This is free software; you can redistribute it and/or modify it under the terms of either:

a) The Lesser General Public License version 2.1 (see license.txt)
b) The BSD License (see BSDL.txt)

*/

using System;
using System.Collections;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

using MediaFoundation.Transform;
using MediaFoundation.MFPlayer;

namespace MediaFoundation
{
    public enum HResult
    {
        #region COM HRESULTs

        S_OK = 0,
        S_FALSE = 1,

        E_PENDING = unchecked((int)0x8000000A),

        /// <summary>Catastrophic failure</summary>
        E_UNEXPECTED = unchecked((int)0x8000FFFF),

        /// <summary>Not implemented</summary>
        E_NOTIMPL = unchecked((int)0x80004001),

        /// <summary>Ran out of memory</summary>
        E_OUTOFMEMORY = unchecked((int)0x8007000E),

        /// <summary>One or more arguments are invalid</summary>
        E_INVALIDARG = unchecked((int)0x80070057),

        /// <summary>No such interface supported</summary>
        E_NOINTERFACE = unchecked((int)0x80004002),

        /// <summary>Invalid pointer</summary>
        E_POINTER = unchecked((int)0x80004003),

        /// <summary>Invalid handle</summary>
        E_HANDLE = unchecked((int)0x80070006),

        /// <summary>Operation aborted</summary>
        E_ABORT = unchecked((int)0x80004004),

        /// <summary>Unspecified error</summary>
        E_FAIL = unchecked((int)0x80004005),

        /// <summary>General access denied error</summary>
        E_ACCESSDENIED = unchecked((int)0x80070005),

        #endregion

        #region Win32 HRESULTs

        /// <summary>The system cannot find the file specified.</summary>
        /// <unmanaged>HRESULT_FROM_WIN32(ERROR_FILE_NOT_FOUND)</unmanaged>
        WIN32_ERROR_FILE_NOT_FOUND = unchecked((int)0x80070002),

        /// <summary>More data is available.</summary>
        /// <unmanaged>HRESULT_FROM_WIN32(ERROR_MORE_DATA)</unmanaged>
        WIN32_ERROR_MORE_DATA = unchecked((int)0x800700ea),

        /// <summary>No more data is available.</summary>
        /// <unmanaged>HRESULT_FROM_WIN32(ERROR_NO_MORE_ITEMS)</unmanaged>
        WIN32_ERROR_NO_MORE_ITEMS = unchecked((int)0x80070103),

        /// <summary>Element not found.</summary>
        /// <unmanaged>HRESULT_FROM_WIN32(ERROR_NOT_FOUND)</unmanaged>
        WIN32_ERROR_NOT_FOUND = unchecked((int)0x80070490),

        #endregion

        #region Structured Storage HRESULTs

        /// <summary>The underlying file was converted to compound file format.</summary>
        STG_S_CONVERTED = unchecked((int)0x00030200),

        /// <summary>Multiple opens prevent consolidated. (commit succeeded).</summary>
        STG_S_MULTIPLEOPENS = unchecked((int)0x00030204),

        /// <summary>Consolidation of the storage file failed. (commit succeeded).</summary>
        STG_S_CONSOLIDATIONFAILED = unchecked((int)0x00030205),

        /// <summary>Consolidation of the storage file is inappropriate. (commit succeeded).</summary>
        STG_S_CANNOTCONSOLIDATE = unchecked((int)0x00030206),

        /// <summary>Unable to perform requested operation.</summary>
        STG_E_INVALIDFUNCTION = unchecked((int)0x80030001),

        /// <summary>The file could not be found.</summary>
        STG_E_FILENOTFOUND = unchecked((int)0x80030002),

        /// <summary>There are insufficient resources to open another file.</summary>
        STG_E_TOOMANYOPENFILES = unchecked((int)0x80030004),

        /// <summary>Access Denied.</summary>
        STG_E_ACCESSDENIED = unchecked((int)0x80030005),

        /// <summary>There is insufficient memory available to complete operation.</summary>
        STG_E_INSUFFICIENTMEMORY = unchecked((int)0x80030008),

        /// <summary>Invalid pointer error.</summary>
        STG_E_INVALIDPOINTER = unchecked((int)0x80030009),

        /// <summary>A disk error occurred during a write operation.</summary>
        STG_E_WRITEFAULT = unchecked((int)0x8003001D),

        /// <summary>A lock violation has occurred.</summary>
        STG_E_LOCKVIOLATION = unchecked((int)0x80030021),

        /// <summary>File already exists.</summary>
        STG_E_FILEALREADYEXISTS = unchecked((int)0x80030050),

        /// <summary>Invalid parameter error.</summary>
        STG_E_INVALIDPARAMETER = unchecked((int)0x80030057),

        /// <summary>There is insufficient disk space to complete operation.</summary>
        STG_E_MEDIUMFULL = unchecked((int)0x80030070),

        /// <summary>The name is not valid.</summary>
        STG_E_INVALIDNAME = unchecked((int)0x800300FC),

        /// <summary>Invalid flag error.</summary>
        STG_E_INVALIDFLAG = unchecked((int)0x800300FF),

        /// <summary>The storage has been changed since the last commit.</summary>
        STG_E_NOTCURRENT = unchecked((int)0x80030101),

        /// <summary>Attempted to use an object that has ceased to exist.</summary>
        STG_E_REVERTED = unchecked((int)0x80030102),

        /// <summary>Can't save.</summary>
        STG_E_CANTSAVE = unchecked((int)0x80030103),

        #endregion

        #region Various HRESULTs

        /// <summary>The function failed because the specified GDI device did not have any monitors associated with it.</summary>
        ERROR_GRAPHICS_NO_MONITORS_CORRESPOND_TO_DISPLAY_DEVICE = unchecked((int)0xC02625E5),

        #endregion

        #region Media Foundation HRESULTs

        MF_E_PLATFORM_NOT_INITIALIZED = unchecked((int)0xC00D36B0),

        MF_E_CAPTURE_ENGINE_ALL_EFFECTS_REMOVED = unchecked((int)0xC00DABE5),
        MF_E_CAPTURE_NO_SAMPLES_IN_QUEUE = unchecked((int)0xC00DABEB),
        MF_E_CAPTURE_PROPERTY_SET_DURING_PHOTO = unchecked((int)0xC00DABEA),
        MF_E_CAPTURE_SOURCE_DEVICE_EXTENDEDPROP_OP_IN_PROGRESS = unchecked((int)0xC00DABE9),
        MF_E_CAPTURE_SOURCE_NO_AUDIO_STREAM_PRESENT = unchecked((int)0xC00DABE8),
        MF_E_CAPTURE_SOURCE_NO_INDEPENDENT_PHOTO_STREAM_PRESENT = unchecked((int)0xC00DABE6),
        MF_E_CAPTURE_SOURCE_NO_VIDEO_STREAM_PRESENT = unchecked((int)0xC00DABE7),
        MF_E_HARDWARE_DRM_UNSUPPORTED = unchecked((int)0xC00D3706),
        MF_E_HDCP_AUTHENTICATION_FAILURE = unchecked((int)0xC00D7188),
        MF_E_HDCP_LINK_FAILURE = unchecked((int)0xC00D7189),
        MF_E_HW_ACCELERATED_THUMBNAIL_NOT_SUPPORTED = unchecked((int)0xC00DABEC),
        MF_E_NET_COMPANION_DRIVER_DISCONNECT = unchecked((int)0xC00D4295),
        MF_E_OPERATION_IN_PROGRESS = unchecked((int)0xC00D3705),
        MF_E_SINK_HEADERS_NOT_FOUND = unchecked((int)0xC00D4A45),
        MF_INDEX_SIZE_ERR = unchecked((int)0x80700001),
        MF_INVALID_ACCESS_ERR = unchecked((int)0x8070000F),
        MF_INVALID_STATE_ERR = unchecked((int)0x8070000B),
        MF_NOT_FOUND_ERR = unchecked((int)0x80700008),
        MF_NOT_SUPPORTED_ERR = unchecked((int)0x80700009),
        MF_PARSE_ERR = unchecked((int)0x80700051),
        MF_QUOTA_EXCEEDED_ERR = unchecked((int)0x80700016),
        MF_SYNTAX_ERR = unchecked((int)0x8070000C),

        MF_E_BUFFERTOOSMALL = unchecked((int)0xC00D36B1),

        MF_E_INVALIDREQUEST = unchecked((int)0xC00D36B2),
        MF_E_INVALIDSTREAMNUMBER = unchecked((int)0xC00D36B3),
        MF_E_INVALIDMEDIATYPE = unchecked((int)0xC00D36B4),
        MF_E_NOTACCEPTING = unchecked((int)0xC00D36B5),
        MF_E_NOT_INITIALIZED = unchecked((int)0xC00D36B6),
        MF_E_UNSUPPORTED_REPRESENTATION = unchecked((int)0xC00D36B7),
        MF_E_NO_MORE_TYPES = unchecked((int)0xC00D36B9),
        MF_E_UNSUPPORTED_SERVICE = unchecked((int)0xC00D36BA),
        MF_E_UNEXPECTED = unchecked((int)0xC00D36BB),
        MF_E_INVALIDNAME = unchecked((int)0xC00D36BC),
        MF_E_INVALIDTYPE = unchecked((int)0xC00D36BD),
        MF_E_INVALID_FILE_FORMAT = unchecked((int)0xC00D36BE),
        MF_E_INVALIDINDEX = unchecked((int)0xC00D36BF),
        MF_E_INVALID_TIMESTAMP = unchecked((int)0xC00D36C0),
        MF_E_UNSUPPORTED_SCHEME = unchecked((int)0xC00D36C3),
        MF_E_UNSUPPORTED_BYTESTREAM_TYPE = unchecked((int)0xC00D36C4),
        MF_E_UNSUPPORTED_TIME_FORMAT = unchecked((int)0xC00D36C5),
        MF_E_NO_SAMPLE_TIMESTAMP = unchecked((int)0xC00D36C8),
        MF_E_NO_SAMPLE_DURATION = unchecked((int)0xC00D36C9),
        MF_E_INVALID_STREAM_DATA = unchecked((int)0xC00D36CB),
        MF_E_RT_UNAVAILABLE = unchecked((int)0xC00D36CF),
        MF_E_UNSUPPORTED_RATE = unchecked((int)0xC00D36D0),
        MF_E_THINNING_UNSUPPORTED = unchecked((int)0xC00D36D1),
        MF_E_REVERSE_UNSUPPORTED = unchecked((int)0xC00D36D2),
        MF_E_UNSUPPORTED_RATE_TRANSITION = unchecked((int)0xC00D36D3),
        MF_E_RATE_CHANGE_PREEMPTED = unchecked((int)0xC00D36D4),
        MF_E_NOT_FOUND = unchecked((int)0xC00D36D5),
        MF_E_NOT_AVAILABLE = unchecked((int)0xC00D36D6),
        MF_E_NO_CLOCK = unchecked((int)0xC00D36D7),
        MF_S_MULTIPLE_BEGIN = unchecked((int)0x000D36D8),
        MF_E_MULTIPLE_BEGIN = unchecked((int)0xC00D36D9),
        MF_E_MULTIPLE_SUBSCRIBERS = unchecked((int)0xC00D36DA),
        MF_E_TIMER_ORPHANED = unchecked((int)0xC00D36DB),
        MF_E_STATE_TRANSITION_PENDING = unchecked((int)0xC00D36DC),
        MF_E_UNSUPPORTED_STATE_TRANSITION = unchecked((int)0xC00D36DD),
        MF_E_UNRECOVERABLE_ERROR_OCCURRED = unchecked((int)0xC00D36DE),
        MF_E_SAMPLE_HAS_TOO_MANY_BUFFERS = unchecked((int)0xC00D36DF),
        MF_E_SAMPLE_NOT_WRITABLE = unchecked((int)0xC00D36E0),
        MF_E_INVALID_KEY = unchecked((int)0xC00D36E2),
        MF_E_BAD_STARTUP_VERSION = unchecked((int)0xC00D36E3),
        MF_E_UNSUPPORTED_CAPTION = unchecked((int)0xC00D36E4),
        MF_E_INVALID_POSITION = unchecked((int)0xC00D36E5),
        MF_E_ATTRIBUTENOTFOUND = unchecked((int)0xC00D36E6),
        MF_E_PROPERTY_TYPE_NOT_ALLOWED = unchecked((int)0xC00D36E7),
        MF_E_PROPERTY_TYPE_NOT_SUPPORTED = unchecked((int)0xC00D36E8),
        MF_E_PROPERTY_EMPTY = unchecked((int)0xC00D36E9),
        MF_E_PROPERTY_NOT_EMPTY = unchecked((int)0xC00D36EA),
        MF_E_PROPERTY_VECTOR_NOT_ALLOWED = unchecked((int)0xC00D36EB),
        MF_E_PROPERTY_VECTOR_REQUIRED = unchecked((int)0xC00D36EC),
        MF_E_OPERATION_CANCELLED = unchecked((int)0xC00D36ED),
        MF_E_BYTESTREAM_NOT_SEEKABLE = unchecked((int)0xC00D36EE),
        MF_E_DISABLED_IN_SAFEMODE = unchecked((int)0xC00D36EF),
        MF_E_CANNOT_PARSE_BYTESTREAM = unchecked((int)0xC00D36F0),
        MF_E_SOURCERESOLVER_MUTUALLY_EXCLUSIVE_FLAGS = unchecked((int)0xC00D36F1),
        MF_E_MEDIAPROC_WRONGSTATE = unchecked((int)0xC00D36F2),
        MF_E_RT_THROUGHPUT_NOT_AVAILABLE = unchecked((int)0xC00D36F3),
        MF_E_RT_TOO_MANY_CLASSES = unchecked((int)0xC00D36F4),
        MF_E_RT_WOULDBLOCK = unchecked((int)0xC00D36F5),
        MF_E_NO_BITPUMP = unchecked((int)0xC00D36F6),
        MF_E_RT_OUTOFMEMORY = unchecked((int)0xC00D36F7),
        MF_E_RT_WORKQUEUE_CLASS_NOT_SPECIFIED = unchecked((int)0xC00D36F8),
        MF_E_INSUFFICIENT_BUFFER = unchecked((int)0xC00D7170),
        MF_E_CANNOT_CREATE_SINK = unchecked((int)0xC00D36FA),
        MF_E_BYTESTREAM_UNKNOWN_LENGTH = unchecked((int)0xC00D36FB),
        MF_E_SESSION_PAUSEWHILESTOPPED = unchecked((int)0xC00D36FC),
        MF_S_ACTIVATE_REPLACED = unchecked((int)0x000D36FD),
        MF_E_FORMAT_CHANGE_NOT_SUPPORTED = unchecked((int)0xC00D36FE),
        MF_E_INVALID_WORKQUEUE = unchecked((int)0xC00D36FF),
        MF_E_DRM_UNSUPPORTED = unchecked((int)0xC00D3700),
        MF_E_UNAUTHORIZED = unchecked((int)0xC00D3701),
        MF_E_OUT_OF_RANGE = unchecked((int)0xC00D3702),
        MF_E_INVALID_CODEC_MERIT = unchecked((int)0xC00D3703),
        MF_E_HW_MFT_FAILED_START_STREAMING = unchecked((int)0xC00D3704),
        MF_S_ASF_PARSEINPROGRESS = unchecked((int)0x400D3A98),
        MF_E_ASF_PARSINGINCOMPLETE = unchecked((int)0xC00D3A98),
        MF_E_ASF_MISSINGDATA = unchecked((int)0xC00D3A99),
        MF_E_ASF_INVALIDDATA = unchecked((int)0xC00D3A9A),
        MF_E_ASF_OPAQUEPACKET = unchecked((int)0xC00D3A9B),
        MF_E_ASF_NOINDEX = unchecked((int)0xC00D3A9C),
        MF_E_ASF_OUTOFRANGE = unchecked((int)0xC00D3A9D),
        MF_E_ASF_INDEXNOTLOADED = unchecked((int)0xC00D3A9E),
        MF_E_ASF_TOO_MANY_PAYLOADS = unchecked((int)0xC00D3A9F),
        MF_E_ASF_UNSUPPORTED_STREAM_TYPE = unchecked((int)0xC00D3AA0),
        MF_E_ASF_DROPPED_PACKET = unchecked((int)0xC00D3AA1),
        MF_E_NO_EVENTS_AVAILABLE = unchecked((int)0xC00D3E80),
        MF_E_INVALID_STATE_TRANSITION = unchecked((int)0xC00D3E82),
        MF_E_END_OF_STREAM = unchecked((int)0xC00D3E84),
        MF_E_SHUTDOWN = unchecked((int)0xC00D3E85),
        MF_E_MP3_NOTFOUND = unchecked((int)0xC00D3E86),
        MF_E_MP3_OUTOFDATA = unchecked((int)0xC00D3E87),
        MF_E_MP3_NOTMP3 = unchecked((int)0xC00D3E88),
        MF_E_MP3_NOTSUPPORTED = unchecked((int)0xC00D3E89),
        MF_E_NO_DURATION = unchecked((int)0xC00D3E8A),
        MF_E_INVALID_FORMAT = unchecked((int)0xC00D3E8C),
        MF_E_PROPERTY_NOT_FOUND = unchecked((int)0xC00D3E8D),
        MF_E_PROPERTY_READ_ONLY = unchecked((int)0xC00D3E8E),
        MF_E_PROPERTY_NOT_ALLOWED = unchecked((int)0xC00D3E8F),
        MF_E_MEDIA_SOURCE_NOT_STARTED = unchecked((int)0xC00D3E91),
        MF_E_UNSUPPORTED_FORMAT = unchecked((int)0xC00D3E98),
        MF_E_MP3_BAD_CRC = unchecked((int)0xC00D3E99),
        MF_E_NOT_PROTECTED = unchecked((int)0xC00D3E9A),
        MF_E_MEDIA_SOURCE_WRONGSTATE = unchecked((int)0xC00D3E9B),
        MF_E_MEDIA_SOURCE_NO_STREAMS_SELECTED = unchecked((int)0xC00D3E9C),
        MF_E_CANNOT_FIND_KEYFRAME_SAMPLE = unchecked((int)0xC00D3E9D),

        MF_E_UNSUPPORTED_CHARACTERISTICS = unchecked((int)0xC00D3E9E),
        MF_E_NO_AUDIO_RECORDING_DEVICE = unchecked((int)0xC00D3E9F),
        MF_E_AUDIO_RECORDING_DEVICE_IN_USE = unchecked((int)0xC00D3EA0),
        MF_E_AUDIO_RECORDING_DEVICE_INVALIDATED = unchecked((int)0xC00D3EA1),
        MF_E_VIDEO_RECORDING_DEVICE_INVALIDATED = unchecked((int)0xC00D3EA2),
        MF_E_VIDEO_RECORDING_DEVICE_PREEMPTED = unchecked((int)0xC00D3EA3),

        MF_E_NETWORK_RESOURCE_FAILURE = unchecked((int)0xC00D4268),
        MF_E_NET_WRITE = unchecked((int)0xC00D4269),
        MF_E_NET_READ = unchecked((int)0xC00D426A),
        MF_E_NET_REQUIRE_NETWORK = unchecked((int)0xC00D426B),
        MF_E_NET_REQUIRE_ASYNC = unchecked((int)0xC00D426C),
        MF_E_NET_BWLEVEL_NOT_SUPPORTED = unchecked((int)0xC00D426D),
        MF_E_NET_STREAMGROUPS_NOT_SUPPORTED = unchecked((int)0xC00D426E),
        MF_E_NET_MANUALSS_NOT_SUPPORTED = unchecked((int)0xC00D426F),
        MF_E_NET_INVALID_PRESENTATION_DESCRIPTOR = unchecked((int)0xC00D4270),
        MF_E_NET_CACHESTREAM_NOT_FOUND = unchecked((int)0xC00D4271),
        MF_I_MANUAL_PROXY = unchecked((int)0x400D4272),
        MF_E_NET_REQUIRE_INPUT = unchecked((int)0xC00D4274),
        MF_E_NET_REDIRECT = unchecked((int)0xC00D4275),
        MF_E_NET_REDIRECT_TO_PROXY = unchecked((int)0xC00D4276),
        MF_E_NET_TOO_MANY_REDIRECTS = unchecked((int)0xC00D4277),
        MF_E_NET_TIMEOUT = unchecked((int)0xC00D4278),
        MF_E_NET_CLIENT_CLOSE = unchecked((int)0xC00D4279),
        MF_E_NET_BAD_CONTROL_DATA = unchecked((int)0xC00D427A),
        MF_E_NET_INCOMPATIBLE_SERVER = unchecked((int)0xC00D427B),
        MF_E_NET_UNSAFE_URL = unchecked((int)0xC00D427C),
        MF_E_NET_CACHE_NO_DATA = unchecked((int)0xC00D427D),
        MF_E_NET_EOL = unchecked((int)0xC00D427E),
        MF_E_NET_BAD_REQUEST = unchecked((int)0xC00D427F),
        MF_E_NET_INTERNAL_SERVER_ERROR = unchecked((int)0xC00D4280),
        MF_E_NET_SESSION_NOT_FOUND = unchecked((int)0xC00D4281),
        MF_E_NET_NOCONNECTION = unchecked((int)0xC00D4282),
        MF_E_NET_CONNECTION_FAILURE = unchecked((int)0xC00D4283),
        MF_E_NET_INCOMPATIBLE_PUSHSERVER = unchecked((int)0xC00D4284),
        MF_E_NET_SERVER_ACCESSDENIED = unchecked((int)0xC00D4285),
        MF_E_NET_PROXY_ACCESSDENIED = unchecked((int)0xC00D4286),
        MF_E_NET_CANNOTCONNECT = unchecked((int)0xC00D4287),
        MF_E_NET_INVALID_PUSH_TEMPLATE = unchecked((int)0xC00D4288),
        MF_E_NET_INVALID_PUSH_PUBLISHING_POINT = unchecked((int)0xC00D4289),
        MF_E_NET_BUSY = unchecked((int)0xC00D428A),
        MF_E_NET_RESOURCE_GONE = unchecked((int)0xC00D428B),
        MF_E_NET_ERROR_FROM_PROXY = unchecked((int)0xC00D428C),
        MF_E_NET_PROXY_TIMEOUT = unchecked((int)0xC00D428D),
        MF_E_NET_SERVER_UNAVAILABLE = unchecked((int)0xC00D428E),
        MF_E_NET_TOO_MUCH_DATA = unchecked((int)0xC00D428F),
        MF_E_NET_SESSION_INVALID = unchecked((int)0xC00D4290),
        MF_E_OFFLINE_MODE = unchecked((int)0xC00D4291),
        MF_E_NET_UDP_BLOCKED = unchecked((int)0xC00D4292),
        MF_E_NET_UNSUPPORTED_CONFIGURATION = unchecked((int)0xC00D4293),
        MF_E_NET_PROTOCOL_DISABLED = unchecked((int)0xC00D4294),
        MF_E_ALREADY_INITIALIZED = unchecked((int)0xC00D4650),
        MF_E_BANDWIDTH_OVERRUN = unchecked((int)0xC00D4651),
        MF_E_LATE_SAMPLE = unchecked((int)0xC00D4652),
        MF_E_FLUSH_NEEDED = unchecked((int)0xC00D4653),
        MF_E_INVALID_PROFILE = unchecked((int)0xC00D4654),
        MF_E_INDEX_NOT_COMMITTED = unchecked((int)0xC00D4655),
        MF_E_NO_INDEX = unchecked((int)0xC00D4656),
        MF_E_CANNOT_INDEX_IN_PLACE = unchecked((int)0xC00D4657),
        MF_E_MISSING_ASF_LEAKYBUCKET = unchecked((int)0xC00D4658),
        MF_E_INVALID_ASF_STREAMID = unchecked((int)0xC00D4659),
        MF_E_STREAMSINK_REMOVED = unchecked((int)0xC00D4A38),
        MF_E_STREAMSINKS_OUT_OF_SYNC = unchecked((int)0xC00D4A3A),
        MF_E_STREAMSINKS_FIXED = unchecked((int)0xC00D4A3B),
        MF_E_STREAMSINK_EXISTS = unchecked((int)0xC00D4A3C),
        MF_E_SAMPLEALLOCATOR_CANCELED = unchecked((int)0xC00D4A3D),
        MF_E_SAMPLEALLOCATOR_EMPTY = unchecked((int)0xC00D4A3E),
        MF_E_SINK_ALREADYSTOPPED = unchecked((int)0xC00D4A3F),
        MF_E_ASF_FILESINK_BITRATE_UNKNOWN = unchecked((int)0xC00D4A40),
        MF_E_SINK_NO_STREAMS = unchecked((int)0xC00D4A41),
        MF_S_SINK_NOT_FINALIZED = unchecked((int)0x000D4A42),
        MF_E_METADATA_TOO_LONG = unchecked((int)0xC00D4A43),
        MF_E_SINK_NO_SAMPLES_PROCESSED = unchecked((int)0xC00D4A44),
        MF_E_VIDEO_REN_NO_PROCAMP_HW = unchecked((int)0xC00D4E20),
        MF_E_VIDEO_REN_NO_DEINTERLACE_HW = unchecked((int)0xC00D4E21),
        MF_E_VIDEO_REN_COPYPROT_FAILED = unchecked((int)0xC00D4E22),
        MF_E_VIDEO_REN_SURFACE_NOT_SHARED = unchecked((int)0xC00D4E23),
        MF_E_VIDEO_DEVICE_LOCKED = unchecked((int)0xC00D4E24),
        MF_E_NEW_VIDEO_DEVICE = unchecked((int)0xC00D4E25),
        MF_E_NO_VIDEO_SAMPLE_AVAILABLE = unchecked((int)0xC00D4E26),
        MF_E_NO_AUDIO_PLAYBACK_DEVICE = unchecked((int)0xC00D4E84),
        MF_E_AUDIO_PLAYBACK_DEVICE_IN_USE = unchecked((int)0xC00D4E85),
        MF_E_AUDIO_PLAYBACK_DEVICE_INVALIDATED = unchecked((int)0xC00D4E86),
        MF_E_AUDIO_SERVICE_NOT_RUNNING = unchecked((int)0xC00D4E87),
        MF_E_TOPO_INVALID_OPTIONAL_NODE = unchecked((int)0xC00D520E),
        MF_E_TOPO_CANNOT_FIND_DECRYPTOR = unchecked((int)0xC00D5211),
        MF_E_TOPO_CODEC_NOT_FOUND = unchecked((int)0xC00D5212),
        MF_E_TOPO_CANNOT_CONNECT = unchecked((int)0xC00D5213),
        MF_E_TOPO_UNSUPPORTED = unchecked((int)0xC00D5214),
        MF_E_TOPO_INVALID_TIME_ATTRIBUTES = unchecked((int)0xC00D5215),
        MF_E_TOPO_LOOPS_IN_TOPOLOGY = unchecked((int)0xC00D5216),
        MF_E_TOPO_MISSING_PRESENTATION_DESCRIPTOR = unchecked((int)0xC00D5217),
        MF_E_TOPO_MISSING_STREAM_DESCRIPTOR = unchecked((int)0xC00D5218),
        MF_E_TOPO_STREAM_DESCRIPTOR_NOT_SELECTED = unchecked((int)0xC00D5219),
        MF_E_TOPO_MISSING_SOURCE = unchecked((int)0xC00D521A),
        MF_E_TOPO_SINK_ACTIVATES_UNSUPPORTED = unchecked((int)0xC00D521B),
        MF_E_SEQUENCER_UNKNOWN_SEGMENT_ID = unchecked((int)0xC00D61AC),
        MF_S_SEQUENCER_CONTEXT_CANCELED = unchecked((int)0x000D61AD),
        MF_E_NO_SOURCE_IN_CACHE = unchecked((int)0xC00D61AE),
        MF_S_SEQUENCER_SEGMENT_AT_END_OF_STREAM = unchecked((int)0x000D61AF),
        MF_E_TRANSFORM_TYPE_NOT_SET = unchecked((int)0xC00D6D60),
        MF_E_TRANSFORM_STREAM_CHANGE = unchecked((int)0xC00D6D61),
        MF_E_TRANSFORM_INPUT_REMAINING = unchecked((int)0xC00D6D62),
        MF_E_TRANSFORM_PROFILE_MISSING = unchecked((int)0xC00D6D63),
        MF_E_TRANSFORM_PROFILE_INVALID_OR_CORRUPT = unchecked((int)0xC00D6D64),
        MF_E_TRANSFORM_PROFILE_TRUNCATED = unchecked((int)0xC00D6D65),
        MF_E_TRANSFORM_PROPERTY_PID_NOT_RECOGNIZED = unchecked((int)0xC00D6D66),
        MF_E_TRANSFORM_PROPERTY_VARIANT_TYPE_WRONG = unchecked((int)0xC00D6D67),
        MF_E_TRANSFORM_PROPERTY_NOT_WRITEABLE = unchecked((int)0xC00D6D68),
        MF_E_TRANSFORM_PROPERTY_ARRAY_VALUE_WRONG_NUM_DIM = unchecked((int)0xC00D6D69),
        MF_E_TRANSFORM_PROPERTY_VALUE_SIZE_WRONG = unchecked((int)0xC00D6D6A),
        MF_E_TRANSFORM_PROPERTY_VALUE_OUT_OF_RANGE = unchecked((int)0xC00D6D6B),
        MF_E_TRANSFORM_PROPERTY_VALUE_INCOMPATIBLE = unchecked((int)0xC00D6D6C),
        MF_E_TRANSFORM_NOT_POSSIBLE_FOR_CURRENT_OUTPUT_MEDIATYPE = unchecked((int)0xC00D6D6D),
        MF_E_TRANSFORM_NOT_POSSIBLE_FOR_CURRENT_INPUT_MEDIATYPE = unchecked((int)0xC00D6D6E),
        MF_E_TRANSFORM_NOT_POSSIBLE_FOR_CURRENT_MEDIATYPE_COMBINATION = unchecked((int)0xC00D6D6F),
        MF_E_TRANSFORM_CONFLICTS_WITH_OTHER_CURRENTLY_ENABLED_FEATURES = unchecked((int)0xC00D6D70),
        MF_E_TRANSFORM_NEED_MORE_INPUT = unchecked((int)0xC00D6D72),
        MF_E_TRANSFORM_NOT_POSSIBLE_FOR_CURRENT_SPKR_CONFIG = unchecked((int)0xC00D6D73),
        MF_E_TRANSFORM_CANNOT_CHANGE_MEDIATYPE_WHILE_PROCESSING = unchecked((int)0xC00D6D74),
        MF_S_TRANSFORM_DO_NOT_PROPAGATE_EVENT = unchecked((int)0x000D6D75),
        MF_E_UNSUPPORTED_D3D_TYPE = unchecked((int)0xC00D6D76),
        MF_E_TRANSFORM_ASYNC_LOCKED = unchecked((int)0xC00D6D77),
        MF_E_TRANSFORM_CANNOT_INITIALIZE_ACM_DRIVER = unchecked((int)0xC00D6D78L),
        MF_E_LICENSE_INCORRECT_RIGHTS = unchecked((int)0xC00D7148),
        MF_E_LICENSE_OUTOFDATE = unchecked((int)0xC00D7149),
        MF_E_LICENSE_REQUIRED = unchecked((int)0xC00D714A),
        MF_E_DRM_HARDWARE_INCONSISTENT = unchecked((int)0xC00D714B),
        MF_E_NO_CONTENT_PROTECTION_MANAGER = unchecked((int)0xC00D714C),
        MF_E_LICENSE_RESTORE_NO_RIGHTS = unchecked((int)0xC00D714D),
        MF_E_BACKUP_RESTRICTED_LICENSE = unchecked((int)0xC00D714E),
        MF_E_LICENSE_RESTORE_NEEDS_INDIVIDUALIZATION = unchecked((int)0xC00D714F),
        MF_S_PROTECTION_NOT_REQUIRED = unchecked((int)0x000D7150),
        MF_E_COMPONENT_REVOKED = unchecked((int)0xC00D7151),
        MF_E_TRUST_DISABLED = unchecked((int)0xC00D7152),
        MF_E_WMDRMOTA_NO_ACTION = unchecked((int)0xC00D7153),
        MF_E_WMDRMOTA_ACTION_ALREADY_SET = unchecked((int)0xC00D7154),
        MF_E_WMDRMOTA_DRM_HEADER_NOT_AVAILABLE = unchecked((int)0xC00D7155),
        MF_E_WMDRMOTA_DRM_ENCRYPTION_SCHEME_NOT_SUPPORTED = unchecked((int)0xC00D7156),
        MF_E_WMDRMOTA_ACTION_MISMATCH = unchecked((int)0xC00D7157),
        MF_E_WMDRMOTA_INVALID_POLICY = unchecked((int)0xC00D7158),
        MF_E_POLICY_UNSUPPORTED = unchecked((int)0xC00D7159),
        MF_E_OPL_NOT_SUPPORTED = unchecked((int)0xC00D715A),
        MF_E_TOPOLOGY_VERIFICATION_FAILED = unchecked((int)0xC00D715B),
        MF_E_SIGNATURE_VERIFICATION_FAILED = unchecked((int)0xC00D715C),
        MF_E_DEBUGGING_NOT_ALLOWED = unchecked((int)0xC00D715D),
        MF_E_CODE_EXPIRED = unchecked((int)0xC00D715E),
        MF_E_GRL_VERSION_TOO_LOW = unchecked((int)0xC00D715F),
        MF_E_GRL_RENEWAL_NOT_FOUND = unchecked((int)0xC00D7160),
        MF_E_GRL_EXTENSIBLE_ENTRY_NOT_FOUND = unchecked((int)0xC00D7161),
        MF_E_KERNEL_UNTRUSTED = unchecked((int)0xC00D7162),
        MF_E_PEAUTH_UNTRUSTED = unchecked((int)0xC00D7163),
        MF_E_NON_PE_PROCESS = unchecked((int)0xC00D7165),
        MF_E_REBOOT_REQUIRED = unchecked((int)0xC00D7167),
        MF_S_WAIT_FOR_POLICY_SET = unchecked((int)0x000D7168),
        MF_S_VIDEO_DISABLED_WITH_UNKNOWN_SOFTWARE_OUTPUT = unchecked((int)0x000D7169),
        MF_E_GRL_INVALID_FORMAT = unchecked((int)0xC00D716A),
        MF_E_GRL_UNRECOGNIZED_FORMAT = unchecked((int)0xC00D716B),
        MF_E_ALL_PROCESS_RESTART_REQUIRED = unchecked((int)0xC00D716C),
        MF_E_PROCESS_RESTART_REQUIRED = unchecked((int)0xC00D716D),
        MF_E_USERMODE_UNTRUSTED = unchecked((int)0xC00D716E),
        MF_E_PEAUTH_SESSION_NOT_STARTED = unchecked((int)0xC00D716F),
        MF_E_PEAUTH_PUBLICKEY_REVOKED = unchecked((int)0xC00D7171),
        MF_E_GRL_ABSENT = unchecked((int)0xC00D7172),
        MF_S_PE_TRUSTED = unchecked((int)0x000D7173),
        MF_E_PE_UNTRUSTED = unchecked((int)0xC00D7174),
        MF_E_PEAUTH_NOT_STARTED = unchecked((int)0xC00D7175),
        MF_E_INCOMPATIBLE_SAMPLE_PROTECTION = unchecked((int)0xC00D7176),
        MF_E_PE_SESSIONS_MAXED = unchecked((int)0xC00D7177),
        MF_E_HIGH_SECURITY_LEVEL_CONTENT_NOT_ALLOWED = unchecked((int)0xC00D7178),
        MF_E_TEST_SIGNED_COMPONENTS_NOT_ALLOWED = unchecked((int)0xC00D7179),
        MF_E_ITA_UNSUPPORTED_ACTION = unchecked((int)0xC00D717A),
        MF_E_ITA_ERROR_PARSING_SAP_PARAMETERS = unchecked((int)0xC00D717B),
        MF_E_POLICY_MGR_ACTION_OUTOFBOUNDS = unchecked((int)0xC00D717C),
        MF_E_BAD_OPL_STRUCTURE_FORMAT = unchecked((int)0xC00D717D),
        MF_E_ITA_UNRECOGNIZED_ANALOG_VIDEO_PROTECTION_GUID = unchecked((int)0xC00D717E),
        MF_E_NO_PMP_HOST = unchecked((int)0xC00D717F),
        MF_E_ITA_OPL_DATA_NOT_INITIALIZED = unchecked((int)0xC00D7180),
        MF_E_ITA_UNRECOGNIZED_ANALOG_VIDEO_OUTPUT = unchecked((int)0xC00D7181),
        MF_E_ITA_UNRECOGNIZED_DIGITAL_VIDEO_OUTPUT = unchecked((int)0xC00D7182),

        MF_E_RESOLUTION_REQUIRES_PMP_CREATION_CALLBACK = unchecked((int)0xC00D7183),
        MF_E_INVALID_AKE_CHANNEL_PARAMETERS = unchecked((int)0xC00D7184),
        MF_E_CONTENT_PROTECTION_SYSTEM_NOT_ENABLED = unchecked((int)0xC00D7185),
        MF_E_UNSUPPORTED_CONTENT_PROTECTION_SYSTEM = unchecked((int)0xC00D7186),
        MF_E_DRM_MIGRATION_NOT_SUPPORTED = unchecked((int)0xC00D7187),

        MF_E_CLOCK_INVALID_CONTINUITY_KEY = unchecked((int)0xC00D9C40),
        MF_E_CLOCK_NO_TIME_SOURCE = unchecked((int)0xC00D9C41),
        MF_E_CLOCK_STATE_ALREADY_SET = unchecked((int)0xC00D9C42),
        MF_E_CLOCK_NOT_SIMPLE = unchecked((int)0xC00D9C43),
        MF_S_CLOCK_STOPPED = unchecked((int)0x000D9C44),
        MF_E_NO_MORE_DROP_MODES = unchecked((int)0xC00DA028),
        MF_E_NO_MORE_QUALITY_LEVELS = unchecked((int)0xC00DA029),
        MF_E_DROPTIME_NOT_SUPPORTED = unchecked((int)0xC00DA02A),
        MF_E_QUALITYKNOB_WAIT_LONGER = unchecked((int)0xC00DA02B),
        MF_E_QM_INVALIDSTATE = unchecked((int)0xC00DA02C),
        MF_E_TRANSCODE_NO_CONTAINERTYPE = unchecked((int)0xC00DA410),
        MF_E_TRANSCODE_PROFILE_NO_MATCHING_STREAMS = unchecked((int)0xC00DA411),
        MF_E_TRANSCODE_NO_MATCHING_ENCODER = unchecked((int)0xC00DA412),

        MF_E_TRANSCODE_INVALID_PROFILE = unchecked((int)0xC00DA413),

        MF_E_ALLOCATOR_NOT_INITIALIZED = unchecked((int)0xC00DA7F8),
        MF_E_ALLOCATOR_NOT_COMMITED = unchecked((int)0xC00DA7F9),
        MF_E_ALLOCATOR_ALREADY_COMMITED = unchecked((int)0xC00DA7FA),
        MF_E_STREAM_ERROR = unchecked((int)0xC00DA7FB),
        MF_E_INVALID_STREAM_STATE = unchecked((int)0xC00DA7FC),
        MF_E_HW_STREAM_NOT_CONNECTED = unchecked((int)0xC00DA7FD),

        MF_E_NO_CAPTURE_DEVICES_AVAILABLE = unchecked((int)0xC00DABE0),
        MF_E_CAPTURE_SINK_OUTPUT_NOT_SET = unchecked((int)0xC00DABE1),
        MF_E_CAPTURE_SINK_MIRROR_ERROR = unchecked((int)0xC00DABE2),
        MF_E_CAPTURE_SINK_ROTATE_ERROR = unchecked((int)0xC00DABE3),
        MF_E_CAPTURE_ENGINE_INVALID_OP = unchecked((int)0xC00DABE4),

        MF_E_DXGI_DEVICE_NOT_INITIALIZED = unchecked((int)0x80041000),
        MF_E_DXGI_NEW_VIDEO_DEVICE = unchecked((int)0x80041001),
        MF_E_DXGI_VIDEO_DEVICE_LOCKED = unchecked((int)0x80041002),

        #endregion
    }
}

namespace MediaFoundation.Misc
{
    #region Wrapper classes

    [StructLayout(LayoutKind.Sequential)]
    public class MfFloat
    {
        private float Value;

        public MfFloat()
            : this(0)
        {
        }

        public MfFloat(float v)
        {
            Value = v;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static implicit operator float(MfFloat l)
        {
            return l.Value;
        }

        public static implicit operator MfFloat(float l)
        {
            return new MfFloat(l);
        }
    }

    /// <summary>
    /// ConstPropVariant is used for [In] parameters.  This is important since
    /// for [In] parameters, you must *not* clear the PropVariant.  The caller
    /// will need to do that himself.
    ///
    /// Likewise, if you want to store a copy of a ConstPropVariant, you should
    /// store it to a PropVariant using the PropVariant constructor that takes a
    /// ConstPropVariant.  If you try to store the ConstPropVariant, when the
    /// caller frees his copy, yours will no longer be valid.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public class ConstPropVariant : IDisposable
    {
        #region Declarations

        [DllImport("ole32.dll", ExactSpelling = true, PreserveSig = false), SuppressUnmanagedCodeSecurity]
        protected static extern void PropVariantCopy(
            [Out, MarshalAs(UnmanagedType.LPStruct)] PropVariant pvarDest,
            [In, MarshalAs(UnmanagedType.LPStruct)] ConstPropVariant pvarSource
            );

        #endregion

        public enum VariantType : short
        {
            None = 0,
            Short = 2,
            Int32 = 3,
            Float = 4,
            Double = 5,
            IUnknown = 13,
            UByte = 17,
            UShort = 18,
            UInt32 = 19,
            Int64 = 20,
            UInt64 = 21,
            String = 31,
            Guid = 72,
            Blob = 0x1000 + 17,
            StringArray = 0x1000 + 31
        }

        [StructLayout(LayoutKind.Sequential), UnmanagedName("BLOB")]
        protected struct Blob
        {
            public int cbSize;
            public IntPtr pBlobData;
        }

        [StructLayout(LayoutKind.Sequential), UnmanagedName("CALPWSTR")]
        protected struct CALPWstr
        {
            public int cElems;
            public IntPtr pElems;
        }

        #region Member variables

        [FieldOffset(0)]
        protected VariantType type;

        [FieldOffset(2)]
        protected short reserved1;

        [FieldOffset(4)]
        protected short reserved2;

        [FieldOffset(6)]
        protected short reserved3;

        [FieldOffset(8)]
        protected short iVal;

        [FieldOffset(8), CLSCompliant(false)]
        protected ushort uiVal;

        [FieldOffset(8), CLSCompliant(false)]
        protected byte bVal;

        [FieldOffset(8)]
        protected int intValue;

        [FieldOffset(8), CLSCompliant(false)]
        protected uint uintVal;

        [FieldOffset(8)]
        protected float fltVal;

        [FieldOffset(8)]
        protected long longValue;

        [FieldOffset(8), CLSCompliant(false)]
        protected ulong ulongValue;

        [FieldOffset(8)]
        protected double doubleValue;

        [FieldOffset(8)]
        protected Blob blobValue;

        [FieldOffset(8)]
        protected IntPtr ptr;

        [FieldOffset(8)]
        protected CALPWstr calpwstrVal;

        #endregion

        public ConstPropVariant()
            : this(VariantType.None)
        {
        }

        protected ConstPropVariant(VariantType v)
        {
            type = v;
        }

        public static explicit operator string(ConstPropVariant f)
        {
            return f.GetString();
        }

        public static explicit operator string[](ConstPropVariant f)
        {
            return f.GetStringArray();
        }

        public static explicit operator byte(ConstPropVariant f)
        {
            return f.GetUByte();
        }

        public static explicit operator short(ConstPropVariant f)
        {
            return f.GetShort();
        }

        [CLSCompliant(false)]
        public static explicit operator ushort(ConstPropVariant f)
        {
            return f.GetUShort();
        }

        public static explicit operator int(ConstPropVariant f)
        {
            return f.GetInt();
        }

        [CLSCompliant(false)]
        public static explicit operator uint(ConstPropVariant f)
        {
            return f.GetUInt();
        }

        public static explicit operator float(ConstPropVariant f)
        {
            return f.GetFloat();
        }

        public static explicit operator double(ConstPropVariant f)
        {
            return f.GetDouble();
        }

        public static explicit operator long(ConstPropVariant f)
        {
            return f.GetLong();
        }

        [CLSCompliant(false)]
        public static explicit operator ulong(ConstPropVariant f)
        {
            return f.GetULong();
        }

        public static explicit operator Guid(ConstPropVariant f)
        {
            return f.GetGuid();
        }

        public static explicit operator byte[](ConstPropVariant f)
        {
            return f.GetBlob();
        }

        // I decided not to do implicits since perf is likely to be
        // better recycling the PropVariant, and the only way I can
        // see to support Implicit is to create a new PropVariant.
        // Also, since I can't free the previous instance, IUnknowns
        // will linger until the GC cleans up.  Not what I think I
        // want.

        public MFAttributeType GetMFAttributeType()
        {
            switch (type)
            {
                case VariantType.None:
                case VariantType.UInt32:
                case VariantType.UInt64:
                case VariantType.Double:
                case VariantType.Guid:
                case VariantType.String:
                case VariantType.Blob:
                case VariantType.IUnknown:
                    {
                        return (MFAttributeType)type;
                    }
                default:
                    {
                        throw new Exception("Type is not a MFAttributeType");
                    }
            }
        }

        public VariantType GetVariantType()
        {
            return type;
        }

        public string[] GetStringArray()
        {
            if (type == VariantType.StringArray)
            {
                string[] sa;

                int iCount = calpwstrVal.cElems;
                sa = new string[iCount];

                for (int x = 0; x < iCount; x++)
                {
                    sa[x] = Marshal.PtrToStringUni(Marshal.ReadIntPtr(calpwstrVal.pElems, x * IntPtr.Size));
                }

                return sa;
            }
            throw new ArgumentException("PropVariant contents not a string array");
        }

        public string GetString()
        {
            if (type == VariantType.String)
            {
                return Marshal.PtrToStringUni(ptr);
            }
            throw new ArgumentException("PropVariant contents not a string");
        }

        public byte GetUByte()
        {
            if (type == VariantType.UByte)
            {
                return bVal;
            }
            throw new ArgumentException("PropVariant contents not a byte");
        }

        public short GetShort()
        {
            if (type == VariantType.Short)
            {
                return iVal;
            }
            throw new ArgumentException("PropVariant contents not a Short");
        }

        [CLSCompliant(false)]
        public ushort GetUShort()
        {
            if (type == VariantType.UShort)
            {
                return uiVal;
            }
            throw new ArgumentException("PropVariant contents not a UShort");
        }

        public int GetInt()
        {
            if (type == VariantType.Int32)
            {
                return intValue;
            }
            throw new ArgumentException("PropVariant contents not an int32");
        }

        [CLSCompliant(false)]
        public uint GetUInt()
        {
            if (type == VariantType.UInt32)
            {
                return uintVal;
            }
            throw new ArgumentException("PropVariant contents not a uint32");
        }

        public long GetLong()
        {
            if (type == VariantType.Int64)
            {
                return longValue;
            }
            throw new ArgumentException("PropVariant contents not an int64");
        }

        [CLSCompliant(false)]
        public ulong GetULong()
        {
            if (type == VariantType.UInt64)
            {
                return ulongValue;
            }
            throw new ArgumentException("PropVariant contents not a uint64");
        }

        public float GetFloat()
        {
            if (type == VariantType.Float)
            {
                return fltVal;
            }
            throw new ArgumentException("PropVariant contents not a Float");
        }

        public double GetDouble()
        {
            if (type == VariantType.Double)
            {
                return doubleValue;
            }
            throw new ArgumentException("PropVariant contents not a double");
        }

        public Guid GetGuid()
        {
            if (type == VariantType.Guid)
            {
                return (Guid)Marshal.PtrToStructure(ptr, typeof(Guid));
            }
            throw new ArgumentException("PropVariant contents not a Guid");
        }

        public byte[] GetBlob()
        {
            if (type == VariantType.Blob)
            {
                byte[] b = new byte[blobValue.cbSize];

                if (blobValue.cbSize > 0)
                {
                    Marshal.Copy(blobValue.pBlobData, b, 0, blobValue.cbSize);
                }

                return b;
            }
            throw new ArgumentException("PropVariant contents not a Blob");
        }

        public object GetBlob(Type t, int offset)
        {
            if (type == VariantType.Blob)
            {
                object o;

                if (blobValue.cbSize > offset)
                {
                    if (blobValue.cbSize >= Marshal.SizeOf(t) + offset)
                    {
                        o = Marshal.PtrToStructure(blobValue.pBlobData + offset, t);
                    }
                    else
                    {
                        throw new ArgumentException("Blob wrong size");
                    }
                }
                else
                {
                    o = null;
                }

                return o;
            }
            throw new ArgumentException("PropVariant contents not a Blob");
        }

        public object GetBlob(Type t)
        {
            return GetBlob(t, 0);
        }

        public object GetIUnknown()
        {
            if (type == VariantType.IUnknown)
            {
                if (ptr != IntPtr.Zero)
                {
                    return Marshal.GetObjectForIUnknown(ptr);
                }
                else
                {
                    return null;
                }
            }
            throw new ArgumentException("PropVariant contents not an IUnknown");
        }

        public void Copy(PropVariant pdest)
        {
            if (pdest == null)
            {
                throw new Exception("Null PropVariant sent to Copy");
            }

            // Copy doesn't clear the dest.
            pdest.Clear();

            PropVariantCopy(pdest, this);
        }

        public override string ToString()
        {
            // This method is primarily intended for debugging so that a readable string will show
            // up in the output window
            string sRet;

            switch (type)
            {
                case VariantType.None:
                    {
                        sRet = "<Empty>";
                        break;
                    }

                case VariantType.Blob:
                    {
                        const string FormatString = "x2"; // Hex 2 digit format
                        const int MaxEntries = 16;

                        byte[] blob = GetBlob();

                        // Number of bytes we're going to format
                        int n = Math.Min(MaxEntries, blob.Length);

                        if (n == 0)
                        {
                            sRet = "<Empty Array>";
                        }
                        else
                        {
                            // Only format the first MaxEntries bytes
                            sRet = blob[0].ToString(FormatString);
                            for (int i = 1; i < n; i++)
                            {
                                sRet += ',' + blob[i].ToString(FormatString);
                            }

                            // If the string is longer, add an indicator
                            if (blob.Length > n)
                            {
                                sRet += "...";
                            }
                        }
                        break;
                    }

                case VariantType.Float:
                    {
                        sRet = GetFloat().ToString();
                        break;
                    }

                case VariantType.Double:
                    {
                        sRet = GetDouble().ToString();
                        break;
                    }

                case VariantType.Guid:
                    {
                        sRet = GetGuid().ToString();
                        break;
                    }

                case VariantType.IUnknown:
                    {
                        object o = GetIUnknown();
                        if (o != null)
                        {
                            sRet = GetIUnknown().ToString();
                        }
                        else
                        {
                            sRet = "<null>";
                        }
                        break;
                    }

                case VariantType.String:
                    {
                        sRet = GetString();
                        break;
                    }

                case VariantType.Short:
                    {
                        sRet = GetShort().ToString();
                        break;
                    }

                case VariantType.UByte:
                    {
                        sRet = GetUByte().ToString();
                        break;
                    }

                case VariantType.UShort:
                    {
                        sRet = GetUShort().ToString();
                        break;
                    }

                case VariantType.Int32:
                    {
                        sRet = GetInt().ToString();
                        break;
                    }

                case VariantType.UInt32:
                    {
                        sRet = GetUInt().ToString();
                        break;
                    }

                case VariantType.Int64:
                    {
                        sRet = GetLong().ToString();
                        break;
                    }

                case VariantType.UInt64:
                    {
                        sRet = GetULong().ToString();
                        break;
                    }

                case VariantType.StringArray:
                    {
                        sRet = "";
                        foreach (string entry in GetStringArray())
                        {
                            sRet += (sRet.Length == 0 ? "\"" : ",\"") + entry + '\"';
                        }
                        break;
                    }
                default:
                    {
                        sRet = base.ToString();
                        break;
                    }
            }

            return sRet;
        }

        public override int GetHashCode()
        {
            // Give a (slightly) better hash value in case someone uses PropVariants
            // in a hash table.
            int iRet;

            switch (type)
            {
                case VariantType.None:
                    {
                        iRet = base.GetHashCode();
                        break;
                    }

                case VariantType.Blob:
                    {
                        iRet = GetBlob().GetHashCode();
                        break;
                    }

                case VariantType.Float:
                    {
                        iRet = GetFloat().GetHashCode();
                        break;
                    }

                case VariantType.Double:
                    {
                        iRet = GetDouble().GetHashCode();
                        break;
                    }

                case VariantType.Guid:
                    {
                        iRet = GetGuid().GetHashCode();
                        break;
                    }

                case VariantType.IUnknown:
                    {
                        iRet = GetIUnknown().GetHashCode();
                        break;
                    }

                case VariantType.String:
                    {
                        iRet = GetString().GetHashCode();
                        break;
                    }

                case VariantType.UByte:
                    {
                        iRet = GetUByte().GetHashCode();
                        break;
                    }

                case VariantType.Short:
                    {
                        iRet = GetShort().GetHashCode();
                        break;
                    }

                case VariantType.UShort:
                    {
                        iRet = GetUShort().GetHashCode();
                        break;
                    }

                case VariantType.Int32:
                    {
                        iRet = GetInt().GetHashCode();
                        break;
                    }

                case VariantType.UInt32:
                    {
                        iRet = GetUInt().GetHashCode();
                        break;
                    }

                case VariantType.Int64:
                    {
                        iRet = GetLong().GetHashCode();
                        break;
                    }

                case VariantType.UInt64:
                    {
                        iRet = GetULong().GetHashCode();
                        break;
                    }

                case VariantType.StringArray:
                    {
                        iRet = GetStringArray().GetHashCode();
                        break;
                    }
                default:
                    {
                        iRet = base.GetHashCode();
                        break;
                    }
            }

            return iRet;
        }

        public override bool Equals(object obj)
        {
            bool bRet;
            PropVariant p = obj as PropVariant;

            if ((((object)p) == null) || (p.type != type))
            {
                bRet = false;
            }
            else
            {
                switch (type)
                {
                    case VariantType.None:
                        {
                            bRet = true;
                            break;
                        }

                    case VariantType.Blob:
                        {
                            byte[] b1;
                            byte[] b2;

                            b1 = GetBlob();
                            b2 = p.GetBlob();

                            if (b1.Length == b2.Length)
                            {
                                bRet = true;
                                for (int x = 0; x < b1.Length; x++)
                                {
                                    if (b1[x] != b2[x])
                                    {
                                        bRet = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                bRet = false;
                            }
                            break;
                        }

                    case VariantType.Float:
                        {
                            bRet = GetFloat() == p.GetFloat();
                            break;
                        }

                    case VariantType.Double:
                        {
                            bRet = GetDouble() == p.GetDouble();
                            break;
                        }

                    case VariantType.Guid:
                        {
                            bRet = GetGuid() == p.GetGuid();
                            break;
                        }

                    case VariantType.IUnknown:
                        {
                            bRet = GetIUnknown() == p.GetIUnknown();
                            break;
                        }

                    case VariantType.String:
                        {
                            bRet = GetString() == p.GetString();
                            break;
                        }

                    case VariantType.UByte:
                        {
                            bRet = GetUByte() == p.GetUByte();
                            break;
                        }

                    case VariantType.Short:
                        {
                            bRet = GetShort() == p.GetShort();
                            break;
                        }

                    case VariantType.UShort:
                        {
                            bRet = GetUShort() == p.GetUShort();
                            break;
                        }

                    case VariantType.Int32:
                        {
                            bRet = GetInt() == p.GetInt();
                            break;
                        }

                    case VariantType.UInt32:
                        {
                            bRet = GetUInt() == p.GetUInt();
                            break;
                        }

                    case VariantType.Int64:
                        {
                            bRet = GetLong() == p.GetLong();
                            break;
                        }

                    case VariantType.UInt64:
                        {
                            bRet = GetULong() == p.GetULong();
                            break;
                        }

                    case VariantType.StringArray:
                        {
                            string[] sa1;
                            string[] sa2;

                            sa1 = GetStringArray();
                            sa2 = p.GetStringArray();

                            if (sa1.Length == sa2.Length)
                            {
                                bRet = true;
                                for (int x = 0; x < sa1.Length; x++)
                                {
                                    if (sa1[x] != sa2[x])
                                    {
                                        bRet = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                bRet = false;
                            }
                            break;
                        }
                    default:
                        {
                            bRet = base.Equals(obj);
                            break;
                        }
                }
            }

            return bRet;
        }

        public static bool operator ==(ConstPropVariant pv1, ConstPropVariant pv2)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(pv1, pv2))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)pv1 == null) || ((object)pv2 == null))
            {
                return false;
            }

            return pv1.Equals(pv2);
        }

        public static bool operator !=(ConstPropVariant pv1, ConstPropVariant pv2)
        {
            return !(pv1 == pv2);
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources;
        /// <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            // If we are a ConstPropVariant, we must *not* call PropVariantClear.  That
            // would release the *caller's* copy of the data, which would probably make
            // him cranky.  If we are a PropVariant, the PropVariant.Dispose gets called
            // as well, which *does* do a PropVariantClear.
            type = VariantType.None;
#if DEBUG
            longValue = 0;
#endif
        }

        #endregion
    }

    [StructLayout(LayoutKind.Explicit)]
    public class PropVariant : ConstPropVariant
    {
        #region Declarations

        [DllImport("ole32.dll", ExactSpelling = true, PreserveSig = false), SuppressUnmanagedCodeSecurity]
        protected static extern void PropVariantClear(
            [In, MarshalAs(UnmanagedType.LPStruct)] PropVariant pvar
            );

        #endregion

        public PropVariant()
            : base(VariantType.None)
        {
        }

        public PropVariant(string value)
            : base(VariantType.String)
        {
            ptr = Marshal.StringToCoTaskMemUni(value);
        }

        public PropVariant(string[] value)
            : base(VariantType.StringArray)
        {
            calpwstrVal.cElems = value.Length;
            calpwstrVal.pElems = Marshal.AllocCoTaskMem(IntPtr.Size * value.Length);

            for (int x = 0; x < value.Length; x++)
            {
                Marshal.WriteIntPtr(calpwstrVal.pElems, x * IntPtr.Size, Marshal.StringToCoTaskMemUni(value[x]));
            }
        }

        public PropVariant(byte value)
            : base(VariantType.UByte)
        {
            bVal = value;
        }

        public PropVariant(short value)
            : base(VariantType.Short)
        {
            iVal = value;
        }

        [CLSCompliant(false)]
        public PropVariant(ushort value)
            : base(VariantType.UShort)
        {
            uiVal = value;
        }

        public PropVariant(int value)
            : base(VariantType.Int32)
        {
            intValue = value;
        }

        [CLSCompliant(false)]
        public PropVariant(uint value)
            : base(VariantType.UInt32)
        {
            uintVal = value;
        }

        public PropVariant(float value)
            : base(VariantType.Float)
        {
            fltVal = value;
        }

        public PropVariant(double value)
            : base(VariantType.Double)
        {
            doubleValue = value;
        }

        public PropVariant(long value)
            : base(VariantType.Int64)
        {
            longValue = value;
        }

        [CLSCompliant(false)]
        public PropVariant(ulong value)
            : base(VariantType.UInt64)
        {
            ulongValue = value;
        }

        public PropVariant(Guid value)
            : base(VariantType.Guid)
        {
            ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(value));
            Marshal.StructureToPtr(value, ptr, false);
        }

        public PropVariant(byte[] value)
            : base(VariantType.Blob)
        {
            blobValue.cbSize = value.Length;
            blobValue.pBlobData = Marshal.AllocCoTaskMem(value.Length);
            Marshal.Copy(value, 0, blobValue.pBlobData, value.Length);
        }

        public PropVariant(object value)
            : base(VariantType.IUnknown)
        {
            if (value == null)
            {
                ptr = IntPtr.Zero;
            }
            else if (Marshal.IsComObject(value))
            {
                ptr = Marshal.GetIUnknownForObject(value);
            }
            else
            {
                type = VariantType.Blob;

                blobValue.cbSize = Marshal.SizeOf(value);
                blobValue.pBlobData = Marshal.AllocCoTaskMem(blobValue.cbSize);

                Marshal.StructureToPtr(value, blobValue.pBlobData, false);
            }
        }

        public PropVariant(IntPtr value)
            : base(VariantType.None)
        {
            Marshal.PtrToStructure(value, this);
        }

        public PropVariant(ConstPropVariant value)
        {
            if (value != null)
            {
                PropVariantCopy(this, value);
            }
            else
            {
                throw new NullReferenceException("null passed to PropVariant constructor");
            }
        }

        ~PropVariant()
        {
            Dispose(false);
        }

        public void Clear()
        {
            PropVariantClear(this);
        }

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            Clear();
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public class FourCC
    {
        private int m_fourCC;

        public FourCC(string fcc)
        {
            if (fcc.Length != 4)
            {
                throw new ArgumentException(fcc + " is not a valid FourCC");
            }

            byte[] asc = Encoding.ASCII.GetBytes(fcc);

            LoadFromBytes(asc[0], asc[1], asc[2], asc[3]);
        }

        public FourCC(char a, char b, char c, char d)
        {
            LoadFromBytes((byte)a, (byte)b, (byte)c, (byte)d);
        }

        public FourCC(int fcc)
        {
            m_fourCC = fcc;
        }

        public FourCC(byte a, byte b, byte c, byte d)
        {
            LoadFromBytes(a, b, c, d);
        }

        public FourCC(Guid g)
        {
            if (!IsA4ccSubtype(g))
            {
                throw new Exception("Not a FourCC Guid");
            }

            byte[] asc;
            asc = g.ToByteArray();

            LoadFromBytes(asc[0], asc[1], asc[2], asc[3]);
        }

        public void LoadFromBytes(byte a, byte b, byte c, byte d)
        {
            m_fourCC = a | (b << 8) | (c << 16) | (d << 24);
        }

        public int ToInt32()
        {
            return m_fourCC;
        }

        public static explicit operator int(FourCC f)
        {
            return f.ToInt32();
        }

        public static explicit operator Guid(FourCC f)
        {
            return f.ToMediaSubtype();
        }

        public Guid ToMediaSubtype()
        {
            return new Guid(m_fourCC, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
        }

        public static bool operator ==(FourCC fcc1, FourCC fcc2)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(fcc1, fcc2))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)fcc1 == null) || ((object)fcc2 == null))
            {
                return false;
            }

            return fcc1.m_fourCC == fcc2.m_fourCC;
        }

        public static bool operator !=(FourCC fcc1, FourCC fcc2)
        {
            return !(fcc1 == fcc2);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FourCC))
                return false;

            return (obj as FourCC).m_fourCC == m_fourCC;
        }

        public override int GetHashCode()
        {
            return m_fourCC.GetHashCode();
        }

        public override string ToString()
        {
            char c;
            char[] ca = new char[4];

            c = Convert.ToChar(m_fourCC & 255);
            if (!Char.IsLetterOrDigit(c))
            {
                c = ' ';
            }
            ca[0] = c;

            c = Convert.ToChar((m_fourCC >> 8) & 255);
            if (!Char.IsLetterOrDigit(c))
            {
                c = ' ';
            }
            ca[1] = c;

            c = Convert.ToChar((m_fourCC >> 16) & 255);
            if (!Char.IsLetterOrDigit(c))
            {
                c = ' ';
            }
            ca[2] = c;

            c = Convert.ToChar((m_fourCC >> 24) & 255);
            if (!Char.IsLetterOrDigit(c))
            {
                c = ' ';
            }
            ca[3] = c;

            string s = new string(ca);

            return s;
        }

        public static bool IsA4ccSubtype(Guid g)
        {
            return (g.ToString().EndsWith("-0000-0010-8000-00aa00389b71"));
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1), UnmanagedName("WAVEFORMATEX")]
    public class WaveFormatEx
    {
        public short wFormatTag;
        public short nChannels;
        public int nSamplesPerSec;
        public int nAvgBytesPerSec;
        public short nBlockAlign;
        public short wBitsPerSample;
        public short cbSize;

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", wFormatTag, nChannels, nSamplesPerSec, wBitsPerSample);
        }

        public IntPtr GetPtr()
        {
            IntPtr ip;

            // See what kind of WaveFormat object we've got
            if (this is WaveFormatExtensibleWithData)
            {
                int iExtensibleSize = Marshal.SizeOf(typeof(WaveFormatExtensible));
                int iWaveFormatExSize = Marshal.SizeOf(typeof(WaveFormatEx));

                // WaveFormatExtensibleWithData - Have to copy the byte array too
                WaveFormatExtensibleWithData pData = this as WaveFormatExtensibleWithData;

                int iExtraBytes = pData.cbSize - (iExtensibleSize - iWaveFormatExSize);

                // Account for copying the array.  This may result in us allocating more bytes than we
                // need (if cbSize < IntPtr.Size), but it prevents us from overrunning the buffer.
                int iUseSize = Math.Max(iExtraBytes, IntPtr.Size);

                // Remember, cbSize include the length of WaveFormatExtensible
                ip = Marshal.AllocCoTaskMem(iExtensibleSize + iUseSize);

                // Copies the waveformatex + waveformatextensible
                Marshal.StructureToPtr(pData, ip, false);

                // Get a pointer to the byte after the copy
                IntPtr ip2 = ip + iExtensibleSize;

                // Copy the extra bytes
                Marshal.Copy(pData.byteData, 0, ip2, pData.cbSize - (iExtensibleSize - iWaveFormatExSize));
            }
            else if (this is WaveFormatExtensible)
            {
                int iWaveFormatExtensibleSize = Marshal.SizeOf(typeof(WaveFormatExtensible));

                // WaveFormatExtensible - Just do a simple copy
                WaveFormatExtensible pExt = this as WaveFormatExtensible;

                ip = Marshal.AllocCoTaskMem(iWaveFormatExtensibleSize);

                Marshal.StructureToPtr(this as WaveFormatExtensible, ip, false);
            }
            else if (this is WaveFormatExWithData)
            {
                int iWaveFormatExSize = Marshal.SizeOf(typeof(WaveFormatEx));

                // WaveFormatExWithData - Have to copy the byte array too
                WaveFormatExWithData pData = this as WaveFormatExWithData;

                // Account for copying the array.  This may result in us allocating more bytes than we
                // need (if cbSize < IntPtr.Size), but it prevents us from overrunning the buffer.
                int iUseSize = Math.Max(pData.cbSize, IntPtr.Size);

                ip = Marshal.AllocCoTaskMem(iWaveFormatExSize + iUseSize);

                Marshal.StructureToPtr(pData, ip, false);

                IntPtr ip2 = ip + iWaveFormatExSize;
                Marshal.Copy(pData.byteData, 0, ip2, pData.cbSize);
            }
            else if (this is WaveFormatEx)
            {
                int iWaveFormatExSize = Marshal.SizeOf(typeof(WaveFormatEx));

                // WaveFormatEx - just do a copy
                ip = Marshal.AllocCoTaskMem(iWaveFormatExSize);
                Marshal.StructureToPtr(this as WaveFormatEx, ip, false);
            }
            else
            {
                // Someone added our custom marshaler to something they shouldn't have
                Debug.Assert(false, "Shouldn't ever get here");
                ip = IntPtr.Zero;
            }

            return ip;
        }

        public static WaveFormatEx PtrToWave(IntPtr pNativeData)
        {
            short wFormatTag = Marshal.ReadInt16(pNativeData);
            WaveFormatEx wfe;

            // WAVE_FORMAT_EXTENSIBLE == -2
            if (wFormatTag != -2)
            {
                short cbSize;

                // By spec, PCM has no cbSize element
                if (wFormatTag != 1)
                {
                    cbSize = Marshal.ReadInt16(pNativeData, 16);
                }
                else
                {
                    cbSize = 0;
                }

                // Does the structure contain extra data?
                if (cbSize == 0)
                {
                    // Create a simple WaveFormatEx struct
                    wfe = new WaveFormatEx();
                    Marshal.PtrToStructure(pNativeData, wfe);

                    // It probably already has the right value, but there is a special case
                    // where it might not, so, just to be safe...
                    wfe.cbSize = 0;
                }
                else
                {
                    WaveFormatExWithData dat = new WaveFormatExWithData();

                    // Manually parse the data into the structure
                    dat.wFormatTag = wFormatTag;
                    dat.nChannels = Marshal.ReadInt16(pNativeData, 2);
                    dat.nSamplesPerSec = Marshal.ReadInt32(pNativeData, 4);
                    dat.nAvgBytesPerSec = Marshal.ReadInt32(pNativeData, 8);
                    dat.nBlockAlign = Marshal.ReadInt16(pNativeData, 12);
                    dat.wBitsPerSample = Marshal.ReadInt16(pNativeData, 14);
                    dat.cbSize = cbSize;

                    dat.byteData = new byte[dat.cbSize];
                    IntPtr ip2 = pNativeData + 18;
                    Marshal.Copy(ip2, dat.byteData, 0, dat.cbSize);

                    wfe = dat as WaveFormatEx;
                }
            }
            else
            {
                short cbSize;
                int extrasize = Marshal.SizeOf(typeof(WaveFormatExtensible)) - Marshal.SizeOf(typeof(WaveFormatEx));

                cbSize = Marshal.ReadInt16(pNativeData, 16);
                if (cbSize == extrasize)
                {
                    WaveFormatExtensible ext = new WaveFormatExtensible();
                    Marshal.PtrToStructure(pNativeData, ext);
                    wfe = ext as WaveFormatEx;
                }
                else
                {
                    WaveFormatExtensibleWithData ext = new WaveFormatExtensibleWithData();
                    int iExtraBytes = cbSize - extrasize;

                    ext.wFormatTag = wFormatTag;
                    ext.nChannels = Marshal.ReadInt16(pNativeData, 2);
                    ext.nSamplesPerSec = Marshal.ReadInt32(pNativeData, 4);
                    ext.nAvgBytesPerSec = Marshal.ReadInt32(pNativeData, 8);
                    ext.nBlockAlign = Marshal.ReadInt16(pNativeData, 12);
                    ext.wBitsPerSample = Marshal.ReadInt16(pNativeData, 14);
                    ext.cbSize = cbSize;

                    ext.wValidBitsPerSample = Marshal.ReadInt16(pNativeData, 18);
                    ext.dwChannelMask = (WaveMask)Marshal.ReadInt16(pNativeData, 20);

                    // Read the Guid
                    byte[] byteGuid = new byte[16];
                    Marshal.Copy(pNativeData + 24, byteGuid, 0, 16);
                    ext.SubFormat = new Guid(byteGuid);

                    ext.byteData = new byte[iExtraBytes];
                    IntPtr ip2 = pNativeData + Marshal.SizeOf(typeof(WaveFormatExtensible));
                    Marshal.Copy(ip2, ext.byteData, 0, iExtraBytes);

                    wfe = ext as WaveFormatEx;
                }
            }

            return wfe;
        }

        public static bool operator ==(WaveFormatEx a, WaveFormatEx b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            bool bRet;
            Type t1 = a.GetType();
            Type t2 = b.GetType();

            if (t1 == t2 &&
                a.wFormatTag == b.wFormatTag &&
                a.nChannels == b.nChannels &&
                a.nSamplesPerSec == b.nSamplesPerSec &&
                a.nAvgBytesPerSec == b.nAvgBytesPerSec &&
                a.nBlockAlign == b.nBlockAlign &&
                a.wBitsPerSample == b.wBitsPerSample &&
                a.cbSize == b.cbSize)
            {
                bRet = true;
            }
            else
            {
                bRet = false;
            }

            return bRet;
        }

        public static bool operator !=(WaveFormatEx a, WaveFormatEx b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return this == (obj as WaveFormatEx);
        }

        public override int GetHashCode()
        {
            return nAvgBytesPerSec + wFormatTag;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1), UnmanagedName("WAVEFORMATEX")]
    public class WaveFormatExWithData : WaveFormatEx
    {
        public byte[] byteData;

        public static bool operator ==(WaveFormatExWithData a, WaveFormatExWithData b)
        {
            bool bRet = ((WaveFormatEx)a) == ((WaveFormatEx)b);

            if (bRet)
            {
                if (b.byteData == null)
                {
                    bRet = a.byteData == null;
                }
                else
                {
                    if (b.byteData.Length == a.byteData.Length)
                    {
                        for (int x = 0; x < b.byteData.Length; x++)
                        {
                            if (b.byteData[x] != a.byteData[x])
                            {
                                bRet = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        bRet = false;
                    }
                }
            }

            return bRet;
        }

        public static bool operator !=(WaveFormatExWithData a, WaveFormatExWithData b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return this == (obj as WaveFormatExWithData);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1), UnmanagedName("WAVEFORMATEXTENSIBLE")]
    public class WaveFormatExtensible : WaveFormatEx
    {
        [FieldOffset(0)]
        public short wValidBitsPerSample;
        [FieldOffset(0)]
        public short wSamplesPerBlock;
        [FieldOffset(0)]
        public short wReserved;
        [FieldOffset(2)]
        public WaveMask dwChannelMask;
        [FieldOffset(6)]
        public Guid SubFormat;

        public static bool operator ==(WaveFormatExtensible a, WaveFormatExtensible b)
        {
            bool bRet = ((WaveFormatEx)a) == ((WaveFormatEx)b);

            if (bRet)
            {
                bRet = (a.wValidBitsPerSample == b.wValidBitsPerSample &&
                    a.dwChannelMask == b.dwChannelMask &&
                    a.SubFormat == b.SubFormat);
            }

            return bRet;
        }

        public static bool operator !=(WaveFormatExtensible a, WaveFormatExtensible b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return this == (obj as WaveFormatExtensible);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1), UnmanagedName("WAVEFORMATEX")]
    public class WaveFormatExtensibleWithData : WaveFormatExtensible
    {
        public byte[] byteData;

        public static bool operator ==(WaveFormatExtensibleWithData a, WaveFormatExtensibleWithData b)
        {
            bool bRet = ((WaveFormatExtensible)a) == ((WaveFormatExtensible)b);

            if (bRet)
            {
                if (b.byteData == null)
                {
                    bRet = a.byteData == null;
                }
                else
                {
                    if (b.byteData.Length == a.byteData.Length)
                    {
                        for (int x = 0; x < b.byteData.Length; x++)
                        {
                            if (b.byteData[x] != a.byteData[x])
                            {
                                bRet = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        bRet = false;
                    }
                }
            }

            return bRet;
        }

        public static bool operator !=(WaveFormatExtensibleWithData a, WaveFormatExtensibleWithData b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return this == (obj as WaveFormatExtensibleWithData);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4), UnmanagedName("BITMAPINFOHEADER")]
    public class BitmapInfoHeader
    {
        public int Size;
        public int Width;
        public int Height;
        public short Planes;
        public short BitCount;
        public int Compression;
        public int ImageSize;
        public int XPelsPerMeter;
        public int YPelsPerMeter;
        public int ClrUsed;
        public int ClrImportant;

        public IntPtr GetPtr()
        {
            IntPtr ip;

            // See what kind of BitmapInfoHeader object we've got
            if (this is BitmapInfoHeaderWithData)
            {
                int iBitmapInfoHeaderSize = Marshal.SizeOf(typeof(BitmapInfoHeader));

                // BitmapInfoHeaderWithData - Have to copy the array too
                BitmapInfoHeaderWithData pData = this as BitmapInfoHeaderWithData;

                // Account for copying the array.  This may result in us allocating more bytes than we
                // need (if cbSize < IntPtr.Size), but it prevents us from overrunning the buffer.
                int iUseSize = Math.Max(pData.bmiColors.Length * 4, IntPtr.Size);

                ip = Marshal.AllocCoTaskMem(iBitmapInfoHeaderSize + iUseSize);

                Marshal.StructureToPtr(pData, ip, false);

                IntPtr ip2 = ip + iBitmapInfoHeaderSize;
                Marshal.Copy(pData.bmiColors, 0, ip2, pData.bmiColors.Length);
            }
            else if (this is BitmapInfoHeader)
            {
                int iBitmapInfoHeaderSize = Marshal.SizeOf(typeof(BitmapInfoHeader));

                // BitmapInfoHeader - just do a copy
                ip = Marshal.AllocCoTaskMem(iBitmapInfoHeaderSize);
                Marshal.StructureToPtr(this as BitmapInfoHeader, ip, false);
            }
            else
            {
                Debug.Assert(false, "Shouldn't ever get here");
                ip = IntPtr.Zero;
            }

            return ip;
        }

        public static BitmapInfoHeader PtrToBMI(IntPtr pNativeData)
        {
            int iEntries;
            int biCompression;
            int biClrUsed;
            int biBitCount;

            biBitCount = Marshal.ReadInt16(pNativeData, 14);
            biCompression = Marshal.ReadInt32(pNativeData, 16);
            biClrUsed = Marshal.ReadInt32(pNativeData, 32);

            if (biCompression == 3) // BI_BITFIELDS
            {
                iEntries = 3;
            }
            else if (biClrUsed > 0)
            {
                iEntries = biClrUsed;
            }
            else if (biBitCount <= 8)
            {
                iEntries = 1 << biBitCount;
            }
            else
            {
                iEntries = 0;
            }

            BitmapInfoHeader bmi;

            if (iEntries == 0)
            {
                // Create a simple BitmapInfoHeader struct
                bmi = new BitmapInfoHeader();
                Marshal.PtrToStructure(pNativeData, bmi);
            }
            else
            {
                BitmapInfoHeaderWithData ext = new BitmapInfoHeaderWithData();

                ext.Size = Marshal.ReadInt32(pNativeData, 0);
                ext.Width = Marshal.ReadInt32(pNativeData, 4);
                ext.Height = Marshal.ReadInt32(pNativeData, 8);
                ext.Planes = Marshal.ReadInt16(pNativeData, 12);
                ext.BitCount = Marshal.ReadInt16(pNativeData, 14);
                ext.Compression = Marshal.ReadInt32(pNativeData, 16);
                ext.ImageSize = Marshal.ReadInt32(pNativeData, 20);
                ext.XPelsPerMeter = Marshal.ReadInt32(pNativeData, 24);
                ext.YPelsPerMeter = Marshal.ReadInt32(pNativeData, 28);
                ext.ClrUsed = Marshal.ReadInt32(pNativeData, 32);
                ext.ClrImportant = Marshal.ReadInt32(pNativeData, 36);

                bmi = ext as BitmapInfoHeader;

                ext.bmiColors = new int[iEntries];
                IntPtr ip2 = pNativeData + Marshal.SizeOf(typeof(BitmapInfoHeader));
                Marshal.Copy(ip2, ext.bmiColors, 0, iEntries);
            }

            return bmi;
        }

        public void CopyFrom(BitmapInfoHeader bmi)
        {
            Size = bmi.Size;
            Width = bmi.Width;
            Height = bmi.Height;
            Planes = bmi.Planes;
            BitCount = bmi.BitCount;
            Compression = bmi.Compression;
            ImageSize = bmi.ImageSize;
            YPelsPerMeter = bmi.YPelsPerMeter;
            ClrUsed = bmi.ClrUsed;
            ClrImportant = bmi.ClrImportant;

            if (bmi is BitmapInfoHeaderWithData)
            {
                BitmapInfoHeaderWithData ext = this as BitmapInfoHeaderWithData;
                BitmapInfoHeaderWithData ext2 = bmi as BitmapInfoHeaderWithData;

                ext.bmiColors = new int[ext2.bmiColors.Length];
                ext2.bmiColors.CopyTo(ext.bmiColors, 0);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4), UnmanagedName("BITMAPINFO")]
    public class BitmapInfoHeaderWithData : BitmapInfoHeader
    {
        public int[] bmiColors;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class MFInt
    {
        protected int m_value;

        public MFInt()
            : this(0)
        {
        }

        public MFInt(int v)
        {
            m_value = v;
        }

        public int GetValue()
        {
            return m_value;
        }

        // While I *could* enable this code, it almost certainly won't do what you
        // think it will.  Generally you don't want to create a *new* instance of
        // MFInt and assign a value to it.  You want to assign a value to an
        // existing instance.  In order to do this automatically, .Net would have
        // to support overloading operator =.  But since it doesn't, use Assign()

        //public static implicit operator MFInt(int f)
        //{
        //    return new MFInt(f);
        //}

        public static implicit operator int(MFInt f)
        {
            return f.m_value;
        }

        public int ToInt32()
        {
            return m_value;
        }

        public void Assign(int f)
        {
            m_value = f;
        }

        public override string ToString()
        {
            return m_value.ToString();
        }

        public override int GetHashCode()
        {
            return m_value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is MFInt)
            {
                return ((MFInt)obj).m_value == m_value;
            }

            return Convert.ToInt32(obj) == m_value;
        }
    }

    /// <summary>
    /// MFRect is a managed representation of the Win32 RECT structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class MFRect
    {
        public int left;
        public int top;
        public int right;
        public int bottom;

        /// <summary>
        /// Empty contructor. Initialize all fields to 0
        /// </summary>
        public MFRect()
        {
        }

        /// <summary>
        /// A parametred constructor. Initialize fields with given values.
        /// </summary>
        /// <param name="left">the left value</param>
        /// <param name="top">the top value</param>
        /// <param name="right">the right value</param>
        /// <param name="bottom">the bottom value</param>
        public MFRect(int l, int t, int r, int b)
        {
            left = l;
            top = t;
            right = r;
            bottom = b;
        }

        /// <summary>
        /// A parametred constructor. Initialize fields with a given <see cref="System.Drawing.Rectangle"/>.
        /// </summary>
        /// <param name="rectangle">A <see cref="System.Drawing.Rectangle"/></param>
        /// <remarks>
        /// Warning, MFRect define a rectangle by defining two of his corners and <see cref="System.Drawing.Rectangle"/> define a rectangle with his upper/left corner, his width and his height.
        /// </remarks>
        public MFRect(Rectangle rectangle)
            : this(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom)
        {
        }

        /// <summary>
        /// Provide de string representation of this MFRect instance
        /// </summary>
        /// <returns>A string formated like this : [left, top - right, bottom]</returns>
        public override string ToString()
        {
            return string.Format("[{0}, {1}] - [{2}, {3}]", left, top, right, bottom);
        }

        public override int GetHashCode()
        {
            return left.GetHashCode() |
                top.GetHashCode() |
                right.GetHashCode() |
                bottom.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is MFRect)
            {
                MFRect cmp = (MFRect)obj;

                return right == cmp.right && bottom == cmp.bottom && left == cmp.left && top == cmp.top;
            }

            if (obj is Rectangle)
            {
                Rectangle cmp = (Rectangle)obj;

                return right == cmp.Right && bottom == cmp.Bottom && left == cmp.Left && top == cmp.Top;
            }

            return false;
        }

        /// <summary>
        /// Checks to see if the rectangle is empty
        /// </summary>
        /// <returns>Returns true if the rectangle is empty</returns>
        public bool IsEmpty()
        {
            return (right <= left || bottom <= top);
        }

        /// <summary>
        /// Define implicit cast between MFRect and System.Drawing.Rectangle for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="MFRect.ToRectangle"/> for similar functionality.
        /// <code>
        ///   // Define a new Rectangle instance
        ///   Rectangle r = new Rectangle(0, 0, 100, 100);
        ///   // Do implicit cast between Rectangle and MFRect
        ///   MFRect mfR = r;
        ///
        ///   Console.WriteLine(mfR.ToString());
        /// </code>
        /// </summary>
        /// <param name="r">a MFRect to be cast</param>
        /// <returns>A casted System.Drawing.Rectangle</returns>
        public static implicit operator Rectangle(MFRect r)
        {
            return r.ToRectangle();
        }

        /// <summary>
        /// Define implicit cast between System.Drawing.Rectangle and MFRect for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="MFRect.FromRectangle"/> for similar functionality.
        /// <code>
        ///   // Define a new MFRect instance
        ///   MFRect mfR = new MFRect(0, 0, 100, 100);
        ///   // Do implicit cast between MFRect and Rectangle
        ///   Rectangle r = mfR;
        ///
        ///   Console.WriteLine(r.ToString());
        /// </code>
        /// </summary>
        /// <param name="r">A System.Drawing.Rectangle to be cast</param>
        /// <returns>A casted MFRect</returns>
        public static implicit operator MFRect(Rectangle r)
        {
            return new MFRect(r);
        }

        /// <summary>
        /// Get the System.Drawing.Rectangle equivalent to this MFRect instance.
        /// </summary>
        /// <returns>A System.Drawing.Rectangle</returns>
        public Rectangle ToRectangle()
        {
            return new Rectangle(left, top, (right - left), (bottom - top));
        }

        /// <summary>
        /// Get a new MFRect instance for a given <see cref="System.Drawing.Rectangle"/>
        /// </summary>
        /// <param name="r">The <see cref="System.Drawing.Rectangle"/> used to initialize this new MFGuid</param>
        /// <returns>A new instance of MFGuid</returns>
        public static MFRect FromRectangle(Rectangle r)
        {
            return new MFRect(r);
        }

        /// <summary>
        /// Copy the members from an MFRect into this object
        /// </summary>
        /// <param name="from">The rectangle from which to copy the values.</param>
        public void CopyFrom(MFRect from)
        {
            left = from.left;
            top = from.top;
            right = from.right;
            bottom = from.bottom;
        }
    }

    /// <summary>
    /// MFGuid is a wrapper class around a System.Guid value type.
    /// </summary>
    /// <remarks>
    /// This class is necessary to enable null paramters passing.
    /// </remarks>
    [StructLayout(LayoutKind.Explicit)]
    public class MFGuid
    {
        [FieldOffset(0)]
        private Guid guid;

        public static readonly MFGuid Empty = Guid.Empty;

        /// <summary>
        /// Empty constructor.
        /// Initialize it with System.Guid.Empty
        /// </summary>
        public MFGuid()
            : this(Empty)
        {
        }

        /// <summary>
        /// Constructor.
        /// Initialize this instance with a given System.Guid string representation.
        /// </summary>
        /// <param name="g">A valid System.Guid as string</param>
        public MFGuid(string g)
            : this(new Guid(g))
        {
        }

        /// <summary>
        /// Constructor.
        /// Initialize this instance with a given System.Guid.
        /// </summary>
        /// <param name="g">A System.Guid value type</param>
        public MFGuid(Guid g)
        {
            this.guid = g;
        }

        /// <summary>
        /// Get a string representation of this MFGuid Instance.
        /// </summary>
        /// <returns>A string representing this instance</returns>
        public override string ToString()
        {
            return this.guid.ToString();
        }

        /// <summary>
        /// Get a string representation of this MFGuid Instance with a specific format.
        /// </summary>
        /// <param name="format"><see cref="System.Guid.ToString"/> for a description of the format parameter.</param>
        /// <returns>A string representing this instance according to the format parameter</returns>
        public string ToString(string format)
        {
            return this.guid.ToString(format);
        }

        public override int GetHashCode()
        {
            return this.guid.GetHashCode();
        }

        /// <summary>
        /// Define implicit cast between MFGuid and System.Guid for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="MFGuid.ToGuid"/> for similar functionality.
        /// <code>
        ///   // Define a new MFGuid instance
        ///   MFGuid mfG = new MFGuid("{33D57EBF-7C9D-435e-A15E-D300B52FBD91}");
        ///   // Do implicit cast between MFGuid and Guid
        ///   Guid g = mfG;
        ///
        ///   Console.WriteLine(g.ToString());
        /// </code>
        /// </summary>
        /// <param name="g">MFGuid to be cast</param>
        /// <returns>A casted System.Guid</returns>
        public static implicit operator Guid(MFGuid g)
        {
            return g.guid;
        }

        /// <summary>
        /// Define implicit cast between System.Guid and MFGuid for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="MFGuid.FromGuid"/> for similar functionality.
        /// <code>
        ///   // Define a new Guid instance
        ///   Guid g = new Guid("{B9364217-366E-45f8-AA2D-B0ED9E7D932D}");
        ///   // Do implicit cast between Guid and MFGuid
        ///   MFGuid mfG = g;
        ///
        ///   Console.WriteLine(mfG.ToString());
        /// </code>
        /// </summary>
        /// <param name="g">System.Guid to be cast</param>
        /// <returns>A casted MFGuid</returns>
        public static implicit operator MFGuid(Guid g)
        {
            return new MFGuid(g);
        }

        /// <summary>
        /// Get the System.Guid equivalent to this MFGuid instance.
        /// </summary>
        /// <returns>A System.Guid</returns>
        public Guid ToGuid()
        {
            return this.guid;
        }

        /// <summary>
        /// Get a new MFGuid instance for a given System.Guid
        /// </summary>
        /// <param name="g">The System.Guid to wrap into a MFGuid</param>
        /// <returns>A new instance of MFGuid</returns>
        public static MFGuid FromGuid(Guid g)
        {
            return new MFGuid(g);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4), UnmanagedName("SIZE")]
    public class MFSize
    {
        public int cx;
        public int cy;

        public MFSize()
        {
            cx = 0;
            cy = 0;
        }

        public MFSize(int iWidth, int iHeight)
        {
            cx = iWidth;
            cy = iHeight;
        }

        public int Width
        {
            get
            {
                return cx;
            }
            set
            {
                cx = value;
            }
        }
        public int Height
        {
            get
            {
                return cy;
            }
            set
            {
                cy = value;
            }
        }
    }

    #endregion

    #region Utility Classes

    public static class MFDump
    {
        public static string LookupName(Type t, Guid gSeeking)
        {
            System.Reflection.FieldInfo[] fia = t.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

            foreach (System.Reflection.FieldInfo fi in fia)
            {
                if (gSeeking.CompareTo(fi.GetValue(t)) == 0)
                    return fi.Name;
            }

            return gSeeking.ToString();
        }

        public static string DumpAttribs(IMFAttributes ia)
        {
            if (ia != null)
            {
                int iCount;
                PropVariant pv = new PropVariant();
                Guid g;
                StringBuilder sb = new StringBuilder(1024);

                HResult hr = ia.GetCount(out iCount);
                MFError.ThrowExceptionForHR(hr);

                for (int x = 0; x < iCount; x++)
                {
                    hr = ia.GetItemByIndex(x, out g, pv);
                    MFError.ThrowExceptionForHR(hr);

                    sb.AppendFormat("{0} {1} {2}\n", LookupName(typeof(MFAttributesClsid), g), pv.GetMFAttributeType(), AttribValueToString(g, pv));
                }

                return sb.ToString();
            }
            else
                return "<null>\n";
        }

        [CLSCompliant(false)]
        public static string UnpackRatio(ulong l)
        {
            int w, h;
            MFExtern.Unpack2UINT32AsUINT64((long)l, out w, out h);

            return string.Format("{0}x{1}", w, h);
        }

        public static string AttribValueToString(Guid gAttrib, ConstPropVariant pv)
        {
            if (gAttrib == MFAttributesClsid.MF_MT_MAJOR_TYPE || gAttrib == MFAttributesClsid.MF_MT_SUBTYPE)
                return LookupName(typeof(MFMediaType), pv.GetGuid());

            else if (gAttrib == MFAttributesClsid.MF_TRANSFORM_CATEGORY_Attribute)
                return LookupName(typeof(MFTransformCategory), pv.GetGuid());

            else if (gAttrib == MFAttributesClsid.MF_TRANSCODE_CONTAINERTYPE)
                return LookupName(typeof(MFTranscodeContainerType), pv.GetGuid());

            else if (gAttrib == MFAttributesClsid.MF_EVENT_TOPOLOGY_STATUS)
                return ((MFTopoStatus)pv.GetUInt()).ToString();

            else if (gAttrib == MFAttributesClsid.MF_MT_INTERLACE_MODE)
                return ((MFVideoInterlaceMode)pv.GetUInt()).ToString();

            else if (gAttrib == MFAttributesClsid.MF_TOPOLOGY_DXVA_MODE)
                return ((MFTOPOLOGY_DXVA_MODE)pv.GetUInt()).ToString();

            else if (gAttrib == MFAttributesClsid.MF_MT_FRAME_SIZE ||
                     gAttrib == MFAttributesClsid.MF_MT_FRAME_RATE ||
                     gAttrib == MFAttributesClsid.MF_MT_PIXEL_ASPECT_RATIO)
                return UnpackRatio(pv.GetULong());

            return pv.ToString();
        }
    }

    [Serializable()]
    public class MFException : Exception, System.Runtime.Serialization.ISerializable
    {
        private string m_Message;

        public MFException(HResult hr) : this((int)hr)
        {
        }

        public MFException(int hr)
            : base()
        {
            this.HResult = hr;
        }

        // This constructor is used for deserialization.
        private MFException(
            System.Runtime.Serialization.SerializationInfo info, 
            System.Runtime.Serialization.StreamingContext context) :
                base( info, context )
        {
        }
        
        public override string Message
        {
            get
            {
                if (m_Message == null)
                    m_Message = MFError.GetErrorText(this.HResult);

                return m_Message;
            }
        }
    }

    public class MFError
    {
        #region externs

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "FormatMessageW", ExactSpelling = true, SetLastError = true), SuppressUnmanagedCodeSecurity]
        private static extern int FormatMessage(
            FormatMessageFlags dwFlags,
            IntPtr lpSource,
            int dwMessageId,
            int dwLanguageId,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpBuffer,
            int nSize,
            IntPtr[] Arguments);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "LoadLibraryExW", SetLastError = true, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
        private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, LoadLibraryExFlags dwFlags);

        #endregion

        #region Declarations

        [Flags, UnmanagedName("#defines in WinBase.h")]
        private enum LoadLibraryExFlags
        {
            DontResolveDllReferences = 0x00000001,
            LoadLibraryAsDataFile = 0x00000002,
            LoadWithAlteredSearchPath = 0x00000008,
            LoadIgnoreCodeAuthzLevel = 0x00000010,
            LoadLibrarySearchSystem32 = 0x00000800, // search system32 dir
        }

        [Flags, UnmanagedName("FORMAT_MESSAGE_* defines")]
        private enum FormatMessageFlags
        {
            AllocateBuffer = 0x00000100,
            IgnoreInserts = 0x00000200,
            FromString = 0x00000400,
            FromHmodule = 0x00000800,
            FromSystem = 0x00001000,
            ArgumentArray = 0x00002000,
            MaxWidthMask = 0x000000FF
        }

        #endregion

        private static IntPtr s_hModule = IntPtr.Zero;
        private static readonly string MESSAGEFILE = "mferror.dll";
        private static readonly MFError s_OK = new MFError();

        private HResult LastError;

        /// <summary>
        /// Construct an empty MFError
        /// </summary>
        public MFError()
            : this(HResult.S_OK)
        {
        }

        /// <summary>
        /// Construct an MFError from an int
        /// </summary>
        /// <param name="hr">an int to turn into an MFError</param>
        public MFError(int hr)
            : this((HResult)hr)
        {
        }

        /// <summary>
        /// Construct an MFError from an MFHResult
        /// </summary>
        /// <param name="hr">an MFHResult to turn into an MFError</param>
        public MFError(HResult hr)
        {
            LastError = hr;
        }

        private static MFError MakeOne(int hr)
        {
            MFError m;

            if (hr == 0)
            {
                m = s_OK;
            }
            else if (hr < 0)
            {
                throw new MFException(hr);
            }
            else
            {
                m = new MFError(hr);
            }

            return m;
        }

        /// <summary>
        /// If hr is a fatal error, an exception is thrown, otherwise an MFError is returned.  This
        /// allows you to check for status codes (like MF_S_PROTECTION_NOT_REQUIRED).
        /// </summary>
        /// <example>
        /// // Throws an exception if the create fails.
        /// MFError hrthrow = new MFError();
        /// hrthrow = MFExtern.MFCreateAttributes(out ia, 6);
        /// </example>
        /// <param name="hr">The value from which to construct the MFError.</param>
        /// <returns>The new MFError</returns>
        public static implicit operator MFError(int hr)
        {
            return MakeOne(hr);
        }
        public static implicit operator MFError(HResult hr)
        {
            return MakeOne((int)hr);
        }

        /// <summary>
        /// Convert an MFError back to an integer
        /// </summary>
        /// <param name="hr">The MFError from which to retrieve the integer</param>
        /// <returns>The MFError as an integer</returns>
        public static implicit operator int(MFError hr)
        {
            return (int)hr.LastError;
        }
        public static implicit operator HResult(MFError hr)
        {
            return hr.LastError;
        }

        /// <summary>
        /// Convert the MFError to the error string
        /// </summary>
        /// <returns>The string</returns>
        public override string ToString()
        {
            return GetErrorText(LastError);
        }

        #region static methods

        public bool Failed()
        {
            return Failed(LastError);
        }
        public static bool Failed(int hr)
        {
            return hr < 0;
        }
        public static bool Failed(HResult hr)
        {
            return Failed((int)hr);
        }

        public bool Succeeded()
        {
            return Succeeded(LastError);
        }
        public static bool Succeeded(HResult hr)
        {
            return Succeeded((int)hr);
        }
        public static bool Succeeded(int hr)
        {
            return hr >= 0;
        }

        public string GetErrorText()
        {
            return GetErrorText(LastError);
        }
        public static string GetErrorText(HResult hr)
        {
            return GetErrorText((int)hr);
        }
        [SecurityCritical]
        public static string GetErrorText(int hr)
        {
            // Make sure the resouce dll is loaded.
            if (s_hModule == IntPtr.Zero)
            {
                // Deal with possible multi-threading problems
                lock (MESSAGEFILE)
                {
                    // Make sure it didn't get set while we waited for the lock
                    if (s_hModule == IntPtr.Zero)
                    {
                        LoadLibraryExFlags f = LoadLibraryExFlags.LoadLibraryAsDataFile | LoadLibraryExFlags.LoadLibrarySearchSystem32;
                        // Load the Media Foundation error message dll
                        s_hModule = LoadLibraryEx(MESSAGEFILE, IntPtr.Zero, f);

                        // LoadLibraryExFlags.LoadLibrarySearchSystem32 may not be supported
                        if (s_hModule == IntPtr.Zero && Marshal.GetLastWin32Error() == 87)
                        {
                            // Perhaps KB2533623 is not installed.  Try again.
                            s_hModule = LoadLibraryEx(MESSAGEFILE, IntPtr.Zero, LoadLibraryExFlags.LoadLibraryAsDataFile);
                        }
                    }
                }
            }

            // Get the text (always returns something)
            string sText = GetErrorTextFromDll(hr, s_hModule);

            return sText;
        }
        [SecurityCritical]
        private static string GetErrorTextFromDll(int hr, IntPtr hLib)
        {
            // Most strings fit in this, but we'll resize if needed.
            StringBuilder sb = new StringBuilder(128);

            FormatMessageFlags dwFormatFlags =
                FormatMessageFlags.IgnoreInserts |
                FormatMessageFlags.MaxWidthMask |
                FormatMessageFlags.FromSystem;

            if (hLib != IntPtr.Zero)
            {
                dwFormatFlags |= FormatMessageFlags.FromHmodule;
            }

            while (true)
            {
                int dwBufferLength = FormatMessage(
                    dwFormatFlags,
                    hLib, // module to get message from (NULL == system)
                    hr, // error number to get message for
                    0, // default language
                    sb,
                    sb.Capacity,
                    null
                    );

                // Got the message.
                if (dwBufferLength > 0)
                {
                    return sb.ToString();
                }

                // Error of some kind (possibly 'not found').
                if (Marshal.GetLastWin32Error() != 122) // 122 = buffer too small
                {
                    // Try getting the name from the enum
                    HResult m = (HResult)hr;
                    string s = m.ToString();

                    // If that didn't work, just use the hex value
                    if (s == hr.ToString())
                        s = string.Format("Unknown error 0x{0:X}", hr);

                    return s;
                }

                // Need a bigger buffer.
                sb = new StringBuilder(sb.Capacity * 2);
            }
        }

        public void ThrowExceptionForHR()
        {
            ThrowExceptionForHR(LastError);
        }

        public static void ThrowExceptionForHR(HResult hr)
        {
            ThrowExceptionForHR((int)hr);
        }

        /// <summary>
        /// If hr has a "failed" status code (E_*), throw an exception.  Note that status
        /// messages (S_*) are not considered failure codes.  If MediaFoundation error text
        /// is available, it is used to build the exception, otherwise a generic com error
        /// is thrown.
        /// </summary>
        /// <param name="hr">The HRESULT to check</param>
        public static void ThrowExceptionForHR(int hr)
        {
            // If a severe error has occurred
            if (hr < 0)
            {
                throw new MFException(hr);
            }
        }

        #endregion
    }

    abstract public class COMBase
    {
        public static bool Succeeded(HResult hr)
        {
            return hr >= 0;
        }

        public static bool Failed(HResult hr)
        {
            return hr < 0;
        }

        public static void SafeRelease(object o)
        {
            if (o != null)
            {
                if (Marshal.IsComObject(o))
                {
                    int i = Marshal.ReleaseComObject(o);
                    Debug.Assert(i >= 0);
                }
                else
                {
                    IDisposable iDis = o as IDisposable;
                    if (iDis != null)
                    {
                        iDis.Dispose();
                    }
                    else
                    {
                        throw new Exception("What the heck was that?");
                    }
                }
            }
        }

        [Conditional("DEBUG")]
        public static void TRACE(string s)
        {
            Debug.WriteLine(s);
        }
    }

    #endregion

    // These classes are used internally and there is probably no reason you
    // will ever need to use them directly.

    #region Internal classes

    // PVMarshaler - Class to marshal PropVariants on parameters that
    // *output* PropVariants.

    // When defining parameters that use this marshaler, you must always
    // declare them as both [In] and [Out].  This will result in *both*
    // MarshalManagedToNative and MarshalNativeToManaged being called.  Since
    // the order they are called depends on exactly what's happening,
    // m_InProcess lets us know which way things are being called.
    //
    // Managed calling unmanaged: 
    // In this case, MarshalManagedToNative is called first with m_InProcess 
    // == 0.  When MarshalManagedToNative is called, we store the managed
    // object (so we know where to copy it back to), then we clear the variant,
    // allocate some COM memory and pass a pointer to the COM memory to the
    // native code.  When the native code is done, MarshalNativeToManaged gets
    // called (m_InProcess == 1) with the pointer to the COM memory.  At that
    // point, we copy the contents back into the (saved) managed object. After
    // that, CleanUpNativeData gets called and we release the COM memory.
    //
    // Unmanaged calling managed:
    // In this case, MarshalNativeToManaged is called first.  We store the
    // native pointer (so we know where to copy the result back to), then we
    // create a managed PropVariant and copy the native value into it.  When
    // the managed code is done, MarshalManagedToNative gets called with the
    // managed PropVariant we created.  At that point, we copy the contents
    // of the managed PropVariant back into the (saved) native pointer.
    //
    // Multi-threading:
    // When marshaling from managed to native, the first thing that happens
    // is the .Net creates an instance of the PVMarshaler class.  It then
    // calls MarshalManagedToNative to send you the managed object into which
    // the unmanaged data will eventually be stored. However it doesn't pass
    // the managed object again when MarshalNativeToManaged eventually gets
    // called.  No problem, you assume, I'll just store it as a data member
    // and use it when MarshalNativeToManaged get called.  Yeah, about that...
    // First of all, if several threads all start calling a method that uses
    // PVMarshaler, .Net WILL create multiple instances of this class.
    // However, it will then DESTRUCT all of them except 1, which it will use
    // from every thread.  Unless you are aware of this behavior and take
    // precautions, having multiple thread using the same instance results in
    // chaos.
    // Also be aware that if two different methods both use PVMarshaler (say
    // GetItem and GetItemByIndex on IMFAttributes), .Net may use the same
    // instance for both methods.  Unless they each have a unique MarshalCookie
    // string.  Using a unique MarshalCookie doesn't help with multi-threading,
    // but it does help keep the marshaling from one method call from
    // interfering with another.
    //
    // Recursion:
    // If managed code calls unmanaged code thru PVMarshaler, then that
    // unmanaged code in turn calls IMFAttribute::GetItem against a managed
    // object, what happens?  .Net will use a single instance of PVMarshaler to
    // handle both calls, even if the actual PropVariant used for the second
    // call is a different instance of the PropVariant class.  It can also use
    // the same managed thread id all the way thru (so using a simple
    // ThreadStatic is not sufficient to keep track of this).  So if you see a
    // call to MarshalNativeToManaged right after a call to
    // MarshalManagedToNative, it might be the second half of a 'normal' bit of
    // marshaling, or it could be the start of a nested call from unmanaged
    // back into managed.
    // There are 2 ways to detect nesting:
    // 1) If the pNativeData sent to MarshalNativeToManaged *isn't* the one
    // you returned from MarshalManagedToNative, you are nesting.
    // 2) m_InProcsss starts at 0.  MarshalManagedToNative increments it to 1.
    // Then MarshalNativeToManaged increments it to 2.  For non-nested, that
    // should be the end.  So if MarshalManagedToNative gets called with
    // m_InProcsss == 2, we are nesting.
    //
    // Warning!  You cannot assume that both marshaling routines will always
    // get called.  For example if calling from unmanaged to managed,
    // MarshalNativeToManaged will get called, but if the managed code throws
    // an exception, MarshalManagedToNative will not.  This can be a problem
    // since .Net REUSES instances of the marshaler.  So it is essential that
    // class members always get cleaned up in CleanUpManagedData &
    // CleanUpNativeData.
    //
    // All this helps explain the otherwise inexplicable complexity of this
    // class:  It uses a ThreadStatic variable to keep instance data from one
    // thread from interfering with another thread, nests instances of MyProps,
    // and uses 2 different methods to check for recursion (which in theory
    // could be nested quite deeply).
    internal class PVMarshaler : ICustomMarshaler
    {
        private class MyProps
        {
            public PropVariant m_obj;
            public IntPtr m_ptr;

            private int m_InProcsss;
            private bool m_IAllocated;
            private MyProps m_Parent = null;

            [ThreadStatic]
            private static MyProps[] m_CurrentProps;

            public int GetStage()
            {
                return m_InProcsss;
            }

            public void StageComplete()
            {
                m_InProcsss++;
            }

            public static MyProps AddLayer(int iIndex)
            {
                MyProps p = new MyProps();
                p.m_Parent = m_CurrentProps[iIndex];
                m_CurrentProps[iIndex] = p;

                return p;
            }

            public static void SplitLayer(int iIndex)
            {
                MyProps t = AddLayer(iIndex);
                MyProps p = t.m_Parent;

                t.m_InProcsss = 1;
                t.m_ptr = p.m_ptr;
                t.m_obj = p.m_obj;

                p.m_InProcsss = 1;
            }

            public static MyProps GetTop(int iIndex)
            {
                // If the member hasn't been initialized, do it now.  And no, we can't
                // do this in the PVMarshaler constructor, since the constructor may 
                // have been called on a different thread.
                if (m_CurrentProps == null)
                {
                    m_CurrentProps = new MyProps[MaxArgs];
                    for (int x = 0; x < MaxArgs; x++)
                    {
                        m_CurrentProps[x] = new MyProps();
                    }
                }
                return m_CurrentProps[iIndex];
            }

            public void Clear(int iIndex)
            {
                if (m_IAllocated)
                {
                    Marshal.FreeCoTaskMem(m_ptr);
                    m_IAllocated = false;
                }
                if (m_Parent == null)
                {
                    // Never delete the last entry.
                    m_InProcsss = 0;
                    m_obj = null;
                    m_ptr = IntPtr.Zero;
                }
                else
                {
                    m_obj = null;
                    m_CurrentProps[iIndex] = m_Parent;
                }
            }

            public IntPtr Alloc(int iSize)
            {
                IntPtr ip = Marshal.AllocCoTaskMem(iSize);
                m_IAllocated = true;
                return ip;
            }
        }

        private readonly int m_Index;

        // Max number of arguments in a single method call that can use
        // PVMarshaler.
        private const int MaxArgs = 2;

        private PVMarshaler(string cookie)
        {
            int iLen = cookie.Length;

            // On methods that have more than 1 PVMarshaler on a
            // single method, the cookie is in the form:
            // InterfaceName.MethodName.0 & InterfaceName.MethodName.1.
            if (cookie[iLen - 2] != '.')
            {
                m_Index = 0;
            }
            else
            {
                m_Index = int.Parse(cookie.Substring(iLen - 1));
                Debug.Assert(m_Index < MaxArgs);
            }
        }

        public IntPtr MarshalManagedToNative(object managedObj)
        {
            // Nulls don't invoke custom marshaling.
            Debug.Assert(managedObj != null);

            MyProps t = MyProps.GetTop(m_Index);

            switch (t.GetStage())
            {
                case 0:
                    {
                        // We are just starting a "Managed calling unmanaged"
                        // call.

                        // Cast the object back to a PropVariant and save it
                        // for use in MarshalNativeToManaged.
                        t.m_obj = managedObj as PropVariant;

                        // This could happen if (somehow) managedObj isn't a
                        // PropVariant.  During normal marshaling, the custom
                        // marshaler doesn't get called if the parameter is
                        // null.
                        Debug.Assert(t.m_obj != null);

                        // Release any memory currently allocated in the
                        // PropVariant.  In theory, the (managed) caller
                        // should have done this before making the call that
                        // got us here, but .Net programmers don't generally
                        // think that way.  To avoid any leaks, do it for them.
                        t.m_obj.Clear();

                        // Create an appropriately sized buffer (varies from
                        // x86 to x64).
                        int iSize = GetNativeDataSize();
                        t.m_ptr = t.Alloc(iSize);

                        // Copy in the (empty) PropVariant.  In theory we could
                        // just zero out the first 2 bytes (the VariantType),
                        // but since PropVariantClear wipes the whole struct,
                        // that's what we do here to be safe.
                        Marshal.StructureToPtr(t.m_obj, t.m_ptr, false);

                        break;
                    }
                case 1:
                    {
                        if (!System.Object.ReferenceEquals(t.m_obj, managedObj))
                        {
                            // If we get here, we have already received a call
                            // to MarshalNativeToManaged where we created a
                            // PropVariant and stored it into t.m_obj.  But
                            // the object we just got passed here isn't the
                            // same one.  Therefore instead of being the second
                            // half of an "Unmanaged calling managed" (as
                            // m_InProcsss led us to believe), this is really
                            // the first half of a nested "Managed calling
                            // unmanaged" (see Recursion in the comments at the
                            // top of this class).  Add another layer.
                            MyProps.AddLayer(m_Index);

                            // Try this call again now that we have fixed
                            // m_CurrentProps.
                            return MarshalManagedToNative(managedObj);
                        }

                        // This is (probably) the second half of "Unmanaged
                        // calling managed."  However, it could be the first
                        // half of a nested usage of PropVariants.  If it is a
                        // nested, we'll eventually figure that out in case 2.

                        // Copy the data from the managed object into the
                        // native pointer that we received in
                        // MarshalNativeToManaged.
                        Marshal.StructureToPtr(t.m_obj, t.m_ptr, false);

                        break;
                    }
                case 2:
                    {
                        // Apparently this is 'part 3' of a 2 part call.  Which
                        // means we are doing a nested call.  Normally we would
                        // catch the fact that this is a nested call with the
                        // ReferenceEquals check above.  However, if the same
                        // PropVariant instance is being passed thru again, we
                        // end up here.
                        // So, add a layer.
                        MyProps.SplitLayer(m_Index);

                        // Try this call again now that we have fixed
                        // m_CurrentProps.
                        return MarshalManagedToNative(managedObj);
                    }
                default:
                    {
                        Environment.FailFast("Something horrible has " +
                                             "happened, probaby due to " +
                                             "marshaling of nested " +
                                             "PropVariant calls.");
                        break;
                    }
            }
            t.StageComplete();

            return t.m_ptr;
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            // Nulls don't invoke custom marshaling.
            Debug.Assert(pNativeData != IntPtr.Zero);

            MyProps t = MyProps.GetTop(m_Index);

            switch (t.GetStage())
            {
                case 0:
                    {
                        // We are just starting a "Unmanaged calling managed"
                        // call.

                        // Caller should have cleared variant before calling
                        // us.  Might be acceptable for types *other* than
                        // IUnknown, String, Blob and StringArray, but it is
                        // still bad design.  We're checking for it, but we
                        // work around it.

                        // Read the 16bit VariantType.
                        Debug.Assert(Marshal.ReadInt16(pNativeData) == 0);

                        // Create an empty managed PropVariant without using
                        // pNativeData.
                        t.m_obj = new PropVariant();

                        // Save the pointer for use in MarshalManagedToNative.
                        t.m_ptr = pNativeData;

                        break;
                    }
                case 1:
                    {
                        if (t.m_ptr != pNativeData)
                        {
                            // If we get here, we have already received a call
                            // to MarshalManagedToNative where we created an
                            // IntPtr and stored it into t.m_ptr.  But the
                            // value we just got passed here isn't the same
                            // one.  Therefore instead of being the second half
                            // of a "Managed calling unmanaged" (as m_InProcsss
                            // led us to believe) this is really the first half
                            // of a nested "Unmanaged calling managed" (see
                            // Recursion in the comments at the top of this
                            // class).  Add another layer.
                            MyProps.AddLayer(m_Index);

                            // Try this call again now that we have fixed
                            // m_CurrentProps.
                            return MarshalNativeToManaged(pNativeData);
                        }

                        // This is (probably) the second half of "Managed
                        // calling unmanaged."  However, it could be the first
                        // half of a nested usage of PropVariants.  If it is a
                        // nested, we'll eventually figure that out in case 2.

                        // Copy the data from the native pointer into the
                        // managed object that we received in
                        // MarshalManagedToNative.
                        Marshal.PtrToStructure(pNativeData, t.m_obj);

                        break;
                    }
                case 2:
                    {
                        // Apparently this is 'part 3' of a 2 part call.  Which
                        // means we are doing a nested call.  Normally we would
                        // catch the fact that this is a nested call with the
                        // (t.m_ptr != pNativeData) check above.  However, if
                        // the same PropVariant instance is being passed thru
                        // again, we end up here.  So, add a layer.
                        MyProps.SplitLayer(m_Index);

                        // Try this call again now that we have fixed
                        // m_CurrentProps.
                        return MarshalNativeToManaged(pNativeData);
                    }
                default:
                    {
                        Environment.FailFast("Something horrible has " +
                                             "happened, probaby due to " +
                                             "marshaling of nested " +
                                             "PropVariant calls.");
                        break;
                    }
            }
            t.StageComplete();

            return t.m_obj;
        }

        public void CleanUpManagedData(object ManagedObj)
        {
            // Note that if there are nested calls, one of the Cleanup*Data
            // methods will be called at the end of each pair:

            // MarshalNativeToManaged
            // MarshalManagedToNative
            // CleanUpManagedData
            //
            // or for recursion:
            //
            // MarshalManagedToNative 1
            // MarshalNativeToManaged 2
            // MarshalManagedToNative 2
            // CleanUpManagedData     2
            // MarshalNativeToManaged 1
            // CleanUpNativeData      1
            
            // Clear() either pops an entry, or clears
            // the values for the next call.
            MyProps t = MyProps.GetTop(m_Index);
            t.Clear(m_Index);
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            // Clear() either pops an entry, or clears
            // the values for the next call.
            MyProps t = MyProps.GetTop(m_Index);
            t.Clear(m_Index);
        }

        // The number of bytes to marshal.  Size varies between x86 and x64.
        public int GetNativeDataSize()
        {
            return Marshal.SizeOf(typeof(PropVariant));
        }

        // This method is called by interop to create the custom marshaler.
        // The (optional) cookie is the value specified in
        // MarshalCookie="asdf", or "" if none is specified.
        private static ICustomMarshaler GetInstance(string cookie)
        {
            return new PVMarshaler(cookie);
        }
    }

    // Used (only) by MFExtern.MFTGetInfo.  In order to perform the marshaling,
    // we need to have the pointer to the array, and the number of elements. To
    // receive all this information in the marshaler, we are using the same
    // instance of this class for multiple parameters.  So ppInputTypes &
    // pcInputTypes share an instance, and ppOutputTypes & pcOutputTypes share
    // an instance.  To make life interesting, we also need to work correctly
    // if invoked on multiple threads at the same time.
    internal class RTIMarshaler : ICustomMarshaler
    {
        private struct MyProps
        {
            public ArrayList m_array;
            public MFInt m_int;
            public IntPtr m_MFIntPtr;
            public IntPtr m_ArrayPtr;
        }

        // When used with MFExtern.MFTGetInfo, there are 2 parameter pairs
        // (ppInputTypes + pcInputTypes, ppOutputTypes + pcOutputTypes).  Each
        // need their own instance, so s_Props is a 2 element array.
        [ThreadStatic]
        static MyProps[] s_Props;

        // Used to indicate the index of s_Props we should be using.  It is
        // derived from the MarshalCookie.
        private int m_Cookie;

        private RTIMarshaler(string cookie)
        {
            m_Cookie = int.Parse(cookie);
        }

        public IntPtr MarshalManagedToNative(object managedObj)
        {
            IntPtr p;

            // s_Props is threadstatic, so we don't need to worry about
            // locking.  And since the only method that RTIMarshaler supports
            // is MFExtern.MFTGetInfo, we know that MarshalManagedToNative gets
            // called first.
            if (s_Props == null)
                s_Props = new MyProps[2];

            // We get called twice: Once for the MFInt, and once for the array.
            // Figure out which call this is.
            if (managedObj is MFInt)
            {
                // Save off the object.  We'll need to use Assign() on this
                // later.
                s_Props[m_Cookie].m_int = managedObj as MFInt;

                // Allocate room for the int and set it to zero;
                p = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(MFInt)));
                Marshal.WriteInt32(p, 0);

                s_Props[m_Cookie].m_MFIntPtr = p;
            }
            else // Must be the array.  FYI: Nulls don't get marshaled.
            {
                // Save off the object.  We'll be calling methods on this in
                // MarshalNativeToManaged.
                s_Props[m_Cookie].m_array = managedObj as ArrayList;

                s_Props[m_Cookie].m_array.Clear();

                // All we need is room for the pointer
                p = Marshal.AllocCoTaskMem(IntPtr.Size);

                // Belt-and-suspenders.  Set this to null.
                Marshal.WriteIntPtr(p, IntPtr.Zero);

                s_Props[m_Cookie].m_ArrayPtr = p;
            }

            return p;
        }

        // We have the MFInt and the array pointer.  Populate the array.
        static void Parse(MyProps p)
        {
            // If we have an array to return things in (ie MFTGetInfo wasn't
            // passed nulls).  Note that the MFInt doesn't get set in that
            // case.
            if (p.m_array != null)
            {
                // Read the count
                int count = Marshal.ReadInt32(p.m_MFIntPtr);
                p.m_int.Assign(count);

                IntPtr ip2 = Marshal.ReadIntPtr(p.m_ArrayPtr);

                // I don't know why this might happen, but it seems worth the
                // check.
                if (ip2 != IntPtr.Zero)
                {
                    try
                    {
                        int iSize = Marshal.SizeOf(typeof(MFTRegisterTypeInfo));
                        IntPtr pos = ip2;

                        // Size the array
                        p.m_array.Capacity = count;

                        // Copy in the values
                        for (int x = 0; x < count; x++)
                        {
                            MFTRegisterTypeInfo rti = new MFTRegisterTypeInfo();
                            Marshal.PtrToStructure(pos, rti);
                            pos += iSize;
                            p.m_array.Add(rti);
                        }
                    }
                    finally
                    {
                        // Free the array we got back
                        Marshal.FreeCoTaskMem(ip2);
                    }
                }
            }
        }

        // Called just after invoking the COM method.  The IntPtr is the same
        // one that just got returned from MarshalManagedToNative.  The return
        // value is unused.
        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            Debug.Assert(s_Props != null);

            // Figure out which (if either) of the MFInts this is.
            for (int x = 0; x < 2; x++)
            {
                if (pNativeData == s_Props[x].m_MFIntPtr)
                {
                    Parse(s_Props[x]);
                    break;
                }
            }

            // This value isn't actually used
            return null;
        }

        public void CleanUpManagedData(object ManagedObj)
        {
            // Never called.
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            if (s_Props[m_Cookie].m_MFIntPtr != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(s_Props[m_Cookie].m_MFIntPtr);

                s_Props[m_Cookie].m_MFIntPtr = IntPtr.Zero;
                s_Props[m_Cookie].m_int = null;
            }
            if (s_Props[m_Cookie].m_ArrayPtr != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(s_Props[m_Cookie].m_ArrayPtr);

                s_Props[m_Cookie].m_ArrayPtr = IntPtr.Zero;
                s_Props[m_Cookie].m_array = null;
            }
        }

        // The number of bytes to marshal out
        public int GetNativeDataSize()
        {
            return -1;
        }

        // This method is called by interop to create the custom marshaler.
        // The (optional) cookie is the value specified in MarshalCookie="xxx",
        // or "" if none is specified.
        private static ICustomMarshaler GetInstance(string cookie)
        {
            return new RTIMarshaler(cookie);
        }
    }

    // Used by MFTRegister.  Note that since it only marshals [In],
    // the class has no members, which makes life much simpler.
    internal class RTAMarshaler : ICustomMarshaler
    {
        public IntPtr MarshalManagedToNative(object managedObj)
        {
            int iSize = Marshal.SizeOf(typeof(MFTRegisterTypeInfo));

            MFTRegisterTypeInfo[] array = managedObj as MFTRegisterTypeInfo[];

            IntPtr p = Marshal.AllocCoTaskMem(array.Length * iSize);
            IntPtr t = p;

            for (int x = 0; x < array.Length; x++)
            {
                Marshal.StructureToPtr(array[x], t, false);
                t += iSize;
            }

            return p;
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            // This value isn't actually used
            return null;
        }

        public void CleanUpManagedData(object ManagedObj)
        {
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            Marshal.FreeCoTaskMem(pNativeData);
        }

        // The number of bytes to marshal out
        public int GetNativeDataSize()
        {
            return -1;
        }

        // This method is called by interop to create the custom marshaler.
        // The (optional) cookie is the value specified in MarshalCookie="xxx"
        // or "" if none is specified.
        private static ICustomMarshaler GetInstance(string cookie)
        {
            return new RTAMarshaler();
        }
    }

    // Class to handle WAVEFORMATEXTENSIBLE.  While it is used for both [In]
    // and [Out], it is never used for [In, Out], which means it has no data
    // members (which makes life much simpler).
    internal class WEMarshaler : ICustomMarshaler
    {
        public IntPtr MarshalManagedToNative(object managedObj)
        {
            WaveFormatEx wfe = managedObj as WaveFormatEx;

            IntPtr ip = wfe.GetPtr();

            return ip;
        }

        // Called just after invoking the COM method.  The IntPtr is the same
        // one that just got returned from MarshalManagedToNative.  The return
        // value is unused.
        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            WaveFormatEx wfe = WaveFormatEx.PtrToWave(pNativeData);

            return wfe;
        }

        public void CleanUpManagedData(object ManagedObj)
        {
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            Marshal.FreeCoTaskMem(pNativeData);
        }

        // The number of bytes to marshal out - never called
        public int GetNativeDataSize()
        {
            return -1;
        }

        // This method is called by interop to create the custom marshaler.
        // The (optional) cookie is the value specified in MarshalCookie="xxx",
        // or "" if none is specified.
        private static ICustomMarshaler GetInstance(string cookie)
        {
            return new WEMarshaler();
        }
    }

    // Class to handle BITMAPINFO.  Only used by MFCalculateBitmapImageSize &
    // MFCreateVideoMediaTypeFromBitMapInfoHeader (as [In]) and 
    // IMFVideoDisplayControl::GetCurrentImage ([In, Out]).  Since
    // IMFVideoDisplayControl can be implemented on a managed class, we must
    // support nesting.
    public class BMMarshaler : ICustomMarshaler
    {
        private class MyProps
        {
            #region Data members

            public BitmapInfoHeader m_obj;
            public IntPtr m_ptr;

            private int m_InProcsss = 0;
            private bool m_IAllocated;
            private MyProps m_Parent = null;

            [ThreadStatic]
            private static MyProps m_CurrentProps;

            #endregion

            public int GetStage()
            {
                return m_InProcsss;
            }

            public void StageComplete()
            {
                m_InProcsss++;
            }

            public IntPtr Allocate()
            {
                IntPtr ip = m_obj.GetPtr();
                m_IAllocated = true;
                return ip;
            }

            public static MyProps AddLayer()
            {
                MyProps p = new MyProps();
                p.m_Parent = m_CurrentProps;
                m_CurrentProps = p;

                return p;
            }

            public static void SplitLayer()
            {
                MyProps t = AddLayer();
                MyProps p = t.m_Parent;

                t.m_InProcsss = 1;
                t.m_ptr = p.m_ptr;
                t.m_obj = p.m_obj;

                p.m_InProcsss = 1;
            }

            public static MyProps GetTop()
            {
                // If the member hasn't been initialized, do it now.  And no,
                // we can't do this in the constructor, since the constructor
                // may have been called on a different thread, and
                // m_CurrentProps is unique to each thread.
                if (m_CurrentProps == null)
                {
                    m_CurrentProps = new MyProps();
                }

                return m_CurrentProps;
            }

            public void Clear()
            {
                if (m_IAllocated)
                {
                    Marshal.FreeCoTaskMem(m_ptr);
                    m_IAllocated = false;
                }

                // Never delete the last entry.
                if (m_Parent == null)
                {
                    m_InProcsss = 0;
                    m_obj = null;
                    m_ptr = IntPtr.Zero;
                }
                else
                {
                    m_obj = null;
                    m_CurrentProps = m_Parent;
                }
            }
        }

        public IntPtr MarshalManagedToNative(object managedObj)
        {
            MyProps t = MyProps.GetTop();

            switch (t.GetStage())
            {
                case 0:
                    {
                        t.m_obj = managedObj as BitmapInfoHeader;

                        Debug.Assert(t.m_obj != null);
                        Debug.Assert(t.m_obj.Size != 0);

                        t.m_ptr = t.Allocate();

                        break;
                    }
                case 1:
                    {
                        if (!System.Object.ReferenceEquals(t.m_obj, managedObj))
                        {
                            MyProps.AddLayer();

                            // Try this call again now that we have fixed
                            // m_CurrentProps.
                            return MarshalManagedToNative(managedObj);
                        }
                        Marshal.StructureToPtr(managedObj, t.m_ptr, false);
                        break;
                    }
                case 2:
                    {
                        MyProps.SplitLayer();

                        // Try this call again now that we have fixed
                        // m_CurrentProps.
                        return MarshalManagedToNative(managedObj);
                    }
                default:
                    {
                        Environment.FailFast("Something horrible has " +
                                             "happened, probaby due to " +
                                             "marshaling of nested " +
                                             "PropVariant calls.");
                        break;
                    }
            }
            t.StageComplete();

            return t.m_ptr;
        }

        // Called just after invoking the COM method.  The IntPtr is the same
        // one that just got returned from MarshalManagedToNative.  The return
        // value is unused.
        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            MyProps t = MyProps.GetTop();

            switch (t.GetStage())
            {
                case 0:
                    {
                        t.m_obj = BitmapInfoHeader.PtrToBMI(pNativeData);
                        t.m_ptr = pNativeData;
                        break;
                    }
                case 1:
                    {
                        if (t.m_ptr != pNativeData)
                        {
                            // If we get here, we have already received a call
                            // to MarshalManagedToNative where we created an
                            // IntPtr and stored it into t.m_ptr.  But the
                            // value we just got passed here isn't the same
                            // one. Therefore instead of being the second half
                            // of a "Managed calling unmanaged" (as m_InProcsss
                            // led us to believe) this is really the first half
                            // of a nested "Unmanaged calling managed" (see
                            // Recursion in the comments at the top of this
                            // class).  Add another layer.
                            MyProps.AddLayer();

                            // Try this call again now that we have fixed
                            // m_CurrentProps.
                            return MarshalNativeToManaged(pNativeData);
                        }
                        BitmapInfoHeader bmi = BitmapInfoHeader.PtrToBMI(pNativeData);

                        t.m_obj.CopyFrom(bmi);
                        break;
                    }
                case 2:
                    {
                        // Apparently this is 'part 3' of a 2 part call.  Which
                        // means we are doing a nested call.  Normally we would
                        // catch the fact that this is a nested call with the
                        // (t.m_ptr != pNativeData) check above.  However, if
                        // the same PropVariant instance is being passed thru
                        // again, we end up here.  So, add a layer.
                        MyProps.SplitLayer();

                        // Try this call again now that we have fixed
                        // m_CurrentProps.
                        return MarshalNativeToManaged(pNativeData);
                    }
                default:
                    {
                        Environment.FailFast("Something horrible has " +
                                             "happened, probaby due to " +
                                             "marshaling of nested " +
                                             "BMMarshaler calls.");
                        break;
                    }
            }
            t.StageComplete();

            return t.m_obj;
        }

        public void CleanUpManagedData(object ManagedObj)
        {
            MyProps t = MyProps.GetTop();
            t.Clear();
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            MyProps t = MyProps.GetTop();
            t.Clear();
        }

        // The number of bytes to marshal out - never called
        public int GetNativeDataSize()
        {
            return -1;
        }

        // This method is called by interop to create the custom marshaler.
        // The (optional) cookie is the value specified in MarshalCookie="xxx",
        // or "" if none is specified.
        private static ICustomMarshaler GetInstance(string cookie)
        {
            return new BMMarshaler();
        }
    }

    // Used only by IMFPMediaPlayerCallback, where it only uses [In].
    internal class EHMarshaler : ICustomMarshaler
    {
        public IntPtr MarshalManagedToNative(object managedObj)
        {
            MFP_EVENT_HEADER eh = managedObj as MFP_EVENT_HEADER;

            IntPtr ip = eh.GetPtr();

            return ip;
        }

        // Called just after invoking the COM method.  The IntPtr is the same one that just got returned
        // from MarshalManagedToNative.  The return value is unused.
        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            // We should never get here.

            MFP_EVENT_HEADER eh = MFP_EVENT_HEADER.PtrToEH(pNativeData);

            return eh;
        }

        public void CleanUpManagedData(object ManagedObj)
        {
            // Never called.
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            Marshal.FreeCoTaskMem(pNativeData);
        }

        // The number of bytes to marshal out - never called
        public int GetNativeDataSize()
        {
            return -1;
        }

        // This method is called by interop to create the custom marshaler.  The (optional)
        // cookie is the value specified in MarshalCookie="asdf", or "" is none is specified.
        private static ICustomMarshaler GetInstance(string cookie)
        {
            return new EHMarshaler();
        }
    }

    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Class)]
    public class UnmanagedNameAttribute : System.Attribute
    {
        private string m_Name;

        public UnmanagedNameAttribute(string s)
        {
            m_Name = s;
        }

        public override string ToString()
        {
            return m_Name;
        }
    }

    #endregion
}
