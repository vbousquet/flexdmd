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
    class SceneQueue : Actor
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly List<Scene> _queue = new List<Scene>();
        private bool _isRendering = false;

        public void Enqueue(Scene scene)
        {
            _isRendering = true;
            scene.Parent = this;
            _queue.Add(scene);
        }

        public void CancelRendering()
        {
            foreach (Scene s in _queue)
            {
                s.Parent = null;
                if (s._active) s.End();
            }
            _isRendering = false;
            _queue.Clear();
        }

        public void CancelRendering(string sceneId)
        {
			int index = _queue.FindIndex(s => s.Id == sceneId);
			if (index >= 0) {
				if (_queue[index]._active) _queue[index].End();
				_queue.RemoveAt(index);
				_isRendering = _queue.Count() > 0;
			}
        }

        public Scene GetSceneById(string sceneId)
        {
			int index = _queue.FindIndex(s => s.Id == sceneId);
			if (index >= 0) return _queue[index];
			return null;
        }

        public Scene GetActiveScene()
        {
            if (_queue.Count() > 0)
            {
                var s = _queue[0];
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
            base.Update(delta);
            SetSize(Parent.Width, Parent.Height);
            if (_queue.Count() > 0)
            {
                var scene = _queue[0];
                if (!scene._active) scene.Begin();
                scene.Update(delta);
                if (scene.IsFinished())
                {
                    _queue.RemoveAt(0);
                    scene.Parent = null;
                    scene.End();
                    _isRendering = _queue.Count() > 0;
                    if (_isRendering)
                    {
                        scene = _queue[0];
                        scene.Begin();
                        scene.Update(0);
                    }
                }
            }
        }
        public override void Draw(Graphics graphics)
        {
            base.Draw(graphics);
            if (Visible && _queue.Count() > 0)
            {
                _queue[0].Draw(graphics);
            }
        }
    }
}
