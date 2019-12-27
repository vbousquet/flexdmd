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
    class JPSScene : BackgroundScene
    {
        private Actor[,] _images;

        public JPSScene(Actor background, AnimationType animateIn, float pauseS, AnimationType animateOut, string id = "") : base(background, animateIn, pauseS, animateOut, id)
        {
            _images = new Image[2, 20];
            for (int col = 0; col < 16; col++)
            {
                _images[0, col] = new Actor();
                AddActor(_images[0, col]);
            }
            for (int col = 0; col < 20; col++)
            {
                _images[1, col] = new Actor();
                AddActor(_images[1, col]);
            }
        }

        public void SetImage(int row, int column, Actor image)
        {
            RemoveActor(_images[row, column]);
            _images[row, column] = image;
            AddActorAt(_images[row, column], row * 16 + column);
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            var topW = Width / 17f;
            var topH = topW * 2f;
            var botW = (Width - topW) / 20f;
            var botH = botW * 2f;
            var padY = (Height - topH - botH) / 3f;
            var x = 0.5f * topW;
            var y = padY;
            for (int col = 0; col < 16; col++)
            {
                _images[0, col].SetBounds(x, y, topW, topH);
                x += topW;
            }
            x = 0.5f * topW;
            y = padY * 2 + topH;
            for (int col = 0; col < 20; col++)
            {
                _images[0, col].SetBounds(x, y, botW, botH);
                x += botW;
            }
        }
    }
}
