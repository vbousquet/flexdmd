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
using System;

namespace FlexDMD
{
    public abstract class AnimatedActor : Actor, IVideoActor
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        protected float _frameTime; // Timestamp of the currently displayed frame
        protected float _frameDuration; // Duration of the currently displayed frame
        protected float _time; // Time of the video
        protected bool _endOfAnimation; // Has the end of animation been reached ?

        public Scaling Scaling { get; set; } = Scaling.Stretch;
        public Alignment Alignment { get; set; } = Alignment.Center;
        public bool Paused { get; set; } = false;
        public bool Loop { get; set; } = true;
        public abstract float Length { get; }

        public override void Update(float delta)
        {
            base.Update(delta);
			if (!Visible) return;
            if (!Paused) Advance(delta);
        }

        public virtual void Seek(float posInSeconds)
        {
            Rewind();
            Advance(posInSeconds);
        }

        private void Advance(float delta)
        {
            _time += delta;
            while (!_endOfAnimation && _time >= _frameTime + _frameDuration)
            {
                try
                {
                    ReadNextFrame();
                }
                catch (Exception e)
                {
                    log.Error(e, "ReadNextFrame failed, scene discarded");
                    _endOfAnimation = true;
                }
            }
            if (_endOfAnimation && Loop)
            {
                var length = _frameTime + _frameDuration;
                _time %= length;
                Rewind();
            }
        }

        protected virtual void Rewind()
        {
            _time = 0;
            _frameTime = 0;
            _endOfAnimation = false;
        }

        protected abstract void ReadNextFrame();

    }
}

