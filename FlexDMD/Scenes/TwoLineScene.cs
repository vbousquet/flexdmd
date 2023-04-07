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
namespace FlexDMD.Scenes
{
    class TwoLineScene : BackgroundScene
    {
        private readonly Label _topText;
        private readonly Label _bottomText;

        public TwoLineScene(IFlexDMD flex, Actor background, string topText, Font topFont, string bottomText, Font bottomFont, AnimationType animateIn, float pauseS, AnimationType animateOut, string id = "") : base(flex, background, animateIn, pauseS, animateOut, id)
        {
            _topText = new Label(flex, topFont, topText);
            _bottomText = new Label(flex, bottomFont, bottomText);
            AddActor(_topText);
            AddActor(_bottomText);
        }

        public void SetText(string topText, string bottomText)
        {
            _topText.Text = topText;
            _bottomText.Text = bottomText;
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            _topText.SetPosition((Width - _topText.Width) / 2, 4);
            _bottomText.SetPosition((Width - _bottomText.Width) / 2, 15);
        }

        public override string ToString()
        {
            return string.Format("TwoLineScene Name={0}, _background={1}, _animateIn={2}, _pauseS={3}, _animateOut={4}", Name, Background, _animateIn, Pause, _animateOut);
        }
    }
}
