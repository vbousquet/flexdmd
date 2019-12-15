using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexDMD.Scenes
{
    abstract class Scene : Group
    {
        protected readonly string _id;
        protected float _pauseS;
        protected AnimationType _animateIn;
        protected AnimationType _animateOut;
        protected float _time;
        public bool _active = false;

        public Scene(AnimationType animateIn, float pauseS, AnimationType animateOut, string id = "")
        {
            _animateIn = animateIn;
            _animateOut = animateOut;
            _pauseS = pauseS;
            _id = id;
        }

        public virtual void Begin()
        {
            SetSize(_parent._width, _parent._height);
            _active = true;
        }

        public virtual void End()
        {
            _active = false;
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            _time += delta;
        }

        public bool IsFinished()
        {
            return _time > _pauseS;
        }
    }
}
