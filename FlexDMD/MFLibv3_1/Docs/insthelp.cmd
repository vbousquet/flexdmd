@echo off
cls
setlocal

rem ----------------------------------------
rem Set dodel to yes to remove the registry keys
rem instead of adding them.

set dodel=
rem set dodel=yes

rem To modify this file for other projects, change these two
rem set statements, and of course the "for" statement
rem below.

set ns=MediaFoundation
set loc="http://MFNet.SourceForge.Net"
rem ----------------------------------------

echo This batch file is intended for use with projects
echo that use the library from %loc%.
echo It will configure VS Help to open the appropriate
echo MSDN page when F1 is pressed on any %ns%
echo interface, interface method, struct or enum.
echo.

rem determine whether 64bit
set useOS=
if "%PROCESSOR_ARCHITECTURE%" == "AMD64" set useOS=Wow6432Node\

echo Supported Visual Studio versions detected:
echo.

set found11=Not found
set found12=Not found
set found14=Not found

reg query HKLM\SOFTWARE\%useOS%Microsoft\Help\v2.0\Catalogs\VisualStudio11 > nul 2>&1
if not errorlevel 1 set found11=Detected

reg query HKLM\SOFTWARE\%useOS%Microsoft\Help\v2.1\Catalogs\VisualStudio12 > nul 2>&1
if not errorlevel 1 set found12=Detected

reg query HKLM\SOFTWARE\%useOS%Microsoft\Help\v2.2\Catalogs\VisualStudio14 > nul 2>&1
if not errorlevel 1 set found14=Detected

echo    Visual Studio 2012... %found11%.
echo    Visual Studio 2013... %found12%.
echo    Visual Studio 2015... %found14%.
echo.

echo To cancel this batch file, press Ctrl-C or
echo to configure the version(s) of VS above,
pause

if "%found11%" == "Detected" call :InstallOne 2.0 11 VS2012
if "%found12%" == "Detected" call :InstallOne 2.1 12 VS2013
if "%found14%" == "Detected" call :InstallOne 2.2 14 VS2015

echo.
echo All configurations complete!

rem Keep window open for 2 seconds
choice /d y /t 2 > nul

goto :eof

rem %1 is MS Help version, %2 is VS version, %3 is VS Name
:InstallOne

echo.
echo Building registration file for %3

set tn=%temp%\xx%3.reg

echo Windows Registry Editor Version 5.00>%tn%
echo.>>%tn%

if /i "%dodel%" == "yes" (
set br=[-HKEY_LOCAL_MACHINE\SOFTWARE\%useOS%Microsoft\Help\v%1\Partner\%ns%

rem Since I don't know what value VendorContent had before
rem installing, I can't "put it back the way it was."
rem Leaving it alone should be (mostly) harmless.

) else (
set br=[HKEY_LOCAL_MACHINE\SOFTWARE\%useOS%Microsoft\Help\v%1\Partner\%ns%

echo [HKEY_LOCAL_MACHINE\SOFTWARE\%useOS%Microsoft\Help\v%1\Catalogs\VisualStudio%2]>>%tn%
echo "VendorContent"=dword:1>>%tn%
echo.>>%tn%
)

echo %br%]>>%tn% && echo "location"=%loc%>>%tn% && echo.>>%tn%

rem Namespaces and static classes

for %%f in (
Alt dxvahd EVR MFPlayer Misc OPM ReadWrite Transform DXVAHDETWGUID OPMExtern MFExtern MF_MEDIA_ENGINE
MFAttributesClsid MF_MEDIA_SHARING_ENGINE MF_CAPTURE_ENGINE MFTranscodeContainerType MFConnector
MFTransformCategory MFEnabletype MFRepresentation MFProperties MFServices MFPKEY CLSID MFMediaType
MFImageFormat MFError MFOpmStatusRequests OpmConstants MFPKEY_ASFMEDIASINK MFASFSampleExtension
) do echo %br%.%%f]>>%tn% && echo "location"=%loc%>>%tn% && echo.>>%tn%

rem MediaFoundation.Alt namespace

for %%f in (
IMFFinalizableMediaSinkAlt IMFGetServiceAlt IMFMediaEventGeneratorAlt IMFMediaEventQueueAlt IMFMediaSinkAlt 
IMFMediaSourceAlt IMFMediaStreamAlt IMFSourceReaderAsync IMFStreamSinkAlt IMFTopologyServiceLookupAlt 
IMFTopologyServiceLookupClientAlt
) do echo %br%.Alt.%%f]>>%tn% && echo "location"=%loc%>>%tn% && echo.>>%tn%

rem MediaFoundation.dxvahd namespace

for %%f in (
IDXVAHD_Device IDXVAHD_VideoProcessor
) do echo %br%.dxvahd.%%f]>>%tn% && echo "location"=%loc%>>%tn% && echo.>>%tn%

rem MediaFoundation.EVR namespace

for %%f in (
IEVRFilterConfig IEVRFilterConfigEx IEVRTrustedVideoPlugin IEVRVideoStreamControl IMFDesiredSample
IMFTopologyServiceLookup IMFTopologyServiceLookupClient IMFTrackedSample IMFVideoDeviceID IMFVideoDisplayControl 
IMFVideoMixerBitmap IMFVideoMixerControl IMFVideoMixerControl2 IMFVideoPositionMapper IMFVideoPresenter 
IMFVideoProcessor IMFVideoRenderer 
) do echo %br%.EVR.%%f]>>%tn% && echo "location"=%loc%>>%tn% && echo.>>%tn%

rem MediaFoundation namespace

for %%f in (
IAdvancedMediaCapture IAdvancedMediaCaptureInitializationSettings IAdvancedMediaCaptureSettings IAudioSourceProvider 
IMF2DBuffer IMF2DBuffer2 IMFActivate IMFASFContentInfo IMFASFIndexer IMFASFMultiplexer IMFASFMutualExclusion 
IMFASFProfile IMFASFSplitter IMFASFStreamConfig IMFASFStreamPrioritization IMFASFStreamSelector IMFAsyncCallback 
IMFAsyncCallbackLogging IMFAsyncResult IMFAttributes IMFAudioMediaType IMFAudioPolicy IMFAudioStreamVolume 
IMFBufferListNotify IMFByteStream IMFByteStreamBuffering IMFByteStreamCacheControl IMFByteStreamCacheControl2 
IMFByteStreamHandler IMFByteStreamProxyClassFactory IMFByteStreamTimeSeek IMFCaptureEngine IMFCaptureEngineClassFactory
IMFCaptureEngineOnEventCallback IMFCaptureEngineOnSampleCallback IMFCaptureEngineOnSampleCallback2
IMFCapturePhotoConfirmation IMFCapturePhotoSink IMFCapturePreviewSink IMFCaptureRecordSink IMFCaptureSink
IMFCaptureSink2 IMFCaptureSource IMFCdmSuspendNotify IMFClock IMFClockStateSink IMFCollection
IMFContentDecryptorContext IMFContentEnabler IMFContentProtectionDevice IMFContentProtectionManager IMFDLNASinkInit
IMFDRMNetHelper IMFDXGIBuffer IMFDXGIDeviceManager IMFDXGIDeviceManagerSource IMFFieldOfUseMFTUnlock
IMFFinalizableMediaSink IMFGetService IMFImageSharingEngine IMFImageSharingEngineClassFactory IMFInputTrustAuthority
IMFMediaBuffer IMFMediaEngine IMFMediaEngineClassFactory IMFMediaEngineClassFactory2 IMFMediaEngineClassFactoryEx
IMFMediaEngineEME IMFMediaEngineEx IMFMediaEngineExtension IMFMediaEngineNeedKeyNotify IMFMediaEngineNotify
IMFMediaEngineOPMInfo IMFMediaEngineProtectedContent IMFMediaEngineSrcElements IMFMediaEngineSrcElementsEx
IMFMediaEngineSupportsSourceTransfer IMFMediaEngineWebSupport IMFMediaError IMFMediaEvent IMFMediaEventGenerators
IMFMediaEventQueue IMFMediaKeys IMFMediaKeySession IMFMediaKeySessionNotify IMFMediaSession IMFMediaSharingEngine
IMFMediaSharingEngineClassFactory IMFMediaSink IMFMediaSinkPreroll IMFMediaSource IMFMediaSourceEx
IMFMediaSourceExtension IMFMediaSourceExtensionNotify IMFMediaSourcePresentationProvider IMFMediaSourceTopologyProvider
IMFMediaStream IMFMediaStreamSourceSampleRequest IMFMediaTimeRange IMFMediaType IMFMediaTypeHandler IMFMetadata
IMFMetadataProvider IMFNetCredential IMFNetCredentialCache IMFNetCredentialManager IMFNetProxyLocator
IMFNetProxyLocatorFactory IMFNetResourceFilter IMFNetSchemeHandlerConfig IMFObjectReferenceStream IMFOutputPolicy
IMFOutputSchema IMFOutputTrustAuthority IMFPluginControl IMFPluginControl2 IMFPMPClient IMFPMPClientApp IMFPMPHost
IMFPMPHostApp IMFPMPServer IMFPresentationClock IMFPresentationDescriptor IMFPresentationTimeSource
IMFProtectedEnvironmentAccess IMFQualityAdvise IMFQualityAdvise2 IMFQualityAdviseLimits IMFQualityManager
IMFRateControl IMFRateSupport IMFRealTimeClient IMFRealTimeClientEx IMFRemoteAsyncCallback IMFRemoteDesktopPlugin
IMFRemoteProxy IMFSAMIStyle IMFSample IMFSampleGrabberSinkCallback IMFSampleGrabberSinkCallback2 IMFSampleOutputStream
IMFSampleProtection IMFSaveJob IMFSchemeHandler IMFSecureChannel IMFSeekInfo IMFSequencerSource
IMFSharingEngineClassFactory IMFShutdown IMFSignedLibrary IMFSimpleAudioVolume IMFSourceBuffer IMFSourceBufferAppendMode
IMFSourceBufferList IMFSourceBufferNotify IMFSourceOpenMonitor IMFSourceResolver IMFSSLCertificateManager
IMFStreamDescriptor IMFStreamingSinkConfig IMFStreamSink IMFSystemId IMFTimecodeTranslate IMFTimedText
IMFTimedTextBinary IMFTimedTextCue IMFTimedTextCueList IMFTimedTextFormattedText IMFTimedTextNotify IMFTimedTextRegion
IMFTimedTextStyle IMFTimedTextTrack IMFTimedTextTrackList IMFTimer IMFTopoLoader IMFTopology IMFTopologyNode
IMFTopologyNodeAttributeEditor IMFTranscodeProfile IMFTranscodeSinkInfoProvider IMFTrustedInput IMFTrustedOutput
IMFVideoMediaType IMFVideoProcessorControl IMFVideoProcessorControl2 IMFVideoSampleAllocator
IMFVideoSampleAllocatorCallback IMFVideoSampleAllocatorEx IMFVideoSampleAllocatorNotify IMFVideoSampleAllocatorNotifyEx
IMFWorkQueueServices IMFWorkQueueServicesEx IPlayToControl IPlayToControlWithCapabilities IPlayToSourceClassFactory
) do echo %br%.%%f]>>%tn% && echo "location"=%loc%>>%tn% && echo.>>%tn%

rem MediaFoundation.MFPlayer namespace

for %%f in (
IMFPMediaItem IMFPMediaPlayer IMFPMediaPlayerCallback
) do echo %br%.MFPlayer.%%f]>>%tn% && echo "location"=%loc%>>%tn% && echo.>>%tn%

rem MediaFoundation.Misc namespace

for %%f in (
INamedPropertyStore IPropertyStore 
) do echo %br%.Misc.%%f]>>%tn% && echo "location"=%loc%>>%tn% && echo.>>%tn%

rem MediaFoundation.OPM namespace

for %%f in (
IOPMVideoOutput
) do echo %br%.OPM.%%f]>>%tn% && echo "location"=%loc%>>%tn% && echo.>>%tn%

rem MediaFoundation.ReadWrite namespace

for %%f in (
IMFReadWriteClassFactory IMFSinkWriter IMFSinkWriterCallback IMFSinkWriterCallback2 IMFSinkWriterEncoderConfig
IMFSinkWriterEx IMFSourceReader IMFSourceReaderCallback IMFSourceReaderCallback2 IMFSourceReaderEx 
) do echo %br%.ReadWrite.%%f]>>%tn% && echo "location"=%loc%>>%tn% && echo.>>%tn%

rem MediaFoundation.Transform namespace

for %%f in (
IMFLocalMFTRegistration IMFTransform
) do echo %br%.Transform.%%f]>>%tn% && echo "location"=%loc%>>%tn% && echo.>>%tn%

echo Registering objects...
regedit /s %tn%

echo Cleanup...
del %tn%

echo Done!
goto :eof