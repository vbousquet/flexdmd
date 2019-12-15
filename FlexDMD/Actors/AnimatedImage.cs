using NLog;
using System.Collections.Generic;
using System.Drawing;

namespace FlexDMD
{
    class AnimatedImage : AnimatedActor
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private readonly int _fps;
        private int _frame;
        private List<Image> _frames = new List<Image>();

        public AnimatedImage(string basePath, string imagelist, int fps = 25, bool loop = false)
        {
            log.Info("Initalizing image list: {0}", imagelist);
            _fps = fps;
            _loop = loop;
            foreach (string path in imagelist.Split(','))
                _frames.Add(new Image(System.IO.Path.Combine(basePath, path.Trim())));
            _frame = 0;
            _frameDuration = 1.0f / fps;
        }

        protected override void Rewind()
        {
            _frame = 0;
            _frameTime = 0;
        }

        protected override void ReadNextFrame()
        {
            if (_frame == _frames.Count - 1)
            {
                _endOfAnimation = true;
            }
            else
            {
                _frame++;
                _frameTime = _frame * _frameDuration;
            }
        }

        public override void Draw(Graphics graphics)
        {
            base.Draw(graphics);
            _frames[_frame].Draw(graphics);
        }
    }

}
