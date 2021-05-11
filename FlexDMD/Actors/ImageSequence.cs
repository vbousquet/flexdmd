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
    class ImageSequence : AnimatedActor
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private int _fps;
        private int _frame;
        private List<Image> _frames = new List<Image>();

        public ImageSequence(AssetManager manager, string paths, string name = "", int fps = 30, bool loop = true)
        {
            _fps = fps;
            Loop = loop;
            Name = name;
            foreach (string path in paths.Split('|'))
                _frames.Add(new Image(manager, path));
            _frame = 0;
            _frameDuration = 1.0f / fps;
            log.Info("ImageSequence '{0}' initalized with {1} frames", name, _frames.Count);
            Pack();
        }

        public override float PrefWidth { get => _frames[0].Width; }
        public override float PrefHeight { get => _frames[0].Height; }
        public override float Length { get => _frames.Count * _frameDuration; }

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

        protected override void OnStageStateChanged()
        {
            foreach (Image frame in _frames)
                frame.OnStage = OnStage;
        }

        protected override void Rewind()
        {
            base.Rewind();
            _frame = 0;
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
