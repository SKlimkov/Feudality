using UnityEngine;

namespace Feudality.GlobalMap.TimeSystem {
    public class DayNightCycle : MonoBehaviour {
        [SerializeField] private Light directionalLight;
        [SerializeField] private Transform sunRoot;
        [SerializeField] private Material skyboxMaterial;

        [Space, Header("Time")]
        [SerializeField] private float dayDuration = 10f;

        [Space, Header("Sky")]
        [SerializeField] private Gradient zenithColor;
        [SerializeField] private Gradient horizonColor;
        [SerializeField] private AnimationCurve dayTimeValueCurve;

        [Space, Header("Sun")]
        [SerializeField] private Gradient sunColor;
        [SerializeField] private AnimationCurve sunColorCurve;
        [SerializeField] private AnimationCurve sunRadiusCurve;

        [Space, Header("Corona")]
        [SerializeField] private Gradient coronaColor;
        [SerializeField] private AnimationCurve coronaColorCurve;
        [SerializeField] private AnimationCurve coronaRadiusCurve;
        [SerializeField] private AnimationCurve coronaIntencityCurve;

        [Space, Header("Sunset")]
        [SerializeField] private AnimationCurve sunsetIntencityCurve;

        [Space, Header("Lightning")]
        [SerializeField] private Gradient lightningColor;
        [SerializeField] private AnimationCurve lightningColorCurve;

        private DateTime sessionStart;
        private int convertionFactor = 100;
        private ulong sessionTime;
        private int playSpeed = 1;
        private bool isPaused;

        private void Awake() {
            directionalLight = sunRoot.GetComponent<Light>();
            sessionStart = new DateTime(354, 3, 18, 9, 0);
        }

        private void FixedUpdate() {
            if (!isPaused) {
                var delta = Time.fixedDeltaTime * convertionFactor * playSpeed;
                sessionTime += (ulong)delta;
            }
            UpdateSkebox();
        }

        private void UpdateSkebox() {
            var partOfDay = (float)GetDateTime().MinutesFromDayStarts / DateTime.MinutesInDay;
            sunRoot.localRotation = Quaternion.Euler(360 * partOfDay, 0, 0);
            skyboxMaterial.SetFloat("_DayTime", dayTimeValueCurve.Evaluate(partOfDay));
            UpdateSun(partOfDay);
        }

        private void UpdateSun(float partOfDay) {
            var timeFacrtor = 360 / dayDuration;
            sunRoot.Rotate(Vector3.right * Time.fixedDeltaTime * timeFacrtor);

            skyboxMaterial.SetVector("_SunDirection", sunRoot.forward);            
            skyboxMaterial.SetColor("_SunColor", sunColor.Evaluate(sunColorCurve.Evaluate(partOfDay)));
            skyboxMaterial.SetFloat("_SunRadiusB", sunRadiusCurve.Evaluate(partOfDay));

            skyboxMaterial.SetFloat("_CoronaRadiusB", coronaRadiusCurve.Evaluate(partOfDay));
            skyboxMaterial.SetColor("_CoronaColor", coronaColor.Evaluate(coronaColorCurve.Evaluate(partOfDay)));

            skyboxMaterial.SetFloat("_HorizonIntencity", sunsetIntencityCurve.Evaluate(partOfDay));
            directionalLight.color = lightningColor.Evaluate(lightningColorCurve.Evaluate(partOfDay));
        }

        public DateTime GetDateTime() {
            return sessionStart + DateTime.TimeToMinutes(sessionTime, dayDuration, convertionFactor);
        }
    }
}
