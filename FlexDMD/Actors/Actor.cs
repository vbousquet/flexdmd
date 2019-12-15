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
using System.Drawing;

namespace FlexDMD
{
    class Actor
    {
        public float _x = 0f;
        public float _y = 0f;
        public float _width = 0f;
        public float _height = 0f;
        public Actor _parent = null;
        public bool _visible = true;

        public void SetPosition(float x, float y)
        {
            _x = x;
            _y = y;
        }

        public void SetSize(float width, float height)
        {
            _width = width;
            _height = height;
        }

        public virtual void Update(float delta)
        {
        }

        public virtual void Draw(Graphics graphics)
        {
        }
    }
}

