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
using Cyotek.Drawing.BitmapFont;
using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;

namespace FlexDMD.Actors
{
    class Font
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly Bitmap[] _textures;

        public BitmapFont BitmapFont { get; }

        public Font(BitmapFont font, float brightness = 1f, float outlineBrightness = -1f)
        {
            BitmapFont = font;
            _textures = new Bitmap[font.Pages.Length];
            var assembly = Assembly.GetExecutingAssembly();
            for (int i = 0; i < font.Pages.Length; i++)
            {
                // _textures[i] = new System.Drawing.Bitmap(font.Pages[i].FileName);
                _textures[i] = new Bitmap(assembly.GetManifestResourceStream("FlexDMD.Resources." + font.Pages[i].FileName));
            }

            if (outlineBrightness >= 0f)
            { // Outlines font (note that the outline is created in the glyph padding area, so the font must have at least a padding of 1 pixel per char on all sides)
                uint outlineValue = (uint)(255 * outlineBrightness);
                uint outline = 0xFF000000 | (outlineValue * 0x00010101);
                uint fillValue = (uint)(255 * brightness);
                uint fill = fillValue * 0x00010101;
                if (brightness >= 0f) fill = fill | 0xFF000000;
                for (int i = 0; i < font.Pages.Length; i++)
                {
                    Bitmap src = _textures[i];
                    int w = src.Width, h = src.Height;
                    Bitmap dst = new Bitmap(w, h, PixelFormat.Format32bppArgb);
                    BitmapData srcData = src.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                    BitmapData dstData = dst.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                    unsafe
                    {
                        int srcStride = srcData.Stride / 4;
                        int dstStride = dstData.Stride / 4;
                        int srcOffset = srcStride - w;
                        int dstOffset = dstStride - w;
                        uint* srcP = (uint*)srcData.Scan0;
                        uint* dstP = (uint*)dstData.Scan0;
                        for (int y = 0; y < h; y++)
                        {
                            for (int x = 0; x < w; x++)
                            {
                                if ((*srcP & 0xFF000000) > 0)
                                {
                                    if (x > 0)
                                    {
                                        if (y > 0) dstP[-1 - dstStride] = outline;
                                        dstP[-1] = outline;
                                        if (y < h - 1) dstP[-1 + dstStride] = outline;
                                    }
                                    if (y > 0) dstP[-dstStride] = outline;
                                    if (y < h - 1) dstP[+dstStride] = outline;
                                    if (x < w - 1)
                                    {
                                        if (y > 0) dstP[+1 - dstStride] = outline;
                                        dstP[+1] = outline;
                                        if (y < h - 1) dstP[+1 + dstStride] = outline;
                                    }
                                }
                                srcP++;
                                dstP++;
                            }
                            srcP += srcOffset;
                            dstP += dstOffset;
                        }
                        srcP = (uint*)srcData.Scan0;
                        dstP = (uint*)dstData.Scan0;
                        for (int y = 0; y < h; y++)
                        {
                            for (int x = 0; x < w; x++)
                            {
                                if ((*srcP & 0xFF000000) > 0) *dstP = fill;
                                srcP++;
                                dstP++;
                            }
                            srcP += srcOffset;
                            dstP += dstOffset;
                        }
                    }
                    src.UnlockBits(srcData);
                    dst.UnlockBits(dstData);
                    _textures[i] = dst;
                }
                /*var charDictionary = new Dictionary<char, Character>();
                foreach (KeyValuePair<char, Character> glyph in BitmapFont.Characters)
                {
                    var character = glyph.Value;
                    // already included by BMFont
                    // character.Bounds = new Rectangle(character.Bounds.X - 1, character.Bounds.Y - 1, character.Bounds.Width + 2, character.Bounds.Height + 2);
                    // character.XAdvance += 1; 
                    charDictionary.Add(glyph.Key, character);
                }
                BitmapFont.Characters = charDictionary;*/
            }
            else if (brightness >= 0f && brightness < 1f)
            { // Modulated brightness font
                for (int i = 0; i < font.Pages.Length; i++)
                {
                    BitmapData bmData = _textures[i].LockBits(new Rectangle(0, 0, _textures[i].Width, _textures[i].Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                    int stride = bmData.Stride;
                    IntPtr Scan0 = bmData.Scan0;
                    int nVal = 0;
                    unsafe
                    {
                        byte* p = (byte*)Scan0;
                        int nOffset = stride - _textures[i].Width * 4;
                        int nWidth = _textures[i].Width;
                        for (int y = 0; y < _textures[i].Height; ++y)
                        {
                            for (int x = 0; x < nWidth; ++x)
                            {
                                nVal = (int)((*p) * brightness);
                                if (nVal < 0) nVal = 0;
                                if (nVal > 255) nVal = 255;
                                *p = (byte)nVal;
                                p++;
                                nVal = (int)((*p) * brightness);
                                if (nVal < 0) nVal = 0;
                                if (nVal > 255) nVal = 255;
                                *p = (byte)nVal;
                                p++;
                                nVal = (int)((*p) * brightness);
                                if (nVal < 0) nVal = 0;
                                if (nVal > 255) nVal = 255;
                                *p = (byte)nVal;
                                p += 2;
                            }
                            p += nOffset;
                        }
                    }
                    _textures[i].UnlockBits(bmData);
                }
            }
        }

        public void DrawCharacter(Graphics g, Character character, float x, float y)
        {
            g.DrawImage(_textures[character.TexturePage], new RectangleF((int)x, (int)y, character.Bounds.Width, character.Bounds.Height), character.Bounds, GraphicsUnit.Pixel);
        }

        public void DrawText(Graphics graphics, float x, float y, string text)
        {
            char previousCharacter = ' ';
            foreach (char character in text)
            {
                switch (character)
                {
                    case '\n':
                        x = 0;
                        y += BitmapFont.LineHeight;
                        break;
                    default:
                        try
                        {
                            Character data = BitmapFont[character];
                            int kerning = BitmapFont.GetKerning(previousCharacter, character);
                            DrawCharacter(graphics, data, x + data.Offset.X + kerning, y + data.Offset.Y);
                            x += data.XAdvance + kerning;
                        }
                        catch (Exception e)
                        {
                            log.Error(e, "Failed to render char '{0}'", character);
                        }
                        break;
                }
                previousCharacter = character;
            }
        }
    }
}
