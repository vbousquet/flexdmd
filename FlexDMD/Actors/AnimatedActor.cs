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
    public abstract class AnimatedActor : Actor
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        protected bool _loop = true;
        protected bool _endOfAnimation;
        protected float _frameTime;
        protected float _frameDuration;
        protected float _time;

        public override void Update(float delta)
        {
            base.Update(delta);
			if (!Visible) return;
            _time += delta;
            while (!_endOfAnimation && _time >= _frameTime + _frameDuration)
            {
                try
                {
                    ReadNextFrame();
                } catch (Exception e)
                {
                    log.Error(e, "ReadNextFrame failed, scene discarded");
                    _endOfAnimation = true;
                }
            }
            if (_endOfAnimation && _loop)
            {
                var length = _frameTime + _frameDuration;
                _time = _time % length;
                _endOfAnimation = false;
                Rewind();
            }
        }
        protected abstract void Rewind();

        protected abstract void ReadNextFrame();

    }
}

