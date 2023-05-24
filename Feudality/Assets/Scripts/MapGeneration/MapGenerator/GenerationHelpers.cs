using AccidentalNoise;
using Feudality.MapGeneration.Settings;
using Feudality.RuntimeSerialization;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Feudality.MapGeneration {
    public static class GenerationHelpers {
        public static async Task<HeightData> GenerateHeightsAsync(Action<float> progressCallback, Point2Int size, ImplicitModuleBase implicitModule) {

            var stepsCount = size.x * size.y;
            var currentStep = 0;

            var result = new HeightData(size);
            var heightMinValue = float.MaxValue;
            var heightMaxValue = float.MinValue;
            for (var x = 0; x < result.Size.x; x++) {
                for (var y = 0; y < result.Size.y; y++) {
                    currentStep++;
                    progressCallback?.Invoke((float)currentStep / stepsCount);

                    // Noise limits
                    float x1 = 0, x2 = 2;
                    float y1 = 0, y2 = 2;
                    float dx = x2 - x1;
                    float dy = y2 - y1;

                    // Noise sampling
                    float s = x / (float)result.Size.x;
                    float t = y / (float)result.Size.y;

                    // Calculating 4d coords
                    float nx = x1 + Mathf.Cos(s * 2 * Mathf.PI) * dx / (2 * Mathf.PI);
                    float ny = y1 + Mathf.Cos(t * 2 * Mathf.PI) * dy / (2 * Mathf.PI);
                    float nz = x1 + Mathf.Sin(s * 2 * Mathf.PI) * dx / (2 * Mathf.PI);
                    float nw = y1 + Mathf.Sin(t * 2 * Mathf.PI) * dy / (2 * Mathf.PI);

                    float heightValue = (float)implicitModule.Get(nx, ny, nz, nw);

                    if (heightValue > heightMaxValue) heightMaxValue = heightValue;
                    if (heightValue < heightMinValue) heightMinValue = heightValue;

                    result.UpdateMinMax(heightMinValue, heightMaxValue);
                    result.UpdateValueForIndex(heightValue, x, y);
                }
            }

            await Task.Yield();

            return result;
        }

        public static async Task<TileData> GenerateTilesAsync(Action<float> progressCallback, HeightData heightData) {
            var size = heightData.Size;
            var result = new TileData(size);

            var heightMinValue = float.MaxValue;
            var heightMaxValue = float.MinValue;

            var stepsCount = size.x * size.y;
            var currentStep = 0;

            for (var x = 0; x < size.x; x++) {
                for (var y = 0; y < size.y; y++) {
                    currentStep++;
                    progressCallback?.Invoke((float)currentStep / stepsCount);

                    //Align values to 0..1 diapasone
                    float value = heightData.Data[x, y];
                    value = (value - heightData.Min) / (heightData.Max - heightData.Min);
                    result.UpdateTile(new Point2Int(x, y), value);

                    //Update minmax values
                    if (value > heightMaxValue) heightMaxValue = value;
                    if (value < heightMinValue) heightMinValue = value;
                }
            }

            await Task.Yield();

            return result;
        }

        public static Texture2D GetTexture(Color[] colors, Point2Int size) {
            var texture = new Texture2D(size.x, size.y);
            texture.SetPixels(colors);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.Apply();
            return texture;
        }

        public static async Task<float[,]> ExpandHeightMap(Action<float> progressCallback, float[,] heightMap) {
            var size = new Vector2Int(heightMap.GetLength(0), heightMap.GetLength(1));
            var doubleSize = size * 2;
            var stepsCount = size.x * size.y + size.x * size.y * 9;
            var currentStep = 0;
            var result = new float[doubleSize.x, doubleSize.y];
            for (var x = 0; x < size.x; x++) {
                for (var y = 0; y < size.y; y++) {
                    currentStep++;
                    progressCallback?.Invoke((float)currentStep / stepsCount);

                    var sourceIndex = new Vector2Int(x, y);
                    var current = heightMap[x, y];
                    var resultIndex = sourceIndex * 2;
                    result[resultIndex.x, resultIndex.y] = current;

                    var isValidLenghtX = sourceIndex.x + 1 < size.x;
                    var isValidLenghtY = sourceIndex.y + 1 < size.y;

                    var right = heightMap[isValidLenghtX ? x + 1 : x - 1, y];
                    var rightIndex = resultIndex + Vector2Int.right;
                    result[rightIndex.x, rightIndex.y] = Math.Clamp(current - GetDeltaHeight(current, right, isValidLenghtX), 0f, 1f);

                    var bottom = heightMap[x, isValidLenghtY ? y + 1 : y - 1];
                    var bottomIndex = resultIndex + Vector2Int.up;
                    result[bottomIndex.x, bottomIndex.y] = Math.Clamp(current - GetDeltaHeight(current, bottom, isValidLenghtY), 0f, 1f);
                }
            }

            for (var x = 0; x < size.x; x++) {
                for (var y = 0; y < size.y; y++) {
                    currentStep++;
                    progressCallback?.Invoke((float)currentStep / stepsCount);

                    var sourceIndex = new Vector2Int(x, y);
                    var resultIndex = sourceIndex * 2 + Vector2Int.one;
                    var summ = 0f;
                    var count = 0;
                    for (var xi = -1; xi < 2; xi++) {
                        for (var yi = -1; yi < 2; yi++) {
                            currentStep++;
                            progressCallback?.Invoke((float)currentStep / stepsCount);

                            var tmpIndex = new Vector2Int(xi, yi);
                            var dIndex = resultIndex + tmpIndex;
                            var distance = dIndex - resultIndex;
                            var isInBoundsOfArray = dIndex.x >= 0 && dIndex.x < result.GetLength(0) && dIndex.y >= 0 && dIndex.y < result.GetLength(1);
                            var isStraight = distance.sqrMagnitude <= 1f && distance.sqrMagnitude > 0;

                            if (isInBoundsOfArray && isStraight) {
                                summ += result[dIndex.x, dIndex.y];
                                count++;
                            }
                        }
                    }
                    result[resultIndex.x, resultIndex.y] = summ / count;
                }
            }

            await Task.Yield();
            return result;
        }

        public static async Task<Color[]> GenerateColorsAsync(Action<float> progressCallback, float[,] heightMap, IColorData colorLimits) {
            var size = new Vector2Int(heightMap.GetLength(0), heightMap.GetLength(1));
            var result = new Color[size.x * size.y];
            var stepsCount = size.x * size.x;
            var currentStep = 0;
            for (var x = 0; x < size.x; x++) {
                for (var y = 0; y < size.y; y++) {
                    currentStep++;
                    progressCallback?.Invoke((float)currentStep / stepsCount);
                    result[size.x * x + y] = colorLimits.GetColor(heightMap[x, y]);
                }
            }

            await Task.Yield();
            return result;
        }

        private static float GetDeltaHeight(float current, float next, bool isNextValid) {
            return (current - next) / 2 * (isNextValid ? 1f : -1f);
        }
    }
}
