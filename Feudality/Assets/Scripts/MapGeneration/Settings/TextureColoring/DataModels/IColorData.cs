using UnityEngine;

namespace Feudality.MapGeneration.Settings {
    public enum EColorType {
        Solid,
        Gradient
    };
    public interface IColorData {
        Color GetColor(float value);
        EColorType ColorType { get; }
    }
}