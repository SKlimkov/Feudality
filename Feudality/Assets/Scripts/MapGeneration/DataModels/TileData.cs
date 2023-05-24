using Feudality.RuntimeSerialization;
using System;

namespace Feudality.MapGeneration {
    [Serializable]
    public class TileData {
        public float[,] HeightData { get; private set; }
        public float[,] ExpandedHeightData { get; private set; }
        public Point2Int Size { get; private set; }
        public TileData(Point2Int size) {
            Size = size;
            HeightData = new float[size.x, size.y];
            ExpandedHeightData = new float[size.x, size.y];
        }

        public void UpdateTile(Point2Int index, float heightValue) {
            HeightData[index.x, index.y] = heightValue;
            ExpandedHeightData[index.x, index.y] = heightValue;
        }

        public void UpdateExpandedHeightData(float[,] heightData) {
            ExpandedHeightData = heightData;
        }
    }
}
