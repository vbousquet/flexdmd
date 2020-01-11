using System.Drawing;
using System.Runtime.InteropServices;

namespace FlexDMD
{

    // [Guid("3EF157CD-9A77-4B70-995F-1F3FAFCA4412"), ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class Frame : Actor, IFrameActor
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
