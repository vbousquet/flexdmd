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
    class Sequence : Group
    {
		private Scene _activeScene = null;
		
		public bool Loop { get; set; } = false;

        public void Enqueue(Scene scene)
        {
			if (_activeScene != null) 
			{
				scene.Visible = false;
			}
			else
			{
				_activeScene = scene;
			}
			AddActor(scene);
        }
		
		public bool IsFinished()
		{
			return _activeScene == null;
		}

        public override void Update(float delta)
        {
			base.Update(delta);
			if (_activeScene == null)
			{
				if (Children.Count() > 0)
				{
					_activeScene = (Scene) Children[0];
					_activeScene.Visible = true;
				}
			}
			else if (_activeScene.IsFinished())
			{
				_activeScene.Visible = false;
				var pos = Children.IndexOf(_activeScene) + 1;
				if (pos >= Children.Count() && Loop) pos = 0;
				if (pos < Children.Count())
				{
					_activeScene = (Scene) Children[pos];
					_activeScene.Visible = true;
				}
			}
        }
    }
}
