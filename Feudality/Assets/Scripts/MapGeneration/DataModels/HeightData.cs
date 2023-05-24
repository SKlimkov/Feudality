using Feudality.RuntimeSerialization;
using UnityEngine;

namespace Feudality.MapGeneration {
    public class HeightData {
        public float[,] Data { get; private set; }
        public Point2Int Size { get; private set; }
        public Vector2 MinMax { get; private set; }
        public float Min => MinMax.x;
        public float Max => MinMax.y;

        public HeightData(Point2Int size) {
            Size = size;
            Data = new float[size.x, size.y];
        }

        public void UpdateValueForIndex(float value, int x, int y) {
            Data[x, y] = value;
        }

        public void UpdateMinMax(float minValue, float maxValue) {
            MinMax = new Vector2(minValue, maxValue);
        }
    }
}
