using AccidentalNoise;
using Feudality.MapGeneration.Settings;
using Feudality.RuntimeSerialization;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Feudality.MapGeneration {

    public class MapGenerationContext : MonoBehaviour {
        private class TextureData {
            public Texture2D Texture;
            public byte[] File;
        }

        [SerializeField] private NoiseSettings noiseSettings;
        [SerializeField] private ColorSettings colorSettings;
        [SerializeField] private FilePathSettings filePathSettings;

        [SerializeField] private Terrain terrain;
        [SerializeField] private TerrainData terrainData;
        [SerializeField] private TerrainLayer terrainLayer;
        [SerializeField] private Transform waterPlane;
        [SerializeField] private float terrainHeightFactor = 0.1f;

        private HeightData heightData;
        private TileData tileData;
        private Color[] colors;
        private TextureData solidTextureData = new TextureData();
        private TextureData gradientTextureData = new TextureData();

        public Texture SolidTexture { get { return solidTextureData.Texture; } }
        public Texture GradientTexture { get { return gradientTextureData.Texture; } }

        public async Task GenerateHeightData(Action<float> progressCallback) {
            var noiseModule = new ImplicitFractal(
                FractalType.MULTI,
                BasisType.SIMPLEX,
                InterpolationType.QUINTIC,
                noiseSettings.TerrainOctaves,
                noiseSettings.TerrainFrequency,
                noiseSettings.Seed);

            heightData = await GenerationHelpers.GenerateHeightsAsync(progressCallback, new Point2Int(noiseSettings.Width, noiseSettings.Height), noiseModule);
        }

        public async Task GenerateTileData(Action<float> progressCallback) {
            tileData = await GenerationHelpers.GenerateTilesAsync(progressCallback, heightData);
        }

        public async Task SaveTileData() {
            var file = tileData.SerializeObject();
            var saveSettings = filePathSettings.GetBinaryPath(noiseSettings);
            Serialization.SaveFile(saveSettings.Path, saveSettings.FileName, file);            
            await Task.Yield();
            Debug.LogFormat("Save tileData {0}", saveSettings.Path);
        }

        public async Task InterpolateHeightMap(Action<float> progressCallback) {
            var result = await GenerationHelpers.ExpandHeightMap(progressCallback, tileData.ExpandedHeightData);
            tileData.UpdateExpandedHeightData(result);
        }

        public async Task GenerateTextureColors(Action<float> progressCallback, EColorType colorType) {
            var textureData = GetTextureData(colorType);
            colors = await GenerationHelpers.GenerateColorsAsync(progressCallback, tileData.ExpandedHeightData, GetColorData(colorType));
        }

        public void GenerateTexture(EColorType colorType) {
            var data = GetTextureData(colorType);
            data.Texture = GenerationHelpers.GetTexture(colors, new Point2Int(tileData.ExpandedHeightData.GetLength(0), tileData.ExpandedHeightData.GetLength(1)));
            data.File = data.Texture.EncodeToJPG(100);
        }

        public async Task SaveTexture(EColorType colorType) {
            var colorData = GetColorData(colorType);
            var textureData = GetTextureData(colorType);
            var saveSettings = filePathSettings.GetTexturePath(noiseSettings, colorData.ColorType);
            Serialization.SaveFile(saveSettings.Path, saveSettings.FileName, textureData.File);
            await Task.Yield();
            Debug.LogFormat("Save texture type of {0}", colorData.ColorType);
        }

        public void SetToTerrain(bool textureGenerationNeeded) {
            terrainData.heightmapResolution = tileData.Size.x + 1;
            var teerraintHeight = tileData.Size.x * terrainHeightFactor;
            terrainData.size = new Vector3(tileData.Size.x, teerraintHeight, tileData.Size.y);
            waterPlane.position = new Vector3(waterPlane.position.x, teerraintHeight * 0.4f, waterPlane.position.z);
            terrainData.SetHeights(0, 0, tileData.HeightData);
            terrainLayer.tileSize = tileData.Size.ToVector2();
            if (textureGenerationNeeded) {
                terrainLayer.diffuseTexture = gradientTextureData.Texture;                
                EditorUtility.SetDirty(terrainLayer);
            }
        }

        public bool CanSetToTerrain(bool textureGenerationNeeded) {
            return tileData != null && terrainLayer != null && (gradientTextureData.Texture != null || !textureGenerationNeeded);
        }

        public async Task LoadTileData() {
            var saveSettings = filePathSettings.GetBinaryPath(noiseSettings);
            var path = Path.Combine(saveSettings.Path, saveSettings.FileName);
            var file = Serialization.LoadFile(path);
            tileData = file.DeserializeObject<TileData>();
            Debug.LogFormat("Tile data loaded from {0}", path);
            await Task.Yield();
        }

        public bool CanLoad() {
            var saveSettings = filePathSettings.GetBinaryPath(noiseSettings);
            var path = Path.Combine(saveSettings.Path, saveSettings.FileName);
            return File.Exists(path);
        }

        private IColorData GetColorData(EColorType colorType) {
            return colorType == EColorType.Solid ? colorSettings.SolidColorData : colorSettings.GradientColorData;
        }
        private TextureData GetTextureData(EColorType colorType) {
            return colorType == EColorType.Solid ? solidTextureData : gradientTextureData;
        }
    }
}
