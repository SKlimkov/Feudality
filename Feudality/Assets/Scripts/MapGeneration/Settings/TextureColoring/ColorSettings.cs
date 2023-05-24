using System.Collections.Generic;
using UnityEngine;

namespace Feudality.MapGeneration.Settings {

    [CreateAssetMenu(fileName = "ColorSettings", menuName = "ScriptableObjects/MapGeneration/CreateColorSettings", order = 1)]
    public class ColorSettings : ScriptableObject {
        public SolidColorData SolidColorData;
        public GradientColorData GradientColorData;
        public List<Gradient> gradientsCache;
    }
}
