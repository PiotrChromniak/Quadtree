﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace QuadtreeVisualization
{
    enum Type { EMPTY, NODE, PARTICLE }

    class Quadtree
    {
        public Quadtree[,] Children { get; set; }
        internal Type Type { get; private set; }
        public Region Region { get; private set; }
        public Point Point { get; set; }

        public static Quadtree Root(Region reg)
        {
            return new Quadtree(Type.EMPTY, reg);
        }

        public static Quadtree[,] GenerateChildrenQuadrants(Quadtree tree)
        {
            var reg = tree.Region;
            var half = tree.Region.Width / 2;

            return new Quadtree[2, 2]
                {
                    {
                        new Quadtree(Type.EMPTY, new Region{X = reg.X, Y = reg.Y, Width = half}),
                        new Quadtree(Type.EMPTY, new Region{X = reg.X + half, Y = reg.Y, Width = half})
                    },
                    {
                        new Quadtree(Type.EMPTY, new Region{X = reg.X, Y = reg.Y + half, Width = half}),
                        new Quadtree(Type.EMPTY, new Region{X = reg.X + half, Y = reg.Y + half, Width = half})
                    }
                };
        }

        private static (int x, int y) MapPointToQuadrant(Quadtree tree, Point p)
        {
            return ((int)((p.X % tree.Region.Width) / (tree.Region.Width / 2)),
                    (int)((p.Y % tree.Region.Width) / (tree.Region.Width / 2)));
        }

        public Quadtree(Point point)
        {
            Point = point;
        }

        public Quadtree(Type type, Region reg)
        {
            Type = type;
            Region = reg;
        }

        public Quadtree(Point point, Type type, Region reg)
        {
            Point = point;
            Type = type;
            Region = reg;
        }

        public void Insert(Point point)
        {
            switch (Type)
            {
                case Type.EMPTY:
                    Type = Type.PARTICLE;
                    Point = point;
                    break;
                case Type.NODE:
                    FindPlaceFor(point).Insert(point);
                    break;
                case Type.PARTICLE:
                    Divide(new Quadtree(point));
                    break;
            }
        }

        public void Divide(Quadtree newTree)
        {
            Quadtree support = null;
            Quadtree current = this;

            do
            {
                current.Type = Type.NODE;
                current.Children = GenerateChildrenQuadrants(current);
                var quad1 = MapPointToQuadrant(current, current.Point);
                var quad2 = MapPointToQuadrant(current, newTree.Point);


                if (!quad1.Equals(quad2))
                {
                    support = current.Children[quad2.y,quad2.x];
                    current = current.Children[quad1.y,quad1.x];
                    break;
                }
                else
                {
                    current = current.Children[quad1.y,quad1.x];
                }
            } while (true);

            current.Type = Type.PARTICLE;
            current.Point = Point;

            support.Type = Type.PARTICLE;
            support.Point = newTree.Point;
        }

        private Quadtree FindPlaceFor(Point newPoint)
        {
            var current = this;

            do
            {
                var quad = MapPointToQuadrant(current, newPoint);
                current = current.Children[quad.y,quad.x];
                if (current.Type != Type.NODE)
                    break;
            } while (true);

            return current;
        }
    }
}
