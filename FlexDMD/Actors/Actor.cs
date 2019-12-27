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
        public float X { get; set; } = 0f;
        public float Y { get; set; } = 0f;
        public float Width { get; set; } = 0f;
        public float Height { get; set; } = 0f;
        public Group Parent { get; set; } = null;
        public bool Visible { get; set; } = true;
		public object Info { get; set; } = null;

        public void SetBounds(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public void SetPosition(float x, float y)
        {
            X = x;
            Y = y;
        }

        public void SetSize(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public virtual void Update(float secondsElapsed)
        {
        }

        public virtual void Draw(Graphics graphics)
        {
        }
    }
}

