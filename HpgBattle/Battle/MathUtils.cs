using System;

namespace HPG.Battle
{
    internal class MathUtils
    {
        public static int sqr(int a)
        {
            return a * a;
        }

        public static bool intersectLineRect(Vec2 p1, Vec2 p2, Vec2 min, Vec2 max)
        {
            Vec2 d = p2 - p1;
            Vec2 e = max - min;
            Vec2 c = p1 * 2 + d - (min + max);
            Vec2 ad = new Vec2(Math.Abs(d.x), Math.Abs(d.y));
            if (Math.Abs(c.x) >= e.x + ad.x)
                return false;
            if (Math.Abs(c.y) >= e.y + ad.y)
                return false;
            if (Math.Abs(d.x * c.y - d.y * c.x) >= e.x * ad.y + e.y * ad.x)
                return false;
            return true;
        }

        public struct HitResult
        {
            public bool hit;
            public int dt;
            public int dr;
            public HitResult(int t, int r) { hit = true; dt = t; dr = r; }
            public static readonly HitResult failed = new HitResult() { hit = false };
            public static readonly HitResult inside = new HitResult(0, 1) { hit = true };
        };

        public static HitResult hitLineRect(Vec2 p1, Vec2 p2, Vec2 min, Vec2 max)
        {
            bool insideX = false;
            bool insideY = false;
            Vec2 test = new Vec2();

            Vec2 dir = p2 - p1;

            if (p1.x < min.x)
                test.x = min.x;
            else if (p1.x > max.x)
                test.x = max.x;
            else
                insideX = true;
            if (p1.y < min.y)
                test.y = min.y;
            else if (p1.y > max.y)
                test.y = max.y;
            else
                insideY = true;

            if (insideX && insideY)
            {
                return HitResult.inside;
            }

            Vec2 maxT;
            Vec2 absD;
            if (!insideX && dir.x != 0)
            {
                if (dir.x > 0)
                {
                    maxT.x = test.x - p1.x;
                    absD.x = dir.x;
                }
                else
                {
                    maxT.x = -(test.x - p1.x);
                    absD.x = -dir.x;
                }
            }
            else
            {
                maxT.x = -1;
                absD.x = 1;
            }
            if (!insideY && dir.y != 0)
                if (dir.y > 0)
                {
                    maxT.y = test.y - p1.y;
                    absD.y = dir.y;
                }
                else
                {
                    maxT.y = -(test.y - p1.y);
                    absD.y = -dir.y;
                }
            else
            {
                maxT.y = -1;
                absD.y = 1;
            }

            int coord;
            if (maxT.x * absD.y < maxT.y * absD.x)
            {
                if (maxT.y < 0 || maxT.y > absD.y)
                    return HitResult.failed;
                coord = p1.x * absD.y + maxT.y * dir.x;
                if (coord < min.x * absD.y || coord > max.x * absD.y)
                    return HitResult.failed;
                return new HitResult(maxT.y, absD.y);
            }
            else
            {
                if (maxT.x < 0 || maxT.x > absD.x)
                    return HitResult.failed;
                coord = p1.y * absD.x + maxT.x * dir.y;
                if (coord < min.y * absD.x || coord > max.y * absD.x)
                    return HitResult.failed;
                return new HitResult(maxT.x, absD.x);
            }
        }


        public static bool ptInPoly(Vec2 pt, Vec2[] poly)
        {
            for (int p = 0; p < poly.Length; ++p)
            {
                int p0 = (p == 0 ? poly.Length : p) - 1;
                Vec2 dp = poly[p] - poly[p0];
                Vec2 d = pt - poly[p0];
                int area = d.x * dp.y - d.y * dp.x;
                if (area < 0)
                    return false;
            }
            return true;
        }

        public static bool ptInIndexedPoly(Vec2 pt, int[] indices, Vec2[] coords)
        {
            for (int i = 0, iend = indices.Length, j = iend - 1; i < iend; j = i++)
            {
                Vec2 vi = coords[indices[i]];
                Vec2 vj = coords[indices[j]];
                if ((vj.x - vi.x) * (pt.y - vi.y) > (vj.y - vi.y) * (pt.x - vi.x))
                    return false;
            }
            return true;
        }

        public static int distSqPointRect(Vec2 pos, Vec2 min, Vec2 max)
        {
            int distSq = 0;
            if (pos.x < min.x)
                distSq += MathUtils.sqr(min.x - pos.x);
            else if (pos.x > max.x)
                distSq += MathUtils.sqr(pos.x - max.x);
            if (pos.y < min.y)
                distSq += MathUtils.sqr(min.y - pos.y);
            else if (pos.y > max.y)
                distSq += MathUtils.sqr(pos.y - max.y);
            return distSq;
        }

        static int[] initSinTable()
        {
            int[] res = new int[181];
            for (int a = 0; a <= 180; ++a)
            {
                res[a] = (int)(Math.Sin(a / 180 * Math.PI) * 1048576 + 0.5);
            }
            return res;
        }
        static int[] sinTable = initSinTable();

        static int sinX1000(int x)
        {
            x = (x % 360 + 360) % 360;
            if (x > 180)
            {
                x = 360 - x;
                return -MathUtils.sinTable[x];
            }
            else
                return MathUtils.sinTable[x];
        }

        public static Vec2 ptInCircle(int radius, int angle)
        {
            return new Vec2(radius * MathUtils.sinX1000(angle) / 1048576, radius * MathUtils.sinX1000(angle + 90) / 1048576);
        }
    }
}
