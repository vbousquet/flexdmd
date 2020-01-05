using System.Drawing;
using System.Runtime.InteropServices;

namespace FlexDMD
{
    public class Frame : Actor
    {
        private SolidBrush _brush = new SolidBrush(Color.White);

        public int Thickness { get; set; } = 2;

        public override void Draw(Graphics graphics)
        {
            base.Draw(graphics);
            graphics.FillRectangle(_brush, new RectangleF(X, Y, Width, Thickness));
            graphics.FillRectangle(_brush, new RectangleF(X, Y + Height - Thickness, Width, Thickness));
            graphics.FillRectangle(_brush, new RectangleF(X, Y + Thickness, Thickness, Height - 2 * Thickness));
            graphics.FillRectangle(_brush, new RectangleF(X + Width - Thickness, Y + Thickness, Thickness, Height - 2 * Thickness));
        }
    }
}
