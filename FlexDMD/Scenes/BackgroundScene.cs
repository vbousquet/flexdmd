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

using Glide;

namespace FlexDMD.Scenes
{
    class BackgroundScene : Scene
    {
        private Actor _background;

        public Actor Background { get => _background; }

        public BackgroundScene(Actor background, AnimationType animateIn, float pauseS, AnimationType animateOut, string id = "") : base(animateIn, pauseS, animateOut, id)
        {
            _background = background;
            if (_background != null) AddActor(_background);
        }

        public override void Begin()
        {
            base.Begin();
            if (_background != null && _background is Video v) v.Open();
        }

        public override void End()
        {
            base.End();
            if (_background != null && _background is Video v) v.Close();
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            _background?.SetSize(Width, Height);
        }
    }
}
