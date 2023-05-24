using UnityEngine;
using UnityEngine.InputSystem;

namespace Feudality.GlobalMap.Input {
    public class CameraScrollBehaviour : CameraInputBehaviour {
        private Vector2 scrollPosition;

        public override void OnInputTrigger(InputAction.CallbackContext context) {
            scrollPosition = context.ReadValue<Vector2>().normalized;
        }

        public override void OnFixedUpdate() {
            if (scrollPosition.sqrMagnitude > 0) {
                var minPosition = MathHelpers.GetMinCamHeight(terrain, cameraRoot.position, waterPlane.position.y, settings.CameraScrollLimits.Min);
                var yDelta = scrollPosition.y * settings.CameraScrollSpeed * Time.fixedDeltaTime;
                var newY = Mathf.Clamp(cameraRoot.position.y + yDelta, minPosition, minPosition + settings.CameraScrollLimits.Max);
                cameraRoot.position = new Vector3(cameraRoot.position.x, newY, cameraRoot.position.z);

                var minPitch = MathHelpers.GetMinCamPitch(terrain, cameraTransform.position, waterPlane.position.y, settings);
                cameraTransform.localRotation = Quaternion.Euler(Mathf.Clamp(cameraTransform.localRotation.eulerAngles.x, minPitch, settings.CameraPitchLimits.Max), 0, 0);
            }
        }
    }
}
