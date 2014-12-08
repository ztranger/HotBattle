using System;

namespace HPG.Battle
{
    public struct Vec2
    {
        public int x;
        public int y;

        public override string ToString()
        {
            return "{" + x + ", " + y + "}";
        }

        public Vec2(int px, int py)
        {
            x = px;
            y = py;
        }

        public int LengthSq()
        {
            return x * x + y * y;
        }

        public int Length()
        {
            return (int)Math.Sqrt(x * x + y * y);
        }

        public static Vec2 operator +(Vec2 a, Vec2 b)
        {
            return new Vec2(a.x + b.x, a.y + b.y);
        }

        public static Vec2 operator -(Vec2 a, Vec2 b)
        {
            return new Vec2(a.x - b.x, a.y - b.y);
        }

        public static Vec2 operator *(Vec2 a, int b)
        {
            return new Vec2(a.x * b, a.y * b);
        }

        public static Vec2 operator /(Vec2 a, int b)
        {
            return new Vec2(a.x / b, a.y / b);
        }

        public static Vec2 operator +(Vec2 a, int b)
        {
            return new Vec2(a.x + b, a.y + b);
        }

        public static Vec2 operator -(Vec2 a, int b)
        {
            return new Vec2(a.x - b, a.y - b);
        }

        public static bool operator ==(Vec2 a, Vec2 b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(Vec2 a, Vec2 b)
        {
            return a.x != b.x || a.y != b.y;
        }

        public static bool operator >=(Vec2 a, Vec2 b)
        {
            return a.x >= b.x && a.y >= b.y;
        }

        public static bool operator <=(Vec2 a, Vec2 b)
        {
            return a.x <= b.x && a.y <= b.y;
        }

        public static bool operator >(Vec2 a, Vec2 b)
        {
            return a.x > b.x && a.y > b.y;
        }

        public static bool operator <(Vec2 a, Vec2 b)
        {
            return a.x < b.x && a.y < b.y;
        }

        public void minimize(Vec2 v)
        {
            if (v.x < x)
                x = v.x;
            if (v.y < y)
                y = v.y;
        }

        public void maximize(Vec2 v)
        {
            if (v.x > x)
                x = v.x;
            if (v.y > y)
                y = v.y;
        }
    }

    public struct Vec2f
    {
        public float x;
        public float y;
    };
}
