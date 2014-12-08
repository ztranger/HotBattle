using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HPG.Battle
{
    internal struct BVHNode
    {
        public int begin;
        public int end;
        public int left;
        public int right;
        public Vec2 min;
        public Vec2 max;
    }
}
