using UnityEngine;
using System.Collections;

namespace CG.SceneToolkit
{
    public class DemoSceneHandler : MonoBehaviour
    {
        [SerializeField] SceneSettings sceneSettings;
        [SerializeField] GameObject demoCanvas;

        [Space]
        [SerializeField] string demoSceneName;

        [Space]
        [SerializeField] string autoTriggerSceneName;
        [SerializeField] string autoTriggerWithDelaySceneName;

        [Space]
        [SerializeField] string manualTriggerWithKeyPressSceneName;
        [SerializeField] string manualTriggerManualActivationSceneName;

        public string DemoSceneName => demoSceneName;
        public string AutoTriggerSceneName => autoTriggerSceneName;
        public string AutoTriggerWithDelaySceneName => autoTriggerWithDelaySceneName;
        public string ManualTriggerWithKeyPressSceneName => manualTriggerWithKeyPressSceneName;
        public string ManualTriggerWithManualActivationSceneName => manualTriggerManualActivationSceneName;

        private void Start() => StartCoroutine(ActivateAfterDelay());

        IEnumerator ActivateAfterDelay()
        {
            yield return new WaitForSeconds(.95f);
            demoCanvas.SetActive(true);
        }

        public void LoadAutoTriggerScene()
        {
            demoCanvas.SetActive(false);
            SceneLoadManager.Load(autoTriggerSceneName, sceneSettings);
        }

        public void LoadAutoTriggerSceneWithDelay()
        {
            demoCanvas.SetActive(false);
            SceneLoadManager.Load(autoTriggerWithDelaySceneName, sceneSettings);
        }

        public void LoadManualTriggerSceneWithKeyPress()
        {
            demoCanvas.SetActive(false);
            SceneLoadManager.Load(manualTriggerWithKeyPressSceneName, sceneSettings);
        }

        public void LoadManualTriggerSceneWithManualActivation()
        {
            demoCanvas.SetActive(false);
            SceneLoadManager.Load(manualTriggerManualActivationSceneName, sceneSettings);
        }
    }
}