using System;
using UnityEngine;

namespace Feudality.RuntimeSerialization {
    [Serializable]
    public struct Point2Int {
        public int x;
        public int y;

        public Point2Int(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public static Point2Int operator -(Point2Int a, Point2Int b) {
            return new Point2Int(a.x - b.x, a.y - b.y);
        }

        public static Point2Int operator +(Point2Int a, Point2Int b) {
            return new Point2Int(a.x + b.x, a.y + b.y);
        }

        public static Point2Int Top {
            get {
                return new Point2Int(0, -1);
            }
        }

        public static Point2Int Bottom {
            get {
                return new Point2Int(0, 1);
            }
        }

        public static Point2Int Left {
            get {
                return new Point2Int(-1, 0);
            }
        }

        public static Point2Int Right {
            get {
                return new Point2Int(1, 0);
            }
        }

        public Vector2 ToVector2() {
            return new Vector2(x, y);
        }
    }
}
