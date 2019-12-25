using System.Drawing;

namespace FlexDMD.Actors
{
    class FadeOverlay : Actor
    {
        private SolidBrush _brush = new SolidBrush(Color.Black);

        public float Alpha { get; set; } = 1f;
        public Color Color { get; set; } = Color.Black;

        public override void Draw(Graphics graphics)
        {
            base.Draw(graphics);
            if (Parent != null)
            {
                X = 0;
                Y = 0;
                Width = Parent.Width;
                Height = Parent.Height;
                _brush.Color = Color.FromArgb((int)(255 * Alpha), Color);
                graphics.FillRectangle(_brush, new RectangleF(X, Y, Width, Height));
            }
        }
    }
}
