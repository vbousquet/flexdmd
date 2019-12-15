using Cyotek.Drawing.BitmapFont;
using NLog;
using System;
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
                var newText = Regex.Replace(value, @"\r\n|\n\r|\n|\r", "\r\n");
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

        private void UpdateBounds()
        {
            if (_text == null || _font == null) return;
            var size = _font._font.MeasureFont(_text);
            _width = size.Width;
            _height = size.Height;
            // log.Info("Label '{0}' {1}x{2}", _text, _width, _height);
        }

        public override void Draw(Graphics graphics)
        {
            base.Draw(graphics);
            if (_visible) _font.DrawText(graphics, _x, _y, _text);
        }
    }
}
