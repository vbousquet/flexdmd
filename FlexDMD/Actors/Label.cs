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
using System.Text.RegularExpressions;

namespace FlexDMD
{
    // [Guid("BCAC5C64-9E46-431A-860C-DDBE38195965"), ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class Label : Actor, ILabelActor
    {
        private Font _font;
        private string _text;
        private string[] _lines;
        private float _textWidth, _textHeight;

        public Label(IFlexDMD flex)
        {
            AutoPack = flex.RuntimeVersion <= 1008;
        }

        public Label(IFlexDMD flex, Font font, string text)
        {
            AutoPack = flex.RuntimeVersion <= 1008;
            _font = font;
            Text = text;
            Pack();
        }

        public Alignment Alignment { get; set; } = Alignment.Center;

        public override float PrefWidth { get => _textWidth; }

        public override float PrefHeight { get => _textHeight; }

        public bool AutoPack { get; set; } = false;

        public string Text
        {
            get => _text;
            set
            {
                string newText;
                if (value == null)
                {
                    newText = "";
                }
                else
                {
                    newText = Regex.Replace(value, @"\r\n|\n\r|\n|\r", "\r\n");
                }
                if (_text == null || !_text.Equals(newText))
                {
                    _text = newText;
                    _lines = _text.Split('\n');
                    UpdateBounds();
                }
            }
        }

        public Font Font
        {
            get => _font;
            set
            {
                if (_font != value)
                {
                    _font = value;
                    UpdateBounds();
                }
            }
        }

        private void UpdateBounds()
        {
            if (_text == null || _font == null) return;
            var size = _font.MeasureFont(_text);
            _textWidth = size.Width;
            _textHeight = size.Height;
            if (AutoPack) Pack();
        }

        public override void Draw(Graphics graphics)
        {
            base.Draw(graphics);
            if (Visible && _font != null && _text != null)
            {
                if (_lines.Length > 1 && Alignment != Alignment.Left && Alignment != Alignment.BottomLeft && Alignment != Alignment.TopLeft)
                {
                    // For multiple lines with cneter or right aligne, we must perform the alignment line by line
                    Layout.Align(Alignment, PrefWidth, PrefHeight, Width, Height, out float x, out float y);
                    foreach (string line in _lines)
                    {
                        Layout.Align(Alignment, _font.MeasureFont(line).Width, PrefHeight, Width, Height, out float lx, out float ly);
                        _font.DrawText(graphics, (int)Math.Floor(X + lx), (int)Math.Floor (Y + y), line);
                        y += _font.BitmapFont.LineHeight;
                    }
                }
                else
                {
                    Layout.Align(Alignment, PrefWidth, PrefHeight, Width, Height, out float x, out float y);
                    _font.DrawText(graphics, (int)Math.Floor(X + x), (int)Math.Floor(Y + y), _text);
                }
            }
        }
    }
}
