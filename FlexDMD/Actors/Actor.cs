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
using System.Collections.Generic;
using System.Drawing;

namespace FlexDMD
{
    public class Actor
    {
        private List<Action> _actions = new List<Action>();

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
        public IActionFactory ActionFactory { get; }

        public Actor()
        {
            ActionFactory = new ActionFactory(this);
        }

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

        public void SetAlignedPosition(float x, float y, Alignment alignment)
        {
            switch (alignment)
            {
                case Alignment.BottomLeft:
                case Alignment.Left:
                case Alignment.TopLeft:
                    X = x;
                    break;
                case Alignment.Bottom:
                case Alignment.Center:
                case Alignment.Top:
                    X = x - Width * 0.5f;
                    break;
                case Alignment.BottomRight:
                case Alignment.Right:
                case Alignment.TopRight:
                    X = x - Width;
                    break;
            }
            switch (alignment)
            {
                case Alignment.BottomLeft:
                case Alignment.Bottom:
                case Alignment.BottomRight:
                    Y = y;
                    break;
                case Alignment.Left:
                case Alignment.Center:
                case Alignment.Right:
                    Y = y + Height * 0.5f;
                    break;
                case Alignment.TopLeft:
                case Alignment.Top:
                case Alignment.TopRight:
                    Y = y + Height;
                    break;
            }
        }

        public void SetSize(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public void AddAction(Action action)
        {
            _actions.Add(action);
        }

        public void ClearActions()
        {
            _actions.Clear();
        }

        public virtual void Update(float secondsElapsed)
        {
            for (int i = 0; i < _actions.Count; i++)
                if (_actions[i].Update(secondsElapsed)) _actions.RemoveAt(i);
        }

        public virtual void Draw(Graphics graphics)
        {
        }
    }
}

