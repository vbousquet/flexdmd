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
using System.Drawing;
using System.Drawing.Imaging;

namespace FlexDMD
{
    class Image : Actor
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        public Bitmap _image = null;

        public Image(string path) 
        {
            // log.Info("Initalizing image: {0}", path);
            _image = new Bitmap(path);
            Rectangle rect = new Rectangle(0, 0, _image.Width, _image.Height);
            BitmapData data = _image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, _image.PixelFormat);
            GraphicUtils.BGRtoRGB(data.Scan0, data.Stride);
            _image.UnlockBits(data);
        }

        public override void Draw(Graphics graphics)
        {
            if (_visible && _image != null) graphics.DrawImage(_image, _x, _y, _image.Width, _image.Height);
        }
    }
}

