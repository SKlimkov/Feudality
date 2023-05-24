using System.IO;
using UnityEngine;

namespace Feudality.MapGeneration.Settings {
    [CreateAssetMenu(fileName = "FilePathSettings", menuName = "ScriptableObjects/MapGeneration/CreateFilePathSettings", order = 1)]
    public class FilePathSettings : ScriptableObject {
        [SerializeField] private string path = "Content/GeneratedMaps";
        [SerializeField] private string mapFileName = "map.bin";
        [SerializeField] private string textureFileName = "texture";
        [SerializeField] private string extension = ".jpg";

        public (string FileName, string Path) GetBinaryPath(NoiseSettings noiseSettings) {
            return (mapFileName, GetPath(noiseSettings));
        }

        public (string FileName, string Path) GetTexturePath(NoiseSettings noiseSettings, EColorType colorType) {
            return ($"{textureFileName}_{colorType.ToString().ToLower()}{extension}", GetPath(noiseSettings));
        }

        private string GetPath(NoiseSettings noiseSettings) {
            return Path.Combine(
                Application.dataPath,
                path, 
                $"{noiseSettings.Width}x{noiseSettings.Height}", 
                noiseSettings.Seed.ToString());
        }
    }
}
