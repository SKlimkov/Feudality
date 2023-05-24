using UnityEngine;
using UnityEngine.InputSystem;

namespace Feudality.GlobalMap.Input {
    public class CameraLookBehaviour : CameraInputBehaviour {
        private Vector2 delta;
        
        public override void OnInputTrigger(InputAction.CallbackContext context) {
            delta = context.ReadValue<Vector2>();
        }

        public override void OnFixedUpdate() {
            if (delta.sqrMagnitude > 0) {
                var smootheedSpeed = settings.CameraLookSpeed * Time.fixedDeltaTime;

                var pitchDelta = -delta.y * smootheedSpeed;
                var minPitch = MathHelpers.GetMinCamPitch(terrain, cameraTransform.position, waterPlane.position.y, settings);
                var clampedPitch = Mathf.Clamp(cameraTransform.localRotation.eulerAngles.x + pitchDelta, minPitch, settings.CameraPitchLimits.Max);
                cameraTransform.localRotation = Quaternion.Euler(clampedPitch, 0, 0);

                var yawDelta = delta.x * smootheedSpeed;
                cameraRoot.rotation = Quaternion.Euler(0, cameraRoot.rotation.eulerAngles.y + yawDelta, 0);
            }
        }
    }
}
