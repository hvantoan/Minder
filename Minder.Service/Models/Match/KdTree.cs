using System;
using System.Collections.Generic;
using System.Linq;

namespace Minder.Service.Models.Match {

    public class KdTree {
        private Node? root;

        public void BuildTree(List<Point> points) {
            root = BuildTreeRecursive(points, 0);
        }

        private Node? BuildTreeRecursive(List<Point> points, int depth) {
            if (points.Count == 0) {
                return null;
            }

            int axis = depth % 2;
            points = points.OrderBy(p => axis == 0 ? p.X : p.Y).ToList();

            int medianIndex = points.Count / 2;
            Point median = points[medianIndex];

            Node node = new(median);
            node.Left = BuildTreeRecursive(points.GetRange(0, medianIndex), depth + 1);
            node.Right = BuildTreeRecursive(points.GetRange(medianIndex + 1, points.Count - medianIndex - 1), depth + 1);

            return node;
        }

        public List<Point> FindNearestPoints(Point target, int k) {
            List<Point> nearestPoints = new();
            FindNearestPointsRecursive(root, target, k, ref nearestPoints);
            return nearestPoints;
        }

        private void FindNearestPointsRecursive(Node? node, Point target, int k, ref List<Point> nearestPoints) {
            if (node == null) {
                return;
            }

            if (nearestPoints.Count < k) {
                nearestPoints.Add(node.Point);
                nearestPoints = nearestPoints.OrderBy(p => p.DistanceTo(target)).ToList();
            } else {
                double maxDistance = nearestPoints.Max(p => p.DistanceTo(target));
                double currentDistance = node.Point.DistanceTo(target);
                if (currentDistance < maxDistance) {
                    nearestPoints.Add(node.Point);
                    nearestPoints.Remove(nearestPoints.OrderByDescending(p => p.DistanceTo(target)).First());
                    maxDistance = nearestPoints.Max(p => p.DistanceTo(target));
                }
            }

            int axis = node.Depth % 2;
            Node? nearerSubtree = target.X < node.Point.X ? node.Left : node.Right;
            Node? furtherSubtree = target.X < node.Point.X ? node.Right : node.Left;

            FindNearestPointsRecursive(nearerSubtree, target, k, ref nearestPoints);

            if (Math.Abs(node.Point.X - target.X) < nearestPoints.Max(p => Math.Abs(p.X - target.X))) {
                FindNearestPointsRecursive(furtherSubtree, target, k, ref nearestPoints);
            }
        }

        public void DrawKDTree() {
            DrawNode(root, "");
        }

        private void DrawNode(Node? node, string indent) {
            if (node == null)
                return;

            Console.WriteLine(indent + "└── (" + node.Point.X + ", " + node.Point.Y + ")");

            indent += "    ";
            DrawNode(node.Left, indent);
            DrawNode(node.Right, indent);
        }

        private class Node {
            public Point Point { get; set; }
            public Node? Left { get; set; }
            public Node? Right { get; set; }
            public int Depth { get; set; }

            public Node(Point point) {
                Point = point;
            }
        }
    }
}