using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HPG.Battle.Entities;

namespace HPG.Battle
{
    internal class KDTree
    {
        int leaf_size;
        int[] ids = new int[0];
        BVHNode[] tree = new BVHNode[0];

        public KDTree(int leaf_size = 16)
        {
            this.leaf_size = leaf_size;
        }

        private void build(List<Unit> objects, int begin, int end, int node)
        {
            BVHNode objNode = new BVHNode();
            objNode.begin = begin;
            objNode.end = end;

            Unit obj = objects[ids[begin]];
            objNode.min = objNode.max = obj.pos;
            for (int i = begin + 1; i < end; ++i)
            {
                obj = objects[ids[i]];

                if (obj.pos.x > objNode.max.x)
                    objNode.max.x = obj.pos.x;
                else if (obj.pos.x < objNode.min.x)
                    objNode.min.x = obj.pos.x;

                if (obj.pos.y > objNode.max.y)
                    objNode.max.y = obj.pos.y;
                else if (obj.pos.y < objNode.min.y)
                    objNode.min.y = obj.pos.y;
            }

            if (end - begin > leaf_size)
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
            tree[node] = objNode;
        }

        private int query(Vec2 pos, int rangeSq, int node, Func<int, int> callback)
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

        public void build(List<Unit> objects)
        {
            if (objects.Count != ids.Length)
            {
                Array.Resize<int>(ref ids, objects.Count);
                if (ids.Length <= 0)
                    return;
                for (int i = 0, iend = objects.Count; i != iend; ++i)
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

    }
}
