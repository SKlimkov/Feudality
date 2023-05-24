using UnityEngine;

namespace Feudality.MapGeneration.Settings {
    [CreateAssetMenu(fileName = "NoiseSettings", menuName = "ScriptableObjects/MapGeneration/CreateNoiseSettings", order = 1)]
    public class NoiseSettings : ScriptableObject {
        public int Width = 512;
        public int Height = 512;
        public int TerrainOctaves = 6;
        public double TerrainFrequency = 1.25;
        public int Seed = int.MaxValue;
    }
}
