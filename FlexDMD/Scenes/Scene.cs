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

namespace FlexDMD.Scenes
{
    abstract class Scene : Group
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        protected readonly string _id;
        protected float _pauseS;
        protected AnimationType _animateIn;
        protected AnimationType _animateOut;
        protected float _time;
        public bool _active = false;

        public string Id { get => _id; }
        public float Time { get => _time; }

        public Scene(AnimationType animateIn, float pauseS, AnimationType animateOut, string id = "")
        {
            _animateIn = animateIn;
            _animateOut = animateOut;
            _pauseS = pauseS;
            _id = id;
            // Minions => Lots of ScrollOnLeft / ScrollOffLeft
            if (animateIn != AnimationType.None || animateOut != AnimationType.None)
                log.Error("Unsupported animation in scene '{0}': {1} => {2}", id, animateIn, animateOut);
        }

        public void SetPause(float pauseS)
        {
            _pauseS = pauseS;
        }

        public virtual void Begin()
        {
            SetSize(Parent.Width, Parent.Height);
            _active = true;
        }

        public virtual void End()
        {
            _active = false;
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            _time += delta;
        }

        public bool IsFinished()
        {
            return _time > _pauseS;
        }
    }
}
