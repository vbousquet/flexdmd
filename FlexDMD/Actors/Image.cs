using FlexDMD.Actors;
using NLog;
using System.Drawing;
using System.Drawing.Imaging;

namespace FlexDMD
{
    class Image : Actor
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        public Bitmap _image = null;

        public Image(string path) 
        {
            // log.Info("Initalizing image: {0}", path);
            _image = new Bitmap(path);
            Rectangle rect = new Rectangle(0, 0, _image.Width, _image.Height);
            BitmapData data = _image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, _image.PixelFormat);
            GraphicUtils.BGRtoRGB(data.Scan0, data.Stride);
            _image.UnlockBits(data);
        }

        public override void Draw(Graphics graphics)
        {
            if (_visible && _image != null) graphics.DrawImage(_image, _x, _y, _image.Width, _image.Height);
        }
    }
}

