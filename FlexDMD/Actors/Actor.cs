using NLog;
using System.Drawing;

namespace FlexDMD
{
    class Actor
    {
        public float _x = 0f;
        public float _y = 0f;
        public float _width = 0f;
        public float _height = 0f;
        public Actor _parent = null;
        public bool _visible = true;

        public void SetPosition(float x, float y)
        {
            _x = x;
            _y = y;
        }

        public void SetSize(float width, float height)
        {
            _width = width;
            _height = height;
        }

        public virtual void Update(float delta)
        {
        }

        public virtual void Draw(Graphics graphics)
        {
        }
    }
}

