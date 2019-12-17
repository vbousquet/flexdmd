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

namespace FlexDMD.Scenes
{
    class SingleLineScene : BackgroundScene
    {
        private readonly Label _text;

        public SingleLineScene(Actor background, string text, Font font, AnimationType animateIn, float pauseS, AnimationType animateOut, string id = "") : base(background, animateIn, pauseS, animateOut, id)
        {
            _text = new Label(font, text);
            AddActor(_text);
        }

        public void SetText(string text)
        {
            _text.Text = text;
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            _text.SetPosition((Width - _text.Width) * 0.5f, (Height - _text.Height) * 0.5f);
        }
    }
}
