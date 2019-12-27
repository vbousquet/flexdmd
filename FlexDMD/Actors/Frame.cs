using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexDMD.Actors
{
    class Frame : Actor
    {
        private SolidBrush _brush = new SolidBrush(Color.White);
        private int _thickness = 2;

        public Frame(int thickness)
        {
            _thickness = thickness;
        }

        public override void Draw(Graphics graphics)
        {
            base.Draw(graphics);
            graphics.FillRectangle(_brush, new RectangleF(X, Y, Width, _thickness));
            graphics.FillRectangle(_brush, new RectangleF(X, Y + Height - _thickness, Width, _thickness));
            graphics.FillRectangle(_brush, new RectangleF(X, Y + _thickness, _thickness, Height - 2 * _thickness));
            graphics.FillRectangle(_brush, new RectangleF(X + Width - _thickness, Y + _thickness, _thickness, Height - 2 * _thickness));
        }
    }
}
