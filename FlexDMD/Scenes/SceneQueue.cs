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
            scene._parent = this;
            _queue.Add(scene);
        }

        public void CancelRendering()
        {
            foreach (Scene s in _queue)
            {
                s._parent = null;
                if (s._active) s.End();
            }
            _isRendering = false;
            _queue.Clear();
        }
        public bool IsRendering()
        {
            return _isRendering;
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            SetSize(_parent._width, _parent._height);
            if (_queue.Count() > 0)
            {
                var scene = _queue[0];
                if (!scene._active) scene.Begin();
                scene.Update(delta);
                if (scene.IsFinished())
                {
                    _queue.RemoveAt(0);
                    scene._parent = null;
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
            if (_visible && _queue.Count() > 0)
            {
                _queue[0].Draw(graphics);
            }
        }
    }
}
