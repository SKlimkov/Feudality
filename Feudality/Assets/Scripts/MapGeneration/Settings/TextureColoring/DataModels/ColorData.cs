using System;
using System.Collections.Generic;
using UnityEngine;

namespace Feudality.MapGeneration.Settings {
    [Serializable]
    public abstract class ColorData<TColorValue> : IColorData {
        [SerializeField, OneLine.OneLine] protected List<TColorValue> ColorList;

        public abstract Color GetColor(float value);
        public abstract EColorType ColorType { get; }
    }
}
