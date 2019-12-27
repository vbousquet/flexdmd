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
using FlexDMD.Actors;
using Glide;
using NLog;
using System;
using System.Drawing;

namespace FlexDMD.Scenes
{
    /// <summary>
    /// The shared scene implementation
    /// 
    /// A scene starts with an 'in' animation, then last for 'pauseS' seconds, and ends with an 'out' animation. 
    /// If the pause is negative, then the scene never ends.
    /// </summary>
    abstract class Scene : Group
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        protected readonly string _id;
        protected readonly Tweener _tweener = new Tweener();
        protected float _pauseS;
        protected float _inAnimLength;
        protected float _outAnimLength;
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
            _active = true;
            SetSize(Parent.Width, Parent.Height);
            _inAnimLength = StartAnimation(_animateIn);
            _outAnimLength = -1;
        }

        private float StartAnimation(AnimationType animation)
        {
            float alphaLength = 0.5f;
            float scrollWLength = 0.5f;
            float scrollHLength = scrollWLength * Height / Width;
            // FIXME Missing animations: ZoomIn = 2, ZoomOut = 3
            switch (animation)
            {
                case AnimationType.FadeIn:
                    {
                        FadeOverlay fade = new FadeOverlay();
                        AddActor(fade);
                        fade.Alpha = 1f;
                        fade.Color = Color.Black;
                        _tweener.Tween(fade, new { Alpha = 0f }, alphaLength);
                        return alphaLength;
                    }
                case AnimationType.FadeOut:
                    {
                        FadeOverlay fade = new FadeOverlay();
                        AddActor(fade);
                        fade.Alpha = 0f;
                        fade.Color = Color.Black;
                        _tweener.Tween(fade, new { Alpha = 1f }, alphaLength);
                        return alphaLength;
                    }
                case AnimationType.ScrollOffLeft:
                    {
                        X = 0f;
                        _tweener.Tween(this, new { X = -Width }, scrollWLength);
                        return scrollWLength;
                    }
                case AnimationType.ScrollOffRight:
                    {
                        X = 0;
                        _tweener.Tween(this, new { X = Width }, scrollWLength);
                        return scrollWLength;
                    }
                case AnimationType.ScrollOnLeft:
                    {
                        X = Width;
                        _tweener.Tween(this, new { X = 0f }, scrollWLength);
                        return scrollWLength;
                    }
                case AnimationType.ScrollOnRight:
                    {
                        X = -Width;
                        _tweener.Tween(this, new { X = 0f }, scrollWLength);
                        return scrollWLength;
                    }
                case AnimationType.ScrollOffUp:
                    {
                        Y = 0f;
                        _tweener.Tween(this, new { Y = -Height }, scrollHLength);
                        return scrollHLength;
                    }
                case AnimationType.ScrollOffDown:
                    {
                        Y = 0f;
                        _tweener.Tween(this, new { Y = Height }, scrollHLength);
                        return scrollHLength;
                    }
                case AnimationType.ScrollOnUp:
                    {
                        Y = Height;
                        _tweener.Tween(this, new { Y = 0f }, scrollHLength);
                        return scrollHLength;
                    }
                case AnimationType.ScrollOnDown:
                    {
                        Y = -Height;
                        _tweener.Tween(this, new { Y = 0f }, scrollHLength);
                        return scrollHLength;
                    }
                case AnimationType.FillFadeIn:
                    {
                        FadeOverlay fade = new FadeOverlay();
                        AddActor(fade);
                        fade.Alpha = 1f;
                        fade.Color = Color.Black;
                        fade = new FadeOverlay();
                        AddActor(fade);
                        fade.Alpha = 0f;
                        fade.Color = Color.White;
                        _tweener.Tween(fade, new { Alpha = 1f }, alphaLength);
                        return alphaLength;
                    }
                case AnimationType.FillFadeOut:
                    {
                        FadeOverlay fade = new FadeOverlay();
                        AddActor(fade);
                        fade.Alpha = 1f;
                        fade.Color = Color.White;
                        fade = new FadeOverlay();
                        AddActor(fade);
                        fade.Alpha = 0f;
                        fade.Color = Color.Black;
                        _tweener.Tween(fade, new { Alpha = 1f }, alphaLength);
                        return alphaLength;
                    }
                case AnimationType.None:
                    return 0f;
                default:
                    log.Error("Unsupported animation in scene '{0}': {1}", _id, animation);
                    return 0f;
            }
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
            if (_pauseS >= 0f && _outAnimLength < 0 && _time >= _inAnimLength + _pauseS)
                _outAnimLength = StartAnimation(_animateOut);
            _tweener.Update(delta);
        }

        public bool IsFinished()
        {
            return _pauseS >= 0f && _outAnimLength >= 0 && _time >= _inAnimLength + _pauseS + _outAnimLength;
        }
    }
}
