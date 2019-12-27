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
using FlexDMD.Scenes;
using NLog;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace FlexDMD.Actors
{
    class SceneQueue : Group
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private bool _isRendering = false;

        public void Enqueue(Scene scene)
        {
            _isRendering = true;
            AddActor(scene);
        }

        public void CancelRendering()
        {
            foreach (Scene s in Children)
                if (s._active) s.End();
            _isRendering = false;
            RemoveAll();
        }

        public void CancelRendering(string sceneId)
        {
            int index = Children.FindIndex(s => ((Scene)s).Id == sceneId);
            if (index >= 0)
            {
                Scene scene = (Scene)Children[index];
                if (scene._active) scene.End();
                scene.Parent = null;
                Children.RemoveAt(index);
                _isRendering = Children.Count() > 0;
            }
        }

        public Scene GetSceneById(string sceneId)
        {
            int index = Children.FindIndex(s => ((Scene)s).Id == sceneId);
            if (index >= 0) return (Scene)Children[index];
            return null;
        }

        public Scene GetActiveScene()
        {
            if (Children.Count() > 0)
            {
                var s = (Scene)Children[0];
                if (!s._active) s.Begin();
                return s;
            }
            else
            {
                return null;
            }
        }

        public bool IsRendering()
        {
            return _isRendering;
        }

        public override void Update(float delta)
        {
            SetSize(Parent.Width, Parent.Height);
            if (Children.Count() > 0)
            {
                var scene = ((Scene)Children[0]);
                if (!scene._active) scene.Begin();
                scene.Update(delta);
                if (scene.IsFinished())
                {
                    Children.RemoveAt(0);
                    scene.Parent = null;
                    scene.End();
                    _isRendering = Children.Count() > 0;
                    if (_isRendering)
                    {
                        scene = ((Scene)Children[0]);
                        scene.Begin();
                        scene.Update(0);
                    }
                }
            }
        }
        public override void Draw(Graphics graphics)
        {
            if (Visible && Children.Count() > 0)
                Children[0].Draw(graphics);
        }
    }
}
