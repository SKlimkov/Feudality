using UnityEngine;
using UnityEngine.InputSystem;

namespace Feudality.GlobalMap.Input {
    public class CameraMoveBehaviour : CameraInputBehaviour {
        private MinMax xMinMax;
        private MinMax yMinMax;
        private Vector2 cameraMoveDirection;

        private void Awake() {
            var size = terrain.terrainData.size;
            xMinMax = new MinMax() { 
                Min = 0 + size.x * settings.MoveLimitOffset,
                Max = size.x - size.x * settings.MoveLimitOffset
            };
            yMinMax = new MinMax() { 
                Min = 0 + size.z * settings.MoveLimitOffset,
                Max = size.z - size.z * settings.MoveLimitOffset
            };
        }

        public override void OnInputTrigger(InputAction.CallbackContext context) {
            cameraMoveDirection = context.ReadValue<Vector2>();
        }

        public override void OnFixedUpdate() {
            if (cameraMoveDirection.sqrMagnitude > 0) {
                var oldPosition = cameraRoot.transform.position;
                var zAxis = cameraRoot.forward * cameraMoveDirection.y;
                var xAxis = cameraRoot.right * cameraMoveDirection.x;
                var delta = (zAxis + xAxis) * settings.CameraMoveSpeed * Time.fixedDeltaTime;

                var oldMinYPosition = MathHelpers.GetMinCamHeight(terrain, oldPosition, waterPlane.position.y, settings.CameraScrollLimits.Min);
                var yDelta = oldPosition.y - oldMinYPosition;
                var newPosition = new Vector3(
                    Mathf.Clamp(oldPosition.x + delta.x, xMinMax.Min, xMinMax.Max),
                    oldPosition.y + delta.y,
                    Mathf.Clamp(oldPosition.z + delta.z, yMinMax.Min, yMinMax.Max));
                var newMinYPosition = MathHelpers.GetMinCamHeight(terrain, newPosition, waterPlane.position.y, settings.CameraScrollLimits.Min);
                var newYPosition = Mathf.Clamp(oldMinYPosition, newMinYPosition + yDelta, newMinYPosition + settings.CameraScrollLimits.Max);
                newPosition = new Vector3(newPosition.x, newYPosition, newPosition.z);
                cameraRoot.transform.position = newPosition;
            }
        }
    }
}
