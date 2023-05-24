using Feudality.GlobalMap.Input;
using UnityEngine;

namespace Feudality {
    public static class MathHelpers {
        public static float GetMinCamHeight(Terrain terrain, Vector3 position, float waterLevel, float minCamScrollLimit) {
            return Mathf.Max(terrain.SampleHeight(position), waterLevel) + minCamScrollLimit;
        }

        public static float GetMinCamPitch(Terrain terrain, Vector3 position, float waterLevel, CameraInputSettings settings) {
            var camMinY = GetMinCamHeight(terrain, position, waterLevel, settings.CameraScrollLimits.Min);
            var camMaxY = camMinY + settings.CameraScrollLimits.Max;

            var expansionValue = (camMaxY - position.y) / (camMaxY - camMinY);
            return settings.CameraPitchLimits.Min - (settings.CameraPitchLimits.Min - settings.CameraPitchLimits.Min * settings.PitchLimitMaxExpansionFactor) * expansionValue;
        }
    }
}
