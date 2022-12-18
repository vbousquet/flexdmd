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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace FlexDMD.Actors
{
    class Sequence : Group
    {
        private readonly List<Scene> _pendingScenes = new List<Scene>();
        private bool _finished = true;

        public Scene ActiveScene { get; private set; } = null;

        public Sequence(IFlexDMD flex) : base(flex) { }

        public void Enqueue(Scene scene)
        {
            _pendingScenes.Add(scene);
            _finished = false;
        }

        public void RemoveAllScenes()
        {
            ActiveScene?.Remove();
            ActiveScene = null;
            _pendingScenes.Clear();
            _finished = true;
        }

        public void RemoveScene(string name)
        {
            if (ActiveScene.Name.Equals(Name))
            {
                ActiveScene.Remove();
                ActiveScene = null;
            }
            _pendingScenes.RemoveAll(s => s.Name.Equals(name));
            _finished = ActiveScene == null && _pendingScenes.Count == 0;
        }

        public bool IsFinished()
        {
            return _finished;
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            if (ActiveScene != null && ActiveScene.IsFinished())
            {
                ActiveScene.Remove();
                ActiveScene = null;
            }
            if (ActiveScene == null && _pendingScenes.Count() > 0)
            {
                ActiveScene = _pendingScenes[0];
                _pendingScenes.RemoveAt(0);
                AddActor(ActiveScene);
                ActiveScene.Update(0);
            }
            _finished = ActiveScene == null && _pendingScenes.Count == 0;
        }

        public override void Draw(Graphics graphics)
        {
            if (Visible && ActiveScene != null)
            {
                graphics.Clear(Color.Black);
                base.Draw(graphics);
            }
        }

    }
}
