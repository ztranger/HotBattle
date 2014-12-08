using UnityEngine;

namespace Tanat.SharedLogic.Defs
{
    public class VectorDef
    {
        public float X { get; internal set; }
        public float Y { get; internal set; }

        public static implicit operator Vector3(VectorDef c)
        {
            return new Vector3(c.X, 0, c.Y);
        }

        public static implicit operator Vector2(VectorDef c)
        {
            return new Vector3(c.X, c.Y);
        }

    }
}

