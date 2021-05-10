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
    class ImageSequence : AnimatedActor, IImageSequenceActor
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private int _fps;
        private int _frame;
        private List<Image> _frames = new List<Image>();

        public ImageSequence(List<Bitmap> images, int fps = 25, bool loop = true)
        {
            log.Info("Initalizing list of {0} images", images.Count);
            _fps = fps;
            Loop = loop;
            foreach (Bitmap bmp in images)
                _frames.Add(new Image(bmp));
            _frame = 0;
            _frameDuration = 1.0f / fps;
            Pack();
        }

        public Scaling Scaling { get; set; } = Scaling.Stretch;

        public Alignment Alignment { get; set; } = Alignment.Center;

        public override float PrefWidth { get => _frames[0].Width; }

        public override float PrefHeight { get => _frames[0].Height; }

        public float Length { get => _frames.Count * _frameDuration; }

        public int FPS
        {
            get => _fps;
            set
            {
                if (_fps == value) return;
                _fps = value;
                _frameDuration = 1.0f / _fps;
            }
        }

        protected override void Rewind()
        {
            _endOfAnimation = false;
            _frame = 0;
            _frameTime = 0;
            _time = 0;
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
            if (Visible)
            {
                _frames[_frame].Scaling = Scaling;
                _frames[_frame].Alignment = Alignment;
                _frames[_frame].SetBounds(X, Y, Width, Height);
                _frames[_frame].Draw(graphics);
            }
        }
    }

}
