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
using FlexDMD.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexDMD.Scenes
{
    class Scene00 : Scene
    {
        private Actor _background;
        private Label _topText;
        private Label _bottomText;

        public Scene00(Actor background, string topText, Font topFont, int topBrightness, string bottomText, Font bottomFont, int bottomBrightness, AnimationType animateIn, float pauseS, AnimationType animateOut, string id = "") : base(animateIn, pauseS, animateOut, id)
        {
            _background = background;
            _topText = new Label(topFont, topText);
            _bottomText = new Label(bottomFont, bottomText);
            if (_background != null) AddActor(_background);
            AddActor(_topText);
            AddActor(_bottomText);
        }

        public void SetText(string topText, string bottomText)
        {
            _topText.Text = topText;
            _bottomText.Text = bottomText;
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
            _topText.SetPosition((_width - _topText._width) * 0.5f, 4);
            _bottomText.SetPosition((_width - _bottomText._width) * 0.5f, 15);
        }

        public override string ToString()
        {
            return string.Format("Scene00 _id={0}, _background={1}, _animateIn={2}, _pauseS={3}, _animateOut={4}", _id, _background, _animateIn, _pauseS, _animateOut);
        }
    }
}
