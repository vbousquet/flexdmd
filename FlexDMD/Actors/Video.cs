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
using MediaFoundation.Interop;
using MediaFoundation.Misc;
using MediaFoundation.ReadWrite;
using NAudio.Wave;
using NLog;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace FlexDMD
{
    class Video : AnimatedActor
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private static int nOpenedVideos = 0;
        private readonly string _path;
        private readonly int _stretchMode; // stretch: 0, crop to top: 1, crop to center: 2, crop to bottom: 3
        private int _videoWidth, _videoHeight;
        private IMFSourceReader _videoReader;
        private MediaFoundationReader _audioReader;
        private WaveOutEvent _audioDevice;
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
            _audioDevice = new WaveOutEvent();
            Rewind();
            nOpenedVideos++;
            log.Info("Video opened: {0} ({1} videos concurrently opened)", _path, nOpenedVideos);
        }

        public void Close()
        {
            if (_audioReader != null)
            {
                _audioReader.Dispose();
                _audioReader = null;
            }
            if (_videoReader != null) ComClass.SafeRelease(ref _videoReader);
            _audioDevice.Dispose();
            _audioDevice = null;
            nOpenedVideos--;
        }

        protected override void Rewind()
        {
            // log.Info("Initalizing video: {0}", _path);
            if (_videoReader != null)
            {
                // should pass in a variant of type VT_I8 which is a long containing time in 100nanosecond units
                var pv = new PropVariant(0L);
                _videoReader.SetCurrentPosition(Guid.Empty, pv);
            }
            else
            {
                HResult hr = MFExtern.MFCreateSourceReaderFromURL(_path, null, out IMFSourceReader reader);
                if (MFError.Succeeded(hr)) _videoReader = SetupVideoDecoder(reader);
                if (_videoReader == null) log.Error("Failed to open video: {0}", _path);
            }
            if (_audioReader != null)
            {
                _audioReader.Seek(0, System.IO.SeekOrigin.Begin);
            }
            else
            {
                _audioReader = new MediaFoundationReader(_path);
                _audioDevice.Init(_audioReader);
                _audioDevice.Play();
            }
            ReadNextFrame();
        }

        private IMFSourceReader SetupVideoDecoder(IMFSourceReader reader)
        {
            // Get the frame size
            HResult hr = reader.GetNativeMediaType((int)MF_SOURCE_READER.FirstVideoStream, 0, out IMFMediaType nativeType);
            if (MFError.Succeeded(hr))
                hr = MFExtern.MFGetAttributeSize(nativeType, MFAttributesClsid.MF_MT_FRAME_SIZE, out _videoWidth, out _videoHeight);
            ComClass.SafeRelease(ref nativeType);

            // Setup decoder to RGB24
            IMFMediaType outputType = null;
            if (MFError.Succeeded(hr))
                hr = MFExtern.MFCreateMediaType(out outputType);
            if (MFError.Succeeded(hr))
                hr = outputType.SetGUID(MFAttributesClsid.MF_MT_MAJOR_TYPE, MFMediaType.Video);
            if (MFError.Succeeded(hr))
                hr = outputType.SetGUID(MFAttributesClsid.MF_MT_SUBTYPE, MFMediaType.RGB24);
            if (MFError.Succeeded(hr))
                hr = reader.SetCurrentMediaType((int)MF_SOURCE_READER.FirstVideoStream, null, outputType);
            if (MFError.Succeeded(hr))
                hr = reader.SetStreamSelection((int)MF_SOURCE_READER.AllStreams, false);
            if (MFError.Succeeded(hr))
                hr = reader.SetStreamSelection((int)MF_SOURCE_READER.FirstVideoStream, true);
            ComClass.SafeRelease(ref outputType);
            if (MFError.Failed(hr))
            {
                log.Error("Failed to initialize video: {0}", _path);
                ComClass.SafeRelease(ref reader);
            }
            return reader;
        }

        protected override void ReadNextFrame()
        {
            if (_videoReader == null) Rewind();
            if (_videoReader == null || _endOfAnimation) return;
            try
            {
                HResult result = _videoReader.ReadSample((int)MF_SOURCE_READER.FirstVideoStream, 0, out int streamIndex, out MF_SOURCE_READER_FLAG flags, out long timeStamp, out IMFSample sample);
                if (result < 0)
                {
                    log.Info("Failed to read sample from {0} {1}", _path, result);
                    ComClass.SafeRelease(ref sample);
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
                    _videoReader = SetupVideoDecoder(_videoReader);
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
                        GraphicUtils.BGRtoRGB(scanLine0, pitch, _videoWidth, _videoHeight);
                        _frame = new Bitmap(_videoWidth, _videoHeight, pitch, PixelFormat.Format24bppRgb, scanLine0);
                        frame.Unlock2D();
                    }
                    else
                    {
                        log.Error("Failed to read frame");
                    }
                    _frameTime = frameTime / 10000000.0f;
                    _frameDuration = frameDuration / 10000000.0f;
                    ComClass.SafeRelease(ref sample);
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
            if (Visible && _frame != null) graphics.DrawImage(_frame, X, Y, Width, Height);
        }

        public override string ToString()
        {
            return string.Format("Video '{0}'", _path);
        }
    }

}
