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

        public AnimatedImage(List<Bitmap> images, int fps = 25, bool loop = true)
        {
            log.Info("Initalizing image list: {0}", images);
            _fps = fps;
            _loop = loop;
            foreach (Bitmap bmp in images)
                _frames.Add(new Image(bmp));
            _frame = 0;
            _frameDuration = 1.0f / fps;
        }
		
		public AnimatedImage newInstance()
		{
			List<Bitmap> bmps = new List<Bitmap>();
            foreach (Image img in _frames)
                bmps.Add(img._image);
			return new AnimatedImage(bmps, _fps, _loop);
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
