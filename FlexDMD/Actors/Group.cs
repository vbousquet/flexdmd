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
using System.Collections.Generic;
using System.Drawing;

namespace FlexDMD
{
    class Group : Actor
    {
        private List<Actor> _children = new List<Actor>();

        public override void Update(float delta)
        {
            foreach (Actor child in _children)
            {
                child.Update(delta);
            }
        }

        public override void Draw(Graphics graphics)
        {
            if (_visible)
            {
                foreach (Actor child in _children)
                {
                    child.Draw(graphics);
                }
            }
        }

        public void AddActor(Actor child)
        {
            child._parent = this;
            _children.Add(child);
        }

        public void AddActorAt(Actor child, int index)
        {
            child._parent = this;
            _children.Insert(index, child);
        }

        public void RemoveActor(Actor child)
        {
            child._parent = null;
            _children.Remove(child);
        }

        public void RemoveAll()
        {
            _children.Clear();
        }

    }
}

