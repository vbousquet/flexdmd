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
            AddActor(_background);
            AddActor(_topText);
            AddActor(_bottomText);
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
