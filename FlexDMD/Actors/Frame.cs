using System.Drawing;
using System.Runtime.InteropServices;

namespace FlexDMD
{

    // [Guid("3EF157CD-9A77-4B70-995F-1F3FAFCA4412"), ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class Frame : Actor, IFrameActor
    {
        private readonly SolidBrush _borderBrush = new SolidBrush(Color.White);
        private readonly SolidBrush _fillBrush = new SolidBrush(Color.Black);

        public int Thickness { get; set; } = 2;
        public Color BorderColor { get; set; } = Color.White;
        public bool Fill { get; set; } = false;
        public Color FillColor { get; set; } = Color.Black;

        public override void Draw(Graphics graphics)
        {
            base.Draw(graphics);
            if (Fill)
            {
                _fillBrush.Color = FillColor;
                graphics.FillRectangle(_fillBrush, X + Thickness, Y + Thickness, Width - 2 * Thickness, Height - 2 * Thickness);
            }
            if (Thickness > 0)
            {
                _borderBrush.Color = BorderColor;
                graphics.FillRectangle(_borderBrush, X, Y, Width, Thickness);
                graphics.FillRectangle(_borderBrush, X, Y + Height - Thickness, Width, Thickness);
                graphics.FillRectangle(_borderBrush, X, Y + Thickness, Thickness, Height - 2 * Thickness);
                graphics.FillRectangle(_borderBrush, X + Width - Thickness, Y + Thickness, Thickness, Height - 2 * Thickness);
            }
        }
    }
}
