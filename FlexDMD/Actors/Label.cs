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
using NLog;
using System.Drawing;
using System.Text.RegularExpressions;

namespace FlexDMD.Actors
{
    class Label : Actor
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private Font _font;
        private string _text;

        public Label(Font font, string text)
        {
            _font = font;
            Text = text;
        }

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

        public void UpdateBounds()
        {
            if (_text == null || _font == null) return;
            var size = _font.MeasureFont(_text);
            Width = size.Width;
            Height = size.Height;
        }

        public override void Draw(Graphics graphics)
        {
            base.Draw(graphics);
            if (Visible) _font.DrawText(graphics, X, Y, _text);
        }
    }
}
