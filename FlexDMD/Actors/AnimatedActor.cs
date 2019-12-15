using NLog;
using System;

namespace FlexDMD
{
    abstract class AnimatedActor : Actor
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        protected bool _loop;
        protected bool _endOfAnimation;
        protected float _frameTime;
        protected float _frameDuration;
        protected float _time;

        public override void Update(float delta)
        {
            base.Update(delta);
            _time += delta;
            while (!_endOfAnimation && _time >= _frameTime + _frameDuration)
            {
                try
                {
                    ReadNextFrame();
                } catch (Exception e)
                {
                    log.Error(e, "ReadNextFrame failed, scene discarded");
                    _endOfAnimation = true;
                }
            }
            if (_endOfAnimation && _loop)
            {
                var length = _frameTime + _frameDuration;
                _time = _time % length;
                _endOfAnimation = false;
                Rewind();
            }
        }
        protected abstract void Rewind();

        protected abstract void ReadNextFrame();

    }
}

