using System;
using System.Linq;
using UnityEngine;

namespace Feudality.MapGeneration.Settings {
    [Serializable]
    public class GradientColorData : ColorData<GradientColorValue> {
        public override Color GetColor(float value) {
            foreach (var limit in ColorList) {
                if (value <= limit.MaxValue) {
                    var percents = (value - limit.MinValue) /
                        (limit.MaxValue - limit.MinValue);
                    return limit.Gradient.Evaluate(percents);
                }
            }
            return ColorList.Last().Gradient.Evaluate(1f);
        }
        public override EColorType ColorType => EColorType.Gradient;
    }
}
