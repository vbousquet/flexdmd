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
using System.IO;

namespace FlexDMD
{
    public class Font
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private Bitmap[] _textures;
        private readonly AssetManager _assets;

        public FontDef FontDef { get; }
        public BitmapFont BitmapFont { get; }

        public Font(AssetManager assets, FontDef fontDef)
        {
            _assets = assets;
            FontDef = fontDef;
            BitmapFont = new BitmapFont();
            using (Stream stream = assets.OpenStream(fontDef.Path))
            {
                BitmapFont.LoadText(stream);
            }
        }

        uint ARGBToABGR(uint argbColor)
        {
            uint r = (argbColor >> 16) & 0xFF;
            uint b = argbColor & 0xFF;
            return (argbColor & 0xFF00FF00) | (b << 16) | r;
        }

        private void PrepareTextures()
        {
            if (_textures != null) return;
            _textures = new Bitmap[BitmapFont.Pages.Length];

            // Load textures (relative to the main font path)
            for (int i = 0; i < BitmapFont.Pages.Length; i++)
                _textures[i] = new Bitmap(_assets.OpenStream(BitmapFont.Pages[i].FileName, FontDef.Path));

            // Render outlines font (note that the outline is created in the glyph padding area, so the font must have a padding of 1 pixel per char on all sides)
            if (FontDef.BorderSize > 0)
            {
				uint outline = ARGBToABGR((uint) FontDef.BorderTint.ToArgb());
                // TODO do not process the complete bitmap but only the glyph areas
                for (int i = 0; i < BitmapFont.Pages.Length; i++)
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
						// First pass draw the borders (spill in the inside of the glyph as well)
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
						// Second pass redraw the glyph (to tint it and clean up border spills)
                        byte* srcPt = (byte*)srcData.Scan0;
                        byte* dstPt = (byte*)dstData.Scan0;
						for (int y = 0; y < h; ++y)
                        {
                            for (int x = 0; x < w; ++x)
                            {
								byte srcR = (byte)*srcPt;
								srcPt++;
								byte srcG = (byte)*srcPt;
								srcPt++;
								byte srcB = (byte)*srcPt;
								srcPt++;
								byte srcA = (byte)*srcPt;
								srcPt++;
								if (srcA == 0)
								{
									dstPt += 4;
								}
								else 
								{
									*dstPt = (byte)((srcR * FontDef.Tint.R) / 255);
									dstPt++;
									*dstPt = (byte)((srcG * FontDef.Tint.G) / 255);
									dstPt++;
									*dstPt = (byte)((srcB * FontDef.Tint.B) / 255);
									dstPt++;
									*dstPt = (byte)((srcA * FontDef.Tint.A) / 255);
									dstPt++;
								}
                            }
                            srcPt += srcOffset * 4;
                            dstPt += dstOffset * 4;
                        }

                    }
                    src.UnlockBits(srcData);
                    dst.UnlockBits(dstData);
                    _textures[i].Dispose();
                    _textures[i] = dst;
                }
                var charDictionary = new Dictionary<char, Character>();
                foreach (KeyValuePair<char, Character> glyph in BitmapFont.Characters)
                {
                    var character = glyph.Value;
                    // character.Bounds = new Rectangle(character.Bounds.X - 1, character.Bounds.Y - 1, character.Bounds.Width + 2, character.Bounds.Height + 2);
                    character.XAdvance += 2; // Adjust to add a padding between characters to account for the outline
                    charDictionary.Add(glyph.Key, character);
                }
                BitmapFont.Characters = charDictionary;
            }

            // Render tinted font
            else if (FontDef.Tint != Color.White)
            {
                for (int i = 0; i < BitmapFont.Pages.Length; i++)
                {
                    BitmapData bmData = _textures[i].LockBits(new Rectangle(0, 0, _textures[i].Width, _textures[i].Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                    int stride = bmData.Stride;
                    unsafe
                    {
                        byte* p = (byte*)bmData.Scan0;
                        int offset = stride - _textures[i].Width * 4;
                        for (int y = 0; y < _textures[i].Height; ++y)
                        {
                            for (int x = 0; x < _textures[i].Width; ++x)
                            {
                                *p = (byte)(((*p) * FontDef.Tint.R) / 255);
                                p++;
                                *p = (byte)(((*p) * FontDef.Tint.G) / 255);
                                p++;
                                *p = (byte)(((*p) * FontDef.Tint.B) / 255);
                                p++;
                                *p = (byte)(((*p) * FontDef.Tint.A) / 255);
                                p++;
                            }
                            p += offset;
                        }
                    }
                    _textures[i].UnlockBits(bmData);
                }
            }
        }

        private void DrawCharacter(Graphics graphics, char character, char previousCharacter, ref float x, ref float y)
        {
            switch (character)
            {
                case '\n':
                    x = 0;
                    y += BitmapFont.LineHeight;
                    break;
                default:
                    Character data;
                    if (BitmapFont.Characters.TryGetValue(character, out data))
                    {
                        int kerning = BitmapFont.GetKerning(previousCharacter, character);
                        graphics?.DrawImage(_textures[data.TexturePage], new RectangleF((int)(x + data.Offset.X + kerning), (int)(y + data.Offset.Y), data.Bounds.Width, data.Bounds.Height), data.Bounds, GraphicsUnit.Pixel);
                        // graphics?.DrawImage(_textures[data.TexturePage], new RectangleF(x + data.Offset.X + kerning, y + data.Offset.Y, data.Bounds.Width, data.Bounds.Height), data.Bounds, GraphicsUnit.Pixel);
                        x += data.XAdvance + kerning;
                    }
                    else if ('a' <= character && character <= 'z' && BitmapFont.Characters.ContainsKey(char.ToUpper(character)))
                    {
                        // log.Error("Missing character '{0}' replaced by '{1}' for font {2}", character, char.ToUpper(character), FontDef.Path);
                        BitmapFont.Characters[character] = BitmapFont[char.ToUpper(character)];
                        DrawCharacter(graphics, character, previousCharacter, ref x, ref y);
                    }
                    else if (BitmapFont.Characters.ContainsKey(' '))
                    {
                        log.Error("Missing character #{0} replaced by ' ' for font {1}", (int)character, FontDef.Path);
                        BitmapFont.Characters[character] = BitmapFont[' '];
                        DrawCharacter(graphics, character, previousCharacter, ref x, ref y);
                    }
                    break;
            }
        }

        public Size MeasureFont(string text)
        {
            // perform missing character swapping before measuring text
            DrawText(null, 0f, 0f, text);
            return BitmapFont.MeasureFont(text);
        }

        public void DrawText(Graphics graphics, float x, float y, string text)
        {
            PrepareTextures();
            char previousCharacter = ' ';
            foreach (char character in text)
            {
                DrawCharacter(graphics, character, previousCharacter, ref x, ref y);
                previousCharacter = character;
            }
        }
    }
}
