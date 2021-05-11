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
using NAudio.CoreAudioApi.Interfaces;
using NAudio.MediaFoundation;
using NAudio.Utils;
using NAudio.Wave;
using NLog;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace FlexDMD
{
    public class Video : AnimatedActor
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private static int nOpenedVideos = 0;
        private readonly string _path;
        private readonly int _videoWidth, _videoHeight;
        private readonly float _length;
        private IMFSourceReader _videoReader;
        private MediaFoundationReader _audioReader;
        private WaveOutEvent _audioDevice;
        private Bitmap _frame;
        private bool _visible = true;
        private bool _opened = false;
        private float _seek = -1f;
        private int format = -1;

        public static readonly Guid MF_MT_FRAME_SIZE = new Guid(0x1652c33d, 0xd6b2, 0x4012, 0xb8, 0x34, 0x72, 0x03, 0x08, 0x49, 0xa3, 0x7d);
        public static readonly Guid MF_SOURCE_READER_ENABLE_VIDEO_PROCESSING = new Guid(0xfb394f3d,0xccf1,0x42ee,0xbb,0xb3,0xf9,0xb8,0x45,0xd5,0x68,0x1d);
        public static readonly Guid MFVideoFormat_RGB24 = new Guid(20, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
        public static readonly Guid MFVideoFormat_ARGB32 = new Guid(21, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
        public static readonly Guid MFVideoFormat_RGB32 = new Guid(22, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("7DC9D5F9-9ED9-44EC-9BBF-0600BB589FBB")]
        public interface IMF2DBuffer
        {
            void Lock2D([Out] out IntPtr pbScanline0, out int plPitch);
            void Unlock2D();
            void GetScanline0AndPitch(out IntPtr pbScanline0, out int plPitch);
            void IsContiguousFormat([MarshalAs(UnmanagedType.Bool)] out bool pfIsContiguous);
            void GetContiguousLength(out int pcbLength);
            void ContiguousCopyTo(IntPtr pbDestBuffer, [In] int cbDestBuffer);
            void ContiguousCopyFrom([In] IntPtr pbSrcBuffer, [In] int cbSrcBuffer);
        }

        public override float Length { get => _length; }
        public override float PrefWidth { get => _videoWidth; }
        public override float PrefHeight { get => _videoHeight; }

        public override bool Visible
        {
            get => _visible;
            set
            {
                _visible = value;
                OnStageStateChanged();
            }
        }

        public Video(string path, string name = "", bool loop = false)
        {
            _path = path;
            Name = name;
            Loop = loop;
            // Get the frame size
            MediaFoundationInterop.MFCreateSourceReaderFromURL(_path, null, out IMFSourceReader reader);
            reader.GetNativeMediaType(MediaFoundationInterop.MF_SOURCE_READER_FIRST_VIDEO_STREAM, 0, out IMFMediaType nativeType);
            nativeType.GetUINT64(MF_MT_FRAME_SIZE, out long size);
            _videoWidth = (int)((size >> 32) & 0x7FFFFFFF);
            _videoHeight = (int)(size & 0x7FFFFFFF);
            _length = GetLength(reader);
            Marshal.ReleaseComObject(nativeType);
            Marshal.ReleaseComObject(reader);
            Pack();
        }

        private float GetLength(IMFSourceReader reader)
        {
            var variantPtr = Marshal.AllocHGlobal(Marshal.SizeOf<PropVariant>());
            try
            {
                // http://msdn.microsoft.com/en-gb/library/windows/desktop/dd389281%28v=vs.85%29.aspx#getting_file_duration
                int hResult = reader.GetPresentationAttribute(MediaFoundationInterop.MF_SOURCE_READER_MEDIASOURCE,
                    MediaFoundationAttributes.MF_PD_DURATION, variantPtr);
                if (hResult == MediaFoundationErrors.MF_E_ATTRIBUTENOTFOUND)
                {
                    // this doesn't support telling us its duration (might be streaming)
                    return 0;
                }
                if (hResult != 0)
                {
                    Marshal.ThrowExceptionForHR(hResult);
                }
                var variant = Marshal.PtrToStructure<PropVariant>(variantPtr);
                return ((long)variant.Value) / 10000000f;
            }
            finally
            {
                PropVariant.Clear(variantPtr);
                Marshal.FreeHGlobal(variantPtr);
            }
        }

        // open/close when entering/exiting rendering graph since MMF has a limited number of concurrently opened sources
        protected override void OnStageStateChanged()
        {
            bool shouldBeOpened = _visible && OnStage;
            if (shouldBeOpened && !_opened)
            {
                _opened = true;
                try
                {
                    _audioReader = new MediaFoundationReader(_path);
                    _audioDevice = new WaveOutEvent();
                    _audioDevice.Init(_audioReader);
                    _audioDevice.Play();
                }
                catch
                {
                    _audioReader?.Dispose();
                    _audioReader = null;
                    _audioDevice?.Dispose();
                    _audioDevice = null;
                }
                try
                {
                    var attributes = MediaFoundationApi.CreateAttributes(1);
                    attributes.SetUINT32(MF_SOURCE_READER_ENABLE_VIDEO_PROCESSING, 1);
                    // attributes.SetUINT32(MediaFoundationAttributes.MF_READWRITE_ENABLE_HARDWARE_TRANSFORMS, 1);
                    // attributes.SetUINT32(MF_SOURCE_READER_ENABLE_ADVANCED_VIDEO_PROCESSING, 1);
                    MediaFoundationInterop.MFCreateSourceReaderFromURL(_path, attributes, out _videoReader);
                    SetupVideoDecoder(_videoReader);
                    ReadNextFrame();
                    nOpenedVideos++;
                    log.Info("Video opened: {0} size={1}x{2} length={3}s ({4} videos are currently opened)", _path, _videoWidth, _videoHeight, _length, nOpenedVideos);
                }
                catch
                {
                    if (_videoReader != null) Marshal.ReleaseComObject(_videoReader);
                    _videoReader = null;
                    log.Info("Failed to open video: {0} ({1} videos are currently opened)", _path, nOpenedVideos);
                }
            }
            else if (!shouldBeOpened && _opened)
            {
                _opened = false;
                _audioReader?.Dispose();
                _audioReader = null;
                _audioDevice?.Dispose();
                _audioDevice = null;
                if (_videoReader != null) Marshal.ReleaseComObject(_videoReader);
                _videoReader = null;
                nOpenedVideos--;
                log.Info("Video closed: {0} size={1}x{2} length={3}s ({4} remaining videos are currently opened)", _path, _videoWidth, _videoHeight, _length, nOpenedVideos);
            }
        }

        public override void Seek(float position)
        {
            _seek = position;
            _time = position;
            if (_audioReader != null)
            {
                _audioReader.CurrentTime = TimeSpan.FromSeconds(position);
            }
            if (_videoReader != null)
            {
                ReadNextFrame();
                _time = _frameTime;
            }
        }

        protected override void Rewind()
        {
            base.Rewind();
            Seek(0f);
        }

        private void SetupVideoDecoder(IMFSourceReader reader)
        {
            // We first try to read the video as RGB24 frames since they support IMF2DBuffer which is better than the
            // default IMFMediaBuffer interface which does not provide the stride and may cause frame copy on access.
            try
            {
                IMFMediaType outputType = MediaFoundationApi.CreateMediaType();
                outputType.SetGUID(MediaFoundationAttributes.MF_MT_MAJOR_TYPE, MediaTypes.MFMediaType_Video);
                outputType.SetGUID(MediaFoundationAttributes.MF_MT_SUBTYPE, MFVideoFormat_RGB24);
                reader.SetCurrentMediaType(MediaFoundationInterop.MF_SOURCE_READER_FIRST_VIDEO_STREAM, IntPtr.Zero, outputType);
                reader.SetStreamSelection(MediaFoundationInterop.MF_SOURCE_READER_ALL_STREAMS, false);
                reader.SetStreamSelection(MediaFoundationInterop.MF_SOURCE_READER_FIRST_VIDEO_STREAM, true);
                Marshal.ReleaseComObject(outputType);
                format = 3;
            } 
            catch (Exception)
            {
                log.Info("Default RGB24 is not supported, trying RGB32 for file '{0}'", _path);
                try
                {
                    IMFMediaType outputType = MediaFoundationApi.CreateMediaType();
                    outputType.SetGUID(MediaFoundationAttributes.MF_MT_MAJOR_TYPE, MediaTypes.MFMediaType_Video);
                    outputType.SetGUID(MediaFoundationAttributes.MF_MT_SUBTYPE, MFVideoFormat_RGB32);
                    reader.SetCurrentMediaType(MediaFoundationInterop.MF_SOURCE_READER_FIRST_VIDEO_STREAM, IntPtr.Zero, outputType);
                    reader.SetStreamSelection(MediaFoundationInterop.MF_SOURCE_READER_ALL_STREAMS, false);
                    reader.SetStreamSelection(MediaFoundationInterop.MF_SOURCE_READER_FIRST_VIDEO_STREAM, true);
                    Marshal.ReleaseComObject(outputType);
                    format = 4;
                }
                catch (Exception e)
                {
                    log.Error(e, "Failed to setup video decoder for file '{0}'", _path);
                    format = -1;
                }
            }
        }

        protected override void ReadNextFrame()
        {
            if (_videoReader == null || _endOfAnimation)
            {
                _endOfAnimation = true;
                return;
            }
            try
            {
                if (_seek >= 0)
                {
                    var pv = PropVariant.FromLong((long)(_seek * 10000000L));
                    var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(pv));
                    Marshal.StructureToPtr(pv, ptr, false);
                    // should pass in a variant of type VT_I8 which is a long containing time in 100nanosecond units
                    _videoReader.SetCurrentPosition(Guid.Empty, ptr);
                    Marshal.FreeHGlobal(ptr);
                    _seek = -1f;
                }
                _videoReader.ReadSample(MediaFoundationInterop.MF_SOURCE_READER_FIRST_VIDEO_STREAM, 0, out int streamIndex, out MF_SOURCE_READER_FLAG flags, out ulong timeStamp, out IMFSample sample);
                if (flags == MF_SOURCE_READER_FLAG.MF_SOURCE_READERF_ENDOFSTREAM)
                {
                    log.Info("End of stream");
                    _endOfAnimation = true;
                }
                if (flags == MF_SOURCE_READER_FLAG.MF_SOURCE_READERF_NEWSTREAM)
                {
                    log.Info("New stream");
                }
                if (flags == MF_SOURCE_READER_FLAG.MF_SOURCE_READERF_NATIVEMEDIATYPECHANGED)
                {
                    log.Info("Native type changed");
                    // The format changed. Reconfigure the decoder.
                    SetupVideoDecoder(_videoReader);
                }
                if (flags == MF_SOURCE_READER_FLAG.MF_SOURCE_READERF_CURRENTMEDIATYPECHANGED)
                {
                    log.Info("Current type changed");
                }
                if (flags == MF_SOURCE_READER_FLAG.MF_SOURCE_READERF_STREAMTICK)
                {
                    log.Info("Stream tick");
                }
                if (sample != null)
                {
                    // in 100-nano sconds units
                    sample.GetSampleTime(out long frameTime);
                    sample.GetSampleDuration(out long frameDuration);
                    sample.GetBufferByIndex(0, out IMFMediaBuffer buffer);

                    // log.Info("Frame read, next frame at {0} + {1} = {2}", _frameTime, _frameDuration, _frameTime + _frameDuration);

                    if (buffer is IMF2DBuffer frame)
                    {
                        frame.Lock2D(out IntPtr scanLine0, out int pitch);
                        if (format == 3)
                        {
                            GraphicUtils.BGRtoRGB(scanLine0, pitch, _videoWidth, _videoHeight);
                            _frame = new Bitmap(_videoWidth, _videoHeight, pitch, PixelFormat.Format24bppRgb, scanLine0);
                        }
                        else if (format == 4)
                        {
                            GraphicUtils.ABGRtoARGB(scanLine0, pitch, _videoWidth, _videoHeight);
                            _frame = new Bitmap(_videoWidth, _videoHeight, pitch, PixelFormat.Format32bppRgb, scanLine0);
                        }
                        frame.Unlock2D();
                    }
                    else
                    {
                        buffer.Lock(out IntPtr ppbBuffer, out int pcbMaxLength, out int pcbCurrentLength);
                        var pitch = _videoWidth * format;
                        if (pcbCurrentLength != _videoHeight * pitch)
                        {
                            log.Error("Invalid amount of data returned for {0}x{1} video: {2}");
                        }
                        else if (format == 3)
                        {
                            GraphicUtils.BGRtoRGB(ppbBuffer, pitch, _videoWidth, _videoHeight);
                            var f = new Bitmap(_videoWidth, _videoHeight, pitch, PixelFormat.Format24bppRgb, ppbBuffer);
                            _frame = new Bitmap(f); // Unlink from the temporary data buffer
                        }
                        else if (format == 4)
                        {
                            GraphicUtils.ABGRtoARGB(ppbBuffer, pitch, _videoWidth, _videoHeight);
                            var f = new Bitmap(_videoWidth, _videoHeight, pitch, PixelFormat.Format32bppRgb, ppbBuffer);
                            _frame = new Bitmap(f); // Unlink from the temporary data buffer
                        }
                        buffer.Unlock();
                    }

                    _frameTime = frameTime / 10000000.0f;
                    _frameDuration = frameDuration / 10000000.0f;
                    Marshal.ReleaseComObject(sample);
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
            if (Visible && _frame != null)
            {
                Layout.Scale(Scaling, PrefWidth, PrefHeight, Width, Height, out float w, out float h);
                Layout.Align(Alignment, w, h, Width, Height, out float x, out float y);
                graphics.DrawImage(_frame, (int)(X + x), (int)(Y + y), (int)w, (int)h);
            }
        }

        public override string ToString()
        {
            return string.Format("Video '{0}'", _path);
        }
    }

}
