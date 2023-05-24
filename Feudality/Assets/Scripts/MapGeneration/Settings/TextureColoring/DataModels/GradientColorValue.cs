using System;
using UnityEngine;

namespace Feudality.MapGeneration.Settings {
    [Serializable]
    public class GradientColorValue {
        public Gradient Gradient;
        public float MinValue;
        public float MaxValue;
    }
}