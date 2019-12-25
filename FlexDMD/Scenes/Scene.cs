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
using Glide;
using NLog;
using System;

namespace FlexDMD.Scenes
{
    abstract class Scene : Group
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        protected readonly string _id;
        protected readonly Tweener _tweener = new Tweener();
        protected float _pauseS;
        protected AnimationType _animateIn;
        protected AnimationType _animateOut;
        protected float _time;
        public bool _active = false;

        public string Id { get => _id; }
        public float Time { get => _time; }

        public Scene(AnimationType animateIn, float pauseS, AnimationType animateOut, string id = "")
        {
            _animateIn = animateIn;
            _animateOut = animateOut;
            _pauseS = pauseS;
            _id = id;
        }

        public void SetPause(float pauseS)
        {
            _pauseS = pauseS;
        }

        public virtual void Begin()
        {
            SetSize(Parent.Width, Parent.Height);
            AddAnimation(_animateIn, false);
            AddAnimation(_animateOut, true);
            _active = true;
        }

        // Minions => Lots of ScrollOnLeft / ScrollOffLeft
        // Kiss => Lots of ScrollOnUp / FadeOut
        private void AddAnimation(AnimationType animation, bool atEnd)
        {
            float scrollWLength = 0.5f;
            float scrollHLength = 0.5f; // scrollWLength * Height / Width;
            /* Missing animations : 
             * FadeIn = 0, // fade color from black
             * FadeOut = 1, // fade color to black
             * ZoomIn = 2, // zoom from a centered small dmd to full size
             * ZoomOut = 3, // zoom from a full sized dmd to an oversize one
             * FillFadeIn = 12, // fade from black to white (the scene won't be seen)
             * FillFadeOut = 13, // fade from white to black (the scene won't be seen)
             */
            switch (animation)
            {
                case AnimationType.ScrollOffLeft:
                    DelayAnim(scrollWLength, atEnd, () =>
                    {
                        X = 0f;
                        _tweener.Tween(this, new { X = -Width }, scrollWLength);
                    });
                    break;
                case AnimationType.ScrollOffRight:
                    DelayAnim(scrollWLength, atEnd, () =>
                    {
                        X = 0;
                        _tweener.Tween(this, new { X = Width }, scrollWLength);
                    });
                    break;
                case AnimationType.ScrollOnLeft:
                    DelayAnim(scrollWLength, atEnd, () =>
                    {
                        X = -Width;
                        _tweener.Tween(this, new { X = 0f }, scrollWLength);
                    });
                    break;
                case AnimationType.ScrollOnRight:
                    DelayAnim(scrollWLength, atEnd, () =>
                    {
                        X = Width;
                        _tweener.Tween(this, new { X = 0f }, scrollWLength);
                    });
                    break;
                case AnimationType.ScrollOffUp:
                    DelayAnim(scrollHLength, atEnd, () =>
                    {
                        Y = 0f;
                        _tweener.Tween(this, new { Y = -Height }, scrollHLength);
                    });
                    break;
                case AnimationType.ScrollOffDown:
                    DelayAnim(scrollHLength, atEnd, () =>
                    {
                        Y = 0f;
                        _tweener.Tween(this, new { Y = Height }, scrollHLength);
                    });
                    break;
                case AnimationType.ScrollOnUp:
                    DelayAnim(scrollHLength, atEnd, () =>
                    {
                        Y = -Height;
                        _tweener.Tween(this, new { Y = 0f }, scrollHLength);
                    });
                    break;
                case AnimationType.ScrollOnDown:
                    DelayAnim(scrollHLength, atEnd, () =>
                    {
                        Y = Height;
                        _tweener.Tween(this, new { Y = 0f }, scrollHLength);
                    });
                    break;
                case AnimationType.None:
                    break;
                default:
                    log.Error("Unsupported animation in scene '{0}': {1}", _id, animation);
                    break;
            }
        }

        private void DelayAnim(float lengthS, bool atEnd, Action action)
        {
            if (atEnd)
                _tweener.Timer(_pauseS - lengthS).OnComplete(action);
            else
                action.Invoke();
        }

        public virtual void End()
        {
            _active = false;
            _tweener.Cancel();
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            _time += delta;
            _tweener.Update(delta);
        }

        public bool IsFinished()
        {
            return _time > _pauseS;
        }
    }
}
