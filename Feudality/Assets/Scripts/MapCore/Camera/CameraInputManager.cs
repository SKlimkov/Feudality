using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Feudality.GlobalMap.Input {
    public class CameraInputManager : MonoBehaviour {
        private List<CameraInputBehaviour> cameraInputBehaviours;

        private void Awake() {
            cameraInputBehaviours = GetComponents<CameraInputBehaviour>().ToList();
        }

        private void FixedUpdate() {
            if (cameraInputBehaviours == null) return;

            foreach (var entry in cameraInputBehaviours) {
                entry.OnFixedUpdate();
            }
        }
    }
}
