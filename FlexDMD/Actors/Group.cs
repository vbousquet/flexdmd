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

