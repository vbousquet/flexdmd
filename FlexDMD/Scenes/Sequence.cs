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
using System.Linq;

namespace FlexDMD.Actors
{
    class Sequence : Group
    {
        private Scene _activeScene = null;
        private bool _finished = true;
        private readonly List<Scene> _scenes = new List<Scene>();

        public bool Loop { get; set; } = false;
        public Scene ActiveScene
        {
            get => (_activeScene != null && _activeScene.Parent == this) ? _activeScene : null;
            private set { _activeScene = (value != null && value.Parent == this) ? value : null; }
        }

        public void Enqueue(Scene scene)
        {
            _scenes.Add(scene);
            _finished = false;
        }

        public void RemoveAllScenes()
        {
            ActiveScene?.Remove();
            _scenes.Clear();
            _finished = true;
        }

        public void RemoveScene(string name)
        {
            var scene = _scenes.First(s => s.Name.Equals(name));
            if (scene != null)
            {
                if (scene.Parent == this)
                {
                    NextScene();
                    if (scene.Parent == this)
                    {
                        RemoveAllScenes();
                        return;
                    }
                }
                _scenes.Remove(scene);
            }
        }

        public bool IsFinished()
        {
            return _finished;
        }

        private void NextScene()
        {
            if (ActiveScene != null)
            {
                var pos = _scenes.IndexOf(ActiveScene) + 1;
                ActiveScene.Remove();
                if (pos >= _scenes.Count() && Loop) pos = 0;
                if (pos < _scenes.Count())
                {
                    AddActor(_scenes[pos]);
                    ActiveScene = _scenes[pos];
                }
                else
                {
                    _finished = true;
                }
            }
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            if (ActiveScene == null)
            {
                if (_scenes.Count() > 0)
                {
                    AddActor(_scenes[0]);
                    ActiveScene = _scenes[0];
                    ActiveScene.Update(delta);
                }
                else
                {
                    _finished = true;
                }
            }
            else if (ActiveScene.IsFinished())
            {
                NextScene();
                ActiveScene?.Update(delta);
            }
        }
    }
}
