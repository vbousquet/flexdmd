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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace FlexDMD
{
    // [Guid("55A9AB7A-DB5D-48E2-B419-80B25E23732D"), ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class Group : Actor, IGroupActor
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private bool _inStage = false;
		
        public List<Actor> Children { get; } = new List<Actor>();

        public override bool InStage
        {
            get => _inStage;
            set
            {
                if (_inStage == value) return;
                _inStage = value;
                foreach (Actor child in Children)
                    child.InStage = value;
            }
        }

        public bool Clip { get; set; } = false;

        public Actor Get(string name)
        {
            if (Name.Equals(name)) return this;
            foreach (Actor child in Children)
            {
                if (child is Group g)
                {
                    var found = g.Get(name);
                    if (found != null) return found;
                }
                else if (child.Name.Equals(name))
                {
                    return child;
                }
            }
            log.Info("Warning actor not found '{0}'", name);
            return null;
        }
        public bool HasChild(string name) => Get(name) != null;
        public IGroupActor GetGroup(string name) => (IGroupActor)Get(name);
        public IFrameActor GetFrame(string name) => (IFrameActor)Get(name);
        public ILabelActor GetLabel(string name) => (ILabelActor)Get(name);
        public IVideoActor GetVideo(string name) => (IVideoActor)Get(name);
        public IImageActor GetImage(string name) => (IImageActor)Get(name);

        public void AddActor(Actor child)
        {
            child.Remove();
            child.Parent = this;
            Children.Add(child);
            child.InStage = _inStage;
        }

        public void AddActorAt(Actor child, int index)
        {
            child.Remove();
            child.Parent = this;
            Children.Insert(index, child);
            child.InStage = _inStage;
        }

        public void RemoveActor(Actor child)
        {
            child.Parent = null;
            Children.Remove(child);
            child.InStage = false;
        }

        public void RemoveAll()
        {
            Children.ForEach(item =>
            {
                item.Parent = null;
                item.InStage = false;
            });
            Children.Clear();
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            for (int i = 0; i < Children.Count; i++)
                Children[i].Update(delta);
        }

        public override void Draw(Graphics graphics)
        {
            if (Visible)
            {
                graphics.TranslateTransform(X, Y);
				if (Clip)
				{
					var clipRegion = new RectangleF(0, 0, Width, Height);
					graphics.SetClip(clipRegion, CombineMode.Replace);
                    base.Draw(graphics);
                    foreach (Actor child in Children) child.Draw(graphics);
					graphics.ResetClip();
				}
				else
				{
                    base.Draw(graphics);
                    foreach (Actor child in Children) child.Draw(graphics);
				}
                graphics.TranslateTransform(-X, -Y);
            }
        }
    }
}

