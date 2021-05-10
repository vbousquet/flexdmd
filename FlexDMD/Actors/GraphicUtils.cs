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
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace FlexDMD
{
    class GraphicUtils
    {
        public static void BGRtoRGB(Bitmap image)
        {
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
            if (image.PixelFormat == PixelFormat.Format32bppArgb)
            {
                BitmapData data = image.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                GraphicUtils.ABGRtoARGB(data.Scan0, data.Stride, image.Width, image.Height);
                image.UnlockBits(data);
            }
            else
            {
                BitmapData data = image.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                GraphicUtils.BGRtoRGB(data.Scan0, data.Stride, image.Width, image.Height);
                image.UnlockBits(data);
            }
        }

        public static void ABGRtoARGB(IntPtr scanLine0, int pitch, int width, int height)
        {
            unsafe
            {
                int offset = pitch - 4 * width;
                byte* ptr = ((byte*)scanLine0.ToPointer());
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte r = *ptr;
                        *ptr = *(ptr + 2);
                        *(ptr + 2) = r;
                        ptr += 4;
                    }
                    ptr += offset;
                }
            }
        }

        public static void BGRtoRGB(IntPtr scanLine0, int pitch, int width, int height)
        {
            unsafe
            {
                int offset = pitch - 3 * width;
                byte* ptr = ((byte*)scanLine0.ToPointer());
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte r = *ptr;
                        *ptr = *(ptr + 2);
                        *(ptr + 2) = r;
                        ptr += 3;
                    }
                    ptr += offset;
                }
            }
        }

        public static uint ARGBToABGR(uint argbColor)
        {
            uint r = (argbColor >> 16) & 0xFF;
            uint b = argbColor & 0xFF;
            return (argbColor & 0xFF00FF00) | (b << 16) | r;
        }
    }
}
