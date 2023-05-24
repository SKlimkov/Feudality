using UnityEngine;
using UnityEngine.InputSystem;

namespace Feudality.GlobalMap.Input {
    public abstract class CameraInputBehaviour : MonoBehaviour {
        [SerializeField] protected CameraInputSettings settings;
        [SerializeField] protected Transform cameraRoot;
        [SerializeField] protected Transform cameraTransform;
        [SerializeField] protected Terrain terrain;
        [SerializeField] protected Transform waterPlane;

        public abstract void OnInputTrigger(InputAction.CallbackContext context);
        public abstract void OnFixedUpdate();
    }
}
