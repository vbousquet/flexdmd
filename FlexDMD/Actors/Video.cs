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
using System.Runtime.InteropServices;

// Based on : https://docs.microsoft.com/en-us/windows/win32/medfound/processing-media-data-with-the-source-reader?redirectedfrom=MSDN
// Using : https://docs.microsoft.com/en-us/windows/win32/medfound/uncompressed-video-buffers
//
// FIXME add sound decoding support (see https://github.com/microsoft/Windows-classic-samples/blob/master/Samples/Win7Samples/multimedia/mediafoundation/AudioClip/main.cpp)
// http://csharphelper.com/blog/2016/08/play-an-audio-resource-in-c/
// https://docs.microsoft.com/en-us/dotnet/api/system.media.soundplayer?view=netframework-4.8
// Wevout : https://www.codeproject.com/Articles/866347/Streaming-Audio-to-the-WaveOut-Device
// https://www.codeproject.com/Articles/3352/A-low-level-audio-player-in-C
// https://stackoverflow.com/questions/32975300/low-level-audio-player-with-c-sharp-playback-stutter-fix
namespace FlexDMD
{
    class Video : AnimatedActor
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private static int nOpenedVideos = 0;
        private readonly string _path;
        private readonly int _stretchMode; // stretch: 0, crop to top: 1, crop to center: 2, crop to bottom: 3
        private int _videoWidth, _videoHeight;
        private IMFSourceReader _reader;
        private Bitmap _frame;

        static woLib WaveOut = new woLib(); // interface to classic windows wave-out device

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
            if (_reader != null) SafeRelease(ref _reader);
            WaveOut.ResetWODevice();
        }

        protected override void Rewind()
        {
            // log.Info("Initalizing video: {0}", _path);
            if (_reader != null) SafeRelease(ref _reader);
            HResult hr = MFExtern.MFCreateSourceReaderFromURL(_path, null, out IMFSourceReader reader);
            if (MFError.Failed(hr))
            {
                log.Error("Failed to open video: {0}", _path);
                return;
            }
            processAudio(reader);
            _reader = SetupVideoDecoder(reader);
            ReadNextFrame();
        }

        private void processAudio(IMFSourceReader reader)
        {
            // HResult hr = MFExtern.MFCreateSourceReaderFromURL(_path, null, out IMFSourceReader reader);
            // if (MFError.Succeeded(hr))
            HResult hr = SetupAudioDecoder(reader);
            if (MFError.Failed(hr))
                return;
            IMFMediaBuffer buffer = null;
            IMFSample sample = null;
            while (true)
            {
                HResult result = reader.ReadSample((int)MF_SOURCE_READER.FirstAudioStream, 0, out int streamIndex, out MF_SOURCE_READER_FLAG flags, out long timeStamp, out sample);
                if (MFError.Failed(hr))
                    break;
                if (flags == MF_SOURCE_READER_FLAG.CurrentMediaTypeChanged)
                    log.Error("Current type changed [unsupported]");
                if (flags == MF_SOURCE_READER_FLAG.EndOfStream)
                {
                    log.Info("End of stream");
                    break;
                }
                if (sample == null)
                    continue;
                hr = sample.ConvertToContiguousBuffer(out buffer);
                if (MFError.Failed(hr))
                    break;
                hr = buffer.Lock(out IntPtr ppbBuffer, out int pcbMaxLength, out int pcbCurrentLength);
                if (MFError.Failed(hr))
                    break;
                // TODO enqueue for playing in waveout
                WaveOut.SendWODevice(ppbBuffer, (uint)pcbCurrentLength);
                hr = buffer.Unlock();
                if (MFError.Failed(hr))
                    break;
                SafeRelease(ref buffer);
                SafeRelease(ref sample);
            }
            SafeRelease(ref buffer);
            SafeRelease(ref sample);
            // SafeRelease(ref reader);
        }

        private HResult SetupAudioDecoder(IMFSourceReader reader)
        {
            // Setup decoder to uncompressed PCM
            HResult hr = MFExtern.MFCreateMediaType(out IMFMediaType outputType);
            if (MFError.Succeeded(hr))
                hr = outputType.SetGUID(MFAttributesClsid.MF_MT_MAJOR_TYPE, MFMediaType.Audio);
            if (MFError.Succeeded(hr))
                hr = outputType.SetGUID(MFAttributesClsid.MF_MT_SUBTYPE, MFMediaType.PCM);
            if (MFError.Succeeded(hr))
                hr = reader.SetCurrentMediaType((int)MF_SOURCE_READER.FirstAudioStream, null, outputType);
            if (MFError.Succeeded(hr))
                hr = reader.SetStreamSelection((int)MF_SOURCE_READER.FirstAudioStream, true);
            SafeRelease(ref outputType);

            // Setup waveout with the right output format
            IMFMediaType currentType = null;
            if (MFError.Succeeded(hr))
                hr = reader.GetCurrentMediaType((int)MF_SOURCE_READER.FirstAudioStream, out currentType);
            int cChannels = 0, samplesPerSec = 0, bitsPerSample = 0;
            if (MFError.Succeeded(hr))
                hr = currentType.GetUINT32(MFAttributesClsid.MF_MT_AUDIO_NUM_CHANNELS, out cChannels);
            if (MFError.Succeeded(hr))
                hr = currentType.GetUINT32(MFAttributesClsid.MF_MT_AUDIO_SAMPLES_PER_SECOND, out samplesPerSec);
            if (MFError.Succeeded(hr))
                hr = currentType.GetUINT32(MFAttributesClsid.MF_MT_AUDIO_BITS_PER_SAMPLE, out bitsPerSample);
            SafeRelease(ref currentType);
            WaveOut.InitWODevice((uint)samplesPerSec, (uint)cChannels, (uint)bitsPerSample, false);
            return hr;
        }

        private IMFSourceReader SetupVideoDecoder(IMFSourceReader reader)
        {
            // Get the frame size
            HResult hr = reader.GetNativeMediaType((int)MF_SOURCE_READER.FirstVideoStream, 0, out IMFMediaType nativeType);
            if (MFError.Succeeded(hr))
                hr = MFExtern.MFGetAttributeSize(nativeType, MFAttributesClsid.MF_MT_FRAME_SIZE, out _videoWidth, out _videoHeight);
            SafeRelease(ref nativeType);

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
                hr = reader.SetStreamSelection((int)MF_SOURCE_READER.FirstVideoStream, true);
            SafeRelease(ref outputType);
            if (MFError.Failed(hr))
            {
                log.Error("Failed to initialize video: {0}", _path);
                SafeRelease(ref reader);
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
                    SafeRelease(ref sample);
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
                    _reader = SetupVideoDecoder(_reader);
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
                    SafeRelease(ref sample);
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

        /// <summary>
        /// Realease a COM instance and set the variable to null.
        /// </summary>
        /// <typeparam name="T">A COM Interface.</typeparam>
        /// <param name="obj">A COM instance to release and then set to null.</param>
        public static void SafeRelease<T>(ref T obj) where T : class
        {
            if (obj == null) return;

            if (!Marshal.IsComObject(obj))
                throw new ArgumentException("obj is not a COM object.", "obj");

            Marshal.ReleaseComObject(obj);
            obj = null;
        }
    }

}
