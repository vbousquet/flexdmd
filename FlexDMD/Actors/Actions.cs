using Glide;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FlexDMD
{
    public abstract class Action
    {
        /// <summary>
        /// Update the action.
        /// </summary>
        /// <param name="secondsElapsed">The number of seconds elapsed since the last call</param>
        /// <returns>True if the action is finished. Calling Update again will restart the animation from it's beginning.</returns>
        public abstract bool Update(float secondsElapsed);
    }

    /// <summary>
    /// Repeat an action for a number of time or indefinitely (when the given count is zero or negative)
    /// </summary>
    public class RepeatAction : Action
    {
        private readonly Action _action;
        private readonly int _count;
        private int _n = 0;

        public RepeatAction(Action action, int count)
        {
            _action = action;
            _count = count;
        }

        public override bool Update(float secondsElapsed)
        {
            if (_action.Update(secondsElapsed))
            {
                _n++;
                if (_n == _count)
                {
                    // Prepare for restart
                    _n = 0;
                    return true;
                }
            }
            return false;
        }
    }

    public class SequenceAction : Action, ICompositeAction
    {
        private readonly List<Action> _actions = new List<Action>();
        private int _pos = 0;

        public ICompositeAction Add(Action action)
        {
            _actions.Add(action);
            return this;
        }

        public override bool Update(float secondsElapsed)
        {
            if (_pos >= _actions.Count)
            {
                // Prepare for restart
                _pos = 0;
                return true;
            }
            if (_actions[_pos].Update(secondsElapsed))
            {
                _pos++;
                if (_pos >= _actions.Count)
                {
                    // Prepare for restart
                    _pos = 0;
                    return true;
                }
            }
            return false;
        }
    }

    public class ParallelAction : Action, ICompositeAction
    {
        private readonly List<Action> _actions = new List<Action>();
        private readonly List<bool> _runMask = new List<bool>();

        public ICompositeAction Add(Action action)
        {
            _actions.Add(action);
            _runMask.Add(true);
            return this;
        }

        public override bool Update(float secondsElapsed)
        {
            var alive = false;
            for (int i = 0; i < _actions.Count; i++)
            {
                if (_runMask[i])
                {
                    if (_actions[i].Update(secondsElapsed))
                    {
                        _runMask[i] = false;
                    }
                    else
                    {
                        alive = true;
                    }
                }
            }
            if (!alive)
            {
                // Prepare for restart
                for (int i = 0; i < _actions.Count; i++)
                    _runMask[i] = true;
                return true;
            }
            return false;
        }
    }

    public class ShowAction : Action
    {
        public Actor Target { get; }
        public bool Visible { get; set; }

        public ShowAction(Actor target, bool visible)
        {
            Target = target;
            Visible = visible;
        }

        public override bool Update(float secondsElapsed)
        {
            Target.Visible = Visible;
            return true;
        }
    }

    public class AddToAction : Action
    {
        public Actor Target { get; }
        public IGroupActor Parent { get; set; }
        public bool Add { get; set; }

        public AddToAction(Actor target, IGroupActor parent, bool add)
        {
            Target = target;
            Parent = parent;
            Add = add;
        }

        public override bool Update(float secondsElapsed)
        {
            if (Add) Parent.AddActor(Target); else Parent.RemoveActor(Target);
            return true;
        }
    }

    public class AddChildAction : Action
    {
        public Group Target { get; }
        public Actor Child { get; set; }
        public bool Add { get; set; }

        public AddChildAction(Group target, Actor child, bool add)
        {
            Target = target;
            Child = child;
            Add = add;
        }

        public override bool Update(float secondsElapsed)
        {
            if (Add) Target.AddActor(Child); else Target.RemoveActor(Child);
            return true;
        }
    }

    public class SeekAction : Action
    {
        public Video Target { get; }
        public float Position { get; set; } = 0f;

        public SeekAction(Video target, float position)
        {
            Target = target;
            Position = position;
        }

        public override bool Update(float secondsElapsed)
        {
            Target.Seek(Position);
            return true;
        }
    }

    public class WaitAction : Action
    {
        public float SecondsToWait { get; set; } = 0f;
        private float _time;
        public WaitAction(float secondsToWait)
        {
            SecondsToWait = secondsToWait;
        }

        public override bool Update(float secondsElapsed)
        {
            _time += secondsElapsed;
            if (_time >= SecondsToWait)
            {
                // Prepare for restart
                _time = 0f;
                return true;
            }
            return false;
        }
    }

    public abstract class TweenAction : Action, ITweenAction
    {
        public Actor Target { get; }
        public Tweener Tweener { get; private set; }
        public Interpolation Ease { get; set; } = Interpolation.Linear;
        public float Duration { get; }
        public bool Done { get; private set; } = false;

        public TweenAction(Actor target, float duration)
        {
            Target = target;
            Duration = duration;
        }

        public override bool Update(float secondsElapsed)
        {
            if (Tweener == null)
            {
                Tweener = new Tweener();
                var tween = Begin();
                tween.OnComplete(() => Done = true);
                switch (Ease)
                {
                    case Interpolation.Linear:
                        tween.Ease(null);
                        break;
                    case Interpolation.ElasticIn:
                        tween.Ease(Glide.Ease.ElasticIn);
                        break;
                    case Interpolation.ElasticOut:
                        tween.Ease(Glide.Ease.ElasticOut);
                        break;
                    case Interpolation.ElasticInOut:
                        tween.Ease(Glide.Ease.ElasticInOut);
                        break;
                    case Interpolation.QuadIn:
                        tween.Ease(Glide.Ease.QuadIn);
                        break;
                    case Interpolation.QuadOut:
                        tween.Ease(Glide.Ease.QuadOut);
                        break;
                    case Interpolation.QuadInOut:
                        tween.Ease(Glide.Ease.QuadInOut);
                        break;
                    case Interpolation.CubeIn:
                        tween.Ease(Glide.Ease.CubeIn);
                        break;
                    case Interpolation.CubeOut:
                        tween.Ease(Glide.Ease.CubeOut);
                        break;
                    case Interpolation.CubeInOut:
                        tween.Ease(Glide.Ease.CubeInOut);
                        break;
                    case Interpolation.QuartIn:
                        tween.Ease(Glide.Ease.QuartIn);
                        break;
                    case Interpolation.QuartOut:
                        tween.Ease(Glide.Ease.QuartOut);
                        break;
                    case Interpolation.QuartInOut:
                        tween.Ease(Glide.Ease.QuartInOut);
                        break;
                    case Interpolation.QuintIn:
                        tween.Ease(Glide.Ease.QuintIn);
                        break;
                    case Interpolation.QuintOut:
                        tween.Ease(Glide.Ease.QuintOut);
                        break;
                    case Interpolation.QuintInOut:
                        tween.Ease(Glide.Ease.QuintInOut);
                        break;
                    case Interpolation.SineIn:
                        tween.Ease(Glide.Ease.SineIn);
                        break;
                    case Interpolation.SineOut:
                        tween.Ease(Glide.Ease.SineOut);
                        break;
                    case Interpolation.SineInOut:
                        tween.Ease(Glide.Ease.SineInOut);
                        break;
                    case Interpolation.BounceIn:
                        tween.Ease(Glide.Ease.BounceIn);
                        break;
                    case Interpolation.BounceOut:
                        tween.Ease(Glide.Ease.BounceOut);
                        break;
                    case Interpolation.BounceInOut:
                        tween.Ease(Glide.Ease.BounceInOut);
                        break;
                    case Interpolation.CircIn:
                        tween.Ease(Glide.Ease.CircIn);
                        break;
                    case Interpolation.CircOut:
                        tween.Ease(Glide.Ease.CircOut);
                        break;
                    case Interpolation.CircInOut:
                        tween.Ease(Glide.Ease.CircInOut);
                        break;
                    case Interpolation.ExpoIn:
                        tween.Ease(Glide.Ease.ExpoIn);
                        break;
                    case Interpolation.ExpoOut:
                        tween.Ease(Glide.Ease.ExpoOut);
                        break;
                    case Interpolation.ExpoInOut:
                        tween.Ease(Glide.Ease.ExpoInOut);
                        break;
                    case Interpolation.BackIn:
                        tween.Ease(Glide.Ease.BackIn);
                        break;
                    case Interpolation.BackOut:
                        tween.Ease(Glide.Ease.BackOut);
                        break;
                    case Interpolation.BackInOut:
                        tween.Ease(Glide.Ease.BackInOut);
                        break;
                }
            }
            Tweener.Update(secondsElapsed);
            if (Done)
            {
                End();
                // Prepare for restart
                Done = false;
                Tweener = null;
                return true;
            }
            return false;
        }

        protected abstract Tween Begin();
        protected void End() { }
    }

    public class MoveToAction : TweenAction
    {
        private readonly float _x, _y;
        public MoveToAction(Actor target, float x, float y, float duration) : base(target, duration) { _x = x; _y = y; }
        protected override Tween Begin() => Tweener.Tween(Target, new { X = _x, Y = _y }, Duration, 0f);
    }

    public class ActionFactory : IActionFactory
    {
        private readonly Actor _target;

        public ActionFactory(Actor target) { _target = target; }

        [return: MarshalAs(UnmanagedType.Struct)] public Action Wait(float secondsToWait) => new WaitAction(secondsToWait);
        public ICompositeAction Parallel() => new ParallelAction();
        public ICompositeAction Sequence() => new SequenceAction();
        [return: MarshalAs(UnmanagedType.Struct)] public Action Repeat([MarshalAs(UnmanagedType.Struct)] Action action, int count) => new RepeatAction(action, count);
        [return: MarshalAs(UnmanagedType.Struct)] public Action Show(bool visible) => new ShowAction(_target, visible);
        [return: MarshalAs(UnmanagedType.Struct)] public Action AddTo(IGroupActor parent) => new AddToAction(_target, parent, true);
        [return: MarshalAs(UnmanagedType.Struct)] public Action RemoveFrom(IGroupActor parent) => new AddToAction(_target, parent, false);
        [return: MarshalAs(UnmanagedType.Struct)] public Action AddChild([MarshalAs(UnmanagedType.Struct)] Actor child) => new AddChildAction((Group)_target, child, true);
        [return: MarshalAs(UnmanagedType.Struct)] public Action RemoveChild([MarshalAs(UnmanagedType.Struct)] Actor child) => new AddChildAction((Group)_target, child, false);
        [return: MarshalAs(UnmanagedType.Struct)] public Action Seek(float pos) => new SeekAction((Video)_target, pos);
        public ITweenAction MoveTo(float x, float y, float duration) => new MoveToAction(_target, x, y, duration);
    }
}
