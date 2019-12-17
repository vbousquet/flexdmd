/* Copyright 2019 Vincent Bousquet

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
   */
using FlexDMD.Actors;
using MediaFoundation;
using MediaFoundation.Misc;
using MediaFoundation.ReadWrite;
using NLog;
using System;
using System.Drawing;
using System.Drawing.Imaging;

// Based on : https://docs.microsoft.com/en-us/windows/win32/medfound/processing-media-data-with-the-source-reader?redirectedfrom=MSDN
// Using : https://docs.microsoft.com/en-us/windows/win32/medfound/uncompressed-video-buffers
//
// Works fine with all diablo wmv files (WMV2 profile which is supported by MMF)
// Fails on all of Metal Slug wmv; IntroMS.wmv, Mision1Start.wmv, Mision2Start.wmv,... (WMV3 profile which is also supported by MMF)
namespace FlexDMD
{
    class Video : AnimatedActor
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private static int nOpenedVideos = 0;
        private readonly string _path;
        private readonly int _stretchMode; // stretch: 0, crop to top: 1, crop to center: 2, crop to bottom: 3
        private IMFSourceReader _reader;
        private Bitmap _frame;

        // FIXME initialize/dispose when entering/exiting rendering graph (MMF has limited number of concurrently opened sources)

        public Video(string path, bool loop = false, int stretchMode = 0)
        {
            _path = path;
            _loop = loop;
            _stretchMode = stretchMode;
        }

        public void Open()
        {
            Rewind();
            nOpenedVideos++;
            log.Info("Video opened: {0} ({1} videos concurrently opened)", _path, nOpenedVideos);
        }

        public void Close()
        {
            nOpenedVideos--;
            if (_reader != null) COMBase.SafeRelease(_reader);
            _reader = null;
        }

        protected override void Rewind()
        {
            // log.Info("Initalizing video: {0}", _path);
            // System.Diagnostics.StackTrace t = new System.Diagnostics.StackTrace();
            // log.Info("Stack trace: {0}", t);
            if (_reader != null) COMBase.SafeRelease(_reader);
            _reader = null;
            HResult result = MFExtern.MFCreateSourceReaderFromURL(_path, null, out IMFSourceReader reader);
            if (result < 0)
            {
                log.Error("Failed to open video: {0}", _path);
                return;
            }
            _reader = SetupDecoder(reader);
            ReadNextFrame();
        }

        private IMFSourceReader SetupDecoder(IMFSourceReader reader)
        {
            HResult result = reader.GetNativeMediaType((int)MF_SOURCE_READER.FirstVideoStream, 0, out IMFMediaType nativeType);
            if (result < 0)
            {
                log.Error("Failed to get native media type of video: {0}", _path);
                COMBase.SafeRelease(reader);
                return null;
            }

            nativeType.GetMajorType(out Guid majorType);
            if (result < 0)
            {
                log.Error("Failed to get major type of video: {0}", _path);
                COMBase.SafeRelease(nativeType);
                COMBase.SafeRelease(reader);
                return null;
            }
            if (majorType != MFMediaType.Video)
            {
                log.Error("Unsupported stream type for video: {0}", _path);
                COMBase.SafeRelease(nativeType);
                COMBase.SafeRelease(reader);
                return null;
            }

            result = MFExtern.MFCreateMediaType(out IMFMediaType outputType);
            if (result < 0)
            {
                log.Error("Failed to create decoder media type for video: {0}", _path);
                COMBase.SafeRelease(nativeType);
                COMBase.SafeRelease(reader);
                return null;
            }
            result = outputType.SetGUID(MFAttributesClsid.MF_MT_MAJOR_TYPE, MFMediaType.Video);
            if (result < 0)
            {
                log.Error("Failed to setup decoder major-type for video: {0}", _path);
                COMBase.SafeRelease(nativeType);
                COMBase.SafeRelease(outputType);
                COMBase.SafeRelease(reader);
                return null;
            }
            result = outputType.SetGUID(MFAttributesClsid.MF_MT_SUBTYPE, MFMediaType.RGB24);
            if (result < 0)
            {
                log.Error("Failed to setup decoder sub-type for video: {0}", _path);
                COMBase.SafeRelease(nativeType);
                COMBase.SafeRelease(outputType);
                COMBase.SafeRelease(reader);
                return null;
            }

            result = reader.SetCurrentMediaType((int)MF_SOURCE_READER.FirstVideoStream, null, outputType);
            if (result < 0)
            {
                log.Error("Failed to setup decoder for video: {0}", _path);
                COMBase.SafeRelease(nativeType);
                COMBase.SafeRelease(outputType);
                COMBase.SafeRelease(reader);
                return null;
            }

            result = reader.SetStreamSelection((int)MF_SOURCE_READER.FirstVideoStream, true);
            if (result < 0)
            {
                log.Error("Failed to select stream for video: {0}", _path);
                COMBase.SafeRelease(nativeType);
                COMBase.SafeRelease(outputType);
                COMBase.SafeRelease(reader);
                return null;
            }
            return reader;
        }

        protected override void ReadNextFrame()
        {
            if (_reader == null) Rewind();
            if (_reader == null || _endOfAnimation) return;
            try
            {
                HResult result = _reader.ReadSample((int)MF_SOURCE_READER.FirstVideoStream, 0, out int streamIndex, out MF_SOURCE_READER_FLAG flags, out long timeStamp, out IMFSample sample);
                if (result < 0)
                {
                    log.Info("Failed to read sample from {0} {1}", _path, result);
                    COMBase.SafeRelease(sample);
                    return;
                }
                if (flags == MF_SOURCE_READER_FLAG.EndOfStream)
                {
                    log.Info("End of stream");
                    _endOfAnimation = true;
                }
                if (flags == MF_SOURCE_READER_FLAG.NewStream)
                {
                    log.Info("New stream");
                }
                if (flags == MF_SOURCE_READER_FLAG.NativeMediaTypeChanged)
                {
                    log.Info("Native type changed");
                    // The format changed. Reconfigure the decoder.
                    _reader = SetupDecoder(_reader);
                }
                if (flags == MF_SOURCE_READER_FLAG.CurrentMediaTypeChanged)
                {
                    log.Info("Current type changed");
                }
                if (flags == MF_SOURCE_READER_FLAG.StreamTick)
                {
                    log.Info("Stream tick");
                }
                if (sample != null)
                {
                    // in 100-nano sconds units
                    sample.GetSampleTime(out long frameTime);
                    sample.GetSampleDuration(out long frameDuration);
                    sample.GetBufferByIndex(0, out IMFMediaBuffer buffer);
                    IMF2DBuffer frame = (IMF2DBuffer)buffer;
                    result = frame.Lock2D(out IntPtr scanLine0, out int pitch);
                    if (result >= 0)
                    {
                        // log.Info("Frame read, next frame at {0} + {1} = {2}", _frameTime, _frameDuration, _frameTime + _frameDuration);
                        GraphicUtils.BGRtoRGB(scanLine0, pitch);
                        _frame = new Bitmap(128, 32, pitch, PixelFormat.Format24bppRgb, scanLine0);
                        frame.Unlock2D();
                    }
                    else
                    {
                        log.Error("Failed to read frame");
                    }
                    _frameTime = frameTime / 10000000.0f;
                    _frameDuration = frameDuration / 10000000.0f;
                    COMBase.SafeRelease(sample);
                }
            }
            catch (Exception e)
            {
                log.Error(e, "ReadNextFrame failed, video discarded");
                _endOfAnimation = true;
                return;
            }
        }

        public override void Draw(Graphics graphics)
        {
            if (Visible && _frame != null) graphics.DrawImage(_frame, X, Y, 128, 32);
        }

        public override string ToString()
        {
            return string.Format("Video '{0}'", _path);
        }
    }

}
