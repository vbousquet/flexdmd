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

        public readonly IFlexDMD FlexDMD;

        public List<Actor> Children { get; } = new List<Actor>();

        public bool Clip { get; set; } = false;

        public int ChildCount { get => Children.Count; }

        public Group Root { 
			get
			{
				var root = this;
				while (root.Parent != null)
					root = root.Parent;
				return root;
			}
		}

		public Group(IFlexDMD flex)
        {
            FlexDMD = flex;
        }

        protected override void OnStageStateChanged()
        {
            foreach (Actor child in Children)
                child.OnStage = OnStage;
        }

        public Actor Get(string name)
        {
            if (Name.Equals(name)) return this;
			if (FlexDMD.RuntimeVersion <= 1008) {
				foreach (Actor child in Children)
				{
                    if (child.Name.Equals(name))
                    {
                        return child;
                    }
                    if (child is Group g)
					{
						var found = g.Get(Name + "/" + name);
						if (found != null) return found;
					}
				}
			} else {
				var pos = name.IndexOf("/");
				if (pos < 0)
				{
					// direct child node search 'xx'
					foreach (Actor child in Children)
						if (child.Name.Equals(name))
							return child;
				}
				else if (pos == 0)
				{
					// absolute path from root '/xx/yy/zz', note that stage node is named 'Stage'
					return Root.Get(name.Substring(1));
				}
				else
				{
					// relative path from current group 'xx/yy/zz'
					var groupName = name.Substring(0, pos);
					foreach (Actor child in Children)
						if (child is Group g && child.Name.Equals(groupName))
							return g.Get(name.Substring(pos + 1));
				}
			}
            log.Info("Warning actor '{0}' not found in children of '{1}'", name, Name);
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
            child.OnStage = OnStage;
        }

        public void AddActorAt(Actor child, int index)
        {
            child.Remove();
            child.Parent = this;
            Children.Insert(index, child);
            child.OnStage = OnStage;
        }

        public void RemoveActor(Actor child)
        {
            child.Parent = null;
            Children.Remove(child);
            child.OnStage = false;
        }

        public void RemoveAll()
        {
            Children.ForEach(item =>
            {
                item.Parent = null;
                item.OnStage = false;
            });
            Children.Clear();
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            if (!OnStage) return; // This may happen if an action has just removed this actor
            int i = 0;
            while (i < Children.Count)
            {
                var child = Children[i];
                child.Update(delta);
                if (i < Children.Count && child == Children[i]) i++; // This child may have been removed
            }
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

