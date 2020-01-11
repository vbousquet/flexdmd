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
using System.Drawing;
using System.Runtime.InteropServices;

namespace FlexDMD
{
    public class Actor
    {
        public string Name { get; set; } = "";
        public float X { get; set; } = 0;
        public float Y { get; set; } = 0;
        public float Width { get; set; } = 0;
        public float Height { get; set; } = 0;
        public Group Parent { get; set; } = null;
        public virtual float PrefWidth { get; } = 0;
        public virtual float PrefHeight { get; } = 0;
        public virtual bool InStage { get; set; } = false;
        public virtual bool Visible { get; set; } = true;
		public object Info { get; set; } = null;

		public void Remove()
		{
			if (Parent != null) Parent.RemoveActor(this);
		}

		public void Pack()
		{
			Width = PrefWidth;
			Height = PrefHeight;
		}

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

