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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace FlexDMD
{
    public class Group : Actor
    {
        public List<Actor> Children { get; } = new List<Actor>();

        public override void Update(float delta)
        {
            foreach (Actor child in Children)
            {
                child.Update(delta);
            }
        }

        public override void Draw(Graphics graphics)
        {
            if (Visible)
            {
                graphics.TranslateTransform(X, Y);
                foreach (Actor child in Children) child.Draw(graphics);
                graphics.TranslateTransform(-X, -Y);
            }
        }

        public void AddActor(Actor child)
        {
            if (child.Parent != null) child.Parent.RemoveActor(child);
            child.Parent = this;
            Children.Add(child);
        }

        public void AddActorAt(Actor child, int index)
        {
            if (child.Parent != null) child.Parent.RemoveActor(child);
            child.Parent = this;
            Children.Insert(index, child);
        }

        public void RemoveActor(Actor child)
        {
            child.Parent = null;
            Children.Remove(child);
        }

        public void RemoveAll()
        {
            Children.ForEach(item => item.Parent = null);
            Children.Clear();
        }

    }
}

