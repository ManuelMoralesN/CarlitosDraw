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
        [SerializeField] string gameplayScene;
        [SerializeField] string joinRoom;

        [Space]
        [SerializeField] string manualTriggerWithKeyPressSceneName;
        [SerializeField] string manualTriggerManualActivationSceneName;

        public string DemoSceneName => demoSceneName;
        public string GameplayScene => gameplayScene;
        public string JoinRoom => joinRoom;
        public string ManualTriggerWithKeyPressSceneName => manualTriggerWithKeyPressSceneName;
        public string ManualTriggerWithManualActivationSceneName => manualTriggerManualActivationSceneName;

        private void Start() => StartCoroutine(ActivateAfterDelay());

        IEnumerator ActivateAfterDelay()
        {
            yield return new WaitForSeconds(.95f);
            demoCanvas.SetActive(true);
        }

        public void LoadGameplay()
        {
            demoCanvas.SetActive(false);
            SceneLoadManager.Load(gameplayScene, sceneSettings);
        }

        public void LoadAutoTriggerSceneWithDelay()
        {
            demoCanvas.SetActive(false);
            SceneLoadManager.Load(joinRoom, sceneSettings);
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