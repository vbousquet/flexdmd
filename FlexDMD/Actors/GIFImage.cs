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
using NLog;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace FlexDMD
{
    class GIFImage : AnimatedActor
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private int _pos = 0;
        private Bitmap _image = null;
        private float[] _frameDelays;

        public GIFImage(string path)
        {
            const int PropertyTagFrameDelay = 0x5100;
            // log.Info("Initalizing image: {0}", path);
            _image = new Bitmap(path);
            var item = _image.GetPropertyItem(PropertyTagFrameDelay);
            _frameDelays = new float[_image.GetFrameCount(FrameDimension.Time)];
            if (item.Type != 4)
            {
                log.Error("Invalid GIF: {0}, frame delays are not of type 4 (32 bits unsigned int) but {1}", path, item.Type);
                _image = null;
                return;
            }
            for (int i = 0; i < _frameDelays.Length; i++)
            {
                _frameDelays[i] = BitConverter.ToInt32(item.Value, i * 4) / 100.0f;
                // log.Info("Frame length: {0}, {1}", path, _frameDelays[i]);
            }
            Rewind();
        }

        protected override void Rewind()
        {
            _pos = 0;
            _frameTime = 0;
            _frameDuration = _frameDelays[0];
        }

        protected override void ReadNextFrame()
        {
            if (_pos >= _frameDelays.Length - 1)
            {
                _endOfAnimation = true;
            }
            else
            {
                _pos++;
                _frameTime = 0;
                for (int i = 0; i < _pos; i++)
                    _frameTime += _frameDelays[i];
                _frameDuration = _frameDelays[_pos];
                _image.SelectActiveFrame(FrameDimension.Time, _pos);
                Rectangle rect = new Rectangle(0, 0, _image.Width, _image.Height);
                BitmapData data = _image.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                GraphicUtils.BGRtoRGB(data.Scan0, data.Stride);
                _image.UnlockBits(data);
            }
        }

        public override void Draw(Graphics graphics)
        {
            if (_visible && _image != null) graphics.DrawImage(_image, _x, _y, _image.Width, _image.Height);
        }
    }
}

