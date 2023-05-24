using System;
using System.Linq;
using UnityEngine;

namespace Feudality.MapGeneration.Settings {
    [Serializable]
    public class SolidColorData : ColorData<SolidColorValue> {
        public override Color GetColor(float value) {
            foreach (var limit in ColorList) {
                if (value <= limit.MaxValue) {
                    return limit.Color;
                }
            }
            return ColorList.Last().Color;
        }
        public override EColorType ColorType => EColorType.Solid;
    }
}
