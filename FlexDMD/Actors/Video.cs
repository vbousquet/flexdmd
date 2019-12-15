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
namespace FlexDMD
{
    class Video : AnimatedActor
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
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
        }

        public void Close()
        {
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
            HResult result = reader.GetNativeMediaType(0, 0, out IMFMediaType nativeType);
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
            result = MFExtern.MFCreateMediaType(out IMFMediaType outputType);
            if (result < 0)
            {
                log.Error("Failed to create decoder media type for video: {0}", _path);
                COMBase.SafeRelease(nativeType);
                COMBase.SafeRelease(reader);
                return null;
            }
            outputType.SetGUID(MFAttributesClsid.MF_MT_MAJOR_TYPE, majorType);
            Guid subtype;
            if (majorType == MFMediaType.Video)
            {
                subtype = MFMediaType.RGB24;
            }
            else if (majorType == MFMediaType.Audio)
            {
                subtype = MFMediaType.PCM;
            }
            else
            {
                log.Error("Unsupported stream type for video: {0}", _path);
                COMBase.SafeRelease(nativeType);
                COMBase.SafeRelease(outputType);
                COMBase.SafeRelease(reader);
                return null;
            }
            result = outputType.SetGUID(MFAttributesClsid.MF_MT_SUBTYPE, subtype);
            if (result < 0)
            {
                log.Error("Failed to setup decoder type for video: {0}", _path);
                COMBase.SafeRelease(nativeType);
                COMBase.SafeRelease(outputType);
                COMBase.SafeRelease(reader);
                return null;
            }
            result = reader.SetCurrentMediaType(0, null, outputType);
            if (result < 0)
            {
                log.Error("Failed to setup decoder for video: {0}", _path);
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
            if (_visible && _frame != null) graphics.DrawImage(_frame, _x, _y, 128, 32);
        }

        public override string ToString()
        {
            return string.Format("Video '{0}'", _path);
        }
    }

}
