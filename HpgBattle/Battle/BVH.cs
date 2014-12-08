using System;
using HPG.Battle.Entities;

namespace HPG.Battle
{
    //internal delegate int QueryDelegate(int id);
    //internal delegate bool QueryLineDelegate(int id);

    internal class Bvh
    {
        int leaf_size;
        int[] ids = new int[0];
        BVHNode[] tree = new BVHNode[0];

        public Bvh(int leaf_size = 16)
        {
            this.leaf_size = leaf_size;
        }

        void build(Obstacle[] objects, int begin, int end, int node)
        {
            BVHNode objNode = new BVHNode();
            objNode.begin = begin;
            objNode.end = end;

            Obstacle obj = objects[ids[begin]];
            objNode.min = obj.min;
            objNode.max = obj.max;
            for (int i = begin + 1; i < end; ++i)
            {
                obj = objects[ids[i]];

                if (obj.max.x > objNode.max.x)
                    objNode.max.x = obj.max.x;
                if (obj.min.x < objNode.min.x)
                    objNode.min.x = obj.min.x;

                if (obj.max.y > objNode.max.y)
                    objNode.max.y = obj.max.y;
                if (obj.min.y < objNode.min.y)
                    objNode.min.y = obj.min.y;
            }

            if (end - begin > this.leaf_size)
            { // no leaf node
                bool vertical = (objNode.max.x - objNode.min.x > objNode.max.y - objNode.min.y); // vertical split
                int splitValue = (vertical ? (objNode.max.x + objNode.min.x) : (objNode.max.y + objNode.min.y)) / 2;

                int l = begin;
                int r = end - 1;
                while (true)
                {
                    if (vertical)
                    {
                        while (l <= r && objects[ids[l]].pos.x < splitValue)
                            ++l;
                        while (r >= l && objects[ids[r]].pos.x >= splitValue)
                            --r;
                    }
                    else
                    {
                        while (l <= r && objects[ids[l]].pos.y < splitValue)
                            ++l;
                        while (r >= l && objects[ids[r]].pos.y >= splitValue)
                            --r;
                    }
                    if (l > r)
                        break;
                    else
                    {
                        //swap
                        int tmp = ids[l];
                        ids[l] = ids[r];
                        ids[r] = tmp;

                        ++l;
                        --r;
                    }
                }

                int leftsize = l - begin;

                if (leftsize == 0)
                {
                    ++leftsize;
                    ++l;
                    ++r;
                }
                else if (leftsize == (end - begin))
                {
                    --leftsize;
                    --l;
                }

                objNode.left = node + 1;
                objNode.right = node + 1 + (2 * leftsize - 1);

                build(objects, begin, l, objNode.left);
                build(objects, l, end, objNode.right);
            }
            this.tree[node] = objNode;
        }

        int query(Vec2 pos, int rangeSq, int node, Func<int, int> callback)
        {
            BVHNode objNode = tree[node];

            if (objNode.end - objNode.begin <= leaf_size)
            {
                for (int i = objNode.begin, iend = objNode.end; i < iend; ++i)
                {
                    rangeSq = callback(ids[i]);
                    if (rangeSq < 0)
                        return rangeSq;
                }
            }
            else
            {
                BVHNode leftNode = tree[objNode.left];
                BVHNode rightNode = tree[objNode.right];
                int distSqLeft = 0;
                int distSqRight = 0;

                if (pos.x < leftNode.min.x)
                    distSqLeft += MathUtils.sqr(leftNode.min.x - pos.x);
                else if (pos.x > leftNode.max.x)
                    distSqLeft += MathUtils.sqr(pos.x - leftNode.max.x);

                if (pos.y < leftNode.min.y)
                    distSqLeft += MathUtils.sqr(leftNode.min.y - pos.y);
                else if (pos.y > leftNode.max.y)
                    distSqLeft += MathUtils.sqr(pos.y - leftNode.max.y);

                if (pos.x < rightNode.min.x)
                    distSqRight += MathUtils.sqr(rightNode.min.x - pos.x);
                else if (pos.x > rightNode.max.x)
                    distSqRight += MathUtils.sqr(pos.x - rightNode.max.x);

                if (pos.y < rightNode.min.y)
                    distSqRight += MathUtils.sqr(rightNode.min.y - pos.y);
                else if (pos.y > rightNode.max.y)
                    distSqRight += MathUtils.sqr(pos.y - rightNode.max.y);

                if (distSqLeft < distSqRight)
                {
                    if (distSqLeft <= rangeSq)
                    {
                        rangeSq = query(pos, rangeSq, objNode.left, callback);
                        if (distSqRight <= rangeSq && rangeSq >= 0)
                            rangeSq = query(pos, rangeSq, objNode.right, callback);
                    }
                }
                else
                {
                    if (distSqRight <= rangeSq)
                    {
                        rangeSq = query(pos, rangeSq, objNode.right, callback);
                        if (distSqLeft <= rangeSq && rangeSq >= 0)
                            rangeSq = query(pos, rangeSq, objNode.left, callback);
                    }
                }
            }

            return rangeSq;
        }

        bool query(Vec2 q1, Vec2 q2, int node, Func<int, bool> callback)
        {
            BVHNode objNode = tree[node];

            if (!MathUtils.intersectLineRect(q1, q2, objNode.min, objNode.max))
                return false;

            if (objNode.end - objNode.begin <= leaf_size)
            {
                for (int i = objNode.begin, iend = objNode.end; i < iend; ++i)
                {
                    if (callback(ids[i]))
                        return true;
                }
                return false;
            }
            else
            {
                BVHNode leftNode = tree[objNode.left];
                BVHNode rightNode = tree[objNode.right];
                int distSqLeft = 0;
                int distSqRight = 0;

                if (q1.x < leftNode.min.x)
                    distSqLeft += MathUtils.sqr(leftNode.min.x - q1.x);
                else if (q1.x > leftNode.max.x)
                    distSqLeft += MathUtils.sqr(q1.x - leftNode.max.x);

                if (q1.y < leftNode.min.y)
                    distSqLeft += MathUtils.sqr(leftNode.min.y - q1.y);
                else if (q1.y > leftNode.max.y)
                    distSqLeft += MathUtils.sqr(q1.y - leftNode.max.y);

                if (q1.x < rightNode.min.x)
                    distSqRight += MathUtils.sqr(rightNode.min.x - q1.x);
                else if (q1.x > rightNode.max.x)
                    distSqRight += MathUtils.sqr(q1.x - rightNode.max.x);

                if (q1.y < rightNode.min.y)
                    distSqRight += MathUtils.sqr(rightNode.min.y - q1.y);
                else if (q1.y > rightNode.max.y)
                    distSqRight += MathUtils.sqr(q1.y - rightNode.max.y);

                if (distSqLeft < distSqRight)
                {
                    if (query(q1, q2, objNode.left, callback))
                        return true;
                    else
                        return query(q1, q2, objNode.right, callback);
                }
                else
                {
                    if (query(q1, q2, objNode.right, callback))
                        return true;
                    else
                        return query(q1, q2, objNode.left, callback);
                }
            }
        }

        public void build(Obstacle[] objects)
        {
            if (objects.Length != ids.Length)
            {
                Array.Resize<int>(ref ids, objects.Length);
                if (ids.Length <= 0)
                    return;
                for (int i = 0, iend = objects.Length; i != iend; ++i)
                    ids[i] = i;
                Array.Resize<BVHNode>(ref tree, 2 * ids.Length - 1);
            }

            if (ids.Length <= 0)
                return;

            build(objects, 0, ids.Length, 0);
        }

        public int query(Vec2 pos, int rangeSq, Func<int, int> callback)
        {
            if (ids.Length <= 0)
                return rangeSq;

            return query(pos, rangeSq, 0, callback);
        }

        public bool query(Vec2 pos0, Vec2 pos1, Func<int, bool> callback)
        {
            if (ids.Length <= 0)
                return true;

            return query(pos0, pos1, 0, callback);
        }
    }
}
