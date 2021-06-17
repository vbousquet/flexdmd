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

    public interface IBitmapFilter
    {
        Bitmap Filter(Bitmap src);
    }

    public class RegionFilter : IBitmapFilter
    {
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public int Width { get; set; } = 0;
        public int Height { get; set; } = 0;

        public Bitmap Filter(Bitmap src)
        {
            var dst = new Bitmap(Width, Height, src.PixelFormat);
            using (Graphics graph = Graphics.FromImage(dst)) { 
                graph.Clear(Color.FromArgb(0, 0, 0, 0));
                graph.DrawImage(src, new RectangleF(0, 0, Width, Height), new RectangleF(X, Y, Width, Height), GraphicsUnit.Pixel);
            }
            /*for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    dst.SetPixel(x, y, src.GetPixel(X + x, Y + y));*/
            return dst;
        }
    }

    public class PadFilter : IBitmapFilter
    {
        public int Left { get; set; } = 0;
		public int Top { get; set; } = 0;
		public int Right { get; set; } = 0;
		public int Bottom { get; set; } = 0;

        public Bitmap Filter(Bitmap src)
        {
            var dst = new Bitmap(src.Width + Left + Right, src.Height + Top + Bottom, src.PixelFormat);
            Graphics graph = Graphics.FromImage(dst);
            graph.Clear(Color.FromArgb(0, 0, 0, 0));
            graph.DrawImage(src, Left, Top);
            graph.Dispose();
            /* for (int y = 0; y < src.Height; y++)
                for (int x = 0; x < src.Width; x++)
                    dst.SetPixel(Left + x, Top + y, src.GetPixel(x, y)); */
            return dst;
        }
    }

    public class DotFilter : IBitmapFilter
    {
        public int DotSize { get; set; } = 2;
        public int Offset { get; set; } = 0;

        public Bitmap Filter(Bitmap src)
        {
            var dst = new Bitmap(src.Width / DotSize, src.Height / DotSize, PixelFormat.Format32bppArgb);
            for (int y = 0; y < dst.Height; y++)
            {
                for (int x = 0; x < dst.Width; x++)
                {
                    int r = 0, g = 0, b = 0, a = 0;
                    for (int i = 0; i < DotSize; i++)
                    {
                        for (int j = 0; j < DotSize; j++)
                        {
                            Color c = src.GetPixel(x * DotSize + i, y * DotSize + j);
                            r += c.R;
                            g += c.G;
                            b += c.B;
                            a += c.A;
                        }
                    }
                    float bright = 1f + DotSize * DotSize / 1.8f;
                    dst.SetPixel(x, y, Color.FromArgb((int) Math.Min(a / bright, 255), (int)Math.Min(r / bright, 255), (int)Math.Min(g / bright, 255), (int)Math.Min(b / bright, 255)));
                    // dst.SetPixel(x, y, src.GetPixel(x * DotSize + (DotSize - 1) - Offset, y * DotSize + (DotSize - 1)));
                }
            }
            return dst;
        }
    }

    public class AdditiveFilter : IBitmapFilter
    {
        public Bitmap Filter(Bitmap src)
        {
            var dst = new Bitmap(src.Width, src.Height, PixelFormat.Format32bppArgb);
            for (int y = 0; y < dst.Height; y++)
            {
                for (int x = 0; x < dst.Width; x++)
                {
                    var pixel = src.GetPixel(x, y);
                    // var alpha = (int)(4 * ((0.21 * pixel.R + 0.72 * pixel.G + 0.07 * pixel.B) * pixel.A) / 255);
                    // if (alpha > 255) alpha = 255;
                    if (pixel.R < 64 && pixel.G < 64 && pixel.B < 64)
                        dst.SetPixel(x, y, Color.FromArgb(0, pixel));
                    else
                        dst.SetPixel(x, y, pixel);
                }
            }
            return dst;
        }
    }
}