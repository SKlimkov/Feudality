using Feudality.MapGeneration.Settings;
using UnityEditor;
using UnityEngine;

namespace Feudality.MapGeneration {
    public class MapGenerationWindow : EditorWindow {
        private const string textureToggleKey = "MapGenerationTextureToggleValue";

        private MapGenerationContext mapGenerationContext;
        private float texturePadding = 5f;
        private float currentProgress;
        private bool isInProgress;
        private bool textureGenerationNeeded;
        private int textureInterpolationCount;

        [MenuItem("Map Generator/Generation window")]
        static void Init() {
            var window = GetWindow(typeof(MapGenerationWindow));
            window.minSize = new Vector2 (600, 605);
            window.Show();
        }

        private void OnEnable() {
            mapGenerationContext = FindObjectOfType<MapGenerationContext>();

            if (PlayerPrefs.HasKey(textureToggleKey)) {
                textureGenerationNeeded = IntToBool(PlayerPrefs.GetInt(textureToggleKey));
            }
        }

        private void OnDisable() {
            PlayerPrefs.SetInt(textureToggleKey, BoolToInt(textureGenerationNeeded));
        }

        private void OnGUI() {
            var halfWidth = position.width / 2;
            ShowButtonsBlock(halfWidth);

            if (mapGenerationContext.SolidTexture != null) {
                GUI.DrawTexture(new Rect(halfWidth, 0, halfWidth, halfWidth), mapGenerationContext.SolidTexture);
            }

            if (mapGenerationContext.GradientTexture != null) {
                GUI.DrawTexture(new Rect(halfWidth, halfWidth + texturePadding, halfWidth, halfWidth), mapGenerationContext.GradientTexture);
            }
        }

        private void ShowButtonsBlock(float halfWidth) {
            GUILayout.BeginVertical(GUILayout.Width(halfWidth));

            var result = GUILayout.Toggle(textureGenerationNeeded, "Generate terrain texture");
            if (textureGenerationNeeded != result) {
                textureGenerationNeeded = result;
                Debug.LogFormat("Change!");
            }

            textureInterpolationCount = (int)EditorGUILayout.Slider("Texture interpolation count", textureInterpolationCount, 0, 2);

            GUI.enabled = !isInProgress && mapGenerationContext.CanLoad();

            if (GUILayout.Button("Load")) {
                OnLoadButtonClick();
            }

            GUI.enabled = !isInProgress;

            if (GUILayout.Button("Generate") && mapGenerationContext != null)
                OnGenerateButtonClick();

            GUI.enabled = !isInProgress && mapGenerationContext.CanSetToTerrain(textureGenerationNeeded);

            if (GUILayout.Button("Set to terrain")) {
                OnSetToTerrainButtonClick();
            }

            if (isInProgress) {
                var rect = EditorGUILayout.GetControlRect();
                var singleLineHeight = EditorGUIUtility.singleLineHeight;
                var space = EditorGUIUtility.standardVerticalSpacing;
                var yPosition = (rect.y + rect.height + space) - singleLineHeight;
                Rect progressBarRect = new Rect(rect.x, yPosition, rect.width, singleLineHeight);
                EditorGUI.ProgressBar(progressBarRect, currentProgress, "Generation");
                EditorGUILayout.Space();
            }

            GUILayout.EndVertical();
        }

        private async void OnLoadButtonClick() {
            isInProgress = true;
            Debug.LogFormat("Loading");
            await mapGenerationContext.LoadTileData();

            if (textureGenerationNeeded) {
                var colorType = EColorType.Solid;
                await mapGenerationContext.GenerateTextureColors(ProgressCallback, colorType);
                mapGenerationContext.GenerateTexture(colorType);
                await mapGenerationContext.SaveTexture(colorType);

                colorType = EColorType.Gradient;
                await mapGenerationContext.GenerateTextureColors(ProgressCallback, colorType);
                mapGenerationContext.GenerateTexture(colorType);
                await mapGenerationContext.SaveTexture(colorType);
            }  
            
            Debug.LogFormat("Finish loading");
            isInProgress = false;
        }

        private async void OnGenerateButtonClick() {
            isInProgress = true;
            Debug.LogFormat("Generation");
            await mapGenerationContext.GenerateHeightData(ProgressCallback);
            await mapGenerationContext.GenerateTileData(ProgressCallback);
            
            if (textureGenerationNeeded) {
                var colorType = EColorType.Solid;
                for (var i = 0; i < textureInterpolationCount; i++) {
                    await mapGenerationContext.InterpolateHeightMap(ProgressCallback);
                }
                await mapGenerationContext.GenerateTextureColors(ProgressCallback, colorType);
                mapGenerationContext.GenerateTexture(colorType);
                await mapGenerationContext.SaveTexture(colorType);

                colorType = EColorType.Gradient;
                await mapGenerationContext.GenerateTextureColors(ProgressCallback, colorType);
                mapGenerationContext.GenerateTexture(colorType);
                await mapGenerationContext.SaveTexture(colorType);
            }

            await mapGenerationContext.SaveTileData();
            Debug.LogFormat("Finish generation");
            isInProgress = false;
        }

        private void OnSetToTerrainButtonClick() {
            Debug.LogFormat("Set to terrain");
            mapGenerationContext.SetToTerrain(textureGenerationNeeded);
        }

        private void ProgressCallback(float progress) {
            currentProgress = progress;
        }

        private int BoolToInt(bool value) {
            return value ? 1 : 0;
        }

        private bool IntToBool(int value) {
            return value > 0;
        }
    }
}
