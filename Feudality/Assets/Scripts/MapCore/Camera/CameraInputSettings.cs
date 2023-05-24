using System;
using UnityEngine;

namespace Feudality.GlobalMap.Input {
    [CreateAssetMenu(fileName = "CameraInputSettings", menuName = "ScriptableObjects/GlobalMap/CameraInput", order = 1)]
    public class CameraInputSettings : ScriptableObject {
        [Space, Header("Move")]
        public float CameraMoveSpeed = 1f;
        public float MoveLimitOffset = 0.1f;

        [Space, Header("Scroll")]
        public float CameraScrollSpeed = 1f;
        [OneLine.OneLine]
        public MinMax CameraScrollLimits;

        [Space, Header("Look")]
        public float CameraLookSpeed = 0.5f;
        [OneLine.OneLine]
        public MinMax CameraPitchLimits;
        public float PitchLimitMaxExpansionFactor = 0.3f;
    }
}
